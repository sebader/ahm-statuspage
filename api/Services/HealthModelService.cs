using System.Text.Json;
using System.Net.Http.Headers;
using Azure.Core;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Api.Models;
using api.Utils;
using System.Web;

namespace api.Services
{
    public interface IHealthModelService
    {
        Task<EntityHistory> GetEntityHistoryAsync(string entityName);
        Task<List<ComponentStatus>> GetStatusAsync(string[] entityNames);
    }

    public class HealthModelService : IHealthModelService
    {
        private readonly ILogger<HealthModelService> _logger;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly bool _isLocalEnvironment;
        private readonly string _entityHistorySamplePath;
        private readonly string _healthModelSamplePath;
        private readonly DefaultAzureCredential _credential;

        public HealthModelService(ILogger<HealthModelService> logger, IConfiguration config, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _config = config;
            _httpClientFactory = httpClientFactory;
            _isLocalEnvironment = _config["IsLocalEnvironment"]?.ToLowerInvariant() == "true";
            _entityHistorySamplePath = "sample_data/entity_history_sample.json";
            _healthModelSamplePath = "sample_data/healthmodel_sample.json";
            _credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                ManagedIdentityClientId = _config["MANAGED_IDENTITY_CLIENT_ID"]
            });
        }

        private async Task<string> GetAuthTokenAsync()
        {
            var token = await _credential.GetTokenAsync(new TokenRequestContext(["https://data.healthmodels.azure.com/.default"]));
            return token.Token;
        }

        private string GetHealthModelsHost()
        {
            var host = _config["HealthModelsHost"];
            if (string.IsNullOrEmpty(host))
            {
                throw new InvalidOperationException("HealthModelsHost configuration is missing");
            }
            return host;
        }

        private async Task<T> ReadLocalDataAsync<T>(string filePath)
        {
            var jsonContent = await File.ReadAllTextAsync(filePath);
            return JsonSerializer.Deserialize<T>(jsonContent) ?? throw new InvalidOperationException($"Failed to deserialize {typeof(T).Name}");
        }

        private async Task<T> FetchRemoteDataAsync<T>(string endpoint)
        {
            var token = await GetAuthTokenAsync();
            using var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            
            var apiResponse = await client.GetAsync($"{GetHealthModelsHost()}{endpoint}");
            apiResponse.EnsureSuccessStatusCode();

            var jsonContent = await apiResponse.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(jsonContent) ?? throw new InvalidOperationException($"Failed to deserialize {typeof(T).Name}");
        }

        private async Task<T> GetDataAsync<T>(string localPath, string remoteEndpoint)
        {
            return _isLocalEnvironment 
                ? await ReadLocalDataAsync<T>(localPath)
                : await FetchRemoteDataAsync<T>(remoteEndpoint);
        }

        public async Task<EntityHistory> GetEntityHistoryAsync(string entityName)
        {
            var response = await GetDataAsync<EntityHistory>(
                _entityHistorySamplePath,
                $"/api/entities/{HttpUtility.UrlEncode(entityName)}/history"
            );
            
            NormalizeEntityHistoryStatuses(response);
            return response;
        }

        public async Task<List<ComponentStatus>> GetStatusAsync(string[] entityNames)
        {
            var response = await GetDataAsync<HealthEngineResponse>(
                _healthModelSamplePath,
                "/api/views/default/v2/query"
            );

            if (response.healthModel.entities == null)
            {
                throw new InvalidOperationException("No entities found in the response");
            }

            var statuses = new List<ComponentStatus>();
            
            // Add root entity first if found
            var rootEntity = response.healthModel.entities.FirstOrDefault(e => e.kind == "System_HealthModelRoot");
            if (rootEntity != null)
            {
                statuses.Add(new ComponentStatus
                {
                    Name = rootEntity.name,
                    DisplayName = "System health",
                    Status = StatusUtils.NormalizeStatus(rootEntity.state),
                    Description = $"Kind: {rootEntity.kind} - Impact: {rootEntity.impact}"
                });
            }

            // Add other configured entities
            statuses.AddRange(response.healthModel.entities
                .Where(e => entityNames.Contains(e.name) && e.kind != "System_HealthModelRoot")
                .Select(e => new ComponentStatus
                {
                    Name = e.name,
                    DisplayName = e.displayName,
                    Status = StatusUtils.NormalizeStatus(e.state),
                    Description = $"Kind: {e.kind} - Impact: {e.impact}"
                }));

            return statuses;
        }

        private void NormalizeEntityHistoryStatuses(EntityHistory response)
        {
            foreach (var transition in response.History.Transitions)
            {
                transition.PreviousState = StatusUtils.NormalizeStatus(transition.PreviousState);
                transition.NewState = StatusUtils.NormalizeStatus(transition.NewState);
            }
        }
    }
}

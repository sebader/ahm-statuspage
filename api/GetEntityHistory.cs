using System.Text.Json;
using System.Net.Http.Headers;
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Api.Models;
using System.Web;
using api.Utils;

namespace api
{
    public class GetEntityHistory
    {
        private readonly ILogger<GetEntityHistory> _logger;
        private readonly IConfiguration _config;
        private readonly string _sampleResponsePath;
        private readonly HttpClient _httpClient;
        private readonly bool _isLocalEnvironment;

        public GetEntityHistory(ILogger<GetEntityHistory> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _sampleResponsePath = "sample_data/entity_history_sample.json";
            _httpClient = new HttpClient();
            _isLocalEnvironment = _config["IsLocalEnvironment"]?.ToLowerInvariant() == "true";
        }

        [Function("GetEntityHistory")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("Processing entity history request");

            try
            {
                // Get entityName from query parameter
                var entityName = req.Query["entityName"].ToString();
                if (string.IsNullOrEmpty(entityName))
                {
                    return new BadRequestObjectResult("Entity name is required");
                }

                EntityHistory response;

                if (_isLocalEnvironment)
                {
                    // Use sample data for local development
                    var jsonContent = await File.ReadAllTextAsync(_sampleResponsePath);
                    response = JsonSerializer.Deserialize<EntityHistory>(jsonContent) ?? throw new InvalidOperationException();
                    
                    // Map Error states to Unhealthy for consistency
                    foreach (var transition in response.History.Transitions)
                    {
                        transition.PreviousState = StatusUtils.NormalizeStatus(transition.PreviousState);
                        transition.NewState = StatusUtils.NormalizeStatus(transition.NewState);
                    }
                }
                else
                {
                    // Get token using managed identity
                    var credential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
                    {
                        ManagedIdentityClientId = _config["MANAGED_IDENTITY_CLIENT_ID"]
                    });
                    var token = await credential.GetTokenAsync(new TokenRequestContext(["https://data.healthmodels.azure.com/.default"]));

                    // Get the host from configuration
                    var healthModelsHost = _config["HealthModelsHost"];
                    if (string.IsNullOrEmpty(healthModelsHost))
                    {
                        throw new InvalidOperationException("HealthModelsHost configuration is missing");
                    }

                    // Make the API call to get entity history
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
                    var apiUrl = $"{healthModelsHost}/api/entities/{HttpUtility.UrlEncode(entityName)}/history?api-version=2023-05-15-preview";
                    var apiResponse = await _httpClient.GetAsync(apiUrl);
                    apiResponse.EnsureSuccessStatusCode();

                    var jsonContent = await apiResponse.Content.ReadAsStringAsync();
                    response = JsonSerializer.Deserialize<EntityHistory>(jsonContent) ?? throw new InvalidOperationException();
                    
                    // Map Error states to Unhealthy for consistency
                    foreach (var transition in response.History.Transitions)
                    {
                        transition.PreviousState = StatusUtils.NormalizeStatus(transition.PreviousState);
                        transition.NewState = StatusUtils.NormalizeStatus(transition.NewState);
                    }
                }

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing entity history request");
                return new StatusCodeResult(500);
            }
        }
    }
}

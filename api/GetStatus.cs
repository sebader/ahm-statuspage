using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Api.Models;

namespace api
{
    public class GetStatus
    {
        private readonly ILogger<GetStatus> _logger;
        private readonly IConfiguration _config;
        private readonly string _sampleResponsePath;
        private readonly HttpClient _httpClient;
        private readonly bool _isLocalEnvironment;

        public GetStatus(ILogger<GetStatus> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _sampleResponsePath = "sample_response_from_healthengine.json";
            _httpClient = new HttpClient();
            _isLocalEnvironment = string.IsNullOrEmpty(Environment.GetEnvironmentVariable("WEBSITE_INSTANCE_ID")); // Azure-specific environment variable
        }

        [Function("GetStatus")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("Processing status request");

            try
            {
                // Read entity names from config
                var entityNamesConfig = _config["EntityNames"] ?? "";
                var entityNames = entityNamesConfig.Split(',', StringSplitOptions.RemoveEmptyEntries);

                if (entityNames.Length == 0)
                {
                    return new BadRequestObjectResult("No entity names configured");
                }

                HealthEngineResponse response;
                
                if (_isLocalEnvironment)
                {
                    // Use sample data for local development
                    var jsonContent = System.IO.File.ReadAllText(_sampleResponsePath);
                    response = JsonSerializer.Deserialize<HealthEngineResponse>(jsonContent);
                }
                else
                {
                    // Get token using managed identity
                    var credential = new DefaultAzureCredential();
                    var token = await credential.GetTokenAsync(new TokenRequestContext(new[] { "https://data.healthmodels.azure.com/.default" }));

                    // Get the host from configuration
                    var healthModelsHost = _config["HealthModelsHost"];
                    if (string.IsNullOrEmpty(healthModelsHost))
                    {
                        throw new InvalidOperationException("HealthModelsHost configuration is missing");
                    }

                    // Make the API call
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
                    var apiUrl = $"{healthModelsHost}/api/views/default/v2/query";
                    var apiResponse = await _httpClient.GetAsync(apiUrl);
                    apiResponse.EnsureSuccessStatusCode();

                    var jsonContent = await apiResponse.Content.ReadAsStringAsync();
                    response = JsonSerializer.Deserialize<HealthEngineResponse>(jsonContent);
                }

                if (response?.healthModel?.entities == null)
                {
                    return new StatusCodeResult(500);
                }

                // Filter and map entities to ComponentStatus
                var statuses = response.healthModel.entities
                    .Where(e => entityNames.Contains(e.name))
                    .Select(e => new ComponentStatus
                    {
                        Name = e.displayName,
                        Status = e.state,
                        Description = $"{e.kind} - Impact: {e.impact}",
                        LastUpdated = DateTime.Parse(e.lastTransitionTimeUtc)
                    })
                    .ToList();

                return new OkObjectResult(statuses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing status request");
                return new StatusCodeResult(500);
            }
        }
    }
}

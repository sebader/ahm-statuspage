using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
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

        public GetStatus(ILogger<GetStatus> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _sampleResponsePath = "sample_response_from_healthengine.json";
        }

        [Function("GetStatus")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
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

                // Read and parse the health engine response
                var jsonContent = System.IO.File.ReadAllText(_sampleResponsePath);
                var response = JsonSerializer.Deserialize<HealthEngineResponse>(jsonContent);

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

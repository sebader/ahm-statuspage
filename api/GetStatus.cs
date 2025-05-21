using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using api.Services;

namespace api
{
    public class GetStatus
    {
        private readonly ILogger<GetStatus> _logger;
        private readonly IConfiguration _config;
        private readonly IHealthModelService _healthModelService;

        public GetStatus(ILogger<GetStatus> logger, IConfiguration config, IHealthModelService healthModelService)
        {
            _logger = logger;
            _config = config;
            _healthModelService = healthModelService;
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

                var statuses = await _healthModelService.GetStatusAsync(entityNames);

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

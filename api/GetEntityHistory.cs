using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using api.Services;

namespace api
{
    public class GetEntityHistory
    {
        private readonly ILogger<GetEntityHistory> _logger;
        private readonly IHealthModelService _healthModelService;

        public GetEntityHistory(ILogger<GetEntityHistory> logger, IHealthModelService healthModelService)
        {
            _logger = logger;
            _healthModelService = healthModelService;
        }

        [Function("GetEntityHistory")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "history/{entityName}")] HttpRequest req, string entityName)
        {
            _logger.LogInformation("Processing entity history request for {EntityName}", entityName);

            try
            {

                var response = await _healthModelService.GetEntityHistoryAsync(entityName);
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

using System;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Api.Models;

namespace api
{
    public class GetStatus
    {
        private readonly ILogger<GetStatus> _logger;

        public GetStatus(ILogger<GetStatus> logger)
        {
            _logger = logger;
        }

        [Function("GetStatus")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            _logger.LogInformation("Processing status request");

            var mockStatuses = new List<ComponentStatus>
            {
                new ComponentStatus 
                { 
                    Name = "Authentication Service",
                    Status = "Operational",
                    Description = "All authentication systems are functioning normally",
                    LastUpdated = DateTime.UtcNow
                },
                new ComponentStatus 
                { 
                    Name = "Payment Processing",
                    Status = "Degraded",
                    Description = "Experiencing slower than normal processing times",
                    LastUpdated = DateTime.UtcNow
                },
                new ComponentStatus 
                { 
                    Name = "User Database",
                    Status = "Operational",
                    Description = "Database systems operating at normal capacity",
                    LastUpdated = DateTime.UtcNow
                },
                new ComponentStatus 
                { 
                    Name = "Email Service",
                    Status = "Outage",
                    Description = "Service currently unavailable - Engineering team investigating",
                    LastUpdated = DateTime.UtcNow
                }
            };

            return new OkObjectResult(mockStatuses);
        }
    }
}

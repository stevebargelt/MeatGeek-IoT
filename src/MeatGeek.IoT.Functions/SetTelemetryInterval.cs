using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace MeatGeek.IoT
{

    public static class SetTelemetryInterval
    {
        private static IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Environment.CurrentDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();
        
        // AppInsights TelemetryClient
        private static TelemetryClient telemetry = new TelemetryClient();                
        private static ServiceClient _iothubServiceClient = ServiceClient.CreateFromConnectionString(Environment.GetEnvironmentVariable("MeatGeekIoTServiceConnection", EnvironmentVariableTarget.Process));
        private const string METHOD_NAME = "TelemetryInterval";
        private const string MODULE_NAME = "Telemetry";

        [FunctionName("telemetryinterval")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "telemetryinterval/{device}")][FromBody] string value, 
            string device,
            ILogger log)
        {
            log.LogInformation($"telemetryinterval function executed at: {DateTime.Now}");

            if (string.IsNullOrEmpty(value)) // TODO: Check value for vaild range... 180 - 400 or whatever.
            {
                log.LogInformation("Request Body value IsNullOrEmpty. Returning BadRequest = Missing body value. Body should be a single integer.");
                return new BadRequestObjectResult("Missing body value. Body should be a single integer.");
            }
            log.LogInformation("Request Body value = " + value);
            var methodRequest = new CloudToDeviceMethod(METHOD_NAME, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(15));

            // Generate a Guid as the correlationId which we use to track the message through the pipeline
            var correlationId = Guid.NewGuid().ToString();

            methodRequest.SetPayloadJson(value);

            var telemetryProperties = new Dictionary<string, string>
            {
                { "correlationId", correlationId },
                { "processingStep", "1-telemetryinterval"}
            };
            
            telemetry.TrackEvent("10-StartMethodInvocation", telemetryProperties);

            try
            {
                log.LogInformation($"Invoking method telemetryinterval on module {device}/{MODULE_NAME}. CorrelationId={correlationId}");
                // Invoke direct method
                var result = await _iothubServiceClient.InvokeDeviceMethodAsync(device, MODULE_NAME, methodRequest).ConfigureAwait(false);

                telemetryProperties.Add("MethodReturnCode", $"{result.Status}");
                if (IsSuccessStatusCode(result.Status))
                {
                    telemetry.TrackEvent("11-SuccessfulMethodInvocation", telemetryProperties);
                    log.LogInformation($"[{device}/{MODULE_NAME}] Successful direct method call result code={result.Status}");
                    
                }
                else
                {
                    telemetry.TrackEvent("15-UnsuccessfulMethodInvocation", telemetryProperties);
                    log.LogWarning($"[{device}/{MODULE_NAME}] Unsuccessful direct method call result code={result.Status}");
                }
                return new ObjectResult(result);
            }
            catch (Exception e)
            {
                telemetryProperties.Add("methodInvocationException", e.Message);
                telemetry.TrackEvent("16-ExceptionInMethodInvocation", telemetryProperties);
                log.LogError(e, $"[{device}/{MODULE_NAME}] Exeception on direct method call");
                return new BadRequestObjectResult("Exception was caught in function app.");
            }
        }
        private static bool IsSuccessStatusCode(int statusCode)
        {
            return (statusCode >= 200) && (statusCode <= 299);
        }

    }
}



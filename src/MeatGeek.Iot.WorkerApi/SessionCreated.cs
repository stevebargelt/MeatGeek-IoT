using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Devices;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using MeatGeek.Shared;
using MeatGeek.Shared.EventSchemas.Sessions;
//using MeatGeek.IoT.WorkerApi.Configurations;

namespace MeatGeek.IoT.WorkerApi
{
    public class SessionCreatedTrigger
    {

        private static ILogger<SessionCreatedTrigger> _log;

        public SessionCreatedTrigger(ILogger<SessionCreatedTrigger> log) {
            _log = log;
        }

        private static ServiceClient _iothubServiceClient = ServiceClient.CreateFromConnectionString(Environment.GetEnvironmentVariable("MeatGeekIoTServiceConnection", EnvironmentVariableTarget.Process));
        private const string METHOD_NAME = "SessionCreated";
        private const string MODULE_NAME = "Telemetry";

        [FunctionName("SessionCreated")]
        public static async Task Run(
            [EventGridTrigger]EventGridEvent eventGridEvent)
        {
            _log.LogInformation("SessionCreated Called");
            _log.LogInformation(eventGridEvent.Data.ToString());
            
            try
            {
                SessionCreatedEventData sessionCreatedEventData;

                if (eventGridEvent.Data is SessionCreatedEventData)
                {
                    sessionCreatedEventData = (SessionCreatedEventData)eventGridEvent.Data;
                    _log.LogInformation("SessionCreated Event Grid Trigger: Event Grid Data --> SessionCreatedEventData");
                }
                else
                {
                    _log.LogInformation("SessionCreated Event Grid Trigger: Event Grid Data is not in expected format.");
                    throw new InvalidOperationException("Event Grid Data is not in expected format.");
                }
                var smokerId = sessionCreatedEventData.SmokerId;
                var sessionId = sessionCreatedEventData.Id;
                _log.LogInformation("SmokerID = " + smokerId);
                _log.LogInformation("SessionID = " + sessionId);
                var methodRequest = new CloudToDeviceMethod(METHOD_NAME, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(15));
                methodRequest.SetPayloadJson(sessionId);

                try
                {
                    _log.LogInformation($"Invoking method telemetryinterval on module {smokerId}/{MODULE_NAME}.");
                    // Invoke direct method
                    var result = await _iothubServiceClient.InvokeDeviceMethodAsync(smokerId, MODULE_NAME, methodRequest).ConfigureAwait(false);

                    if (IsSuccessStatusCode(result.Status))
                    {
                        _log.LogInformation($"[{smokerId}/{MODULE_NAME}] Successful direct method call result code={result.Status}");
                        
                    }
                    else
                    {
                        _log.LogWarning($"[{smokerId}/{MODULE_NAME}] Unsuccessful direct method call result code={result.Status}");
                    }
                    //return new ObjectResult(result);
                }
                catch (Exception e)
                {
                    _log.LogError(e, $"[{smokerId}/{MODULE_NAME}] Exeception on direct method call");
                    //return new BadRequestObjectResult("Exception was caught in function app.");
                }               

            }
            catch (Exception ex)
            {
                _log.LogError(ex, "<-- SessionCreated Event Grid Trigger: Unhandled exception");
                return new BadRequestObjectResult("SessionCreated: Unhandled Exception in function app.");
            }
        }
        
        private static bool IsSuccessStatusCode(int statusCode)
        {
            return (statusCode >= 200) && (statusCode <= 299);
        }        
    }
}

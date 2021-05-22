using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Extensions.Logging;
using MeatGeek.Shared;
using MeatGeek.Shared.EventSchemas.Sessions;

namespace MeatGeek.IoT.WorkerApi
{
    public static class SessionCreated
    {
        private const string METHOD_NAME = "SessionCreated";
        private const string MODULE_NAME = "";

        [FunctionName("SessionCreated")]
        public static void Run(
            [EventGridTrigger]EventGridEvent eventGridEvent,
            ILogger log)
        {
            log.LogInformation("SessionCreated Called");
            log.LogInformation(eventGridEvent.Data.ToString());
            
            try
            {
                SessionCreatedEventData sessionCreatedEventData;

                if (eventGridEvent.Data is SessionCreatedEventData)
                {
                    sessionCreatedEventData = (SessionCreatedEventData)eventGridEvent.Data;
                }
                else
                {
                    log.LogInformation("SessionCreated Event Grid Trigger: Event Grid Data is not in expected format.");
                    throw new InvalidOperationException("Event Grid Data is not in expected format.");
                }
                var smokerId = sessionCreatedEventData.SmokerId;
                var sessionId = sessionCreatedEventData.Id;
                log.LogInformation("SmokerID = " + smokerId);
                log.LogInformation("SessionID = " + sessionId);

            }
            catch (Exception ex)
            {
                log.LogError(ex, "<-- SessionCreated Event Grid Trigger: Unhandled exception");
                
            }
        }

        
    }
}

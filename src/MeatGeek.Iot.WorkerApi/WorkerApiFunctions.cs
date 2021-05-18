using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Extensions.Logging;

using MeatGeek.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;

namespace MeatGeek.IoT.WorkerApi
{
    public static class WorkerApiFunctions
    {
        private static readonly IEventGridSubscriberService EventGridSubscriberService = new EventGridSubscriberService();
        //private static readonly ICategoriesService CategoriesService = new CategoriesService(new CategoriesRepository(), new ImageSearchService(new Random(), new HttpClient()), new SynonymService(new HttpClient()), new EventGridPublisherService());

        [FunctionName("AddSession")]
        public static async Task<IActionResult> AddSession(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post")]HttpRequest req,
            ILogger log)
        {
            log.LogInformation("AddSession Called");

            // authenticate to Event Grid if this is a validation event
            var eventGridValidationOutput = EventGridSubscriberService.HandleSubscriptionValidationEvent(req);
            if (eventGridValidationOutput != null)
            {
                log.LogInformation("Responding to Event Grid subscription verification.");
                return eventGridValidationOutput;
            }
            
            try
            {
                var (eventGridEvent, smokerId, sessionId) = EventGridSubscriberService.DeconstructEventGridMessage(req);
                
                // process the category item
                //await CategoriesService.ProcessAddItemEventAsync(eventGridEvent, uId);
                log.LogInformation("this worked, I think!!!!!!!");
                log.LogInformation("SessionID = " + sessionId);

                return new OkResult();
            }
            catch (Exception ex)
            {
                log.LogError("Unhandled exception", ex);
                return new ExceptionResult(ex, false);
            }
        }

        
    }
}

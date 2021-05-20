using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Extensions.Logging;

using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;

// using MeatGeek.Shared;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Host;

namespace MeatGeek.IoT.WorkerApi
{
    public static class WorkerApiFunctions
    {
        //private static readonly IEventGridSubscriberService EventGridSubscriberService = new EventGridSubscriberService();
        //private static readonly ICategoriesService CategoriesService = new CategoriesService(new CategoriesRepository(), new ImageSearchService(new Random(), new HttpClient()), new SynonymService(new HttpClient()), new EventGridPublisherService());

        [FunctionName("SessionCreated")]
        public static async Task<IActionResult> SessionCreated(
            [EventGridTrigger]EventGridEvent eventGridEvent,
            ILogger log)
        {
        
            log.LogInformation("SessionCreated Called");
            log.LogInformation(eventGridEvent.Data.ToString());
            
            try
            {
                //var (eventGridEvent, smokerId, sessionId) = EventGridSubscriberService.DeconstructEventGridMessage(req);
                
                // process the category item
                //await CategoriesService.ProcessAddItemEventAsync(eventGridEvent, uId);
                log.LogInformation("this worked, I think!!!!!!!");
                //log.LogInformation("SessionID = " + sessionId);

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

using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventGrid.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.EventGrid;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

// using MeatGeek.Shared;


namespace MeatGeek.IoT.WorkerApi
{
    public static class SessionCreated
    {
        //private static readonly IEventGridSubscriberService EventGridSubscriberService = new EventGridSubscriberService();
        //private static readonly ICategoriesService CategoriesService = new CategoriesService(new CategoriesRepository(), new ImageSearchService(new Random(), new HttpClient()), new SynonymService(new HttpClient()), new EventGridPublisherService());

        [FunctionName("SessionCreated")]
        public static void Run(
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

            }
            catch (Exception ex)
            {
                log.LogError(ex, "<-- SessionCreated Event Grid Trigger: Unhandled exception");
                
            }
        }

        
    }
}

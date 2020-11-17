using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using MeatGeek.IoT.Models;

namespace MeatGeek.IoT
{
    public static class MeatGeekIoTHubTriggerTest
    {
        [FunctionName("MeatGeekIoTHubTriggerTest")]
        public static void Run([IoTHubTrigger("messages/events", Connection = "IoTHubConnection")]EventData message, ILogger log)
        {
            log.LogInformation($"MeatGeekIoTHubTriggerTest received a message: {Encoding.UTF8.GetString(message.Body.Array)}");
        
        }
    }

}
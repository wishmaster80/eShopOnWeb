using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FunctionApp
{
    public static class OrderItemsReserver
    {
        private static string _connectionString = "DefaultEndpointsProtocol=https;AccountName=ordersadamstorage;AccountKey=D9Uehfk2FBZXmAzz/7arVDti2ucspQr8nVI+NBukuhUfmijbulKUM6tkAvSgg6rUIHoeIM9YOIbu+ASt41SStA==;EndpointSuffix=core.windows.net";
        private static BlobContainerClient _containerClient;

        //private static string _connectionString = "DefaultEndpointsProtocol=https;AccountName=functionapp2023121810121;AccountKey=CltTwYwj5ewgtpl6o8LjEa2NH7DSiStYuBxkhkHgiQzDeidU0Edk61tjqE80YzVOd0RfEZnTxz9L+AStb7oafg==;EndpointSuffix=core.windows.net";
        public static void Initialize()
        {
            List<string> names = new List<string>();
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient("orders");
        }


        public static async Task<IEnumerable<string>> GetNames()
        {
            Initialize();

            try
            {
                List<string> names = new List<string>();
                //BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
                //BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("orders");
                AsyncPageable<BlobItem> blobs = _containerClient.GetBlobsAsync();

                await foreach (var blob in blobs)
                {
                    names.Add(blob.Name);
                }
                return names;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static async Task<string> UploadFile(string body)
        {
            Initialize();
            var content = Encoding.UTF8.GetBytes(body);
            var fileName = $"Order{DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss")}.json";
            using var ms = new MemoryStream(content);
            {
                await _containerClient.UploadBlobAsync(fileName, ms);
            }
            return fileName;

        }
        // order-item-storage
        // https://functionapp2023121810121.blob.core.windows.net/scm-releases
        // ordersadamstorage
        // ordersadamstorage_1702897618664
        [FunctionName("OrderItemsReserver")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
        ILogger log)
        {
            //var x = (await GetNames()).ToList();
            log.LogInformation("C# HTTP trigger function processed a request.");

            //string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

            string responseMessage = await UploadFile(requestBody);

            return new OkObjectResult(responseMessage);
        }
    }
}

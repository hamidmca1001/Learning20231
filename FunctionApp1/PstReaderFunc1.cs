using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using Microsoft.Office.Interop.Outlook;
using System.Runtime.InteropServices;

namespace FunctionApp1
{
    public  class PstReaderFunc1
    {
        [FunctionName("PstReaderFunc1")]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                // TOdo use configuration
                string connectionString = "DefaultEndpointsProtocol=https;AccountName=solutionwebstorage;AccountKey=/g1x4TZg9GsgBNhwRWbKlaDfOG6ix+Ek+24J7cBiXHsrNaFxyroepsRnNQgN10xF5MM0fqTuZpJK+AStzrEDzA==;EndpointSuffix=core.windows.net";
                string containerName = "solutionwebcontainer";
                string blobName = @"PstFileData\backupMyN1.pst";

                ReadPstFromAzureBlob(connectionString, containerName, blobName, log);
            }

            catch (System.Exception ex)
            {
                log.LogInformation($"Exception throw : {ex.Message}.");
                return new BadRequestResult();
            }

            return new OkObjectResult("Data Exported To CSV file Successfully");
        }


        #region Private Work with AzureBlob
        static void ReadPstFromAzureBlob(string connectionString, string containerName, string blobName, ILogger log)
        {
            log.LogInformation($"ReadPstFromAzureBlob Start.");
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            log.LogInformation($"blobClient Processed.");

            using (MemoryStream memoryStream = new MemoryStream())
            {
                log.LogInformation($"DownloadTo Start.");
                blobClient.DownloadTo(memoryStream);
                memoryStream.Position = 0;
                log.LogInformation($"DownloadTo Start.");

                // Process the .pst file using the appropriate library or method
                ProcessPst(memoryStream, log);
            }
            log.LogInformation($"ReadPstFromAzureBlob End.");
        }


        static void ProcessPst(Stream stream, ILogger log)
        {
            log.LogInformation($"ProcessPst Start.");
            Application application = new Application();
            NameSpace outlookNamespace = application.GetNamespace("MAPI");

            // Create a temporary .pst file path
            string tempPstFilePath = Path.GetTempFileName() + ".pst";

            // Save the .pst file contents from the stream to the temporary file
            using (FileStream fileStream = System.IO.File.Create(tempPstFilePath))
            {
                stream.CopyTo(fileStream);
            }

            // Open the .pst file
            log.LogInformation($"AddStore Start.");
            outlookNamespace.AddStore(tempPstFilePath);
            MAPIFolder rootFolder = outlookNamespace.GetDefaultFolder(OlDefaultFolders.olFolderInbox);
            log.LogInformation($"AddStore End.");

            // Csv File Path
            var dir1 = Directory.GetCurrentDirectory();
            string csvFilePath = Path.GetTempFileName() + ".csv";
            var csvFileName = Path.GetFileName(csvFilePath);
            using (FileStream fileStream = System.IO.File.Create(csvFilePath))
            {
                stream.CopyTo(fileStream);
            }

            // Call a recursive function to process all the folders and items
            using (StreamWriter writer = new StreamWriter(csvFilePath))
            {
                writer.WriteLine("Subject,Sender, Receiver, ReceivedTime, Body");
                // Process the folders and messages
                ProcessFolder(rootFolder, writer);
            }

            // Save Csv File in Blob Storage
            log.LogInformation($"UploadFileInBlobStorage Start.");
            UploadFileInBlobStorage(csvFilePath, csvFileName);
            log.LogInformation($"UploadFileInBlobStorage End.");



            // Release COM objects
            Marshal.ReleaseComObject(rootFolder);
            Marshal.ReleaseComObject(outlookNamespace);
            Marshal.ReleaseComObject(application);
        }



        private static void UploadFileInBlobStorage(string csvFilePath, string csvFileName)
        {
            // Upload file

            // TOdo use configuration
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=solutionwebstorage;AccountKey=/g1x4TZg9GsgBNhwRWbKlaDfOG6ix+Ek+24J7cBiXHsrNaFxyroepsRnNQgN10xF5MM0fqTuZpJK+AStzrEDzA==;EndpointSuffix=core.windows.net";
            string containerName = "solutionwebcontainer";
            string filePath = csvFilePath;
            string blobName = @"CsvFileMailData\" + csvFileName;
            UploadPstToContainer(connectionString, containerName, filePath, blobName);
            Console.WriteLine("Csv file uploaded successfully.");
        }

        static void ProcessFolder(MAPIFolder folder, StreamWriter writer)
        {
            // Process the messages in the current folder
            Items items = folder.Items;
            foreach (object item in items)
            {
                if (item is MailItem mailItem)
                {
                    // Process mail item properties
                    writer.WriteLine($"{mailItem.Subject}, {mailItem.SenderEmailAddress},{mailItem.ReceivedByName}, {mailItem.ReceivedTime}, {mailItem.Body}");
                }
            }

            // Recursively process sub-folders
            Folders subFolders = folder.Folders;
            foreach (MAPIFolder subFolder in subFolders)
            {
                ProcessFolder(subFolder, writer);
            }

            // Release COM objects
            Marshal.ReleaseComObject(items);
            Marshal.ReleaseComObject(subFolders);
        }

        /// <summary>
        /// https://solutionwebstorage.blob.core.windows.net/solutionwebcontainer/
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="containerName"></param>
        /// <param name="filePath"></param>
        /// <param name="blobName"></param>
        static void UploadPstToContainer(string connectionString, string containerName, string filePath, string blobName)
        {
            try
            {
                BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
                using (FileStream fileStream = System.IO.File.OpenRead(filePath))
                {
                    containerClient.UploadBlob(blobName, fileStream);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion





       
    }
}



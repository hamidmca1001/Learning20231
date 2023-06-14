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
using Independentsoft.Pst;
using System.Text;

namespace FunctionApp1
{
    public  class FileReaderFunc
    {
        [FunctionName("FileReaderFunc")]
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

        static void ReadPstFromAzureBlob(string connectionString, string containerName, string blobName, ILogger log)
        {
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            using (MemoryStream memoryStream = new MemoryStream())
            {
                blobClient.DownloadTo(memoryStream);
                memoryStream.Position = 0;

                // Process the .pst file using the appropriate library or method
                ProcessPst(memoryStream, log);




                //// Create a StreamReader to read the data from the MemoryStream
                //using (StreamReader reader = new StreamReader(memoryStream, Encoding.UTF8))
                //{
                //    // Read the entire content of the MemoryStream and convert it to a string
                //    string content = reader.ReadToEnd();

                //    // Use the converted string as needed
                //    Console.WriteLine(content);
                //}
            }


        }

        static void ProcessPst(Stream stream, ILogger log)
        {


            // Create a temporary .pst file path
            string tempPstFilePath = Path.GetTempFileName() + ".pst";

            // Save the .pst file contents from the stream to the temporary file
            using (FileStream fileStream = System.IO.File.Create(tempPstFilePath))
            {
                stream.CopyTo(fileStream);
            }

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
                log.LogInformation($"ReadPstFile Start");
                ReadPstFile(tempPstFilePath, writer, log);
                log.LogInformation($"ReadPstFile End");

                //ProcessFolder(rootFolder, writer);
            }

            // Save Csv File in Blob Storage
            log.LogInformation($"UploadFileInBlobStorage Start.");
            UploadFileInBlobStorage(csvFilePath, csvFileName);
            log.LogInformation($"UploadFileInBlobStorage End.");

        }



        private static void ReadPstFile(string pstFilePath, StreamWriter writer, ILogger log)
        {
           // NewMethod(pstFilePath);

            using (PstFile pstFile = new PstFile(pstFilePath))
            {
                log.LogInformation($"ProcessFolder Start");
                ProcessFolder(pstFile.Root, writer, log);
                log.LogInformation($"ProcessFolder End");
            }
        }

        private static void NewMethod(string pstFilePath)
        {
            string csvFilePath = @"D:\HclOfficeProject\file1.csv";
            // Open the .pst file for reading
            using (FileStream pstStream = new FileStream(pstFilePath, FileMode.Open, FileAccess.Read))
            {
                // Create a reader to read from the .pst file
                using (BinaryReader pstReader = new BinaryReader(pstStream))
                {
                    // Open the .csv file for writing
                    using (StreamWriter csvWriter = new StreamWriter(csvFilePath))
                    {
                        csvWriter.WriteLine("Subject,Sender,Receiver,Date"); // Write the header row to the .csv file

                        // Read the .pst file data and process each message
                        while (pstReader.BaseStream.Position < pstReader.BaseStream.Length)
                        {
                            // Read the fields of the message from the .pst file
                            string subject = ReadStringFromPST(pstReader);
                            string sender = ReadStringFromPST(pstReader);
                            string receiver = ReadStringFromPST(pstReader);
                            string dateString = ReadStringFromPST(pstReader);
                            DateTime date = DateTime.Parse(dateString);

                            // Write the fields to the .csv file
                            csvWriter.WriteLine($"{subject},{sender},{receiver},{date}");
                        }
                    }
                }
            }
        }

        private static void ProcessFolder(Folder folder, StreamWriter writer, ILogger log)
        {

            log.LogInformation($"ProcessFolder Start");
            if (folder.ItemCount > 0)
            {
                var items = folder.GetItems();
                foreach (Item item in items)
                {
                    log.LogInformation($"Subject: {item.Subject}");
                    // Process mail item properties
                    writer.WriteLine($"{item.Subject}, {item.SenderEmailAddress},{item.ReceivedByName}, {item.MessageDeliveryTime}, {item.Body}");

                    //if (item is MailItem message)
                    //{
                    //    Console.WriteLine($"item is MailItem");
                    //}

                    //Console.WriteLine($"Subject: {item.Subject}");
                    //Console.WriteLine($"Sender: {item.SenderName}");
                    //Console.WriteLine($"Received: {item.MessageDeliveryTime}");
                    ////Console.WriteLine($"Received: {message.ReceivedTime}");
                    //Console.WriteLine($"Body: {item.Body}");
                    ////Console.WriteLine($"Body: {message.BodyPlainText}");
                    //Console.WriteLine("-----------------------------------------");

                }
            }

            if (folder.HasSubFolders)
            {
                var subF = folder.GetFolders();
                foreach (Folder subfolder in subF)
                {
                    ProcessFolder(subfolder, writer, log);
                }
            }
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




        /// 14th June analysis
        /// 

        // Helper method to read a string from the .pst file
        public static string ReadStringFromPST(BinaryReader reader)
        {
            int length = reader.ReadInt32();
            byte[] bytes = reader.ReadBytes(length);
            var content = System.Text.Encoding.UTF8.GetString(bytes);

            return content;
        }





    }
}


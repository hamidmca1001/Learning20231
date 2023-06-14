using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Independentsoft.Pst;
using Microsoft.Office.Interop.Outlook;
using Azure.Storage.Blobs;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PstFileController : ControllerBase
    {
        #region A1: using Independentsoft
        [HttpGet]
        [Route("ReadPstFile1")]
        public async Task<IActionResult> ReadPstFile1()
        {
            var response = "";

            string pstFilePath = @"D:\HclOfficeProject\MapiNoteToPST_out.pst";
            ReadPstFile(pstFilePath);
            return Ok(response);
        }



        private static void ReadPstFile(string pstFilePath)
        {
            using (PstFile pstFile = new PstFile(pstFilePath))
            {
                ProcessFolder(pstFile.Root);
            }
        }

        private static void ProcessFolder(Independentsoft.Pst.Folder folder)
        {
            if (folder.ItemCount > 0)
            {
                var items = folder.GetItems();
                foreach (Item item in items)
                {
                    if (item is MailItem message)
                    {
                        Console.WriteLine($"item is MailItem");
                    }

                    Console.WriteLine($"Subject: {item.Subject}");
                    Console.WriteLine($"Sender: {item.SenderName}");
                    Console.WriteLine($"Received: {item.MessageDeliveryTime}");
                    //Console.WriteLine($"Received: {message.ReceivedTime}");
                    Console.WriteLine($"Body: {item.Body}");
                    //Console.WriteLine($"Body: {message.BodyPlainText}");
                    Console.WriteLine("-----------------------------------------");

                    //if (item is Message message)
                    //{
                    //    Console.WriteLine($"Subject: {message.Subject}");
                    //    Console.WriteLine($"Sender: {message.SenderName}");
                    //    Console.WriteLine($"Received: {message.MessageDeliveryTime}");
                    //    //Console.WriteLine($"Received: {message.ReceivedTime}");
                    //    Console.WriteLine($"Body: {message.Body}");
                    //    //Console.WriteLine($"Body: {message.BodyPlainText}");
                    //    Console.WriteLine("-----------------------------------------");
                    //}
                }
            }

            if (folder.HasSubFolders)
            {
                var subF = folder.GetFolders();
                foreach (Independentsoft.Pst.Folder subfolder in subF)
                {
                    ProcessFolder(subfolder);
                }
                //foreach (Folder subfolder in folder.)
                ////foreach (Folder subfolder in folder.Subfolders)
                //{
                //    ProcessFolder(subfolder);
                //}
            }
        }

        #endregion

        #region  A1-Write PST Response in Console
        [HttpGet]
        [Route("ReadPstFileA1")]
        public async Task<IActionResult> ReadPstFileA1()
        {
            var response = "";

            try
            {
                string pstFilePath = @"D:\HclOfficeProject\MapiNoteToPST_out.pst";
                //@"C:\Users\hamidali.h\OneDrive - HCL Technologies Ltd\Documents\Outlook Files\backup.pst";
                //@"C:\MyCustomDoc\Newfolder\backupN123.pst";
                ReadPstFileA1(pstFilePath);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Ok(response);
        }



        private void ReadPstFileA1(string pstFilePath)
        {
            Application outlookApp = new Application();
            NameSpace outlookNamespace = outlookApp.GetNamespace("MAPI");

            // Add the .pst file to the Outlook profile
            outlookNamespace.AddStore(pstFilePath);


            // Get the root folder of the .pst file
            //Folder rootFolder = outlookNamespace.Folders.GetLast();
            var rootFolder = outlookNamespace.Folders.GetLast();


            // Call a recursive function to process all the folders and items
            ProcessFolderA1(rootFolder);
        }


        private void ProcessFolderA1(MAPIFolder folder)
        {

            // Process items in the current folder
            Items items = folder.Items;
            foreach (object itemObj in items)
            {
                if (itemObj is MailItem mailItem)
                {



                    // Process mail item properties
                    Console.WriteLine($"Subject: {mailItem.Subject}");
                    Console.WriteLine($"Sender: {mailItem.SenderEmailAddress}");
                    Console.WriteLine($"Received: {mailItem.ReceivedTime}");
                    Console.WriteLine($"Body: {mailItem.Body}");
                    Console.WriteLine("-----------------------------------------");
                }
            }





            // Recursively process subfolders
            Folders subfolders = folder.Folders;
            foreach (Microsoft.Office.Interop.Outlook.Folder subfolder in subfolders)
            {
                ProcessFolderA1(subfolder);
            }
        }



        #endregion


        #region A3 Using Blog+Independentsoft
        [HttpGet]
        [Route("ReadPstFileBlogIndependentsoft")]
        public async Task<IActionResult> ReadPstFileBlogIndependentsoft()
        {
            var response = "";

            try
            {
                // TOdo use configuration
                string connectionString = "DefaultEndpointsProtocol=https;AccountName=solutionwebstorage;AccountKey=/g1x4TZg9GsgBNhwRWbKlaDfOG6ix+Ek+24J7cBiXHsrNaFxyroepsRnNQgN10xF5MM0fqTuZpJK+AStzrEDzA==;EndpointSuffix=core.windows.net";
                string containerName = "solutionwebcontainer";
                string blobName = @"PstFileData\backupMyN1.pst";

                ReadPstFromAzureBlob(connectionString, containerName, blobName);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
               
            }
           

            return Ok(response);
        }

        static void ReadPstFromAzureBlob(string connectionString, string containerName, string blobName)
        {

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);


            using (MemoryStream memoryStream = new MemoryStream())
            {

                blobClient.DownloadTo(memoryStream);
                memoryStream.Position = 0;


                // Process the .pst file using the appropriate library or method
                ProcessPst(memoryStream);
            }

        }

        static void ProcessPst(Stream stream)
        {
            // Create a temporary .pst file path
            string tempPstFilePath = Path.GetTempFileName() + ".pst";

            // Save the .pst file contents from the stream to the temporary file
            using (FileStream fileStream = System.IO.File.Create(tempPstFilePath))
            {
                stream.CopyTo(fileStream);
            }

            ReadPstFile11(tempPstFilePath);


        }

        private static void ReadPstFile11(string pstFilePath)
        {
            using (PstFile pstFile = new PstFile(pstFilePath))
            {
                ProcessFolder(pstFile.Root);
            }
        }

        private static void ProcessFolder11(Independentsoft.Pst.Folder folder)
        {
            if (folder.ItemCount > 0)
            {
                var items = folder.GetItems();
                foreach (Item item in items)
                {
                    if (item is MailItem message)
                    {
                        Console.WriteLine($"item is MailItem");
                    }

                    Console.WriteLine($"Subject: {item.Subject}");
                    Console.WriteLine($"Sender: {item.SenderName}");
                    Console.WriteLine($"Received: {item.MessageDeliveryTime}");
                    //Console.WriteLine($"Received: {message.ReceivedTime}");
                    Console.WriteLine($"Body: {item.Body}");
                    //Console.WriteLine($"Body: {message.BodyPlainText}");
                    Console.WriteLine("-----------------------------------------");


                }
            }

            if (folder.HasSubFolders)
            {
                var subF = folder.GetFolders();
                foreach (Independentsoft.Pst.Folder subfolder in subF)
                {
                    ProcessFolder11(subfolder);
                }

            }
        }


        #endregion



    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.IO;
using System.Text;
using Microsoft.Office.Interop.Outlook;
using Aspose.Email.Mapi;
using System;
using System.IO;
using System.Reflection;
using Aspose.Email.Storage.Pst;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileReaderController : ControllerBase
    {

        /// <summary>
        /// Read Pst File
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("ReadPstFile")]
        public async Task<IActionResult> ReadPstFile()
        {
            var response = "";

            string pstFilePath = @"D:\HclOfficeProject\MapiNoteToPST_out.pst";
            string csvFilePath = @"D:\HclOfficeProject\file1.csv";

            ConvertPstToCsv(pstFilePath, csvFilePath);




            return Ok(response);
        }


        private static void ConvertPstToCsv(string pstFilePath, string csvFilePath, string password = null)
        {
            using (PersonalStorage pst = PersonalStorage.FromFile(pstFilePath))
            {
                FolderInfo folderInfo = pst.RootFolder;
                ExtractMessages(folderInfo, csvFilePath);
            }
        }

        private static void ExtractMessages(FolderInfo folderInfo, string csvFilePath)
        {
            using (StreamWriter writer = new StreamWriter(csvFilePath))
            {
                writer.WriteLine("Subject,Sender,Received");

                ExtractMessagesRecursive(folderInfo, writer);
            }
        }

        private static void ExtractMessagesRecursive(FolderInfo folderInfo, StreamWriter writer)
        {
            foreach (var messageInfo in folderInfo.EnumerateMessages())
            {
                var message = folderInfo.GetContents(); // .GetMessage(messageInfo.EntryId);
                string subject = message.First().Subject;
                // string sender = message.From.Address;
                // DateTime received = message.Date;

                writer.WriteLine($"{subject},");

                //writer.WriteLine($"{subject},{sender},{received}");
            }

            foreach (FolderInfo subFolder in folderInfo.GetSubFolders())
            {
                ExtractMessagesRecursive(subFolder, writer);
            }
        }




       // private static void ConvertPstToCsv(string pstFilePath, string csvFilePath)
        //{
        //    using (PersonalStorage pst = PersonalStorage.FromFile(pstFilePath))
        //    {
        //        FolderInfo folderInfo = pst.RootFolder;
        //        var messages = folderInfo.GetContents();

        //        using (StreamWriter writer = new StreamWriter(csvFilePath))
        //        {
        //            writer.WriteLine("Subject,Sender,Received");

        //            foreach (var message in messages)
        //            {
        //                string subject = message.Subject;
        //                string sender = message.SenderRepresentativeName;  //From.Address;
        //                //DateTime received = message;

        //                writer.WriteLine($"{subject},{sender},");
        //            }
        //        }
        //    }
        //}


        //private static void ConvertPstToCsv(string pstFilePath, string csvFilePath)
        //{
        //    using (PersonalStorage pst = PersonalStorage.FromFile(pstFilePath))
        //    {
        //        FolderInfo folderInfo = pst.RootFolder;
        //        MailMessage[] messages = folderInfo.GetContents();

        //        using (StreamWriter writer = new StreamWriter(csvFilePath))
        //        {
        //            writer.WriteLine("Subject,Sender,Received");

        //            foreach (MailMessage message in messages)
        //            {
        //                string subject = message.Subject;
        //                string sender = message.From.Address;
        //                DateTime received = message.Date;

        //                writer.WriteLine($"{subject},{sender},{received}");
        //            }
        //        }
        //    }
        //}



        private static void ReadPstAndSaveAsCsv(string pstFilePath, string csvFilePath)
        {
            Application outlookApp = new Application();
            NameSpace outlookNs = outlookApp.GetNamespace("MAPI");

            // Add the .pst file as a data store
            outlookNs.AddStore(pstFilePath);

            // Get the data store from the Stores collection
            Store store = null;
            foreach (Store s in outlookNs.Stores)
            {
                if (s.FilePath == pstFilePath)
                {
                    store = s;
                    break;
                }
            }

            if (store != null)
            {
                Folder rootFolder = (Folder)store.GetRootFolder();

                using (StreamWriter writer = new StreamWriter(csvFilePath))
                {
                    writer.WriteLine("Subject,Sender,Received");

                    ProcessFolder(rootFolder, writer);
                }

                outlookNs.RemoveStore((MAPIFolder)store);
            }

            outlookApp.Quit();
        }


        private static void ProcessFolder(Folder folder, StreamWriter writer)
        {
            Items items = folder.Items;

            foreach (object item in items)
            {
                if (item is MailItem mailItem)
                {
                    string subject = mailItem.Subject;
                    string sender = mailItem.SenderEmailAddress;
                    DateTime received = mailItem.ReceivedTime;

                    writer.WriteLine($"{subject},{sender},{received}");
                }

                if (item is Folder subFolder)
                {
                    ProcessFolder(subFolder, writer);
                }
            }
        }

    }
}

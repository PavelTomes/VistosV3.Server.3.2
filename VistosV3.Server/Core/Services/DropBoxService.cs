//**using Dropbox.Api;
//**using Dropbox.Api.Files;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services
{
    public class DropBoxService
    {
        //**private DropboxClient Client;
        private string DocumentsFolder = "/Vistos Documents";
        private string EmailsFolder = "/Vistos Emails";
        private string EmailAttachmentsSubFolder = "/Attachments";

        private string EmailBodyFileName = "Body.dat";
        private string EmailBodyTextFileName = "BodyText.dat";

        public DropBoxService(string accessToken)
        {
            // Specify socket level timeout which decides maximum waiting time when no bytes are
            // received by the socket.
            //**var httpClient = new HttpClient(new WebRequestHandler { ReadWriteTimeout = 10 * 1000 })
            var httpClient = new HttpClient()
            {
                // Specify request level timeout which decides maximum time that can be spent on
                // download/upload files.
                Timeout = TimeSpan.FromMinutes(20)
            };

            //**var config = new DropboxClientConfig("Vistos")
            //**{
            //**    HttpClient = httpClient
            //**};
            //**
            //**Client = new DropboxClient(accessToken, config);
        }

        public void UploadDocAttachment(byte[] docAttachmentContent, int docAttachmentId, string docAttachmentFileName)
        {
            string filePath = DocumentsFolder + "/" + docAttachmentId.ToString() + "/" + docAttachmentFileName;
            var task = Task.Run(() => UploadFile(docAttachmentContent, filePath));
            task.Wait();
        }

        public void UploadEmailAttachment(byte[] attachmentContent, int emailId, int emailAttachmentId, string attachmentFileName)
        {
            string filePath = EmailsFolder + "/" + emailId.ToString() + EmailAttachmentsSubFolder + "/" + emailAttachmentId.ToString() + "/" + (attachmentFileName ?? "file.dat");
            var task = Task.Run(() => UploadFile(attachmentContent, filePath));
            task.Wait();
        }

        public void UploadEmailBody(string body, string bodyText, int emailId)
        {
            string filePath = EmailsFolder + "/" + emailId.ToString() + "/" + EmailBodyFileName;
            byte[] bytes = Encoding.Unicode.GetBytes(body ?? string.Empty);
            var task1 = Task.Run(() => UploadFile(bytes, filePath));
            task1.Wait();

            filePath = EmailsFolder + "/" + emailId.ToString() + "/" + EmailBodyTextFileName;
            bytes = Encoding.Unicode.GetBytes(bodyText ?? string.Empty);
            var task2 = Task.Run(() => UploadFile(bytes, filePath));
            task2.Wait();
        }

        public byte[] DownloadDocAttachment(int docAttachmentId, string docAttachmentFileName)
        {
            string filePath = DocumentsFolder + "/" + docAttachmentId.ToString() + "/" + docAttachmentFileName;
            var task = Task.Run(() => DownloadFile(filePath));
            task.Wait();
            return task.Result;
        }

        public byte[] DownloadEmailAttachment(int emailId, int emailAttachmentId, string attachmentFileName)
        {
            string filePath = EmailsFolder + "/" + emailId.ToString() + EmailAttachmentsSubFolder + "/" + emailAttachmentId.ToString() + "/" + (attachmentFileName ?? "file.dat");
            var task = Task.Run(() => DownloadFile(filePath));
            task.Wait();
            return task.Result;
        }

        public string DownloadEmailBody(int emailId, bool textVersion)
        {
            string filePath = EmailsFolder + "/" + emailId.ToString() + "/" + (textVersion ? EmailBodyTextFileName : EmailBodyFileName);
            var task = Task.Run(() => DownloadFile(filePath));
            task.Wait();
            return Encoding.Unicode.GetString(task.Result);
        }

        public Uri GetUriDocAttachment(int docAttachmentId, string docAttachmentFileName)
        {
            string filePath = DocumentsFolder + "/" + docAttachmentId.ToString() + "/" + docAttachmentFileName;
            var task = Task.Run(() => GetUrlFile(filePath));
            task.Wait();
            return task.Result;
        }

        public Uri GetUriEmailAttachment(int emailId, int emailAttachmentId, string attachmentFileName)
        {
            string filePath = EmailsFolder + "/" + emailId.ToString() + EmailAttachmentsSubFolder + "/" + emailAttachmentId.ToString() + "/" + (attachmentFileName ?? "file.dat");
            var task = Task.Run(() => GetUrlFile(filePath));
            task.Wait();
            return task.Result;
        }

        public void DeleteDocAttachment(int docAttachmentId)
        {
            string filePath = DocumentsFolder + "/" + docAttachmentId.ToString();
            var task = Task.Run(() => DeleteFile(filePath));
            task.Wait();
        }

        public void DeleteEmailAttachment(int emailId, int emailAttachmentId)
        {
            string filePath = EmailsFolder + "/" + emailId.ToString() + EmailAttachmentsSubFolder + "/" + emailAttachmentId.ToString();
            var task = Task.Run(() => DeleteFile(filePath));
            task.Wait();
        }

        public void DeleteEmailBody(int emailId)
        {
            string filePath = EmailsFolder + "/" + emailId.ToString() + "/" + EmailBodyFileName;
            var task1 = Task.Run(() => DeleteFile(filePath));
            task1.Wait();

            filePath = EmailsFolder + "/" + emailId.ToString() + "/" + EmailBodyTextFileName;
            var task2 = Task.Run(() => DeleteFile(filePath));
            task2.Wait();
        }

        private async Task<byte[]> DownloadFile(string filePath)
        {
            //**using (var response = await Client.Files.DownloadAsync(filePath))
            //**{
            //**    return await response.GetContentAsByteArrayAsync();
            //**}
            return null;
        }

        private async Task<Uri> GetUrlFile(string filePath)
        {
            //**var link = await Client.Files.GetTemporaryLinkAsync(filePath);
            //**return new Uri(link.Link);
            return null;
        }

        private async Task UploadFile(byte[] content, string filePath)
        {
            //**using (var stream = new MemoryStream(content))
            //**{
            //**    var response = await Client.Files.UploadAsync(filePath, WriteMode.Overwrite.Instance, body: stream);
            //**}
        }

        private async Task DeleteFile(string filePath)
        {
            //**await Client.Files.DeleteV2Async(filePath);
        }
    }
}

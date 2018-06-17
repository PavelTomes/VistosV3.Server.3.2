using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Renci.SshNet;
using System.IO;


namespace Core.Services
{
    public class FtpService
    {
        private string type = string.Empty;
        private string host = string.Empty;
        private int? port = 0;
        private string userName = string.Empty;
        private string password = string.Empty;
        private string privateKey = string.Empty;
        private string passPhrase = string.Empty;
        private string root = string.Empty;

        private List<AuthenticationMethod> authMethods = new List<AuthenticationMethod>();
        private ConnectionInfo connectionInfo = null;
        public FtpService(
            string type
            , string host
            , int? port
            , string userName
            , string password
            , string privateKey
            , string passPhrase
            , string root
        )
        {
            this.type = type;
            this.host = host;
            this.port = port;
            this.userName = userName;
            this.password = password;
            this.privateKey = privateKey;
            this.passPhrase = passPhrase;
            this.root = root;

            if (type == "SFTP_WithPrivateKey")
            {
                MemoryStream stream = new MemoryStream();
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(this.privateKey);
                writer.Flush();
                stream.Position = 0;

                PrivateKeyFile keyFile = null;
                if (string.IsNullOrEmpty(passPhrase))
                {
                    keyFile = new PrivateKeyFile(stream);
                }
                else
                {
                    keyFile = new PrivateKeyFile(stream, passPhrase);
                }
                PrivateKeyFile[] keyFiles = new[] { keyFile };
                authMethods.Add(new PrivateKeyAuthenticationMethod(this.userName, keyFiles));
                connectionInfo = new ConnectionInfo(this.host, this.port.Value, this.userName, authMethods.ToArray());
            }
        }
        public void UploadDocAttachment(byte[] docAttachmentContent, int docAttachmentId, string docAttachmentFileName)
        {
            using (SftpClient client = new SftpClient(connectionInfo))
            {
                client.Connect();
                string filePath = this.root + "/" + docAttachmentId.ToString() + "_" + docAttachmentFileName;
                client.BufferSize = 4 * 1024;
                client.UploadFile(new MemoryStream(docAttachmentContent), filePath);
                client.Disconnect();
            }
        }

        public byte[] DownloadDocAttachment(int docAttachmentId, string docAttachmentFileName)
        {
            using (SftpClient client = new SftpClient(connectionInfo))
            {
                client.Connect();
                string filePath = this.root + "/" + docAttachmentId.ToString() + "_" + docAttachmentFileName;
                MemoryStream ms = new MemoryStream();
                    client.DownloadFile(filePath, ms);
                client.Disconnect();
                return ms.ToArray();
            }
        }
    }
}

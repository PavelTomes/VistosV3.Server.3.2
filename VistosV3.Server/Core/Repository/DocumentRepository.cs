using Core.Models;
using Core.Services;
using Core.Tools;
using Core.VistosDb;
using Core.VistosDb.Objects;
using System;
using System.Linq;

namespace Core.Repository
{
    public class DocumentRepository : IDisposable
    {
        private vwSystemSettings SystemSettings;

        public DocumentRepository(vwSystemSettings systemSettings)
        {
            SystemSettings = systemSettings;
        }

        public string SaveNewRichTextAttachment(byte[] byteArray, UserInfo userInfo)
        {
            using (VistosDbContext ctx = new VistosDbContext())
            {

                RichTextAttachment att = new RichTextAttachment();
                att.Deleted = false;
                att.Created = DateTime.Now;
                att.Modified = DateTime.Now;
                att.CreatedBy_FK = userInfo.UserId;
                att.Content = byteArray;
                att.UniqueGuid = Guid.NewGuid();
                ctx.RichTextAttachment.Add(att);
                ctx.SaveChanges();
                return att.UniqueGuid.ToString();
            }
        }

        public RichTextAttachment GetRichTextAttachment(Guid id)
        {
            using (VistosDbContext ctx = new VistosDbContext())
            {
                return ctx.RichTextAttachment.First(x => x.UniqueGuid == id);
            }
        }

        public DocumentExt SaveNewDocumentWithAttachment(string documentName, byte[] byteArray, string type, UserInfo userInfo)
        {
            using (VistosDbContext ctx = new VistosDbContext())
            {
                int enumDocsId = 0;
                if (!string.IsNullOrEmpty(type))
                {
                    int enumDocsTypeId = ctx.EnumerationType.Where(e => !e.Deleted && e.Type == "DocsType").Select(e => e.Id).First();
                    enumDocsId = ctx.Enumeration.Where(e => e.EnumerationType_FK == enumDocsTypeId && e.Description == type && !e.Deleted).Select(e => e.Id).FirstOrDefault();
                }

                bool storeInDropBox = !string.IsNullOrEmpty(SystemSettings.DropBoxSecurityToken);
                bool storeInFtp = !string.IsNullOrEmpty(SystemSettings.FtpType);

                Document doc = new Document();
                int lastDot = documentName.LastIndexOf('.');
                doc.Deleted = false;
                doc.CreatedBy_FK = userInfo.UserId;
                doc.Modified = DateTime.Now;
                doc.Created = DateTime.Now;
                doc.Account_FK = userInfo.AccountId;
                doc.Contact_FK = userInfo.ContactId;
                if (enumDocsId > 0)
                    doc.Type_FK = enumDocsId;
                doc.Name = lastDot <= 0 ? documentName : documentName.Substring(0, lastDot);
                ctx.Document.Add(doc);
                ctx.SaveChanges();

                DocumentAttachment da = new DocumentAttachment();
                da.Deleted = false;
                da.CreatedBy_FK = userInfo.UserId;
                da.Modified = DateTime.Now;
                da.Created = DateTime.Now;
                da.StoreInDropBox = storeInDropBox;
                da.StoreInFtp = storeInFtp;
                da.DocName = documentName;
                da.Attachment = !storeInDropBox && !storeInFtp ? byteArray : null;
                da.DataLength = byteArray?.LongLength ?? 0;
                da.UploadDate = DateTime.Now;
                da.Document_FK = doc.Id;
                da.ContentType = MimeMapping.MimeUtility.GetMimeMapping(documentName);
                da.Icon = Icon.GetIconByExtension(System.IO.Path.GetExtension(documentName));

                DocumentExt docExt = new DocumentExt()
                {
                    Id = doc.Id,
                    Name = doc.Name,
                    Type_FK = doc.Type_FK,
                    Icon = da.Icon
                };

                ctx.DocumentAttachment.Add(da);
                ctx.SaveChanges();

                if (storeInFtp)
                {
                    FtpService ftpService = new FtpService(
                        SystemSettings.FtpType
                        , SystemSettings.FtpHost
                        , SystemSettings.FtpPort
                        , SystemSettings.FtpUserName
                        , SystemSettings.FtpPassword
                        , SystemSettings.FtpPrivateKey
                        , SystemSettings.FtpPassPhrase
                        , SystemSettings.FtpRoot
                        );
                    ftpService.UploadDocAttachment(byteArray, da.Id, da.DocName);
                }

                if (storeInDropBox)
                {
                    DropBoxService dropBox = new DropBoxService(SystemSettings.DropBoxSecurityToken);
                    dropBox.UploadDocAttachment(byteArray, da.Id, da.DocName);
                }

                return docExt;
            }
        }

        //public void SaveNewDocumentWithAttachment(HttpPostedFileBase file, string parentEntityName, int parentEntityId, string type, UserInfo userInfo)
        //{
        //    byte[] byteArray = new byte[file.InputStream.Length];
        //    Document doc = SaveNewDocumentWithAttachment(file.FileName, byteArray, type, userInfo);

        //    if (doc != null && doc.Document_ID > 0)
        //    {
        //        SaveEntityDocumentRelation(parentEntityName, parentEntityId, doc.Document_ID, userInfo);
        //    }
        //}

        //public void SaveEntityDocumentRelation(string parentEntityName, int parentEntityId, int documentID, UserInfo userInfo)
        //{
        //    using (var ctx = new DAL.Database.VistosApi())
        //    {
        //        vwProjection parentEntity = ctx.vwProjection.FirstOrDefault(x => x.Projection_Name == parentEntityName);
        //        if (parentEntity != null && parentEntityId > 0)
        //        {
        //            Entity_Document ed = new Entity_Document();
        //            ed.Deleted = false;
        //            ed.CreatedBy_FK = userInfo.UserId;
        //            ed.Modified = DateTime.Now;
        //            ed.Created = DateTime.Now;
        //            ed.Document_FK = documentID;
        //            ed.EntityName = parentEntity.DbObject_Name;
        //            ed.Entity_FK = parentEntityId;
        //            ctx.Entity_Document.Add(ed);
        //            ctx.SaveChanges();
        //        }
        //    }
        //}

        public void Dispose()
        {
        }
    }
}

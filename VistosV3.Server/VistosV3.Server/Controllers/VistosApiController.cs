using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Core.Models;
using Core.Models.ApiRequest;
using Core.Repository;
using Core.Services;
using Core.Tools;
using Core.VistosDb;
using Core.VistosDb.Objects;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using NetStdTools;
using NetStdTools.ReportExecution;
using Newtonsoft.Json;
using VistosV3.Server.Code;

namespace VistosV3.Server.Controllers
{
    public class VistosApiController : BaseVistosApiController
    {
        public VistosApiController(IAuditService audit) : base(audit)
        {
        }

        #region Execute
        [HttpPost]
        public object Execute(string id)
        {
            string json = null;
            using (var reader = new StreamReader(Request.Body))
            {
                json = reader.ReadToEnd();
            }

            object resultset = ExecuteInternal(id, json, null);
            if (resultset.GetType().ToString() == "System.String")
            {
                return Content(resultset.ToString(), "application/json");
            }
            else
            {
                return resultset;
            }
        }

        public object ExecuteInternal(string id, string json, UserInfo userInfoSystem)
        {
            ApiRequest request = null;
            UserInfo userInfo = null;

            if (Response != null)
            {
                Response.ContentType = "application/json";
            }

            try
            {
                this.auditService.SetRequestJson(json);

                request = JsonConvert.DeserializeObject<ApiRequest>(json);

                if (userInfoSystem != null)
                {
                    userInfo = userInfoSystem;
                }
                else
                {
                    if (request != null && !string.IsNullOrEmpty(request.UserToken))
                    {
                        DbRepository repository = new DbRepository(null, this.auditService);
                        userInfo = repository.GetUserByToken(request.UserToken);
                    }
                }

                this.auditService.ParseRequestOperation(request, userInfo);

                ApiManager apiM = new ApiManager(request, userInfo, this.auditService);
                ApiResponse response;

                if (request.GetExportParam != null)
                {
                    response = apiM.ExecuteExport();
                    Response.StatusCode = (int)response.Status;
                    return ((ApiExportResponse)response).DataExportResult;
                }
                else
                {
                    response = apiM.Execute();
                    this.auditService.SetResponseJson(response.DataJSon);
                    if (Response != null)
                        Response.StatusCode = (int)response.Status;
                    return response.GetRepositoryResponse();
                }
            }
            catch (Exception ex)
            {
                if (request != null)
                {
                    Logger.SaveLogError(LogLevel.Error, "Execute", ex, request, userInfo);
                }
                else
                {
                    Logger.SaveLogError(LogLevel.Error, "Execute", ex, null, userInfo);
                }
                Response.StatusCode = 210;
                return $"{{ \"error\": \"{ex.Message}\"}}";
            }
        }
        #endregion

        #region Report
        [HttpGet]
        public async Task<IActionResult> GetReport(
                    int id,
                    ReportMode reportMode,
                    ExportType exportType,
                    string rdlReportName,
                    string entityName,
                    string ids,
                    string reportJsonData,
                    string userToken)
        {
            if (string.IsNullOrEmpty(userToken))
            {
                return null;
            }
            UserInfo userInfo = GetUserInfoFromUserToken(userToken);
            string reportJsonData1 = string.Empty;
            if (!string.IsNullOrEmpty(reportJsonData))
            {
                byte[] encodedDataAsBytes = System.Convert.FromBase64String(reportJsonData);
                string dataEncoded = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
                reportJsonData1 = HttpUtility.UrlDecode(dataEncoded);
            }

            try
            {
                if (userInfo != null)
                {
                    ReportGenerator gen = new ReportGenerator(
                    Settings.GetInstance.SystemSettings.ReportServerUrl + "/ReportExecution2005.asmx",
                    Settings.GetInstance.SystemSettings.ReportServerUserName,
                    Settings.GetInstance.SystemSettings.ReportServerPassword
                    );
                    string reportPath = Settings.GetInstance.SystemSettings.ReportServerFormsPath;
                    if (!reportPath.EndsWith("/")) reportPath += "/";

                    if (reportMode == ReportMode.MultipleByMultipleIds)
                    {
                        byte[] encodedDataAsBytes = System.Convert.FromBase64String(ids);
                        string idsEncoded = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
                        string idsUrlEncoded = HttpUtility.UrlDecode(idsEncoded).Replace("[", "").Replace("]", "");
                        string[] idsUrlEncodedSplited = idsUrlEncoded.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                        if (idsUrlEncodedSplited != null && idsUrlEncodedSplited.Length == 1)
                        {
                            int recordId = int.Parse(idsUrlEncodedSplited[0]);
                            return await this.RenderReport(userInfo, entityName, recordId, reportPath + rdlReportName, exportType, gen); ;
                        }
                        else
                        {
                            using (MemoryStream compressedFileStream = new MemoryStream())
                            {
                                using (ZipArchive zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Update, false))
                                {
                                    foreach (string recordIdStr in idsUrlEncodedSplited)
                                    {
                                        int recordId = int.Parse(recordIdStr);
                                        byte[] repData = await this.RenderReportData(userInfo, entityName, recordId, reportPath + rdlReportName, exportType, gen);
                                        SqlReport sqlReport = new SqlReport(userInfo, this.auditService);
                                        string reportNameReal = sqlReport.GetReportName(entityName, id);

                                        ZipArchiveEntry zipEntry = zipArchive.CreateEntry(reportNameReal);
                                        using (MemoryStream originalFileStream = new MemoryStream(repData))
                                        {
                                            using (Stream zipEntryStream = zipEntry.Open())
                                            {
                                                originalFileStream.CopyTo(zipEntryStream);
                                            }
                                        }
                                    }
                                }
                                return new FileContentResult(compressedFileStream.ToArray(), "application/zip") { FileDownloadName = "Reports.zip" };
                            }
                        }
                    }
                    //**else if (reportMode == ReportMode.OneByMultipleIds)
                    //**{
                    //**    byte[] encodedDataAsBytes = System.Convert.FromBase64String(ids);
                    //**    string idsEncoded = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
                    //**    string idsUrlEncoded = HttpUtility.UrlDecode(idsEncoded);
                    //**
                    //**    Vistos3Api.Code.Reports.ReportSettings reportSettings = new Vistos3Api.Code.Reports.ReportSettings();
                    //**    reportSettings.ReportPath = Settings.GetInstance.SystemSettings.ReportServerFormsPath;
                    //**    reportSettings.ReportServerUserName = Settings.GetInstance.SystemSettings.ReportServerUserName;
                    //**    reportSettings.ReportServerPassword = Settings.GetInstance.SystemSettings.ReportServerPassword;
                    //**    reportSettings.ReportServerUrl = Settings.GetInstance.SystemSettings.ReportServerUrl;
                    //**    if (!reportSettings.ReportPath.EndsWith("/")) reportSettings.ReportPath += "/";
                    //**    reportSettings.ReportPath += rdlReportName;
                    //**    reportSettings.ReportParameters.Add(new Microsoft.Reporting.WebForms.ReportParameter("ids", idsUrlEncoded));
                    //**    if (!string.IsNullOrEmpty(reportJsonData1))
                    //**    {
                    //**        reportSettings.ReportParameters.Add(new Microsoft.Reporting.WebForms.ReportParameter("jsonData", reportJsonData1));
                    //**    }
                    //**
                    //**    string reportNameReal = reportSettings.GetReportName(entityName, id, userInfo);
                    //**    ReportResult rr = new ReportResult(reportNameReal,
                    //**        exportType,
                    //**        reportSettings,
                    //**        entityName,
                    //**        userInfo,
                    //**        entityName,
                    //**        id,
                    //**        false);
                    //**
                    //**    return rr;
                    //**}
                    //**else
                    else if (reportMode == ReportMode.OneByOneId)
                    {
                        return await this.RenderReport(userInfo, entityName, id, reportPath + rdlReportName, exportType, gen);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.SaveLogError(LogLevel.Error, "Execute", ex, null, userInfo);
            }
            return NotFound();
        }

        private async Task<byte[]> RenderReportData(UserInfo userInfo, string entityName, int id, string reportName, ExportType exportType, ReportGenerator gen)
        {
            ParameterValue[] parameters = {
                            new ParameterValue { Name = "id", Value = id.ToString() },
                            new ParameterValue { Name = "userId", Value = userInfo.UserId.ToString() },
                            new ParameterValue { Name = "codeLanguage", Value = userInfo.UserLanguage },
                            new ParameterValue { Name = "entityName", Value = entityName }
                        };

            byte[] repData = await gen.RenderReport(reportName, parameters, exportType.ToString());
            return repData;
        }

            private async Task<IActionResult> RenderReport(UserInfo userInfo, string entityName, int id, string reportName, ExportType exportType, ReportGenerator gen)
        {
            byte[] repData = await this.RenderReportData(userInfo, entityName, id, reportName, exportType, gen);

            if (repData != null && repData.Length > 0)
            {
                SqlReport sqlReport = new SqlReport(userInfo, this.auditService);
                string reportNameReal = sqlReport.GetReportName(entityName, id);
                return File(repData, ReportHelper.GetMimeType(exportType), reportNameReal + "." + ReportHelper.GetExtension(exportType));
            }
            return NotFound();
        }
        #endregion

        #region Avatar
        [HttpGet]
        [ResponseCache(Duration = 60)]
        //**[ResponseCache(Duration = 600, VaryByQueryKeys = new string[] { "id" })]
        public ActionResult GetThumbnailAvatar(int id)
        {
            if (id > 0)
            {
                UserAvatar userAvatar = null;
                using (VistosDbContext ctx = new VistosDbContext())
                {
                    vwUser vwUser1 = ctx.vwUser.Where(u => u.Id == id).FirstOrDefault();
                    if (vwUser1 != null && vwUser1.Id > 0 && vwUser1.User_Avatar_FK.HasValue)
                    {
                        userAvatar = ctx.UserAvatar.Where(ua => ua.Id == vwUser1.User_Avatar_FK.Value).FirstOrDefault();
                    }
                }
                if (userAvatar != null && userAvatar.Id > 0 && userAvatar.Avatar != null && userAvatar.Avatar.Length > 0)
                {
                    MemoryStream ms = null;
                    ms = new MemoryStream(userAvatar.Avatar);
                    return new FileStreamResult(ms, "image/jpeg");
                }
            }
            return File(Url.Content("~/Data/profil-default.jpg"), "image/jpeg");
        }

        [HttpPost]
        public JsonResult ThumbnailAvatarUploadImage(string userToken)
        {
            UserInfo userInfo = GetUserInfoFromUserToken(userToken);

            try
            {
                if (userInfo != null && Request.Form.Files != null && Request.Form.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Form.Files.Count; i++)
                    {
                        IFormFile file = Request.Form.Files[i];

                        if (file.Length > 0)
                        {
                            byte[] byteArray = null;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                file.CopyTo(ms);
                                byteArray = ms.ToArray();
                            }

                            if (byteArray != null && byteArray.Length > 0)
                            {
                                byte[] resizeArr = ImageTool.ResizeImage(byteArray, new System.Drawing.Size(200, 200));
                                using (VistosDbContext ctx = new VistosDbContext())
                                {
                                    UserAvatar userAvatar = new UserAvatar()
                                    {
                                        Deleted = false,
                                        Created = DateTime.Now,
                                        Modified = DateTime.Now,
                                        CreatedBy_FK = userInfo.UserId,
                                        Avatar = resizeArr
                                    };
                                    ctx.UserAvatar.Add(userAvatar);
                                    ctx.SaveChanges();

                                    var staleItem = Url.Action("GetThumbnailAvatar", "VistosApi", new
                                    {
                                        id = userInfo.UserId
                                    });
                                    //**Response.RemoveOutputCacheItem(staleItem);

                                    return Json(new { id = userAvatar.Id });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.SaveLogError(LogLevel.Error, "ThumbnailAvatarUploadImage", ex, null, userInfo);
            }
            return null;
        }
        #endregion

        [HttpGet]
        public ActionResult GetSignatureImageByGuid(string id)
        {
            Guid guid = !string.IsNullOrEmpty(id) ? Guid.Parse(id) : Guid.Empty;
            Signature signature = null;

            using (VistosDbContext ctx = new VistosDbContext())
            {
                signature = ctx.Signature.Where(s => !s.Deleted && s.UniqueGuid == guid).FirstOrDefault();
            }
            MemoryStream ms = null;
            if (signature != null && signature.Id > 0)
            {
                ms = new MemoryStream(signature.Bitmap);
                return new FileStreamResult(ms, "image/jpeg");
            }
            return NotFound();
        }

        #region RichTextAttachment
        [HttpGet]
        public ActionResult RichTextAttachmentGetImage(string id)
        {
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    DocumentRepository rep = new DocumentRepository(Settings.GetInstance.SystemSettings);
                    RichTextAttachment richTextAttachment = rep.GetRichTextAttachment(new Guid(id));
                    if (richTextAttachment != null)
                    {
                        System.IO.MemoryStream ms = null;
                        ms = new System.IO.MemoryStream(richTextAttachment.Content);
                        return new FileStreamResult(ms, "image/png");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.SaveLogError(LogLevel.Error, "RichTextAttachmentGetImage", ex, null, null);
                return NotFound();
            }
            return NotFound();
        }

        [HttpPost]
        public JsonResult RichTextAttachmentUploadImage(string userToken)
        {
            UserInfo userInfo = GetUserInfoFromUserToken(userToken);

            try
            {
                if (userInfo != null && Request.Form.Files != null && Request.Form.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Form.Files.Count; i++)
                    {
                        IFormFile file = Request.Form.Files[i];

                        if (file.Length > 0)
                        {
                            byte[] byteArray = null;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                file.CopyTo(ms);
                                byteArray = ms.ToArray();
                            }

                            if (byteArray != null && byteArray.Length > 0)
                            {
                                DocumentRepository rep = new DocumentRepository(Settings.GetInstance.SystemSettings);
                                string fileName = rep.SaveNewRichTextAttachment(byteArray, userInfo);
                                return Json(new { location = fileName });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.SaveLogError(LogLevel.Error, "RichTextAttachmentUploadImage", ex, null, userInfo);
            }
            return null;
        }
        #endregion

        #region Email
        [HttpPost]
        public ActionResult AddEmailAttachment(string userToken)
        {
            List<object> list = new List<object>();
            UserInfo userInfo = GetUserInfoFromUserToken(userToken);
            try
            {
                if (Request.Form.Files != null && Request.Form.Files.Count > 0)
                {
                    for (int i = 0; i < Request.Form.Files.Count; i++)
                    {
                        IFormFile file = Request.Form.Files[i];

                        if (file.Length > 0)
                        {
                            byte[] byteArray = null;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                file.CopyTo(ms);
                                byteArray = ms.ToArray();
                            }

                            if (byteArray != null && byteArray.Length > 0)
                            {
                                EmailAttachment item = new EmailAttachment();
                                item.Deleted = false;
                                item.Created = DateTime.Now;
                                item.CreatedBy_FK = userInfo.UserId;
                                item.Modified = DateTime.Now;
                                item.FileName = file.FileName;
                                item.Data = byteArray;
                                item.DataLength = byteArray.Length;
                                item.Type = file.ContentType;

                                using (VistosDbContext ctx = new VistosDbContext())
                                {
                                    ctx.EmailAttachment.Add(item);
                                    ctx.SaveChanges();

                                    list.Add(new
                                    {
                                        Id = item.Id,
                                        FileName = item.FileName,
                                        DataLength = item.DataLength,
                                        Type = item.ContentType,
                                        Deleted = false
                                    });
                                }
                            }
                        }
                    }
                }
                return Json(list.ToArray());
            }
            catch (Exception ex)
            {
                Logger.SaveLogError(LogLevel.Error, "Execute", ex, null, userInfo);
            }
            return new ContentResult() { Content = "AttachmentUploadError", ContentType = "text/html" };
        }

        [HttpGet]
        public ActionResult DownloadEmailAttachment(int attachmentId, string userToken)
        {
            UserInfo userInfo = GetUserInfoFromUserToken(userToken);

            try
            {
                EmailAttachment emailAttachment = null;
                using (VistosDbContext ctx = new VistosDbContext())
                {
                    emailAttachment = ctx.EmailAttachment.FirstOrDefault(a => a.Id == attachmentId && !a.Deleted);
                }
                if (emailAttachment != null && emailAttachment.Id > 0)
                {
                    if (emailAttachment.Data == null && emailAttachment.DataLength > 0 && emailAttachment.Email_FK.HasValue)
                    {
                        if (emailAttachment.StoredInDropbox)
                        {
                            DropBoxService dropBox = new DropBoxService(Settings.GetInstance.SystemSettings.DropBoxSecurityToken);
                            Uri dropBoxAttachUri = dropBox.GetUriEmailAttachment(emailAttachment.Email_FK.Value, emailAttachment.Id, emailAttachment.FileName);
                            return new RedirectResult(dropBoxAttachUri.ToString(), false);
                        }
                        else
                        {
                            // Refresh attachment content from IMAP server
                            //**Service.Model.EmailViewModel model = new Service.Model.EmailViewModel(emailAttachment.Email_FK.Value, userInfo);
                            //**model.RefreshAttachmentsFromIMAP();

                            using (VistosDbContext ctx = new VistosDbContext())
                            {
                                emailAttachment = ctx.EmailAttachment.FirstOrDefault(a => a.Id == attachmentId && !a.Deleted);
                            }
                        }
                    }

                    return File(emailAttachment.Data, emailAttachment.Type, emailAttachment.FileName);
                }
            }
            catch (Exception ex)
            {
                Logger.SaveLogError(LogLevel.Error, "Execute", ex, null, null);
            }
            return NotFound();
        }
        #endregion

        #region Document
        [HttpGet]
        public ActionResult Download(
            int id
            , string entityName
            , string userToken
        )
        {
            UserInfo userInfo = GetUserInfoFromUserToken(userToken);

            try
            {
                if (userInfo != null)
                {
                    if (entityName.ToLower() == "document")
                    {
                        List<DocumentAttachment> docAttachmentList = new List<DocumentAttachment>();

                        using (VistosDbContext ctx = new VistosDbContext())
                        {
                            docAttachmentList = ctx.DocumentAttachment.Where(d => !d.Deleted && d.Document_FK == id).ToList();
                        }

                        if (docAttachmentList.Count == 1)
                        {
                            DocumentAttachment att = docAttachmentList.Single();

                            if (att.StoreInFtp)
                            {
                                FtpService ftpService = new FtpService(
                                    Settings.GetInstance.SystemSettings.FtpType
                                    , Settings.GetInstance.SystemSettings.FtpHost
                                    , Settings.GetInstance.SystemSettings.FtpPort
                                    , Settings.GetInstance.SystemSettings.FtpUserName
                                    , Settings.GetInstance.SystemSettings.FtpPassword
                                    , Settings.GetInstance.SystemSettings.FtpPrivateKey
                                    , Settings.GetInstance.SystemSettings.FtpPassPhrase
                                    , Settings.GetInstance.SystemSettings.FtpRoot
                                    );
                                byte[] content = ftpService.DownloadDocAttachment(att.Id, att.DocName);
                                return File(content, att.ContentType, att.DocName);
                            }
                            else if (att.StoreInDropBox)
                            {
                                DropBoxService dropBox = new DropBoxService(Settings.GetInstance.SystemSettings.DropBoxSecurityToken);
                                Uri dropBoxAttachUri = dropBox.GetUriDocAttachment(att.Id, att.DocName);
                                return new RedirectResult(dropBoxAttachUri.ToString(), false);
                            }
                            else
                            {
                                return File(att.Attachment, att.ContentType, att.DocName);
                            }
                        }
                        else
                        {
                            using (MemoryStream str = new MemoryStream())
                            {
                                using (ZipArchive zipFile = new ZipArchive(str, ZipArchiveMode.Update, false))
                                {
                                    foreach (DocumentAttachment att in docAttachmentList)
                                    {
                                        byte[] content;

                                        if (att.StoreInFtp)
                                        {
                                            FtpService ftpService = new FtpService(
                                                Settings.GetInstance.SystemSettings.FtpType
                                                , Settings.GetInstance.SystemSettings.FtpHost
                                                , Settings.GetInstance.SystemSettings.FtpPort
                                                , Settings.GetInstance.SystemSettings.FtpUserName
                                                , Settings.GetInstance.SystemSettings.FtpPassword
                                                , Settings.GetInstance.SystemSettings.FtpPrivateKey
                                                , Settings.GetInstance.SystemSettings.FtpPassPhrase
                                                , Settings.GetInstance.SystemSettings.FtpRoot
                                            );
                                            content = ftpService.DownloadDocAttachment(att.Id, att.DocName);
                                        }
                                        else if (att.StoreInDropBox)
                                        {
                                            DropBoxService dropBox = new DropBoxService(Settings.GetInstance.SystemSettings.DropBoxSecurityToken);
                                            content = dropBox.DownloadDocAttachment(att.Id, att.DocName);
                                        }
                                        else
                                        {
                                            content = att.Attachment;
                                        }

                                        if (content != null)
                                        {
                                            ZipArchiveEntry zipEntry = zipFile.CreateEntry(att.DocName);
                                            using (MemoryStream originalFileStream = new MemoryStream(content))
                                            {
                                                using (Stream zipEntryStream = zipEntry.Open())
                                                {
                                                    originalFileStream.CopyTo(zipEntryStream);
                                                }
                                            }
                                        }
                                    }
                                }
                                return File(str.ToArray(), "application/octet-stream", "Download.zip");
                            }
                        }
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return NotFound();
                throw;
            }
        }

        [HttpGet]
        public ActionResult Preview(
            int id
            , string entityName
            , string userToken
        )
        {
            UserInfo userInfo = GetUserInfoFromUserToken(userToken);

            try
            {
                if (userInfo != null)
                {
                    if (entityName.ToLower() == "document")
                    {
                        List<DocumentAttachment> docAttachmentList = new List<DocumentAttachment>();

                        using (VistosDbContext ctx = new VistosDbContext())
                        {
                            docAttachmentList = ctx.DocumentAttachment.Where(d => !d.Deleted && d.Document_FK == id).ToList();
                        }

                        if (docAttachmentList.Count == 1)
                        {
                            DocumentAttachment att = docAttachmentList.Single();

                            if (att.StoreInFtp)
                            {
                                FtpService ftpService = new FtpService(
                                    Settings.GetInstance.SystemSettings.FtpType
                                    , Settings.GetInstance.SystemSettings.FtpHost
                                    , Settings.GetInstance.SystemSettings.FtpPort
                                    , Settings.GetInstance.SystemSettings.FtpUserName
                                    , Settings.GetInstance.SystemSettings.FtpPassword
                                    , Settings.GetInstance.SystemSettings.FtpPrivateKey
                                    , Settings.GetInstance.SystemSettings.FtpPassPhrase
                                    , Settings.GetInstance.SystemSettings.FtpRoot
                                    );
                                byte[] content = ftpService.DownloadDocAttachment(att.Id, att.DocName);

                                MemoryStream dataStream = new MemoryStream();
                                dataStream.Write(content, 0, content.Length);
                                dataStream.Position = 0;
                                return new FileStreamResult(dataStream, att.ContentType);
                            }
                            else if (att.StoreInDropBox)
                            {
                                DropBoxService dropBox = new DropBoxService(Settings.GetInstance.SystemSettings.DropBoxSecurityToken);
                                Uri dropBoxAttachUri = dropBox.GetUriDocAttachment(att.Id, att.DocName);
                                return new RedirectResult(dropBoxAttachUri.ToString(), false);
                            }
                            else
                            {
                                MemoryStream dataStream = new MemoryStream();
                                dataStream.Write(att.Attachment, 0, att.Attachment.Length);
                                dataStream.Position = 0;
                                return new FileStreamResult(dataStream, att.ContentType);
                            }
                        }
                    }
                }
                return NotFound();
            }
            catch (Exception ex)
            {
                return NotFound();
                throw;
            }
        }

        [HttpPost]
        public ActionResult Upload(string type, string userToken)
        {
            UserInfo userInfo = GetUserInfoFromUserToken(userToken);
            List<DocumentExt> documentList = new List<DocumentExt>();

            try
            {
                if (userInfo != null)
                {
                    for (int i = 0; i < Request.Form.Files.Count; i++)
                    {
                        IFormFile file = Request.Form.Files[i];

                        if (file.Length > 0)
                        {
                            byte[] byteArray = null;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                file.CopyTo(ms);
                                byteArray = ms.ToArray();
                            }

                            if (byteArray != null && byteArray.Length > 0)
                            {
                                DocumentRepository rep = new DocumentRepository(Settings.GetInstance.SystemSettings);
                                documentList.Add(rep.SaveNewDocumentWithAttachment(file.FileName, byteArray, type, userInfo));
                            }
                        }
                    }
                }
                return Json(documentList.Select(x => new
                {
                    Deleted = false,
                    Id = x.Id,
                    Subject = x.Name,
                    Icon = x.Icon,
                    AttachmentCount = 1,
                    Type_FK = x.Type_FK.HasValue ? (Settings.GetInstance.GetEnumerationCaptionById("DocsType", x.Type_FK.Value, userInfo.UserLanguage)) : string.Empty
                }
                    )
                );
            }
            catch (Exception ex)
            {
                Logger.SaveLogError(LogLevel.Error, "Execute", ex, null, userInfo);
            }
            return new ContentResult() { Content = "DocumentUploadError", ContentType = "text/html" };
        }

        #endregion
    }
}
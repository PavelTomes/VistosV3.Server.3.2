using Core.Models;
using Core.Repository;
using Core.Services;
using Core.VistosDb.Objects;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;

namespace Core.Tools
{
    public static class ReportHelper
    {
        public static string GetExtension(ExportType exportType)
        {
            switch (exportType)
            {
                case ExportType.Excel: return "xls";
                case ExportType.PDF: return "pdf";
                case ExportType.Word: return "doc";
                case ExportType.ImagePNG: return "png";
                case ExportType.ImageJPG: return "jpg";
                case ExportType.HTML: return "html";

                default: throw new NotSupportedException("Not supported extension");
            }
        }

        public static string GetMimeType(ExportType exportType)
        {
            switch (exportType)
            {
                case ExportType.Excel: return "application/vnd.ms-excel";
                case ExportType.PDF: return "application/pdf";
                case ExportType.Word: return "application/vnd.ms-word";
                case ExportType.ImagePNG: return "application/png";
                case ExportType.ImageJPG: return "application/jpg";
                case ExportType.HTML:
                case ExportType.HTMLOWC:
                case ExportType.MHTML:
                    return "text/html";

                default: throw new NotSupportedException("Not supported MIME type: " + exportType.ToString());
            }
        }

        public static string GetRSFormat(ExportType exportType)
        {
            switch (exportType)
            {
                case ExportType.PDF:
                    return "PDF";

                case ExportType.Excel:
                    return "Excel";

                case ExportType.Word:
                    return "Word";

                case ExportType.HTML:
                    return "HTML4.0";

                case ExportType.MHTML:
                    return "MHTML";

                case ExportType.HTMLOWC:
                    return "HTMLOWC";

                case ExportType.ImagePNG:
                case ExportType.ImageJPG:

                    return "Image";

                default:
                    throw new NotSupportedException("Reporting services does not support " + exportType + " format");
            }
        }
    }

    public class SqlReport
    {
        public IAuditService auditService { get; private set; }
        public UserInfo userInfo { get; private set; }

        public SqlReport(UserInfo userInfo, IAuditService auditService)
        {
            this.userInfo = userInfo;
            this.auditService = auditService;
        }

        public string GetReportName(string entityName, int recordId)
        {
            string rpName = string.Empty;
            if (!string.IsNullOrEmpty(entityName) && recordId > 0)
            {
                vwNumberingSequence numSeq = Settings.GetInstance.VwNumberingSequence.Where(x => x.Projection_Name == entityName && x.Profile_Id == this.userInfo.ProfileId && x.AccessRightsType_Id.HasValue && x.AccessRightsType_Id.Value > 0).FirstOrDefault();
                if (numSeq != null && numSeq.Id > 0 && !string.IsNullOrEmpty(numSeq.NumericProjectionColumn_Name))
                {
                    DbRepository dbRepository = new DbRepository(this.userInfo, this.auditService);
                    string json = dbRepository.GetById(entityName, recordId, true);
                    if (!string.IsNullOrEmpty(json))
                    {
                        JToken objects = JToken.Parse(json);
                        if (objects[numSeq.NumericProjectionColumn_Name] != null)
                        {
                            rpName = objects[numSeq.NumericProjectionColumn_Name].ToString();
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(rpName))
            {
                rpName = entityName + "_" + recordId.ToString();
            }
            return rpName;
        }
    }
}

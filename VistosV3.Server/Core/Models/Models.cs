using Core.ActionResults;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Core.Models
{

    public static class GlobalVar
    {
        public const string GdprAnonymText = "Anonymizovaný kontakt";
    }

    public class Event
    {
        public string id { get; set; }
        public string entityName { get; set; }
        public int entityId { get; set; }
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public bool allDay { get; set; }
        public string color { get; set; }
    }

    public class Column
    {
        public string ColumnName { get; set; }
        public string LocalizationString { get; set; }
        public DbColumnTypeEnum Type_FK { get; set; }
    }

    public class UserInfo
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string UserLanguage { get; set; }
        public int UserLanguageId { get; set; }
        public int ProfileId { get; set; }
        public int? ContactId { get; set; }
        public int? AccountId { get; set; }
        public int? DefaultEmailAccount_FK { get; set; }
        public string DefaultEmail { get; set; }

        public UserInfo(int userId, string userName, string userLanguage, int userLanguageId, int profileId)
        {
            this.UserId = userId;
            this.UserName = userName;
            this.UserLanguage = userLanguage;
            this.UserLanguageId = userLanguageId;
            this.ProfileId = profileId;
        }
    }

    public class ApiResponse
    {
        public ResponseStatusCode Status { get; set; }
        public string Message { get; set; }
        public string DataJSon { get; set; }

        public string GetRepositoryResponse()
        {
            if (!string.IsNullOrEmpty(DataJSon))
            {
                return $"{{ \"response_guid\": \"{Guid.NewGuid().ToString()}\", \"request_guid\": \"\", \"version\":\"3\", \"datetime\":{JsonConvert.SerializeObject(DateTime.Now)}, \"status\":\"" + Status.ToString() + "\", \"message\":\"" + Message + "\", \"data\": " + this.DataJSon + "}";
            }
            else
            {
                return $"{{ \"response_guid\": \"{Guid.NewGuid().ToString()}\", \"request_guid\": \"\", \"version\":\"3\", \"datetime\":{JsonConvert.SerializeObject(DateTime.Now)}, \"status\":\"" + Status.ToString() + "\", \"message\":\"" + Message + "\", \"data\": null}";
            }
        }
    }

    public class ApiExportResponse : ApiResponse
    {
        public DataTable DataTable { get; set; }
        public DataExportResult DataExportResult { get; set; }
    }

    public class ResponseBuilder
    {

        public ApiResponse ReturnRepositoryResponseError(string message)
        {
            return new ApiResponse() { Status = ResponseStatusCode.Error, Message = message };
        }

        public ApiResponse ReturnRepositoryResponseOK(string json, string message = "")
        {
            return new ApiResponse() { DataJSon = json, Status = ResponseStatusCode.OK, Message = message };
        }

        public ApiResponse ReturnRepositoryResponseWarning(string json, string message = "")
        {
            return new ApiResponse() { DataJSon = json, Status = ResponseStatusCode.Warning, Message = message };
        }

        public ApiResponse ReturnRepositoryResponseNotFound(string message)
        {
            return new ApiResponse() { DataJSon = null, Status = ResponseStatusCode.NotFound, Message = message };
        }
        public ApiResponse ReturnRepositoryResponseInvalidLogin(string message)
        {
            return new ApiResponse() { DataJSon = null, Status = ResponseStatusCode.InvalidLogin, Message = message };
        }
        public ApiResponse ReturnRepositoryResponseUnauthenticated(string message)
        {
            return new ApiResponse() { DataJSon = null, Status = ResponseStatusCode.Unauthenticated, Message = message };
        }
        public ApiResponse ReturnRepositoryResponseUnauthorized(string message = "")
        {
            return new ApiResponse() { DataJSon = null, Status = ResponseStatusCode.Unauthorized, Message = message };
        }

        public ApiResponse ReturnRepositoryResponseInvalidJson(IList<ValidationError> errList)
        {
            StringBuilder sb = new StringBuilder();
            if (errList != null && errList.Count > 0)
            {
                foreach (ValidationError validationError in errList)
                {
                    sb.AppendLine(validationError.Message + " - Path:" + validationError.Path);
                }
            }
            return new ApiResponse() { DataJSon = null, Status = ResponseStatusCode.InvalidJson, Message = sb.ToString() };
        }

        public ApiExportResponse ReturnRepositoryExportResponseOk(DataTable dataTable, string message = "")
        {
            return new ApiExportResponse() { DataTable = dataTable, Status = ResponseStatusCode.OK, Message = message };
        }
    }
}
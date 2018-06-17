using Core.Models;
using Core.Models.ApiRequest;
using Core.VistosDb;
using Core.VistosDb.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Services
{
    public static class Logger
    {
        public static void SaveLogError(LogLevel logLevel, IList<ValidationError> errList, Exception ex, ApiRequest request, UserInfo userInfo)
        {
            StringBuilder sb = new StringBuilder();
            if (errList != null && errList.Count > 0)
            {
                foreach (ValidationError validationError in errList)
                {
                    sb.AppendLine(validationError.Message + " - Path:" + validationError.Path);
                }
            }
            SaveLogError(logLevel, sb.ToString(), ex, request, userInfo);
        }
        public static void SaveLogError(LogLevel logLevel, string message, Exception ex, ApiRequest request, UserInfo userInfo)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(message);
            if (request != null)
            {
                try
                {
                    sb.AppendLine();
                    sb.AppendLine("ApiRequest");
                    sb.AppendLine("------------------------------------------");
                    sb.AppendLine(JsonConvert.SerializeObject(request, Formatting.Indented));
                }
                catch (Exception ex1)
                {
                    string s = ex1.Message;
                }
            }
            if (userInfo != null)
            {
                try
                {
                    sb.AppendLine();
                    sb.AppendLine("UserInfo");
                    sb.AppendLine("-----------------------------------------");
                    sb.AppendLine(JsonConvert.SerializeObject(userInfo, Formatting.Indented));
                }
                catch (Exception ex1)
                {
                    string s = ex1.Message;
                }
            }

            using (VistosDbContext ctx = new VistosDbContext())
            {
                Log log = new Log()
                {
                    Time = DateTime.Now,
                    Application = "Vistos v3",
                    Level = logLevel.ToString(),
                    Message = sb.ToString(),
                    RequestGuid = request != null ? request.RequestGuid : null,
                    Exception = ex != null ? ex.ToString() : null,
                    StackTrace = ex != null ? ex.StackTrace : null
                };
                ctx.Log.Add(log);
                ctx.SaveChanges();
            }
        }

        public static void SaveLogInfo(string requestGuid, string message)
        {
            using (VistosDbContext ctx = new VistosDbContext())
            {
                Log log = new Log()
                {
                    Time = DateTime.Now,
                    Application = "Vistos v3",
                    Level = LogLevel.Info.ToString(),
                    Message = message,
                    RequestGuid = requestGuid
                };
                ctx.Log.Add(log);
                ctx.SaveChanges();
            }
        }
        public static void SaveLogWarn(string requestGuid, string message)
        {
           using (VistosDbContext ctx = new VistosDbContext())
           {
               Log log = new Log()
               {
                   Time = DateTime.Now,
                   Application = "Vistos v3",
                   Level = LogLevel.Warning.ToString(),
                   Message = message,
                   RequestGuid = requestGuid
               };
               ctx.Log.Add(log);
               ctx.SaveChanges();
           }
        }

    }
}
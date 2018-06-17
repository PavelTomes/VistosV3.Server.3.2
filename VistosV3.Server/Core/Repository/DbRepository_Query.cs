using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Models;
using Core.QueryBuilder;
using Core.Services;
using Core.VistosDb.Objects;
using Core.VistosDb;
using NetStdTools;
using NetStdTools.ReportExecution;
using Core.Tools;

namespace Core.Repository
{
    public partial class DbRepository
    {

        public String GetItemsForAutocomplete(string entityName, string searchText, string filter)
        {
            String json = GetItemsForAutocomplete_Text(entityName, searchText, filter);

            return json;
        }

        private String GetItemsForAutocomplete_Text(string entityName, string searchText, string filter)
        {
            StringBuilder json = new StringBuilder();

            TemplateQueryBuilder queryBuilder = new TemplateQueryBuilder();
            string sql = queryBuilder.GetItemsForAutocompleteQuery(entityName, this.userInfo, filter);
            auditService.SetQueryBuilderSql(sql);
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                cmd.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
                cmd.Parameters.Add(new SqlParameter("@userLanguageId", this.userInfo.UserLanguageId));
                cmd.Parameters.Add(new SqlParameter("@searchText", searchText));
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    json.Append(rdr.GetString(0));
                }
            }
            return json.ToString();
        }


        public async Task<string> Remove(string entityName, int id, string parentProjectionName, int? parentEntityId, string gridMode)
        {
            ProjectionMethodMode projectionMethodMode = Settings.GetInstance.GetProjectionMethodMode(userInfo.ProfileId, entityName, "Remove");
            var result = "{}";
            switch (projectionMethodMode)
            {
                case ProjectionMethodMode.StoredProcedure:
                    result = null;
                    break;
                default:
                    result = Remove_Text(entityName, id, parentProjectionName, parentEntityId, gridMode);
                    break;
            }

            //**var trackChanges = await Worker.ExecuteTrackChanges(new TrackChangesWorkerSettings(trackChangesSettings, userInfo));
            //**
            //**var nws = new GenerateNotificationWorkerSettings(this.userInfo, trackChanges, DbConnectionString);
            //**Worker.ExecuteGenerateNotification(nws);
            return result;
        }

        private String Delete_StorageProcedure(string entityName, int id)
        {
            throw new Exception("Not Implemented");
        }

        private String Remove_Text(string entityName, int id, string parentProjectionName, int? parentEntityId, string gridMode)
        {
            string json = string.Empty;

            TemplateQueryBuilder queryBuilder = new TemplateQueryBuilder();
            string sql = queryBuilder.RemoveQuery(entityName, this.userInfo, parentProjectionName, parentEntityId, gridMode);
            auditService.SetQueryBuilderSql(sql);
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    SqlCommand cmd1 = new SqlCommand(sql, conn);
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                    cmd1.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                    cmd1.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
                    cmd1.Parameters.Add(new SqlParameter("@userLanguageId", this.userInfo.UserLanguageId));
                    cmd1.Parameters.Add(new SqlParameter("@id", id));
                    cmd1.Transaction = tran;
                    json = cmd1.ExecuteScalar().ToString();

                    ProjectionMethodMode projectionMethodMode = Settings.GetInstance.GetProjectionMethodMode(userInfo.ProfileId, entityName, "Remove");
                    if (projectionMethodMode == ProjectionMethodMode.StoredProcedureAfterQueryBuilder)
                    {
                        SqlCommand cmd2 = new SqlCommand("sp_api3_AfterUpdateOrInsert_" + entityName);
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                        cmd2.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                        cmd2.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
                        cmd2.Parameters.Add(new SqlParameter("@userLanguageId", this.userInfo.UserLanguageId));
                        cmd2.Parameters.Add(new SqlParameter("@entityName", entityName));
                        cmd2.Parameters.Add(new SqlParameter("@id", id));
                        cmd2.Connection = conn;
                        cmd2.Transaction = tran;
                        cmd2.ExecuteNonQuery();
                    }
                    tran.Commit();
                }

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            return json.ToString();
        }

        public String GetById(string projectionName, int id, bool simple)
        {
            if (id == 0)
            {
                return this.NewEntity(projectionName).ToString();
            }
            else if (id > 0)
            {
                StringBuilder json = new StringBuilder();
                using (SqlConnection conn = new SqlConnection(DbConnectionString))
                {
                    conn.Open();
                    var cmd = this.GetRecordByIdCommand(projectionName, id, userInfo.ProfileId, simple);
                    cmd.Connection = conn;
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        json.Append(rdr.GetString(0));
                    }

                    if (projectionName == "Email")
                    {
                        // Dotáhnout případně chybějící tělo e-mailu z IMAPu nebo Dropboxu a doplnit do JSONu
                        return FillEmailBody(id, json.ToString());
                    }

                    return json.ToString();
                }
            }
            else
            {
                return "{}";
            }
        }

        private string FillEmailBody(int id, string json)
        {
            JObject jObj = JObject.Parse(json);

            string body = (string)((jObj["Body"] as JValue)?.Value);
            string bodyText = (string)((jObj["BodyText"] as JValue)?.Value);

            if (body == null && bodyText == null)
            {
                bool bodyStoredInDropbox = (bool)((jObj["BodyStoredInDropbox"] as JValue)?.Value ?? false);

                if (bodyStoredInDropbox && !string.IsNullOrEmpty(Settings.GetInstance.SystemSettings.DropBoxSecurityToken))
                {
                    DropBoxService dropbox = new DropBoxService(Settings.GetInstance.SystemSettings.DropBoxSecurityToken);
                    jObj["Body"] = dropbox.DownloadEmailBody(id, false);
                    jObj["BodyText"] = dropbox.DownloadEmailBody(id, true);

                    return jObj.ToString();
                }

                string messageID = (string)((jObj["MessageID"] as JValue)?.Value);

                if (!string.IsNullOrEmpty(messageID))
                {
                    //**    Service.Model.EmailViewModel model = new Service.Model.EmailViewModel(id, userInfo);
                    //**    model.RefreshBodyFromIMAP();
                    //**
                    //**    jObj["Body"] = model.ApiEmail.Body;
                    //**    jObj["BodyText"] = model.ApiEmail.BodyText;
                    //**
                    return jObj.ToString();
                }
            }

            return json;
        }

        //public String GetRecordById(string projectionName, int? id, int profileId)
        //{
        //    if (!id.HasValue)
        //    {
        //        throw new ArgumentNullException(nameof(id), String.Format("Method: {0} cannot has null parameter: {1}.", "GetRecordById", nameof(id)));
        //    }

        //    StringBuilder json = new StringBuilder();
        //    using (SqlConnection conn = new SqlConnection(DbConnectionString))
        //    {
        //        conn.Open();
        //        var cmd = this.GetRecordByIdCommand(projectionName, id.Value, profileId);
        //        cmd.Connection = conn;
        //        SqlDataReader rdr = cmd.ExecuteReader();
        //        while (rdr.Read())
        //        {
        //            json.Append(rdr.GetString(0));
        //        }

        //        return json.ToString();
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectionName"></param>
        /// <param name="recordId"></param>
        /// <returns>SqlDataReader</returns>
        public SqlCommand GetRecordByIdCommand(string projectionName, int recordId, int profileId, bool simple)
        {
            ProjectionMethodMode projectionMethodMode = Settings.GetInstance.GetProjectionMethodMode(profileId, projectionName, "GetById");
            switch (projectionMethodMode)
            {
                case ProjectionMethodMode.StoredProcedure:
                    return GetById_StorageProcedure(projectionName, recordId, simple);
                default:
                    return GetById_Text(projectionName, recordId, simple);
            }
        }

        public String CreateEntityFrom(string entityNameFrom, int entityIdFrom, string entityNameTarget, ProjectionActionType actionTypeName, ProjectionMethodMode methodMode, ProjectionActionResultType resultType, JObject extData)
        {
            if (methodMode == ProjectionMethodMode.QueryBuilder)
            {
                JObject newE = NewEntity(entityNameTarget);
                JObject newEF = JObject.Parse(NewEntityFrom(entityNameFrom, entityIdFrom, entityNameTarget, actionTypeName, methodMode, resultType, extData));
                newE.Merge(newEF);
                if (entityNameTarget == "Email")
                {
                    ExportType exportType = ExportType.PDF;
                    string rdlReportName = string.Empty;

                    if (extData != null)
                    {
                        foreach (KeyValuePair<string, JToken> property in extData)
                        {
                            if (property.Key == "ExportType")
                                exportType = (ExportType)Enum.Parse(typeof(ExportType), property.Value.ToString(), true);
                            if (property.Key == "RdlReportName")
                                rdlReportName = property.Value.ToString();
                        }
                    }

                    JObject emailAttachment = SaveEntityReportAsEmailAttachment(entityNameFrom, entityIdFrom, exportType, rdlReportName);
                    newE.Merge(emailAttachment);
                }
                return newE.ToString();
            }
            if (methodMode == ProjectionMethodMode.StoredProcedure)
            {
                string jsonResult = NewEntityFrom(entityNameFrom, entityIdFrom, entityNameTarget, actionTypeName, methodMode, resultType, extData);
                return jsonResult;
            }
            return "{}";
        }

        private JObject SaveEntityReportAsEmailAttachment(string entityName, int entityId, ExportType exportType, string rdlReportName)
        {
            string paramName = "id";
            if (!string.IsNullOrEmpty(rdlReportName) && !string.IsNullOrEmpty(paramName))
            {
                ReportGenerator gen = new ReportGenerator(
                 Settings.GetInstance.SystemSettings.ReportServerUrl + "/ReportExecution2005.asmx",
                 Settings.GetInstance.SystemSettings.ReportServerUserName,
                 Settings.GetInstance.SystemSettings.ReportServerPassword
                 );
                string reportPath = Settings.GetInstance.SystemSettings.ReportServerFormsPath;
                if (!reportPath.EndsWith("/")) reportPath += "/";

                ParameterValue[] parameters = {
                            new ParameterValue { Name = "id", Value = entityId.ToString() },
                            new ParameterValue { Name = "userId", Value = userInfo.UserId.ToString() },
                            new ParameterValue { Name = "codeLanguage", Value = userInfo.UserLanguage },
                            new ParameterValue { Name = "entityName", Value = entityName }
                        };

                Task<byte[]> repDataAsync = gen.RenderReport(reportPath + rdlReportName, parameters, exportType.ToString());
                byte[] repData = repDataAsync.Result;

                if (repData != null && repData.Length > 0)
                {
                    SqlReport sqlReport = new SqlReport(userInfo, this.auditService);
                    string fileName = sqlReport.GetReportName(entityName, entityId) + "." + ReportHelper.GetExtension(exportType);
                    string fileType = ReportHelper.GetMimeType(exportType);

                    EmailAttachment item = new EmailAttachment();
                    item.Deleted = false;
                    item.Created = DateTime.Now;
                    item.CreatedBy_FK = userInfo.UserId;
                    item.Modified = DateTime.Now;
                    item.FileName = fileName;
                    item.Data = repData;
                    item.DataLength = repData.Length;
                    item.Type = fileType;

                    using (VistosDbContext ctx = new VistosDbContext())
                    {
                        ctx.EmailAttachment.Add(item);
                        ctx.SaveChanges();
                    }
                    return JObject.Parse($"{{\"EmailAttachment\":[{{\"Id\": {item.Id.ToString()}, \"Deleted\": false, \"FileName\": \"{fileName}\", \"Type\": \"{fileType}\"}}]}}");
                }
            }
            return JObject.Parse($"{{\"EmailAttachment\":[]}}");
        }
        private string NewEntityFrom(string entityNameFrom, int entityIdFrom, string entityNameTarget, ProjectionActionType actionTypeName, ProjectionMethodMode methodMode, ProjectionActionResultType resultType, JObject extData)
        {
            StringBuilder json = new StringBuilder();
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = this.getNewEntityFromCommand(entityNameFrom, entityIdFrom, entityNameTarget, actionTypeName, methodMode, resultType, extData.ToString(Formatting.None));
                cmd.Connection = conn;
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        json.Append(reader.GetString(0));
                    }
                }
            }
            if (string.IsNullOrEmpty(json.ToString()))
                return "[]";
            return json.ToString();
        }

        private SqlCommand getNewEntityFromCommand(string entityNameFrom, int entityIdFrom, string entityNameTarget, ProjectionActionType actionTypeName, ProjectionMethodMode methodMode, ProjectionActionResultType resultType, String extData)
        {
            if (methodMode == ProjectionMethodMode.QueryBuilder)
            {
                TemplateQueryBuilder queryBuilder = new TemplateQueryBuilder();
                string sql = queryBuilder.CreateFromEntityQuery(entityNameFrom, entityNameTarget, actionTypeName, this.userInfo);
                auditService.SetQueryBuilderSql(sql);

                SqlCommand cmd = new SqlCommand(sql);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                cmd.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
                cmd.Parameters.Add(new SqlParameter("@userLanguageId", this.userInfo.UserLanguageId));
                cmd.Parameters.Add(new SqlParameter("@id", entityIdFrom));
                cmd.Parameters.Add(new SqlParameter("@extData", extData));

                return cmd;
            }
            if (methodMode == ProjectionMethodMode.StoredProcedure)
            {
                SqlCommand cmd = new SqlCommand("sp_api3_createEntityFrom");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                cmd.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
                cmd.Parameters.Add(new SqlParameter("@entityIdFrom", entityIdFrom));
                cmd.Parameters.Add(new SqlParameter("@entityNameFrom", entityNameFrom));
                cmd.Parameters.Add(new SqlParameter("@entityNameTarget", entityNameTarget));
                cmd.Parameters.Add(new SqlParameter("@actionTypeName", actionTypeName));
                cmd.Parameters.Add(new SqlParameter("@extData", extData));
                return cmd;
            }
            return null;
        }

        private JObject NewEntity(string entityName)
        {
            StringBuilder json = new StringBuilder();
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                var cmd = this.getNewEntityCommand(entityName);
                cmd.Connection = conn;
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    if (!rdr.IsDBNull(0))
                    {
                        json.Append(rdr.GetString(0));
                    }
                }
            }
            return JObject.Parse(json.ToString());
        }


        private SqlCommand getNewEntityCommand(String entityName)
        {
            TemplateQueryBuilder queryBuilder = new TemplateQueryBuilder();
            string sql = queryBuilder.NewEntityQuery(entityName, this.userInfo);
            auditService.SetQueryBuilderSql(sql);

            SqlCommand cmd = new SqlCommand(sql);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
            cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
            cmd.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
            cmd.Parameters.Add(new SqlParameter("@userLanguageId", this.userInfo.UserLanguageId));
            return cmd;
        }

        private SqlCommand GetById_StorageProcedure(string entityName, int id, bool simple)
        {
            StringBuilder json = new StringBuilder();

            //using (SqlConnection conn = new SqlConnection(DbConnectionString))
            //{
            //    conn.Open();
            SqlCommand cmd = new SqlCommand("sp_api3_getData");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
            cmd.Parameters.Add(new SqlParameter("@entityName", entityName));
            cmd.Parameters.Add(new SqlParameter("@query", id));
            return cmd;
            //SqlDataReader rdr = cmd.ExecuteReader();
            //while (rdr.Read())
            //{
            //json.Append(rdr.GetString(0));
            //}
            //}
            //return json.ToString();
        }


        private SqlCommand GetById_Text(string projectionName, int id, bool simple)
        {
            StringBuilder json = new StringBuilder();

            TemplateQueryBuilder queryBuilder = new TemplateQueryBuilder();
            string sql = queryBuilder.GetByIdQuery(projectionName, this.userInfo, simple);
            auditService.SetQueryBuilderSql(sql);

            SqlCommand cmd = new SqlCommand(sql);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
            cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
            cmd.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
            cmd.Parameters.Add(new SqlParameter("@userLanguageId", this.userInfo.UserLanguageId));
            cmd.Parameters.Add(new SqlParameter("@id", id));

            return cmd;
        }

        public async Task<string> GdprDeleteData(string projectionName, int recordId, string[] columns)
        {
            if (columns == null || columns.Length == 0)
            {
                return "{}";
            }
            List<vwProjectionColumn> projectionColumns = Settings.GetInstance.GetVwProjectionColumnList(userInfo.ProfileId, projectionName)
                .Where(c => c.Column_GdprStatus == 2 && columns.Contains(c.ProjectionColumn_Name)).ToList();

            string originalJsonData;
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                originalJsonData = this.GetProjectionDataRecord(projectionName, this.userInfo.ProfileId, recordId, conn, null);
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            if (!string.IsNullOrEmpty(originalJsonData))
            {
                JObject jsonObj = JObject.Parse(originalJsonData);

                foreach (vwProjectionColumn column in projectionColumns)
                {
                    switch (column.DbColumnType_Id)
                    {
                        case (int)DbColumnTypeEnum.String:
                            {
                                switch (column.DbColumn_Name)
                                {
                                    case "FirstName":
                                    case "LastName":
                                    case "Name":
                                        {
                                            jsonObj[column.ProjectionColumn_Name] = Core.Models.GlobalVar.GdprAnonymText;
                                        }
                                        break;
                                    default:
                                        {
                                            jsonObj[column.ProjectionColumn_Name] = null;
                                        }
                                        break;
                                }
                            }
                            break;
                        case (int)DbColumnTypeEnum.Bit:
                            {
                                jsonObj[column.ProjectionColumn_Name] = false;
                            }
                            break;
                        case (int)DbColumnTypeEnum.PhoneNumber:
                            {
                                jsonObj[column.ProjectionColumn_Name] = "000 000 000";
                            }
                            break;
                        default:
                            {
                                jsonObj[column.ProjectionColumn_Name] = null;
                            }
                            break;
                    }
                }

                await this.Update(projectionName, recordId, jsonObj);

                List<int> dbColumnsIds = projectionColumns.Select(c => c.DbColumn_Id).Distinct().ToList();
                Dictionary<int, vwProjectionColumn> dbColumnsTypes = projectionColumns.Distinct().ToDictionary(k => k.DbColumn_Id, v => v);
                using (VistosDbContext ctx = new VistosDbContext())
                {

                    List<TrackChanges> trackChanges = ctx.TrackChanges.Where(t =>
                        t.DbColumn_FK.HasValue
                        && dbColumnsIds.Contains(t.DbColumn_FK.Value)
                        && t.RecordId == recordId
                    ).ToList();
                    if (trackChanges != null && trackChanges.Count > 0)
                    {
                        foreach (TrackChanges trackChange in trackChanges)
                        {
                            vwProjectionColumn col1 = dbColumnsTypes[trackChange.DbColumn_FK.Value];
                            switch (col1.DbColumnType_Id)
                            {
                                case (int)DbColumnTypeEnum.String:
                                    {
                                        switch (col1.DbColumn_Name)
                                        {
                                            case "FirstName":
                                            case "LastName":
                                            case "Name":
                                                {
                                                    if (!string.IsNullOrEmpty(trackChange.OldValue))
                                                        trackChange.OldValue = Core.Models.GlobalVar.GdprAnonymText;
                                                    if (!string.IsNullOrEmpty(trackChange.NewValue))
                                                        trackChange.NewValue = Core.Models.GlobalVar.GdprAnonymText;
                                                }
                                                break;
                                            default:
                                                {
                                                    trackChange.OldValue = null;
                                                    trackChange.NewValue = null;
                                                }
                                                break;
                                        }
                                    }
                                    break;
                                case (int)DbColumnTypeEnum.PhoneNumber:
                                    {
                                        if (!string.IsNullOrEmpty(trackChange.OldValue))
                                            trackChange.OldValue = "000 000 000";
                                        if (!string.IsNullOrEmpty(trackChange.NewValue))
                                            trackChange.NewValue = "000 000 000";
                                    }
                                    break;
                                default:
                                    {
                                        trackChange.OldValue = null;
                                        trackChange.NewValue = null;
                                        trackChange.OldValueCaption = null;
                                        trackChange.NewValueCaption = null;
                                        trackChange.ReferenceId = null;
                                    }
                                    break;
                            }
                        }
                    }
                    ctx.SaveChanges();
                }
            }
            return "{}";
        }

        public async Task<string> Update(string projectionName, int recordId, JObject json)
        {
            //**var trackChangesSettings = TrackChangesService.CreateSettings(this.userInfo, projectionName, recordId, TrackChangesAction.UPDATE);

            bool useTrackChanges = this.UseTrackChanges();
            IList<TrackChanges> trackChanges = null;
            ProjectionMethodMode projectionMethodMode = Settings.GetInstance.GetProjectionMethodMode(userInfo.ProfileId, projectionName, "Update");

            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    // Load original record data
                    //**if (useTrackChanges) { trackChangesSettings.SetOriginalJsonData(this.GetProjectionDataRecord(projectionName, this.userInfo.ProfileId, recordId, conn, tran)); }

                    // Update record
                    var updateCommand = GetUpdateCommand(projectionName, recordId, json);
                    updateCommand.Connection = conn;
                    updateCommand.Transaction = tran;
                    updateCommand.ExecuteNonQuery();

                    if (projectionMethodMode == ProjectionMethodMode.StoredProcedureAfterQueryBuilder)
                    {
                        SqlCommand cmd = new SqlCommand("sp_api3_AfterUpdateOrInsert_" + projectionName);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                        cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                        cmd.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
                        cmd.Parameters.Add(new SqlParameter("@userLanguageId", this.userInfo.UserLanguageId));
                        cmd.Parameters.Add(new SqlParameter("@entityName", projectionName));
                        cmd.Parameters.Add(new SqlParameter("@id", recordId));
                        cmd.Connection = conn;
                        cmd.Transaction = tran;
                        cmd.ExecuteNonQuery();
                    }

                    // Load ended record
                    //**if (useTrackChanges) { trackChangesSettings.SetNewJsonData(this.GetProjectionDataRecord(projectionName, this.userInfo.ProfileId, recordId, conn, tran)); }

                    //**// create track changes
                    //**if (useTrackChanges
                    //**    && trackChangesSettings.SaveInTransaction) { trackChanges = await Worker.ExecuteTrackChanges(new TrackChangesWorkerSettings(trackChangesSettings, userInfo)); }

                    tran.Commit();
                }

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            //**if (useTrackChanges && !trackChangesSettings.SaveInTransaction)
            //**{
            //**    //**trackChanges = await Worker.ExecuteTrackChanges(new TrackChangesWorkerSettings(trackChangesSettings, userInfo));
            //**}

            if (Settings.GetInstance.SystemSettings.PohodaConnectorEnabled && trackChanges.Count > 0)
            {
                vwProjectionColumn columnSyncToPohoda = Settings.GetInstance.GetVwProjectionColumnList(userInfo.ProfileId, projectionName).Where(c => c.DbColumn_Name == "Pohoda_SyncToPohoda").FirstOrDefault();
                if (columnSyncToPohoda != null && columnSyncToPohoda.DbColumn_Id > 0)
                {
                    vwPohodaDbObjectConfiguration pohodaDbObjectConfiguration = Settings.GetInstance.VwPohodaDbObjectConfigurationList.Where(d => d.DbObject_Id == columnSyncToPohoda.DbObject_Id).FirstOrDefault();
                    if (pohodaDbObjectConfiguration != null && pohodaDbObjectConfiguration.Conf_PohodaImportEnabled)
                    {
                        //**DAL.Database.TrackChanges trackChangesColumnSyncToPohoda = trackChanges.Where(c => c.DbColumn_FK == columnSyncToPohoda.DbColumn_Id).FirstOrDefault();
                        //**if (trackChangesColumnSyncToPohoda != null
                        //**    && (string.IsNullOrEmpty(trackChangesColumnSyncToPohoda.OldValue) || trackChangesColumnSyncToPohoda.OldValue.ToLower() == "false")
                        //**    && !string.IsNullOrEmpty(trackChangesColumnSyncToPohoda.NewValue)
                        //**    && trackChangesColumnSyncToPohoda.NewValue.ToLower() == "true"
                        //**    )
                        //**{
                        //**    Workers.PohodaImportWorker pohodaImportWorker = new Workers.PohodaImportWorker(projectionName, recordId);
                        //**    pohodaImportWorker.Init();
                        //**    pohodaImportWorker.Execute();
                        //**}
                    }
                }
            }


            //**if (Settings.GetInstance.SystemSettings.RemindersEnabled && trackChanges.Count > 0)
            {
                //**var rws = new ReminderWorkerSettings(userInfo,
                //**                                     trackChangesSettings.Projection.Projection_Id,
                //**                                     trackChangesSettings.Projection.DbObject_Id,
                //**                                     recordId,
                //**                                     trackChanges,
                //**                                     trackChangesSettings.ProjectionColumns);
                //**
                //**Worker.ExecuteReminders(rws);
            }

            //**var nws = new GenerateNotificationWorkerSettings(this.userInfo, trackChanges, DbConnectionString);
            //**Worker.ExecuteGenerateNotification(nws);

            if (projectionMethodMode == ProjectionMethodMode.PlugInAfterQueryBuilder)
            {
                string recordJson = this.GetById(projectionName, recordId, false);
                //**this.CallCustomMethod("AfterUpdate", projectionName, recordId, JObject.Parse(recordJson), trackChanges);
            }

            return "{}";
        }

        private bool UseTrackChanges()
        {
            if (Settings.GetInstance.SystemSettings.TrackChangesEnabled)
            {
                return true;
            }
            else
            {
                if (Settings.GetInstance.SystemSettings.RemindersEnabled)
                    return true;
            }

            return false;
        }

        private String GetProjectionDataRecord(String projectionName, int profileId, int recordId, SqlConnection conn, SqlTransaction tran = null)
        {
            var getRecordCommandById = this.GetRecordByIdCommand(projectionName, recordId, profileId, false);
            getRecordCommandById.Connection = conn;
            getRecordCommandById.Transaction = tran;
            var newRecordReader = getRecordCommandById.ExecuteReader();
            var json = GetJsonDataFromReader(newRecordReader);
            newRecordReader.Close();
            return json;
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <param name="json"></param>
        /// <returns>NonQuery</returns>
        private SqlCommand GetUpdateCommand(string entityName, int id, JObject json)
        {
            ProjectionMethodMode projectionMethodMode = Settings.GetInstance.GetProjectionMethodMode(userInfo.ProfileId, entityName, "Update");
            switch (projectionMethodMode)
            {
                case ProjectionMethodMode.StoredProcedure:
                    return Update_StorageProcedure(entityName, id, json);
                default:
                    return Update_Text(entityName, id, json);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <param name="json"></param>
        /// <returns>NonQuery</returns>
        private SqlCommand Update_StorageProcedure(string entityName, int id, JObject json)
        {
            SqlCommand cmd = new SqlCommand("sp_api3_updateData");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
            cmd.Parameters.Add(new SqlParameter("@entityName", entityName));
            cmd.Parameters.Add(new SqlParameter("@id", id));
            cmd.Parameters.Add(new SqlParameter("@json", json.ToString(Formatting.None)));
            //cmd.ExecuteNonQuery();

            return cmd;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <param name="json"></param>
        /// <returns>NonQuery</returns>
        private SqlCommand Update_Text(string entityName, int id, JObject json)
        {
            TemplateQueryBuilder queryBuilder = new TemplateQueryBuilder();
            string sql = queryBuilder.GetUpdateQuery(entityName, this.userInfo);
            auditService.SetQueryBuilderSql(sql);

            SqlCommand cmd = new SqlCommand(sql);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
            cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
            cmd.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
            cmd.Parameters.Add(new SqlParameter("@userLanguageId", this.userInfo.UserLanguageId));
            cmd.Parameters.Add(new SqlParameter("@id", id));
            cmd.Parameters.Add(new SqlParameter("@json", json.ToString(Formatting.None)));

            return cmd;
            //    cmd.ExecuteNonQuery();

        }



        public async Task<string> Add(string entityName, JObject json)
        {
            ProjectionMethodMode projectionMethodMode = Settings.GetInstance.GetProjectionMethodMode(userInfo.ProfileId, entityName, "Add");
            String jsonResult = null;
            switch (projectionMethodMode)
            {
                case ProjectionMethodMode.QueryBuilder:
                case ProjectionMethodMode.PlugInAfterQueryBuilder:
                case ProjectionMethodMode.StoredProcedureAfterQueryBuilder:
                    jsonResult = Add_Text(entityName, json, projectionMethodMode);
                    break;
                case ProjectionMethodMode.StoredProcedure:
                    jsonResult = Add_StorageProcedure(entityName, json.ToString(Formatting.None));
                    break;
                case ProjectionMethodMode.PlugIn:
                    jsonResult = this.CallCustomMethod("Create", entityName, null, json, null);
                    break;

                default:
                    break;
            }

            bool useTrackChanges = this.UseTrackChanges();
            //**if (!String.IsNullOrEmpty(jsonResult))
            //**{
            //**    int newRecordId = (int)JObject.Parse(jsonResult).GetValue("EntityID");
            //**    string projectionName = (string)JObject.Parse(jsonResult).GetValue("EntityName");
            //**
            //**    if (newRecordId > 0)
            //**    {
            //**        var trackChangesSettings = TrackChangesService.CreateSettings(this.userInfo, projectionName);
            //**        trackChangesSettings.Action = TrackChangesAction.INSERT;
            //**
            //**        using (var sqlConn = new SqlConnection(DbConnectionString))
            //**        {
            //**            sqlConn.Open();
            //**            string recordJson = this.GetProjectionDataRecord(trackChangesSettings.Projection.Projection_Name, trackChangesSettings.ProfileId, newRecordId, sqlConn);
            //**            if (string.IsNullOrEmpty(recordJson))
            //**            {
            //**                recordJson = "{}";
            //**            }
            //**            trackChangesSettings.SetNewJsonData(recordJson);
            //**            trackChangesSettings.SetOriginalJsonData("{}");
            //**            trackChangesSettings.RecordId = newRecordId;
            //**            trackChangesSettings.Action = TrackChangesAction.INSERT;
            //**            sqlConn.Close();
            //**        }
            //**
            //**        var trackChanges = await Worker.ExecuteTrackChanges(new TrackChangesWorkerSettings(trackChangesSettings, userInfo));
            //**
            //**        var rws = new ReminderWorkerSettings(userInfo,
            //**                                             trackChangesSettings.Projection.Projection_Id,
            //**                                             trackChangesSettings.Projection.DbObject_Id,
            //**                                             newRecordId,
            //**                                             trackChanges,
            //**                                             trackChangesSettings.ProjectionColumns);
            //**        rws.NewRecordData = json.ToString(Formatting.None);
            //**        rws.IsNewRecord = true;
            //**
            //**        Worker.ExecuteReminders(rws);
            //**
            //**        var nws = new GenerateNotificationWorkerSettings(this.userInfo, trackChanges, DbConnectionString);
            //**        Worker.ExecuteGenerateNotification(nws);
            //**    }
            //**}

            return jsonResult;
        }

        private String Add_StorageProcedure(string entityName, string jsonData)
        {
            StringBuilder json = new StringBuilder();
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_addData_" + entityName, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                cmd.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
                cmd.Parameters.Add(new SqlParameter("@entityName", entityName));
                cmd.Parameters.Add(new SqlParameter("@json", jsonData));

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    json.Append(rdr.GetString(0));
                }
            }
            return json.ToString();
        }

        private String Add_Text(string entityName, JObject json, ProjectionMethodMode projectionMethodMode)
        {
            string jsonResult = string.Empty;

            TemplateQueryBuilder queryBuilder = new TemplateQueryBuilder();
            string sql = queryBuilder.GetInsertQuery(entityName, this.userInfo);
            auditService.SetQueryBuilderSql(sql);

            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();

                using (SqlTransaction tran = conn.BeginTransaction())
                {
                    SqlCommand cmd1 = new SqlCommand(sql, conn);
                    cmd1.CommandType = CommandType.Text;
                    cmd1.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                    cmd1.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                    cmd1.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
                    cmd1.Parameters.Add(new SqlParameter("@userLanguageId", this.userInfo.UserLanguageId));
                    cmd1.Parameters.Add(new SqlParameter("@json", json.ToString(Formatting.None)));
                    cmd1.Transaction = tran;
                    string newID = cmd1.ExecuteScalar().ToString();

                    if (projectionMethodMode == ProjectionMethodMode.StoredProcedureAfterQueryBuilder)
                    {
                        SqlCommand cmd2 = new SqlCommand("sp_api3_AfterUpdateOrInsert_" + entityName);
                        cmd2.CommandType = CommandType.StoredProcedure;
                        cmd2.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                        cmd2.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                        cmd2.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
                        cmd2.Parameters.Add(new SqlParameter("@userLanguageId", this.userInfo.UserLanguageId));
                        cmd2.Parameters.Add(new SqlParameter("@entityName", entityName));
                        cmd2.Parameters.Add(new SqlParameter("@id", int.Parse(newID)));
                        cmd2.Transaction = tran;
                        cmd2.Connection = conn;
                        cmd2.ExecuteNonQuery();
                    }
                    tran.Commit();
                    if (projectionMethodMode == ProjectionMethodMode.PlugInAfterQueryBuilder)
                    {
                        string recordJson = this.GetById(entityName, int.Parse(newID), false);
                        this.CallCustomMethod("AfterCreate", entityName, int.Parse(newID), JObject.Parse(recordJson), null);
                    }

                    jsonResult = $"{{\"EntityName\": \"{entityName.Replace("QuickCreate", "")}\", \"EntityID\": {newID}}}";
                }

                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
            return jsonResult;
        }

        public String AddManyToMany(string projectionName, int[] entityIds, string parentProjectionName, int parentEntityId)
        {
            TemplateQueryBuilder queryBuilder = new TemplateQueryBuilder();
            string sql = queryBuilder.GetInsertManyTomanyQuery(projectionName, parentProjectionName, this.userInfo);
            auditService.SetQueryBuilderSql(sql);

            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@parentId", parentEntityId));
                cmd.Parameters.Add(new SqlParameter("@ids", String.Join(",", entityIds)));
                cmd.ExecuteNonQuery();
            }

            return "{}";
        }

        public String GetGridCount(
            string entityName,
            string parentProjectionName,
            int? parentEntityId,
            string projectionRelationName
            )
        {
            //ProjectionMethodMode projectionMethodMode = Settings.GetInstance.GetProjectionMethodMode(userInfo.ProfileId, entityName, "GetGridCount");
            //switch (projectionMethodMode)
            //{
            //    case ProjectionMethodMode.StoredProcedure:
            //        return GetPage_StorageProcedure(entityName, parentProjectionName, parentEntityId, projectionRelationName);
            //    default:
            return GetGridCount_Text(entityName, parentProjectionName, parentEntityId, projectionRelationName);
            //}
        }

        public String GetPage(
            string entityName,
            int draw,
            int start,
            int length,
            string sortOrderColumnName,
            string sortOrderDirection,
            string fulltext,
            string filter,
            string parentProjectionName,
            int? parentEntityId,
            Column[] columns,
            string projectionRelationName,
            string gridMode
            )
        {

            ProjectionMethodMode projectionMethodMode = Settings.GetInstance.GetProjectionMethodMode(userInfo.ProfileId, entityName, "GetPage");
            switch (projectionMethodMode)
            {
                case ProjectionMethodMode.StoredProcedure:
                    return GetPage_StorageProcedure(
                             entityName,
                            draw,
                            start,
                            length,
                            sortOrderColumnName,
                            sortOrderDirection,
                            fulltext,
                            filter,
                            parentProjectionName,
                            parentEntityId.HasValue ? parentEntityId.Value : 0
                        );
                default:
                    return GetPage_Text(
                             entityName,
                            draw,
                            start,
                            length,
                            sortOrderColumnName,
                            sortOrderDirection,
                            fulltext,
                            filter,
                            parentProjectionName,
                            parentEntityId,
                            columns,
                            projectionRelationName,
                            gridMode
                        );
            }
        }

        private String GetPage_StorageProcedure(
            string entityName,
            int draw,
            int start,
            int length,
            string sortOrderColumnName,
            string sortOrderDirection,
            string fulltext,
            string filter,
            string parentProjectionName,
            int parentEntityId)
        {
            string jsonResult = string.Empty;

            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_GetPage", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@entityName", entityName));
                cmd.Parameters.Add(new SqlParameter("@draw", draw));
                cmd.Parameters.Add(new SqlParameter("@start", start));
                cmd.Parameters.Add(new SqlParameter("@length", length));
                cmd.Parameters.Add(new SqlParameter("@sortOrderName", sortOrderColumnName));
                cmd.Parameters.Add(new SqlParameter("@sortOrderDirection", sortOrderDirection));
                cmd.Parameters.Add(new SqlParameter("@filter", filter));
                cmd.Parameters.Add(new SqlParameter("@parentProjectionName", parentProjectionName));
                cmd.Parameters.Add(new SqlParameter("@parentEntityId", parentEntityId));

                SqlDataReader rdr = cmd.ExecuteReader();
                StringBuilder json = new StringBuilder();
                rdr.Read();
                json.Append(rdr.GetString(0));
                while (rdr.Read())
                {
                    json.Append(rdr.GetString(0));
                }

                jsonResult = json.ToString();
                if (!jsonResult.EndsWith("]}]}") && !jsonResult.Contains("\"data\":["))
                    jsonResult = jsonResult.Replace("}]", ",\"data\":[]}]");
            }
            return jsonResult;
        }

        private String GetGridCount_Text(
            string entityName,
            string parentProjectionName,
            int? parentEntityId,
            string projectionRelationName
           )
        {
            string count = "0";
            if (!string.IsNullOrEmpty(entityName) && !string.IsNullOrEmpty(parentProjectionName) && parentEntityId.HasValue && !string.IsNullOrEmpty(projectionRelationName))
            {
                TemplateQueryBuilder queryBuilder = new TemplateQueryBuilder();
                string sql = queryBuilder.GetGridCountQuery(
                            this.userInfo,
                            entityName,
                            parentProjectionName,
                            parentEntityId.Value,
                            projectionRelationName
                    );
                auditService.SetQueryBuilderSql(sql);

                using (SqlConnection conn = new SqlConnection(DbConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(sql, conn);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                    cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                    cmd.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
                    cmd.Parameters.Add(new SqlParameter("@userLanguageId", this.userInfo.UserLanguageId));
                    count = cmd.ExecuteScalar().ToString();
                }
            }
            return count;
        }

        private String GetPage_Text(
            string entityName,
            int draw,
            int start,
            int length,
            string sortOrderColumnName,
            string sortOrderDirection,
            string fulltext,
            string filter,
            string parentProjectionName,
            int? parentEntityId,
            Column[] columns,
            string projectionRelationName,
            string gridMode
            )
        {
            string jsonFinal = string.Empty;
            string[] columnsArr = columns.Select(c => c.ColumnName).ToArray();
            string sql = string.Empty;
            TemplateQueryBuilder queryBuilder = new TemplateQueryBuilder();

            if (entityName == "EntityActivity")
            {
                sql = queryBuilder.GetPageQuery_EntityActivity(
                        this.userInfo,
                        draw,
                        start,
                        length,
                        sortOrderColumnName,
                        sortOrderDirection,
                        filter,
                        parentProjectionName,
                        parentEntityId,
                        columnsArr
                );
            }
            else
            {
                sql = queryBuilder.GetPageQuery(
                        this.userInfo,
                        entityName,
                        draw,
                        start,
                        length,
                        sortOrderColumnName,
                        sortOrderDirection,
                        filter,
                        parentProjectionName,
                        parentEntityId,
                        columnsArr,
                        projectionRelationName,
                        gridMode
                );
            }
            auditService.SetQueryBuilderSql(sql);

            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                cmd.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
                cmd.Parameters.Add(new SqlParameter("@userLanguageId", this.userInfo.UserLanguageId));

                SqlDataReader rdr = cmd.ExecuteReader();
                StringBuilder json = new StringBuilder();
                while (rdr.Read())
                {
                    json.Append(rdr.GetString(0));
                }

                jsonFinal = json.ToString();
                if (!jsonFinal.EndsWith("]}]}") && !jsonFinal.Contains("\"data\":["))
                    jsonFinal = jsonFinal.Replace("}]", ",\"data\":[]}]");
            }
            return jsonFinal;
        }

        public DataTable GetExportDataTable_Text(
            string entityName,
            string sortOrderColumnName,
            string sortOrderDirection,
            string filter,
            Column[] columns,
            Column[] columnsVisible,
            string parentProjectionName,
            int? parentEntityId,
            string projectionRelationName,
            string gridMode
            )
        {
            string jsonFinal = string.Empty;
            string[] columnsArr = columnsVisible.Select(c => c.ColumnName).ToArray();

            TemplateQueryBuilder queryBuilder = new TemplateQueryBuilder();
            string sql = queryBuilder.GetExportQuery(
                        this.userInfo,
                        entityName,
                        sortOrderColumnName,
                        sortOrderDirection,
                        filter,
                        columnsArr,
                        parentProjectionName,
                        parentEntityId,
                        projectionRelationName,
                        gridMode
                );
            auditService.SetQueryBuilderSql(sql);

            DataTable ds = new DataTable();
            using (SqlConnection con = new SqlConnection(DbConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, con))
            {
                con.Open();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                cmd.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
                cmd.Parameters.Add(new SqlParameter("@userLanguageId", this.userInfo.UserLanguageId));
                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    /// set schema for formating value in DataTable
                    da.FillSchema(ds, SchemaType.Source);
                    foreach (DataColumn item in ds.Columns)
                    {
                        item.DataType = typeof(String);
                    }
                    da.Fill(ds);
                }
            }
            return ds;
        }

        public String GetByFilter(string entityName, string filter)
        {
            StringBuilder json = new StringBuilder();
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("sp_api3_getByFilter", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@entityName", entityName));
                cmd.Parameters.Add(new SqlParameter("@query", filter));

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    json.Append(rdr.GetString(0));
                }

            }
            return json.ToString();
        }

        public String MassUpdate(
            string entityName,
            int[] ids,
            JObject data
            )
        {
            TemplateQueryBuilder queryBuilder = new TemplateQueryBuilder();
            string sql = queryBuilder.MassUpdateQuery(
                        this.userInfo,
                        entityName,
                        ids,
                        data
                );
            auditService.SetQueryBuilderSql(sql);

            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                cmd.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
                cmd.Parameters.Add(new SqlParameter("@userLanguageId", this.userInfo.UserLanguageId));
                cmd.Parameters.Add(new SqlParameter("@json", data.ToString(Formatting.None)));
                cmd.ExecuteNonQuery();
            }
            return "{}";
        }

        public String GetSpData(string spNam, JObject spParameters)
        {
            StringBuilder json = new StringBuilder();
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(spNam, conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                cmd.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
                cmd.Parameters.Add(new SqlParameter("@json", spParameters.ToString(Formatting.None)));
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    json.Append(rdr.GetString(0));
                }
            }
            return json.ToString();
        }

        protected String GetJsonDataFromReader(SqlDataReader reader)
        {
            var stringBuilder = new StringBuilder();
            while (reader.Read())
            {
                stringBuilder.Append(reader.GetString(0));
            }

            return stringBuilder.ToString();
        }

        public String Import(string projectionName, string pairingColumn, string[] columns, JArray json)
        {
            string jsonResult = string.Empty;

            TemplateQueryBuilder queryBuilder = new TemplateQueryBuilder();
            string sql = queryBuilder.GetImportQuery(projectionName, pairingColumn, columns, this.userInfo);
            auditService.SetQueryBuilderSql(sql);

            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                cmd.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
                cmd.Parameters.Add(new SqlParameter("@userLanguageId", this.userInfo.UserLanguageId));
                cmd.Parameters.Add(new SqlParameter("@json", json.ToString(Formatting.None)));
                int newID = cmd.ExecuteNonQuery();
                jsonResult = "{}";
            }
            return jsonResult;
        }
    }
}

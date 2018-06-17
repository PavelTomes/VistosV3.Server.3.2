using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using System.Reflection;
using System.Net.Security;
using System.Net;
using Core.VistosDb.Objects;
using Core.Services;
using Core.QueryBuilder;
using Core.VistosDb;
using System.Data.Common;
using Microsoft.Extensions.Configuration;

namespace Core.Repository
{
    public partial class DbRepository : IDisposable
    {
        private UserInfo userInfo = null;
        public IAuditService auditService { get; private set; }
        private string DbConnectionString = null;
        private string MerkWebApiUrl = null;

        public DbRepository(IAuditService auditService)
        {
            Init();
        }

        public DbRepository(UserInfo userInfo, IAuditService auditService)
        {
            this.userInfo = userInfo;
            this.auditService = auditService;
            Init();
        }

        private void Init()
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
              .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
              .AddJsonFile("appsettings.json")
              .Build();
            this.DbConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        #region Login
        public UserInfo GetUserByToken(string userToken)
        {
            if (!string.IsNullOrEmpty(userToken))
            {
                vwUserAuthToken userAuthToken = Settings.GetInstance.VwUserAuthTokenList.Where(u => u.Token == userToken).FirstOrDefault();
                if (userAuthToken != null && userAuthToken.UserId > 0 && userAuthToken.UserLanguageId.HasValue && userAuthToken.UserLanguageId.Value > 0 && userAuthToken.ProfileId.HasValue & userAuthToken.ProfileId.Value > 0)
                {
                    UserInfo userInfo = new UserInfo(userAuthToken.UserId, userAuthToken.UserName, userAuthToken.UserLanguage, userAuthToken.UserLanguageId.Value, userAuthToken.ProfileId.Value);
                    return userInfo;
                }
            }
            return null;
        }

        public String Login(string userName, string password)
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_authenticateUser", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userName", userName));
                cmd.Parameters.Add(new SqlParameter("@password", password));

                SqlDataReader rdr = cmd.ExecuteReader();
                StringBuilder json = new StringBuilder();
                while (rdr.Read())
                {
                    json.Append(rdr.GetString(0));
                }
                return json.ToString();
            }
        }
        #endregion

        #region Grid
        public String SaveGridSettings(string projectionName, string gridName, GridSettingsType gridSettingsType, string json)
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_saveGridSettings", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                cmd.Parameters.Add(new SqlParameter("@projectionName", projectionName));
                cmd.Parameters.Add(new SqlParameter("@gridName", gridName));
                cmd.Parameters.Add(new SqlParameter("@gridSettingsType", gridSettingsType));
                cmd.Parameters.Add(new SqlParameter("@json", json));
                cmd.ExecuteNonQuery();
            }
            return "{}";
        }

        public String GetGridSettings(string projectionName, string gridName, GridSettingsType gridSettingsType)
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_getGridSettings", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                cmd.Parameters.Add(new SqlParameter("@projectionName", projectionName));
                cmd.Parameters.Add(new SqlParameter("@gridName", gridName));
                cmd.Parameters.Add(new SqlParameter("@gridSettingsType", gridSettingsType));

                SqlDataReader rdr = cmd.ExecuteReader();
                StringBuilder json = new StringBuilder();
                while (rdr.Read())
                {
                    json.Append(rdr.GetString(0));
                }
                return json.ToString();
            }
        }
        #endregion

        #region Filter
        public String SaveFilterSettings(string entityName, string filterName, string filterType, string json)
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_saveFilterSettings", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@entityName", entityName));
                cmd.Parameters.Add(new SqlParameter("@filterName", filterName));
                cmd.Parameters.Add(new SqlParameter("@filterType", filterType));
                cmd.Parameters.Add(new SqlParameter("@json", json));
                cmd.ExecuteNonQuery();
            }
            return "{}";
        }

        public String GetFilterSettings(string entityName, string filterName, string filterType)
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_getFilterSettings", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@entityName", entityName));
                cmd.Parameters.Add(new SqlParameter("@filterName", filterName));
                cmd.Parameters.Add(new SqlParameter("@filterType", filterType));

                SqlDataReader rdr = cmd.ExecuteReader();
                StringBuilder json = new StringBuilder();
                while (rdr.Read())
                {
                    json.Append(rdr.GetString(0));
                }
                return json.ToString();
            }
        }
        #endregion

        #region AccessRights
        public String GetProjectionAccessRights(JObject profileId)
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_getProjectionAccessRights", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@json", profileId.ToString(Formatting.None)));

                SqlDataReader rdr = cmd.ExecuteReader();
                StringBuilder json = new StringBuilder();
                while (rdr.Read())
                {
                    json.Append(rdr.GetString(0));
                }
                return json.ToString();
            }
        }
        public String SaveProjectionAccessRights(JArray data, int forProfil)
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_saveProjectionAccessRights", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@json", data.ToString(Formatting.None)));
                cmd.Parameters.Add(new SqlParameter("@forProfil", forProfil));

                cmd.ExecuteNonQuery();
                return "{}";
            }
        }
        public String SaveProjectionAccessRightsColumns(JArray data)
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_saveProjectionAccessRightColumns", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@json", data.ToString(Formatting.None)));

                cmd.ExecuteNonQuery();
                return "{}";
            }
        }
        public String GetProjectionAccessRightsColumns(JObject profileId)
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_getProjectionAccessRightColumns", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@json", profileId.ToString(Formatting.None)));
                cmd.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));

                SqlDataReader rdr = cmd.ExecuteReader();
                StringBuilder json = new StringBuilder();
                while (rdr.Read())
                {
                    json.Append(rdr.GetString(0));
                }
                return json.ToString();
            }
        }
        #endregion

        #region Layout
        public String SaveLayout(string entityName, string json, string mode)
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_saveLayout", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@entityName", entityName));
                cmd.Parameters.Add(new SqlParameter("@json", json));
                cmd.Parameters.Add(new SqlParameter("@mode", mode));
                cmd.ExecuteNonQuery();
            }
            return "{}";
        }
        public String ResetLayout(string entityName, string mode)
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_resetLayout", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@entityName", entityName));
                cmd.Parameters.Add(new SqlParameter("@mode", mode));
                cmd.ExecuteNonQuery();
                return "{}";
            }
        }
        #endregion

        #region Discussion
        public String GetDiscussionByProjection(int recordId, string projectionName)
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_GetDiscussion", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                cmd.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
                cmd.Parameters.Add(new SqlParameter("@projectionName", projectionName));
                cmd.Parameters.Add(new SqlParameter("@recordId ", recordId));

                SqlDataReader rdr = cmd.ExecuteReader();
                StringBuilder json = new StringBuilder();
                while (rdr.Read())
                {
                    json.Append(rdr.GetString(0));
                }
                return json.ToString();
            }
        }

        public String SaveNewDiscussionByEntity(string text, int recordId, string projectionName, string hierarchyId)
        {
            var dbObjectId = Settings.GetInstance.GetVwProjectionList(userInfo.ProfileId).Find(x => x.Projection_Name == projectionName).DbObject_Id;
            var discussionMessageId = 0;

            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {

                    try
                    {
                        #region 1. Save new discussion message
                        var saveNewDiscussionByEntityCommand = this.GetSaveNewDiscussionByEntityCommand(text, recordId, projectionName, hierarchyId);
                        saveNewDiscussionByEntityCommand.Connection = conn;
                        saveNewDiscussionByEntityCommand.Transaction = trans;
                        object a = saveNewDiscussionByEntityCommand.ExecuteScalar();
                        discussionMessageId = (int)a;

                        #endregion

                        #region 2. Add information to TrackChanges
                        var trackChanges = new TrackChanges();
                        trackChanges.Created = DateTime.Now;

                        using (VistosDbContext ctx = new VistosDbContext())
                        {
                            trackChanges.RecordId = recordId;
                            trackChanges.ReferenceId = (int)discussionMessageId;
                            trackChanges.DbObject_FK = dbObjectId;
                            //**trackChanges.Type_FK = (int)BAL.Services.TrackChanges.TrackChangesType.AddDiscusionMessage;
                            trackChanges.NewValue = text;
                            trackChanges.User_FK = userInfo.UserId;
                            ctx.TrackChanges.Add(trackChanges);
                            ctx.SaveChanges();
                        }
                        #endregion
                        trans.Commit();

                        //**var nws = new GenerateNotificationWorkerSettings(this.userInfo, new List<TrackChanges>() { trackChanges }, DbConnectionString);
                        //**Worker.ExecuteGenerateNotification(nws);
                    }
                    catch (Exception ex)
                    {
                        // TODO: log exception
                        Logger.SaveLogError(LogLevel.Error, ex.Message, ex, null, userInfo);
                        trans.Rollback();
                    }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                        {
                            conn.Close();
                        }
                    }
                }
            }



            return "{}";
        }

        /*
        public async void ExecProcDiscussionGenerateNotification(Int32? projectionId, Int32? recordId, String connString)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                var cmd = this.GetDiscussionGenerateNotificationsCommand(projectionId, recordId);
                cmd.Connection = conn;
                cmd.ExecuteNonQuery();
                conn.Close();
            }
        }*/

        public String EditDiscussionMessage(string text, bool isSystemMessage, int entityFk, string entityName, string hierarchyID)
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_editDiscussionMessage", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userID", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@entityFk", entityFk));
                cmd.Parameters.Add(new SqlParameter("@projectionName", entityName));
                cmd.Parameters.Add(new SqlParameter("@text", text));
                cmd.Parameters.Add(new SqlParameter("@IsSystemMessage", isSystemMessage));
                cmd.Parameters.Add(new SqlParameter("@stringHIERARCHYID", hierarchyID));


                SqlDataReader rdr = cmd.ExecuteReader();
                StringBuilder json = new StringBuilder();
                while (rdr.Read())
                {
                    json.Append(rdr.GetString(0));
                }
                return json.ToString();
            }
        }

        private SqlCommand GetDiscussionGenerateNotificationsCommand(Int32? projectionId, Int32? recordId)
        {
            SqlCommand cmd = new SqlCommand("sp_api3_DiscussionGenerateNotifications");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@par_Projection_ID", projectionId));
            cmd.Parameters.Add(new SqlParameter("@par_Record_ID", recordId));

            return cmd;
        }

        private SqlCommand GetSaveNewDiscussionByEntityCommand(string text, int recordId, string projectionName, string hierarchyId)
        {
            SqlCommand cmd = new SqlCommand("sp_api3_SaveDiscussionMessage");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@stringHierarchyId", hierarchyId));
            cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
            cmd.Parameters.Add(new SqlParameter("@projectionName", projectionName));
            cmd.Parameters.Add(new SqlParameter("@recordId", recordId));
            cmd.Parameters.Add(new SqlParameter("@text", text));
            return cmd;
        }

        public SqlCommand GetDiscussionUnreadMessages(Int32 discussionId)
        {
            SqlCommand cmd = new SqlCommand("sp_api3_getDiscussionUnreadMessages");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@discussionID", discussionId));

            return cmd;
        }
        #endregion

        #region Email
        public String GetEmailFolders()
        {
            StringBuilder json = new StringBuilder();

            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_getEmailFolders", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                cmd.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));

                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    json.Append(rdr.GetString(0));
                }
            }
            return json.ToString();
        }
        public String MoveEmailsToFolder(int folderId, int[] emailIds)
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_moveEmailsToFolder", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                cmd.Parameters.Add(new SqlParameter("@folderId", folderId));
                cmd.Parameters.Add(new SqlParameter("@emailIds", JsonConvert.SerializeObject(emailIds)));
                cmd.ExecuteNonQuery();
            }
            return "{}";
        }
        public String SetEmailIsRead(int emailId, bool read)
        {
            //**EmailViewModel model = new EmailViewModel(emailId, this.userInfo);
            //**model.SetIsSeen(read);
            //**model.Save(this.userInfo);
            return "{}";
        }
        public String SetEmailFolderIsRead(int folderId, bool read)
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_setEmailFolderIsRead", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                cmd.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
                cmd.Parameters.Add(new SqlParameter("@folderId", folderId));
                cmd.Parameters.Add(new SqlParameter("@read", read));

                cmd.ExecuteNonQuery();
                return "{}";
            }
        }


        public String SetEmailIsLinkedWithVistos(int emailId, bool flagged)
        {
            //**EmailViewModel model = new EmailViewModel(emailId, this.userInfo);
            //**model.RefreshAttachmentsFromIMAP();
            //**model.RefreshBodyFromIMAP();
            //**model.SetEmailIsLinkedWithVistos(flagged);
            //**
            //**if (Settings.GetInstance.SystemSettings.StoreEmailAttachmentsInDropbox)
            //**{
            //**    model.MoveAttachmentsToDropbox();
            //**}
            //**
            //**if (Settings.GetInstance.SystemSettings.StoreEmailBodyInDropbox)
            //**{
            //**    model.MoveBodyToDropbox();
            //**}
            //**
            //**model.Save(this.userInfo);

            return "{}";
        }

        public String SetEmailIsFlagged(int emailId, bool flagged)
        {
            //**EmailViewModel model = new EmailViewModel(emailId, this.userInfo);
            //**model.SetIsFlagged(flagged);
            //**model.Save(this.userInfo);
            return "{}";
        }
        #endregion

        #region Notification

        public SqlCommand GetInsertNotificationMessageCommand(NewDiscussionNotificationMessage message, int notificationTypeFk)
        {
            SqlCommand cmd = new SqlCommand("sp_api3_InsertNotificationMessage");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@User_ID", message.ToUserId));
            cmd.Parameters.Add(new SqlParameter("@Created", message.Created));
            cmd.Parameters.Add(new SqlParameter("@EmailTo", message.EmailTo));
            cmd.Parameters.Add(new SqlParameter("@TextHeader", message.HtmlHeader));
            cmd.Parameters.Add(new SqlParameter("@TextBody", message.HtmlBody));
            cmd.Parameters.Add(new SqlParameter("@Subject", message.HtmlSubject));
            cmd.Parameters.Add(new SqlParameter("@Projection_ID", message.HtmlProjectionId));
            cmd.Parameters.Add(new SqlParameter("@Record_ID", message.HtmlRecord));
            cmd.Parameters.Add(new SqlParameter("@NotificationType_FK", notificationTypeFk));

            return cmd;
        }

        public SqlCommand GetInsertNotificationMessageCommand(NewTrackChangesNotificationMessage message, int notificationTypeFk)
        {
            SqlCommand cmd = new SqlCommand("sp_api3_InsertNotificationMessage");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@User_ID", message.ToUserId));
            cmd.Parameters.Add(new SqlParameter("@Created", message.Created));
            cmd.Parameters.Add(new SqlParameter("@EmailTo", message.EmailTo));
            cmd.Parameters.Add(new SqlParameter("@TextHeader", message.HtmlHeader));
            cmd.Parameters.Add(new SqlParameter("@TextBody", message.HtmlBody));
            cmd.Parameters.Add(new SqlParameter("@Subject", message.HtmlSubject));
            cmd.Parameters.Add(new SqlParameter("@Projection_ID", message.HtmlProjectionId));
            cmd.Parameters.Add(new SqlParameter("@Record_ID", message.HtmlRecord));
            cmd.Parameters.Add(new SqlParameter("@NotificationType_FK", notificationTypeFk));

            return cmd;
        }

        public SqlCommand GetUpdateWasCreatedNotificationMessagesCommand(IList<int> messages)
        {
            SqlCommand cmd = new SqlCommand("sp_api3_UpdateWasCreatedNotificationMessages");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ids", string.Join<int>(",", messages)));

            return cmd;
        }

        public SqlCommand GetUpdateWasCreatedNotificationMessagesTrackChangesCommand(IList<int> messages)
        {
            SqlCommand cmd = new SqlCommand("sp_api3_UpdateTrackChangesWasCreatedNotificationMessages");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ids", string.Join<int>(",", messages)));

            return cmd;
        }
        public List<ConfNotification> GetConfNotification(Int32? confNotificationId, String projectionName, Int32? userId, Int32? recordId)
        {
            List<ConfNotification> resultList = new List<ConfNotification>();

            //using (SqlConnection conn = new SqlConnection(DbConnectionString))
            //{
            //    conn.Open();

            //    var command = this.GetConfNotificationCommand(confNotificationId, projectionName, userId, recordId);
            //    command.Connection = conn;

            //    using (DbDataReader reader = command.ExecuteReader())
            //    {
            //        while (reader.Read())
            //        {
            //            resultList.Add(new ConfNotification()
            //            {
            //                ConfNotificationId = reader.GetInt32(0),
            //                UserId = reader.GetInt32(1),
            //                ProjectionId = reader.GetInt32(2),
            //                RecordId = reader.IsDBNull(3) ? null : (Int32?)reader.GetInt32(3),
            //                InTime = reader.IsDBNull(4) ? null : (TimeSpan?)reader.GetValue(4),
            //                OnDay = reader.IsDBNull(5) ? null : reader.GetString(5),
            //                FrequencyTypeId = reader.IsDBNull(6) ? null : (Int32?)reader.GetInt32(6),
            //                NotificationTypeId = reader.GetInt32(6)
            //            });
            //        }
            //    }
            //}

            return resultList;
        }

        private SqlCommand GetConfNotificationCommand(Int32? confNotificationId, String projectionName, Int32? userId, Int32? recordId)
        {
            SqlCommand cmd = new SqlCommand("sp_GetConfNotification");
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add(new SqlParameter("@ConfNotification_ID", confNotificationId));
            cmd.Parameters.Add(new SqlParameter("@User_ID", userId));
            cmd.Parameters.Add(new SqlParameter("@EntityName", projectionName));
            cmd.Parameters.Add(new SqlParameter("@Entity_ID", recordId));

            return cmd;
        }

        public void CreateOrUpdateConfNotification(Int32? confNotificationId, Int32 projectionId, Int32 userId, Int32? recordId, Int32? frequencyTypeId, TimeSpan? inTime, String onDay, Int32 notificationTypeId)
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                var command = this.GetCreateOrUpdateConfNotification(confNotificationId, projectionId, userId, recordId, frequencyTypeId, inTime, onDay, notificationTypeId);
                command.Connection = conn;
                conn.Open();

                int count = command.ExecuteNonQuery();
            }
        }

        private SqlCommand GetCreateOrUpdateConfNotification(Int32? confNotificationId, Int32 projectionId, Int32 userId, Int32? recordId, Int32? frequencyTypeFk, TimeSpan? inTime, String onDay, Int32 notificationTypeId)
        {
            SqlCommand command = new SqlCommand("sp_api3_CreateOrUpdateConfNotification");
            command.Parameters.Add(new SqlParameter("@ConfNotification_ID", ((confNotificationId.HasValue && confNotificationId == 0) ? null : confNotificationId)));
            command.Parameters.Add(new SqlParameter("@Projection_ID", projectionId));
            command.Parameters.Add(new SqlParameter("@User_ID", userId));
            command.Parameters.Add(new SqlParameter("@Record_ID", recordId));
            command.Parameters.Add(new SqlParameter("@FrequencyType_FK", frequencyTypeFk));
            command.Parameters.Add(new SqlParameter("@InTime", inTime));
            command.Parameters.Add(new SqlParameter("@OnDay", onDay));
            command.Parameters.Add(new SqlParameter("@NotificationType_FK", notificationTypeId));

            return command;
        }

        public void RemoveConfNotification(Int32 confNotificationId)
        {
            using (var conn = new SqlConnection(DbConnectionString))
            {
                var command = this.GetRemoveConfNotificationCommand(confNotificationId);

                int count = command.ExecuteNonQuery();
            }
        }

        private SqlCommand GetRemoveConfNotificationCommand(Int32 confNotificationId)
        {
            SqlCommand command = new SqlCommand("sp_RemoveConfNotification");
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add(new SqlParameter("ConfNotification_ID", confNotificationId));
            return command;
        }

        #endregion

        #region Localization
        public String SaveLocalization(JArray data)
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_saveLocalization", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@json", data.ToString(Formatting.None)));

                SqlDataReader rdr = cmd.ExecuteReader();
                StringBuilder json = new StringBuilder();
                while (rdr.Read())
                {
                    json.Append(rdr.GetString(0));
                }
                return json.ToString();
            }
        }
        public String GetLocalization(JObject filter)
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_getLocalization", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@json", filter.ToString(Formatting.None)));

                SqlDataReader rdr = cmd.ExecuteReader();
                StringBuilder json = new StringBuilder();
                while (rdr.Read())
                {
                    json.Append(rdr.GetString(0));
                }
                return json.ToString();
            }
        }
        #endregion

        #region TrackChanges
        public SqlCommand GetTrackChangesUnread(Int32? projectionId = null, Int32? recordId = null)
        {
            SqlCommand cmd = new SqlCommand("sp_api3_getTrackChangesUnread");
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@ProjectionId", projectionId));
            cmd.Parameters.Add(new SqlParameter("@RecordId", recordId));

            return cmd;
        }
        #endregion

        #region Categories
        public String GetCategoriesByProjectionName(string projectionName)
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_getCategoriesByEntityName", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@projectionName", projectionName));

                SqlDataReader rdr = cmd.ExecuteReader();
                StringBuilder json = new StringBuilder();
                while (rdr.Read())
                {
                    json.Append(rdr.GetString(0));
                }
                return json.ToString();
            }
        }
        #endregion

        #region Merk
        public String GetMerkInfoRegNumber(string regNumber, string countryCode, bool advanced)
        {
            if (advanced && !Settings.GetInstance.SystemSettings.MerkAdvancedEnabled)
            {
                return "{}";
            }
            string countryCode1 = !string.IsNullOrEmpty(countryCode) && countryCode.Length > 1 ? countryCode.Substring(countryCode.Length - 2, 2) : "cz";

            if (!string.IsNullOrEmpty(regNumber) && !string.IsNullOrEmpty(countryCode1))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                   delegate { return true; }
                );
                string url = $"{MerkWebApiUrl}/GetMerkInfoByRegNumber?RegNumber={regNumber}&CountryCode={countryCode1.ToLower()}";
                System.Text.UTF8Encoding encoder = new UTF8Encoding();
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "GET";
                httpWebRequest.AllowAutoRedirect = false;

                using (var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    string responseText = string.Empty;
                    using (var reader = new System.IO.StreamReader(httpWebResponse.GetResponseStream(), ASCIIEncoding.UTF8))
                    {
                        responseText = reader.ReadToEnd();
                    }
                    return responseText;
                }
            }

            return "{}";
        }

        public String GetMerkSuggest(string regNumber, string email, string name, string countryCode)
        {
            string countryCode1 = !string.IsNullOrEmpty(countryCode) && countryCode.Length > 1 ? countryCode.Substring(countryCode.Length - 2, 2) : "cz";

            if ((!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(regNumber) || !string.IsNullOrEmpty(email)) && !string.IsNullOrEmpty(countryCode1))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback
                (
                   delegate { return true; }
                );
                string url = $"{MerkWebApiUrl}/Suggest?RegNumber={regNumber}&Email={email}&Name={name}&OnlyActive=false&CountryCode={countryCode1.ToLower()}";

                System.Text.UTF8Encoding encoder = new UTF8Encoding();
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.Method = "GET";
                httpWebRequest.AllowAutoRedirect = false;

                using (var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse())
                {
                    string responseText = string.Empty;
                    using (var reader = new System.IO.StreamReader(httpWebResponse.GetResponseStream(), ASCIIEncoding.UTF8))
                    {
                        responseText = reader.ReadToEnd();
                    }
                    if (!string.IsNullOrEmpty(responseText))
                    {
                        return responseText;
                    }
                }
            }

            return "[]";
        }
        #endregion

        #region Calendar
        public String UpdateCalendarData(string entityName, int entityId, DateTime startDate, DateTime endDate)
        {
            TemplateQueryBuilder queryBuilder = new TemplateQueryBuilder();

            string sql = queryBuilder.UpdateCalendarEntity(entityName, this.userInfo);
            auditService.SetQueryBuilderSql(sql);

            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@entityId", entityId));
                cmd.Parameters.Add(new SqlParameter("@startDate", startDate));
                cmd.Parameters.Add(new SqlParameter("@endDate", endDate));

                cmd.ExecuteNonQuery();
                return "{}";
            }
        }

        public String GetCalendarAppointments(DateTime startDate, DateTime endDate, Dictionary<string, List<int>> entitiesAndRoles, List<int> userIDs)
        {
            List<Event> events = new List<Event>();

            TemplateQueryBuilder queryBuilder = new TemplateQueryBuilder();

            string sql = queryBuilder.GetCalendarEntities(entitiesAndRoles, userIDs, this.userInfo);
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
                cmd.Parameters.Add(new SqlParameter("@fromDate", startDate));
                cmd.Parameters.Add(new SqlParameter("@toDate", endDate));
                SqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    events.Add(
                            new Event()
                            {
                                id = (string)rdr["EntityName"] + (int)rdr["Id"],
                                entityName = (string)rdr["EntityName"],
                                entityId = (int)rdr["Id"],
                                title = (string)rdr["Name"],
                                start = ((DateTime)rdr["StartDate"]).ToString("yyyy-MM-ddTHH:mm:ss"),
                                end = (DateTime)rdr["EndDate"] <= (DateTime)rdr["StartDate"] ? ((DateTime)rdr["StartDate"]).AddMinutes(15).ToString("yyyy-MM-ddTHH:mm:ss") : ((DateTime)rdr["EndDate"]).ToString("yyyy-MM-ddTHH:mm:ss"),
                                allDay = (bool)rdr["IsAllDay"],
                                color = (string)rdr["CalendarBackgroundColor"]
                            }
                        );
                }
            }
            return JsonConvert.SerializeObject(events);
        }
        #endregion

        #region
        #endregion

        #region
        #endregion

        public String GetEnumerationByType(string enumType, int? parentValue, string filterText)
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_getEnumerationByType", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                cmd.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
                cmd.Parameters.Add(new SqlParameter("@enumType", enumType));

                SqlParameter param1 = new SqlParameter("@parentValue", SqlDbType.Int);
                param1.Value = (object)parentValue ?? DBNull.Value;
                cmd.Parameters.Add(param1);

                SqlParameter param2 = new SqlParameter("@filterText", SqlDbType.NVarChar);
                param2.Value = (object)filterText ?? DBNull.Value;
                cmd.Parameters.Add(param2);

                SqlDataReader rdr = cmd.ExecuteReader();
                StringBuilder jsonSb = new StringBuilder();
                while (rdr.Read())
                {
                    jsonSb.Append(rdr.GetString(0));
                }
                string json = jsonSb.ToString();
                return !string.IsNullOrEmpty(json) ? json : "[]";
            }
        }

        public String FullTextSearch(string text, bool includeDiscussionMessage, JArray dbObjectIdArrayJson)
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("sp_api3_FullText_GeneralSearch", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                cmd.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
                cmd.Parameters.Add(new SqlParameter("@userLanguageId", this.userInfo.UserLanguageId));
                cmd.Parameters.Add(new SqlParameter("@Text", text));
                cmd.Parameters.Add(new SqlParameter("@IncludeDiscussionMessage", includeDiscussionMessage));
                cmd.Parameters.Add(new SqlParameter("@DbObjectIdArrayJson", JsonConvert.SerializeObject(dbObjectIdArrayJson)));

                SqlDataReader rdr = cmd.ExecuteReader();
                StringBuilder json = new StringBuilder();
                while (rdr.Read())
                {
                    json.Append(rdr.GetString(0));
                }
                return json.ToString();
            }
        }

        public String GetEntityList(string entityName)
        {
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("sp_api3_getEntityList", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@entityName", entityName));

                SqlDataReader rdr = cmd.ExecuteReader();
                StringBuilder json = new StringBuilder();
                while (rdr.Read())
                {
                    json.Append(rdr.GetString(0));
                }

                return json.ToString();
            }
        }

        public String GetGridIds(
           string entityName,
           string filter,
           string parentProjectionName,
           int? parentEntityId,
           string projectionRelationName,
           bool ignoreSwitchIsVisibleOnFilter
           )
        {
            string jsonFinal = string.Empty;

            TemplateQueryBuilder queryBuilder = new TemplateQueryBuilder();
            string sql = queryBuilder.GetGridIdsQuery(
                        entityName,
                        this.userInfo,
                        filter,
                        parentProjectionName,
                        parentEntityId,
                        projectionRelationName,
                        ignoreSwitchIsVisibleOnFilter
                );
            auditService.SetQueryBuilderSql(sql);
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));

                SqlDataReader rdr = cmd.ExecuteReader();
                StringBuilder json = new StringBuilder();
                while (rdr.Read())
                {
                    json.Append(rdr.GetString(0));
                }
                jsonFinal = json.ToString();
            }
            return jsonFinal;
        }

        public String MassAction(string entityName, string actionName, string jsonEntityIds, int? value)
        {
            StringBuilder json = new StringBuilder();
            using (SqlConnection conn = new SqlConnection(DbConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("sp_api3_massAction", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@userId", this.userInfo.UserId));
                cmd.Parameters.Add(new SqlParameter("@profileId", this.userInfo.ProfileId));
                cmd.Parameters.Add(new SqlParameter("@userLanguage", this.userInfo.UserLanguage));
                cmd.Parameters.Add(new SqlParameter("@entityName", entityName));
                cmd.Parameters.Add(new SqlParameter("@actionName", actionName));
                cmd.Parameters.Add(new SqlParameter("@jsonEntityIds", jsonEntityIds));
                cmd.Parameters.Add(new SqlParameter("@value", value));

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

        public String GetSchema()
        {
            return Settings.GetInstance.GetSchema(this.userInfo);
        }


        public String GetMenu()
        {
            return Settings.GetInstance.GetMenu(this.userInfo.UserId);
        }



        public String CallCustomMethod(string methodName, string entityName, int? entityId, JObject json, IList<TrackChanges> trackChanges)
        {
            // ************ Debug Plugin
            //PlugIn.Truckdata.Code.ApiManager a = new PlugIn.Truckdata.Code.ApiManager();
            //a.AfterUpdate(entityName, entityId.Value, json, this.userInfo, trackChanges);
            //return "{}";
            // ************ Debug Plugin

            string plugInDllName = Settings.GetInstance.SystemSettings.PlugInDllName;
            string plugInClassNamespace = Settings.GetInstance.SystemSettings.PlugInClassNamespace;
            if (!string.IsNullOrEmpty(plugInDllName) && !string.IsNullOrEmpty(plugInClassNamespace))
            {
                var assembly = Assembly.LoadFile(AppDomain.CurrentDomain.BaseDirectory + plugInDllName);
                var type = (assembly != null) ? assembly.GetType(plugInClassNamespace) : null;

                var method = (type != null) ? type.GetMethod(methodName) : null;

                if (method == null)
                {
                    throw new NullReferenceException("Method not found");
                }

                if (method.IsStatic)
                {
                    object result = method.Invoke(null, new object[] { entityName, entityId, json, this.userInfo, trackChanges });
                    JObject jsonResult = (JObject)result;
                    return jsonResult.ToString(Formatting.None);
                }
                else
                {
                    ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                    if (constructor != null)
                    {
                        object constructorObject = constructor.Invoke(new object[] { });
                        object result = method.Invoke(constructorObject, new object[] { entityName, entityId, json, this.userInfo, trackChanges });
                        JObject jsonResult = (JObject)result;
                        return jsonResult.ToString(Formatting.None);
                    }
                    else
                    {
                        throw new Exception("Called method hasn't default constructor!");
                    }
                }
            }
            throw new NullReferenceException("Plugin or Method not found");
        }

        public void Dispose()
        {

        }
    }
}


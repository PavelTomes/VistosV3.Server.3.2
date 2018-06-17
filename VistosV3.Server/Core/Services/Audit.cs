using Core.Models;
using Core.Models.ApiRequest;
using Core.VistosDb;
using System;
using System.Threading.Tasks;

namespace Core.Services
{
    public interface IAuditService
    {
        Task SaveAudit();
        void ParseRequestOperation(ApiRequest request, UserInfo userInfo);
        void SetRequestInfo(String action, string ipAddress);

        void SetRequestJson(string json);
        void SetResponseJson(string json);
        void SetQueryBuilderSql(string sql);

    }

    public class AuditService : IAuditService
    {
        private Core.VistosDb.Objects.Audit auditDB { get; set; }
        public AuditService()
        {
            auditDB = new Core.VistosDb.Objects.Audit();
        }

        public async Task SaveAudit()
        {
            try
            {
                if (auditDB.Operation == "GetPage" ||
                    auditDB.Operation == "Delete" ||
                    auditDB.Operation == "GetById" ||
                    auditDB.Operation == "Save" ||
                    auditDB.Operation == "Create" ||
                    auditDB.Operation == "Login" ||
                    auditDB.Operation == "Remove" ||
                    auditDB.Operation == "GetCalendarData" ||
                    auditDB.Operation == "UpdateCalendarData" ||
                    auditDB.Operation == "SaveLayout" ||
                    auditDB.Operation == "SaveGridSettings" ||
                    auditDB.Operation == "MassActionGrid" ||
                    auditDB.Operation == "AddSignature" ||
                    auditDB.Operation == "UpdateCalendarData" ||
                    auditDB.Operation == "CreateEntityFrom" ||
                    auditDB.Operation == "MassActionUpdate" ||
                    auditDB.Operation == "AddExistingManyToMany" ||
                    auditDB.Operation == "SaveAccessRights" ||
                    auditDB.Operation == "ResetLayout" ||
                    auditDB.Operation == "SaveLocalization" ||
                    auditDB.Operation == "SaveAccessRightsColumns" ||
                    auditDB.Operation == "SaveDiscussionMessage" ||
                    auditDB.Operation == "EditDiscussionMessage" ||
                    auditDB.Operation == "RestartAPI" ||
                    auditDB.Operation == "CleanCache"
                    )
                {
                    using (VistosDbContext ctx = new VistosDbContext())
                    {

                        ctx.Audit.Add(auditDB);
                        await ctx.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                // LOG ERR
            }
        }

        public void ParseRequestOperation(ApiRequest request, UserInfo userInfo)
        {
            try
            {
                if (userInfo != null)
                {
                    auditDB.User_FK = userInfo.UserId;
                }

                if (request != null)
                {
                    auditDB.Device = request.Device;
                    auditDB.RequestGuid = request.RequestGuid;
                    auditDB.UserToken = request.UserToken;

                    if (request.LoginParam != null)
                    {
                        auditDB.Operation = "Login";
                    }
                    if (request.GetPageParam != null)
                    {
                        auditDB.Operation = "GetPage";
                        auditDB.EntityName = request.GetPageParam.EntityName;
                    }
                    if (request.GetExportParam != null)
                    {
                        auditDB.Operation = "GetExport";
                        auditDB.EntityName = request.GetExportParam.EntityName;
                    }
                    if (request.GetGridIdsParam != null)
                    {
                        auditDB.Operation = "GetGridIds";
                        auditDB.EntityName = request.GetGridIdsParam.EntityName;
                    }
                    if (request.GetByIdParam != null)
                    {
                        auditDB.Operation = "GetById";
                        auditDB.EntityName = request.GetByIdParam.EntityName;
                        auditDB.EntityId = request.GetByIdParam.EntityId;
                    }
                    if (request.GetAutocompleteParam != null)
                    {
                        auditDB.Operation = "GetAutocomplete";
                        auditDB.EntityName = request.GetAutocompleteParam.EntityName;
                    }
                    if (request.GetEntityListParam != null)
                    {
                        auditDB.Operation = "GetEntityList";
                        auditDB.EntityName = request.GetEntityListParam.EntityName;
                    }
                    if (request.GetMenu != null)
                    {
                        auditDB.Operation = "GetMenu";
                    }
                    if (request.GetSchema != null)
                    {
                        auditDB.Operation = "GetSchema";
                    }
                    if (request.GetEnumerationParam != null)
                    {
                        auditDB.Operation = "GetEnumeration";
                    }
                    if (request.GetCategoriesByProjectionNameParam != null)
                    {
                        auditDB.Operation = "GetCategoriesByEntityName";
                        auditDB.EntityName = request.GetCategoriesByProjectionNameParam.ProjectionName;
                    }
                    if (request.SaveParam != null)
                    {
                        auditDB.Operation = "Save";
                        auditDB.EntityName = request.SaveParam.EntityName;
                        auditDB.EntityId = request.SaveParam.EntityId;
                    }
                    if (request.MassActionGridParam != null)
                    {
                        auditDB.Operation = "MassActionGrid";
                        auditDB.EntityName = request.MassActionGridParam.EntityName;
                    }
                    if (request.RemoveParam != null)
                    {
                        auditDB.Operation = "Remove";
                        auditDB.EntityName = request.RemoveParam.EntityName;
                        auditDB.EntityId = request.RemoveParam.EntityId;
                    }
                    if (request.CreateParam != null)
                    {
                        auditDB.Operation = "Create";
                        auditDB.EntityName = request.CreateParam.EntityName;
                    }
                    if (request.SaveLayoutParam != null)
                    {
                        auditDB.Operation = "SaveLayout";
                        auditDB.EntityName = request.SaveLayoutParam.EntityName;
                    }
                    if (request.GetGridSettingsParam != null)
                    {
                        auditDB.Operation = "GetGridSettings";
                        auditDB.EntityName = request.GetGridSettingsParam.ProjectionName;
                    }
                    if (request.SaveGridSettingsParam != null)
                    {
                        auditDB.Operation = "SaveGridSettings";
                        auditDB.EntityName = request.SaveGridSettingsParam.ProjectionName;
                    }
                    if (request.AddSignatureParam != null)
                    {
                        auditDB.Operation = "AddSignature";
                    }
                    if (request.GetCalendarDataParam != null)
                    {
                        auditDB.Operation = "GetCalendarData";
                    }
                    if (request.UpdateCalendarDataParam != null)
                    {
                        auditDB.Operation = "UpdateCalendarData";
                        auditDB.EntityName = request.UpdateCalendarDataParam.EntityName;
                        auditDB.EntityId = request.UpdateCalendarDataParam.EntityId;
                    }
                    if (request.AddSignatureParam != null)
                    {
                        auditDB.Operation = "AddSignature";
                    }
                    if (request.CreateEntityFromParam != null)
                    {
                        auditDB.Operation = "CreateEntityFrom";
                        auditDB.EntityName = request.CreateEntityFromParam.EntityNameFrom;
                        auditDB.EntityId = request.CreateEntityFromParam.EntityIdFrom;
                    }
                    if (request.MassActionUpdateParam != null)
                    {
                        auditDB.Operation = "MassActionUpdate";
                        auditDB.EntityName = request.MassActionUpdateParam.EntityName;
                    }
                    if (request.AddExistingManyToManyParam != null)
                    {
                        auditDB.Operation = "AddExistingManyToMany";
                        auditDB.EntityName = request.AddExistingManyToManyParam.ParentEntityName;
                        auditDB.EntityId = request.AddExistingManyToManyParam.ParentEntityId;
                    }
                    if (request.SaveAccessRightsParam != null)
                    {
                        auditDB.Operation = "SaveAccessRights";
                        auditDB.EntityName = "Profil";
                        auditDB.EntityId = request.SaveAccessRightsParam.ForProfil;
                    }
                    if (request.ResetLayoutParam != null)
                    {
                        auditDB.Operation = "ResetLayout";
                        auditDB.EntityName = request.ResetLayoutParam.EntityName;
                    }
                    if (request.SaveLocalizationParam != null)
                    {
                        auditDB.Operation = "SaveLocalization";
                    }
                    if (request.SaveAccessRightsColumnsParam != null)
                    {
                        auditDB.Operation = "SaveAccessRightsColumns";
                    }
                    if (request.SaveDiscussionMessageParam != null)
                    {
                        auditDB.Operation = "SaveDiscussionMessage";
                        auditDB.EntityName = request.SaveDiscussionMessageParam.ProjectionName;
                        auditDB.EntityId = request.SaveDiscussionMessageParam.RecordId;
                    }
                    if (request.EditDiscussionMessageParam != null)
                    {
                        auditDB.Operation = "EditDiscussionMessage";
                        auditDB.EntityName = request.EditDiscussionMessageParam.EntityName;
                        auditDB.EntityId = request.EditDiscussionMessageParam.EntityFk;
                    }
                    if (request.RestartAPIParam != null)
                    {
                        auditDB.Operation = "RestartAPI";
                    }
                    if (request.CleanCacheParam != null)
                    {
                        auditDB.Operation = "CleanCache";
                    }
                }
            }
            catch (Exception ex)
            {
                // LOG ERR
            }
        }

        public void SetRequestInfo(string action, string ipAddress)
        {
            auditDB.Time = DateTime.Now;
            auditDB.Action = action;
            auditDB.IpAddress = ipAddress;
        }

        public void SetRequestJson(string json)
        {
            auditDB.JsonRequest = json;
        }

        public void SetResponseJson(string json)
        {
            auditDB.JsonResponse = json;
        }

        public void SetQueryBuilderSql(string sql)
        {
            auditDB.QueryBuilderSql = auditDB.QueryBuilderSql + Environment.NewLine + Environment.NewLine + sql;
        }
    }
}
using Newtonsoft.Json;
using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Core.Models;
using Core.Models.ApiRequest.Params;
using Core.Repository;
using Core.Services;

namespace VistosV3.Server.Code
{
    public class MethodManager
    {
        DbRepository repository = null;
        UserInfo userInfo = null;
        public IAuditService auditService { get; private set; }

        public MethodManager(UserInfo userInfo, IAuditService auditService)
        {
            this.userInfo = userInfo;
            this.auditService = auditService;
            this.repository = new DbRepository(userInfo, this.auditService);
        }

        public String GetGridCount(MethodGetGridCountParam p)
        {
            return repository.GetGridCount(
                                 p.EntityName,
                                 p.ParentEntityName,
                                 p.ParentEntityId,
                                 p.ProjectionRelationName
                                 );
        }

        public String GetPage(MethodGetPageParam p)
        {
            return repository.GetPage(
                                 p.EntityName,
                                 p.Draw,
                                 p.Start,
                                 p.Length,
                                 p.SortOrder != null && p.SortOrder.Count > 0 ? p.SortOrder[0].ColumnName : "",
                                 p.SortOrder != null && p.SortOrder.Count > 0 ? p.SortOrder[0].Direction : "",
                                 p.Fulltext,
                                 JsonConvert.SerializeObject(p.Filter),
                                 p.ParentEntityName,
                                 p.ParentEntityId,
                                 p.Columns,
                                 p.ProjectionRelationName,
                                 p.GridMode
                                 );
        }

        public String GetReports(MethodGetByIdParam p = null)
        {
            return repository.GetReports();
        }

        public String GetGridIds(MethodGetGridIdsParam p)
        {
            var json = repository.GetGridIds(
                          p.EntityName,
                          JsonConvert.SerializeObject(p.Filter),
                          p.ParentEntityName,
                          p.ParentEntityId,
                          p.ProjectionRelationName,
                          p.IgnoreVisibleOnFilter
                          );

            return json;
        }

        public String MoveEmailsToFolder(MethodMoveEmailsToFolderParam p)
        {
            var json = repository.MoveEmailsToFolder(
                          p.FolderId,
                          p.EmailIds
                          );

            return json;
        }

        public String GetById(MethodGetByIdParam p, bool simple)
        {

            var json = repository.GetById(p.EntityName, p.EntityId, simple);

            return json;
        }

        public String GetItemsForAutocomplete(MethodGetAutocompleteParam p)
        {
            var json = repository.GetItemsForAutocomplete(
                                    p.EntityName,
                                    p.SearchText,
                                    JsonConvert.SerializeObject(p.Filter)
                                   );

            return json;
        }

        public String GetByFilter(MethodGetEntityFilteredParam p)
        {
            var json = repository.GetByFilter(
                                   p.EntityName,
                                   JsonConvert.SerializeObject(p.Filter)
                                   );

            return json;
        }

        public String GetEntityList(MethodGetEntityListParam p)
        {
            var json = repository.GetEntityList(
                                p.EntityName
                                );
            return json;
        }

        public String GetMenu(MethodGetMenuParam p = null)
        {
            return repository.GetMenu();
        }

        public String GetMerkInfoRegNumber(MethodGetMerkInfoRegNumber p = null)
        {
            return repository.GetMerkInfoRegNumber(p.RegNumber, p.CountryCode, p.Advanced);
        }
        public String GetMerkSuggest(MethodGetMerkSuggest p = null)
        {
            return repository.GetMerkSuggest(p.RegNumber, p.Email, p.Name, p.CountryCode);
        }

        public String GetEmailFolders(MethodGetEmailFoldersParam p = null)
        {
            return repository.GetEmailFolders();
        }

        public String GetSchema(MethodGetSchemaParam p = null)
        {
            return repository.GetSchema();
        }

        public String GetEnumerationByType(MethodGetEnumerationParam p)
        {
            var json = repository.GetEnumerationByType(p.EnumerationType, p.ParentValue, p.FilterText);

            return json;
        }
        public String GetCategoriesByProjectionName(MethodGetCategoriesByProjectionNameParam p)
        {
            var json = repository.GetCategoriesByProjectionName(p.ProjectionName);
            return json;
        }

        public String GdprDeleteData(MethodGdprDeleteDataParam p)
        {
            var json = repository.GdprDeleteData(p.EntityName, p.EntityId, p.Columns);
            return json.Result;
        }

        public String UpdateRecord(MethodSaveParam p)
        {
            var json = repository.Update(p.EntityName, p.EntityId, p.Data);
            return json.Result;
        }

        public String Import(MethodImportParam p)
        {
            var json = repository.Import(p.ProjectionName, p.PairingColumn, p.Columns, p.Data);
            return json;
        }

        public String SetEmailIsRead(MethodSetEmailIsReadParam p)
        {
            var json = repository.SetEmailIsRead(p.EmailId, p.IsRead);
            return json;
        }

        public String SetEmailsIsRead(MethodSetEmailsIsReadParam p)
        {
            foreach (int emailId in p.EmailsId)
            {
                repository.SetEmailIsRead(emailId, p.IsRead);
            }
            return "{}";
        }

        public String SetEmailFolderIsRead(MethodSetEmailFolderIsReadParam p)
        {
            var json = repository.SetEmailFolderIsRead(
                               p.FolderId,
                               p.IsRead
                               );

            return json;
        }

        public String SetEmailIsLinkedWithVistos(MethodSetEmailIsFlaggedParam p)
        {
            var json = repository.SetEmailIsLinkedWithVistos(
                               p.EmailId,
                               p.IsFlagged
                               );
            return json;
        }

        public String SetEmailIsFlagged(MethodSetEmailIsFlaggedParam p)
        {
            var json = repository.SetEmailIsFlagged(
                               p.EmailId,
                               p.IsFlagged
                               );
            return json;
        }

        public String MassAction(MethodMassActionGridParam p)
        {
            var json = repository.MassAction(
                               p.EntityName,
                               p.ActionName,
                               JsonConvert.SerializeObject(p.EntityId),
                               p.Value
                               );

            return json;
        }

        public String MassRemove(MethodMassRemoveParam p)
        {
            if (p.EntityIds != null && p.EntityIds.Length > 0)
            {
                foreach (int entityId in p.EntityIds)
                {
                    if (entityId > 0)
                    {
                       //**var trackChangesSettings = TrackChangesService.CreateSettings(this.userInfo, p.EntityName);
                       //**trackChangesSettings.Action = TrackChangesAction.REMOVE;
                       //**trackChangesSettings.RecordId = entityId;
                       //**
                       //**var json = repository.Remove(
                       //**                       p.EntityName,
                       //**                       entityId,
                       //**                       null,
                       //**                       null,
                       //**                       null,
                       //**                       trackChangesSettings
                       //**                       );
                    }
                }
            }
            return "{}";
        }

        public String Remove(MethodRemoveParam p)
        {
            //**var trackChangesSettings = TrackChangesService.CreateSettings(this.userInfo, p.EntityName);
            //**trackChangesSettings.Action = TrackChangesAction.REMOVE;
            //**trackChangesSettings.RecordId = p.EntityId;
            //**
            //**var json = repository.Remove(
            //**                       p.EntityName,
            //**                       p.EntityId,
            //**                       p.ParentEntityName,
            //**                       p.ParentEntityId,
            //**                       p.GridMode,
            //**                       trackChangesSettings
            //**                       );
            //**
            //**return json.Result;
            return "{}";
        }

        public String Add(MethodCreateParam p)
        {
            var json = repository.Add(p.EntityName,
                                      p.Data);
            return json.Result;
        }

        public String SaveLayout(SaveLayoutParam p)
        {
            var json = repository.SaveLayout(
                                   p.EntityName,
                                   JsonConvert.SerializeObject(p.Layout),
                                   p.Mode
                                   );
            return json;
        }

        public String GetGridSettings(GetGridSettingsParam p)
        {
            var json = repository.GetGridSettings(
                                p.ProjectionName,
                                p.GridName,
                                p.GridSettingsType
                            );

            return json;
        }

        public String SaveGridSettings(SaveGridSettingsParam p)
        {
            var json = repository.SaveGridSettings(
                                p.ProjectionName,
                                p.GridName,
                                p.GridSettingsType,
                                JsonConvert.SerializeObject(p.GridSettings)
                                );

            return json;
        }

        public String FullTextSearch(MethodFullTextSearchParam p)
        {
            var json = repository.FullTextSearch(
                                p.Text,
                                p.IncludeDiscussionMessage,
                                p.DbObjectIdArrayJson
                                );

            return json;
        }

        public String GetCalendarAppointments(MethodGetCalendarDataParam p)
        {
            var entitiesAndRoles = new Dictionary<string, List<int>>();
            List<int> userIDs = new List<int>();

            foreach (var item in p.Filter as JObject)
            {
                if (item.Key == "UserID")
                {
                    if (item.Value.GetType().ToString() == "Newtonsoft.Json.Linq.JArray")
                    {
                        foreach (var userID in item.Value as JArray)
                        {
                            int tmpInt;
                            if (int.TryParse(userID.ToString(), out tmpInt))
                            {
                                userIDs.Add(tmpInt);
                            }
                        }
                    }
                }
                else if (!item.Key.StartsWith("Roles_"))
                {
                    bool tmpBool;
                    if (bool.TryParse(item.Value.ToString(), out tmpBool) && tmpBool)
                    {
                        entitiesAndRoles.Add(item.Key, new List<int>());

                        var token1 = (p.Filter as JObject)["Roles_" + item.Key];
                        if (token1 != null && token1.Type == JTokenType.Array)
                        {
                            entitiesAndRoles[item.Key] = token1.ToObject<List<int>>();
                        }
                        else
                        {
                            entitiesAndRoles[item.Key] = Settings.GetInstance.VwRoleList.Select(r => r.Role_ID).ToList();
                        }
                    }
                }
            }

            var json = repository.GetCalendarAppointments(
                           p.StartDate,
                           p.EndDate,
                           entitiesAndRoles,
                           userIDs
                           );

            return json;
        }

        public String UpdateCalendarData(MethodUpdateCalendarDataParam p)
        {
            var json = repository.UpdateCalendarData(
                                    p.EntityName,
                                    p.EntityId,
                                    p.StartDate,
                                    p.EndDate
                                    );

            return json;
        }

        public String CreateEntityFrom(MethodCreateEntityFromParam p)
        {
            var json = repository.CreateEntityFrom(
                                   p.EntityNameFrom,
                                   p.EntityIdFrom,
                                   p.EntityNameTarget,
                                   p.ActionTypeName,
                                   p.MethodMode,
                                   p.ResultType,
                                   p.ExtData
                                   );

            return json;
        }

        public String GetFilterSettings(FilterSettingsParam p)
        {
            var json = repository.GetFilterSettings(
                                p.EntityName,
                                p.FilterName,
                                p.FilterType
                                );

            return json;
        }

        public String SaveFilterSettings(FilterSettingsParam p)
        {
            var json = repository.SaveFilterSettings(
                             p.EntityName,
                             p.FilterName,
                             p.FilterType,
                             JsonConvert.SerializeObject(p.FilterSettings)
                             );

            return json;
        }

        public String MassUpdate(MethodMassActionUpdateParam p)
        {
            var json = repository.MassUpdate(
                                       p.EntityName,
                                       p.EntityId,
                                       p.Data
                                       );

            return json;
        }

        public String AddManyToMany(MethodAddExistingManyToManyParam p)
        {
            var json = repository.AddManyToMany(
                                        p.EntityName,
                                        p.EntityId,
                                        p.ParentEntityName,
                                        p.ParentEntityId
                                        );

            return json;
        }

        public String GetSpData(MethodGetSpDataParam p)
        {
            var json = repository.GetSpData(p.SpName, p.SpParameters);
            return json;
        }

        public String GetProjectionAccessRights(MethodGetProjectionAccessRightsParam p)
        {
            var json = repository.GetProjectionAccessRights(
                                p.Filter
                                );
            return json;
        }

        public String SaveProjectionAccessRights(MethodSaveAccessRightsParam p)
        {
            if (this.userInfo.ProfileId != Settings.PROFILE_SYS_ADMIN_ID)
            {
                throw new Exception("Active Profile has not access right on: Save Projection");
            }

            var json = repository.SaveProjectionAccessRights(
                    p.Data,
                    p.ForProfil
                    );

            return json;
        }

        public String ResetLayout(MethodResetLayoutParam resetLayoutParam, SaveLayoutParam saveLayoutParam)
        {
            var json = repository.ResetLayout(
                            resetLayoutParam.EntityName,
                            saveLayoutParam.Mode
                        );

            return json;
        }

        public String GetProjectionAccessRightsColumns(MethodGetAccessRightsColumnByProjection p)
        {
            if (this.userInfo.ProfileId != Settings.PROFILE_SYS_ADMIN_ID)
            {
                throw new Exception("Active Profile has not access right on: Get Projection Access Rights Columns");
            }

            var json = repository.GetProjectionAccessRightsColumns(p.Filter);

            return json;
        }

        public String GetLocalization(MethodGetLocalizationParam p)
        {
            if (this.userInfo.ProfileId != Settings.PROFILE_SYS_ADMIN_ID)
            {
                throw new Exception("Active Profile has not access right on: Get Localization");
            }

            var json = repository.GetLocalization(p.Filter);
            return json;
        }

        public String SaveLocalization(MethodSaveLocalizationParam p)
        {
            if (this.userInfo.ProfileId != Settings.PROFILE_SYS_ADMIN_ID)
            {
                throw new Exception("Active Profile has not access right on: Save Localization");
            }

            var json = repository.SaveLocalization(p.Data);

            return json;
        }

        public String SaveProjectionAccessRightsColumns(MethodSaveAccessRightsColumnsParam p)
        {
            var json = repository.SaveProjectionAccessRightsColumns(p.Data);
            return json;
        }

        public String GetDiscussionByProjection(MethodGetDiscussionByEntitParam p)
        {
            var json = repository.GetDiscussionByProjection(p.RecordId, p.ProjectionName);
            return json;
        }

        public String SaveNewDiscussionMessage(MethodSaveDiscussionMessageParam p, int userId)
        {
            var json = repository.SaveNewDiscussionByEntity(
                p.Text,
                p.RecordId,
                p.ProjectionName,
                p.HierarchyId
                );

            return json;
        }

        public String EditDiscussionMessage(MethodEditDiscussionMessageParam p)
        {
            var json = repository.EditDiscussionMessage(
                           p.Text,
                           p.IsSystemMessage,
                           p.EntityFk,
                           p.EntityName,
                           p.HierarchyID
                           );
            return json;
        }

        public String CallCustomMethod(MethodCallCustomMethodParam p)
        {
            var json = repository.CallCustomMethod(
                             p.MethodName,
                             p.EntityName,
                             p.EntityId,
                             p.Data,
                             null
                             );

            return json;
        }

        public DataTable GetExport_Text(MethodGetExportParam p)
        {
            DataTable dataTable = repository.GetExportDataTable_Text(
                                  p.EntityName,
                                  p.SortOrder != null && p.SortOrder.Count > 0 ? p.SortOrder[0].ColumnName : "",
                                  p.SortOrder != null && p.SortOrder.Count > 0 ? p.SortOrder[0].Direction : "",
                                  JsonConvert.SerializeObject(p.Filter),
                                  p.Schema.ToArray(),
                                  p.Columns,
                                  p.ParentEntityName,
                                  p.ParentEntityId,
                                  p.ProjectionRelationName,
                                  p.GridMode

                     );

            if (dataTable == null)
            {
                throw new NullReferenceException("Export - DataTable can not be null!");
            }

            return dataTable;

        }

        public bool IsAllowedCallQueryBuilder(string entityName, OperationAccessRightsEnum operationAccessRightsEnum)
        {
            return Settings.GetInstance.Authorize(userInfo, entityName, operationAccessRightsEnum);
        }

    }
}
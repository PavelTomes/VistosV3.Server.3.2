using Core.Extensions;
using Core.Models;
using Core.Models.ApiRequest;
using Core.Repository;
using Core.Services;
using Core.VistosDb;
using Core.VistosDb.Objects;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
using System.Data;

namespace VistosV3.Server.Code
{
    public class ApiManager
    {
        private ApiRequest request = null;
        private UserInfo userInfo = null;
        private ResponseBuilder responseBuilder = null;

        private MethodManager service = null;
        public IAuditService auditService { get; private set; }

        public ApiManager(ApiRequest request, UserInfo userInfo, IAuditService auditService)
        {
            this.auditService = auditService;
            this.request = request;
            this.userInfo = userInfo;
            this.service = new MethodManager(userInfo, this.auditService);
            this.responseBuilder = new ResponseBuilder();
        }

        public ApiResponse Execute()
        {
            try
            {
                if (this.userInfo == null)
                {
                    if (request.LoginParam != null)
                    {
                        var repository = new DbRepository(this.auditService);
                        var json = repository.Login(request.LoginParam.UserName, request.LoginParam.Password);

                        if (!String.IsNullOrEmpty(json))
                        {
                            Settings.GetInstance.VwUserAuthTokenListReset();
                            return responseBuilder.ReturnRepositoryResponseOK(json);
                        }

                        return responseBuilder.ReturnRepositoryResponseInvalidLogin("");
                    }
                    return responseBuilder.ReturnRepositoryResponseUnauthenticated("");
                }
                else
                {
                    if (request.GetGridCountParam != null)
                    {
                        if (service.IsAllowedCallQueryBuilder(request.GetGridCountParam.EntityName, OperationAccessRightsEnum.GetPageParam))
                        {
                            try
                            {
                                var json = service.GetGridCount(request.GetGridCountParam);
                                return responseBuilder.ReturnRepositoryResponseOK(json);
                            }
                            catch (Exception ex)
                            {
                                Logger.SaveLogError(LogLevel.Error, this.ToString(), ex, request, userInfo);
                                return responseBuilder.ReturnRepositoryResponseWarning("{}", "ERROR_LOAD_DATAGRID_RECORDS");
                            }
                        }
                        return responseBuilder.ReturnRepositoryResponseWarning("{}", "ERROR_LOAD_DATAGRID_RECORDS");
                    }

                    if (request.GetPageParam != null)
                    {
                        if (service.IsAllowedCallQueryBuilder(request.GetPageParam.EntityName, OperationAccessRightsEnum.GetPageParam))
                        {
                            try
                            {
                                var json = service.GetPage(request.GetPageParam);
                                return responseBuilder.ReturnRepositoryResponseOK(json);
                            }
                            catch (Exception ex)
                            {
                                Logger.SaveLogError(LogLevel.Error, this.ToString(), ex, request, userInfo);
                                return responseBuilder.ReturnRepositoryResponseWarning("{}", "ERROR_LOAD_DATAGRID_RECORDS");
                            }
                        }
                        return responseBuilder.ReturnRepositoryResponseUnauthorized();
                    }

                    if (request.MoveEmailsToFolderParam != null)
                    {
                        var json = service.MoveEmailsToFolder(request.MoveEmailsToFolderParam);
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.GetRsReportList != null)
                    {
                        try
                        {
                            var json = service.GetReports(); //repository.GetReports();
                            return responseBuilder.ReturnRepositoryResponseOK(json);
                        }
                        catch (Exception ex)
                        {
                            Logger.SaveLogError(LogLevel.Error, this.ToString(), ex, request, userInfo);
                            return responseBuilder.ReturnRepositoryResponseError(ex.Message);
                        }
                    }
                    if (request.GetGridIdsParam != null)
                    {
                        if (service.IsAllowedCallQueryBuilder(request.GetGridIdsParam.EntityName, OperationAccessRightsEnum.GetGridIdsParam))
                        {
                            var json = service.GetGridIds(request.GetGridIdsParam);
                            return responseBuilder.ReturnRepositoryResponseOK(json);
                        }
                        return responseBuilder.ReturnRepositoryResponseUnauthorized("");
                    }
                    if (request.GetByIdParam != null)
                    {
                        if (service.IsAllowedCallQueryBuilder(request.GetByIdParam.EntityName, OperationAccessRightsEnum.GetByIdParam) || request.GetByIdParam.EntityId == 0)
                        {
                            String json = service.GetById(request.GetByIdParam, false);
                            return responseBuilder.ReturnRepositoryResponseOK(json, "");
                        }

                        return responseBuilder.ReturnRepositoryResponseUnauthorized("");
                    }
                    if (request.GetByIdSimpleParam != null)
                    {
                        if (service.IsAllowedCallQueryBuilder(request.GetByIdSimpleParam.EntityName, OperationAccessRightsEnum.GetByIdParam) || request.GetByIdSimpleParam.EntityId == 0)
                        {
                            String json = service.GetById(request.GetByIdSimpleParam, true);
                            return responseBuilder.ReturnRepositoryResponseOK(json, "");
                        }

                        return responseBuilder.ReturnRepositoryResponseUnauthorized("");
                    }
                    if (request.ImportParam != null)
                    {
                        if (request.ImportParam.Data != null)
                        {
                            if (service.IsAllowedCallQueryBuilder(request.ImportParam.ProjectionName, OperationAccessRightsEnum.SaveParam))
                            {
                                var json = service.Import(request.ImportParam);
                                return responseBuilder.ReturnRepositoryResponseOK(json);
                            }
                            return responseBuilder.ReturnRepositoryResponseUnauthorized("");
                        }
                    }
                    if (request.GetAutocompleteParam != null)
                    {
                        if (service.IsAllowedCallQueryBuilder(request.GetAutocompleteParam.EntityName, OperationAccessRightsEnum.GetAutocompleteParam))
                        {
                            var json = service.GetItemsForAutocomplete(request.GetAutocompleteParam);
                            return responseBuilder.ReturnRepositoryResponseOK(json);
                        }
                        return responseBuilder.ReturnRepositoryResponseUnauthorized("");
                    }
                    if (request.GetEntityFilteredParam != null)
                    {
                        if (service.IsAllowedCallQueryBuilder(request.GetEntityFilteredParam.EntityName, OperationAccessRightsEnum.GetEntityFilteredParam))
                        {
                            var json = service.GetByFilter(request.GetEntityFilteredParam);
                            return responseBuilder.ReturnRepositoryResponseOK(json);
                        }
                        return responseBuilder.ReturnRepositoryResponseUnauthorized("");
                    }
                    if (request.GetEntityListParam != null)
                    {
                        var json = service.GetEntityList(request.GetEntityListParam);

                        if (!String.IsNullOrEmpty(json))
                        {
                            return responseBuilder.ReturnRepositoryResponseOK(json);
                        }
                        else
                        {
                            return responseBuilder.ReturnRepositoryResponseError("EntityList is empty!");
                        }

                    }
                    if (request.GetMenu != null)
                    {
                        var json = service.GetMenu();
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.GetMerkInfoRegNumberParam != null)
                    {
                        var json = service.GetMerkInfoRegNumber(request.GetMerkInfoRegNumberParam);
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.GetMerkSuggestParam != null)
                    {
                        var json = service.GetMerkSuggest(request.GetMerkSuggestParam);
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.GetEmailFoldersParam != null)
                    {
                        var json = service.GetEmailFolders();
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.GetSchema != null)
                    {
                        var json = service.GetSchema();
                        if (String.IsNullOrEmpty(json))
                        {
                            return responseBuilder.ReturnRepositoryResponseError("Get schema error");
                        }

                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.GetEnumerationParam != null)
                    {
                        var json = service.GetEnumerationByType(request.GetEnumerationParam);
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.GetCategoriesByProjectionNameParam != null)
                    {
                        var json = service.GetCategoriesByProjectionName(request.GetCategoriesByProjectionNameParam);
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.GdprDeleteDataParam != null)
                    {
                        if (request.GdprDeleteDataParam.Columns != null)
                        {
                            if (service.IsAllowedCallQueryBuilder(request.GdprDeleteDataParam.EntityName, OperationAccessRightsEnum.SaveParam))
                            {
                                var json = service.GdprDeleteData(request.GdprDeleteDataParam);
                                return responseBuilder.ReturnRepositoryResponseOK(json);
                            }
                            return responseBuilder.ReturnRepositoryResponseUnauthorized("");
                        }
                    }
                    if (request.SaveParam != null)
                    {
                        if (request.SaveParam.Data != null)
                        {
                            IList<ValidationError> errList = new List<ValidationError>();
                            JObject jsonToValidate = (JObject)request.SaveParam.Data.DeepClone();
                            if (!jsonToValidate.Validate(request.SaveParam.EntityName, this.userInfo, out errList) || (errList != null && errList.Count > 0))
                            {
                                Logger.SaveLogError(LogLevel.JsonValidationError, errList, new System.ArgumentException("Json Validation Error - Update"), request, userInfo);
                                responseBuilder.ReturnRepositoryResponseInvalidJson(errList);
                            }
                            if (service.IsAllowedCallQueryBuilder(request.SaveParam.EntityName, OperationAccessRightsEnum.SaveParam))
                            {
                                var json = service.UpdateRecord(request.SaveParam);
                                return responseBuilder.ReturnRepositoryResponseOK(json);
                            }
                            return responseBuilder.ReturnRepositoryResponseUnauthorized("");
                        }
                    }
                    if (request.SetEmailIsReadParam != null)
                    {
                        var json = service.SetEmailIsRead(request.SetEmailIsReadParam);
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.SetEmailsIsReadParam != null)
                    {
                        var json = service.SetEmailsIsRead(request.SetEmailsIsReadParam);
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.SetEmailFolderIsReadParam != null)
                    {
                        var json = service.SetEmailFolderIsRead(request.SetEmailFolderIsReadParam);
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.SetEmailIsLinkedWithVistosParam != null)
                    {
                        var json = service.SetEmailIsLinkedWithVistos(request.SetEmailIsLinkedWithVistosParam);
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.SetEmailIsFlaggedParam != null)
                    {
                        var json = service.SetEmailIsFlagged(request.SetEmailIsFlaggedParam);
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.MassActionGridParam != null)
                    {
                        if (!string.IsNullOrEmpty(request.MassActionGridParam.ActionName))
                        {
                            if (service.IsAllowedCallQueryBuilder(request.MassActionGridParam.EntityName, OperationAccessRightsEnum.MassActionGridParam))
                            {
                                var json = service.MassAction(request.MassActionGridParam);
                                return responseBuilder.ReturnRepositoryResponseOK(json);
                            }
                            return responseBuilder.ReturnRepositoryResponseUnauthorized("");
                        }
                    }
                    if (request.RemoveParam != null)
                    {
                        if (service.IsAllowedCallQueryBuilder(request.RemoveParam.EntityName, OperationAccessRightsEnum.RemoveParam))
                        {
                            var json = service.Remove(request.RemoveParam);
                            return responseBuilder.ReturnRepositoryResponseOK(json);
                        }
                        return responseBuilder.ReturnRepositoryResponseUnauthorized("");
                    }
                    if (request.MassRemoveParam != null)
                    {
                        if (service.IsAllowedCallQueryBuilder(request.MassRemoveParam.EntityName, OperationAccessRightsEnum.RemoveParam))
                        {
                            var json = service.MassRemove(request.MassRemoveParam);
                            return responseBuilder.ReturnRepositoryResponseOK(json);
                        }
                        return responseBuilder.ReturnRepositoryResponseUnauthorized("");
                    }
                    if (request.CreateParam != null)
                    {
                        if (request.CreateParam.EntityName == "Log")
                        {
                            return responseBuilder.ReturnRepositoryResponseOK(null, "");
                        }

                        if (request.CreateParam.Data != null)
                        {
                            IList<ValidationError> errList = new List<ValidationError>();
                            JObject jsonToValidate = (JObject)request.CreateParam.Data.DeepClone();
                            if (!jsonToValidate.Validate(request.CreateParam.EntityName, this.userInfo, out errList) || (errList != null && errList.Count > 0))
                            {
                                Logger.SaveLogError(LogLevel.JsonValidationError, errList, new System.ArgumentException("Json Validation Error - Create"), request, userInfo);
                                responseBuilder.ReturnRepositoryResponseInvalidJson(errList);
                            }
                            if (service.IsAllowedCallQueryBuilder(request.CreateParam.EntityName, OperationAccessRightsEnum.CreateParam))
                            {
                                var json = service.Add(request.CreateParam);
                                return responseBuilder.ReturnRepositoryResponseOK(json);
                            }
                            return responseBuilder.ReturnRepositoryResponseUnauthorized("");
                        }
                    }
                    if (request.SaveLayoutParam != null)
                    {
                        if (request.SaveLayoutParam.Layout != null)
                        {
                            var json = service.SaveLayout(request.SaveLayoutParam);
                            return responseBuilder.ReturnRepositoryResponseOK(json);
                        }
                    }
                    if (request.GetGridSettingsParam != null)
                    {
                        var json = service.GetGridSettings(request.GetGridSettingsParam);
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.SaveGridSettingsParam != null)
                    {
                        if (this.userInfo.ProfileId == Settings.PROFILE_SYS_ADMIN_ID)
                        {
                            var json = service.SaveGridSettings(request.SaveGridSettingsParam);
                            return responseBuilder.ReturnRepositoryResponseOK(json);
                        }
                        return responseBuilder.ReturnRepositoryResponseOK("{}");
                    }
                    if (request.FullTextSearchParam != null)
                    {
                        var json = service.FullTextSearch(request.FullTextSearchParam);
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.AddSignatureParam != null)
                    {
                        if (!string.IsNullOrWhiteSpace(request.AddSignatureParam.Data))
                        {
                            byte[] bytes = System.Convert.FromBase64String(request.AddSignatureParam.Data.Replace("data:image/png;base64,", ""));
                            Guid guid = Guid.NewGuid();
                            using (VistosDbContext ctx = new VistosDbContext())
                            {
                                Signature signature = new Signature();
                                signature.Deleted = false;
                                signature.CreatedBy_FK = this.userInfo.UserId;
                                signature.Modified = DateTime.Now;
                                signature.Created = DateTime.Now;
                                signature.Value = string.Empty;
                                signature.UniqueGuid = guid;
                                signature.Bitmap = bytes;
                                ctx.Signature.Add(signature);
                                ctx.SaveChanges();
                                return responseBuilder.ReturnRepositoryResponseOK($"\"{guid.ToString()}\"", "");
                            }
                        }
                    }
                    if (request.GetCalendarDataParam != null)
                    {
                        var json = service.GetCalendarAppointments(request.GetCalendarDataParam);
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.UpdateCalendarDataParam != null)
                    {
                        if (service.IsAllowedCallQueryBuilder(request.UpdateCalendarDataParam.EntityName, OperationAccessRightsEnum.UpdateCalendarDataParam))
                        {
                            var json = service.UpdateCalendarData(request.UpdateCalendarDataParam);
                            return responseBuilder.ReturnRepositoryResponseOK(json);
                        }
                        return responseBuilder.ReturnRepositoryResponseUnauthorized("");
                    }
                    if (request.CreateEntityFromParam != null)
                    {
                        if (service.IsAllowedCallQueryBuilder(request.CreateEntityFromParam.EntityNameFrom, OperationAccessRightsEnum.CreateEntityFromParam))
                        {
                            var json = service.CreateEntityFrom(request.CreateEntityFromParam);
                            return responseBuilder.ReturnRepositoryResponseOK(json);
                        }
                        return responseBuilder.ReturnRepositoryResponseUnauthorized("");
                    }
                    if (request.GetFiltersParam != null)
                    {
                        var json = service.GetFilterSettings(request.GetFiltersParam);
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.SaveFiltersParam != null)
                    {
                        var json = service.SaveFilterSettings(request.SaveFiltersParam);
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.MassActionUpdateParam != null)
                    {
                        if (request.MassActionUpdateParam.Data.HasValues)
                        {
                            if (service.IsAllowedCallQueryBuilder(request.MassActionUpdateParam.EntityName, OperationAccessRightsEnum.MassActionUpdateParam))
                            {
                                var json = service.MassUpdate(request.MassActionUpdateParam);
                                return responseBuilder.ReturnRepositoryResponseOK(json);
                            }
                            return responseBuilder.ReturnRepositoryResponseUnauthorized("");
                        }
                        return responseBuilder.ReturnRepositoryResponseOK("{}", "");
                    }
                    if (request.AddExistingManyToManyParam != null)
                    {
                        var json = service.AddManyToMany(request.AddExistingManyToManyParam);
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.GetSpDataParam != null)
                    {
                        try
                        {
                            var json = service.GetSpData(request.GetSpDataParam);
                            return responseBuilder.ReturnRepositoryResponseOK(json);
                        }
                        catch (Exception ex)
                        {
                            Logger.SaveLogError(LogLevel.ErrorGetSpData, this.ToString(), ex, request, userInfo);
                            return responseBuilder.ReturnRepositoryResponseWarning("{}", "ERROR_GET_SP_DATA");
                        }
                    }
                    if (request.GetProjectionAccessRightsParam != null)
                    {
                        if (this.userInfo.ProfileId == 1)
                        {
                            var json = service.GetProjectionAccessRights(request.GetProjectionAccessRightsParam);
                            return responseBuilder.ReturnRepositoryResponseOK(json);
                        }
                        return responseBuilder.ReturnRepositoryResponseUnauthorized("");
                    }
                    if (request.SaveAccessRightsParam != null)
                    {
                        try
                        {
                            var json = service.SaveProjectionAccessRights(request.SaveAccessRightsParam);
                            return responseBuilder.ReturnRepositoryResponseOK(json);
                        }
                        catch (Exception ex)
                        {
                            Logger.SaveLogError(LogLevel.Error, this.ToString(), ex, request, userInfo);
                            return responseBuilder.ReturnRepositoryResponseUnauthorized("");
                        }
                    }
                    if (request.ResetLayoutParam != null)
                    {
                        // TODO: proc jsou tady dva parametry (ResetLayoutParam,SaveLayoutParam)?
                        var json = service.ResetLayout(request.ResetLayoutParam, request.SaveLayoutParam);
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.GetAccessRightsColumnByProjectionParam != null)
                    {
                        try
                        {
                            var json = service.GetProjectionAccessRightsColumns(
                                request.GetAccessRightsColumnByProjectionParam
                                );

                            return responseBuilder.ReturnRepositoryResponseOK(json);
                        }
                        catch (Exception ex)
                        {
                            Logger.SaveLogError(LogLevel.Error, this.ToString(), ex, request, userInfo);
                            return responseBuilder.ReturnRepositoryResponseUnauthorized("");
                        }

                    }
                    if (request.GetLocalizationParam != null)
                    {
                        try
                        {
                            var json = service.GetLocalization(request.GetLocalizationParam);
                            return responseBuilder.ReturnRepositoryResponseOK(json);
                        }
                        catch (Exception ex)
                        {
                            Logger.SaveLogError(LogLevel.Error, this.ToString(), ex, request, userInfo);
                            return responseBuilder.ReturnRepositoryResponseUnauthorized("");
                        }
                    }
                    if (request.SaveLocalizationParam != null)
                    {
                        try
                        {
                            var json = service.SaveLocalization(request.SaveLocalizationParam);
                            return responseBuilder.ReturnRepositoryResponseOK(json);
                        }
                        catch (Exception ex)
                        {
                            Logger.SaveLogError(LogLevel.Error, this.ToString(), ex, request, userInfo);
                            return responseBuilder.ReturnRepositoryResponseUnauthorized("");
                        }
                    }

                    if (request.SaveAccessRightsColumnsParam != null)
                    {
                        var json = service.SaveProjectionAccessRightsColumns(request.SaveAccessRightsColumnsParam);
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.GetDiscussionByEntitParam != null)
                    {
                        var json = service.GetDiscussionByProjection(request.GetDiscussionByEntitParam);
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.SaveDiscussionMessageParam != null)
                    {
                        var json = service.SaveNewDiscussionMessage(request.SaveDiscussionMessageParam, userInfo.UserId);
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.EditDiscussionMessageParam != null)
                    {
                        var json = service.EditDiscussionMessage(request.EditDiscussionMessageParam);
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                    if (request.RestartAPIParam != null)
                    {
                        if (this.userInfo.ProfileId == Settings.PROFILE_SYS_ADMIN_ID)
                        {
                            Settings.Restart();
                            return responseBuilder.ReturnRepositoryResponseOK("", "Restart OK");
                        }
                        return responseBuilder.ReturnRepositoryResponseUnauthorized("");
                    }
                    if (request.CleanCacheParam != null)
                    {
                        if (this.userInfo.ProfileId == Settings.PROFILE_SYS_ADMIN_ID)
                        {
                            Settings.Restart();
                            return responseBuilder.ReturnRepositoryResponseOK("", "Clean cache OK");
                        }
                        return responseBuilder.ReturnRepositoryResponseUnauthorized("");
                    }
                    if (request.CallCustomMethodParam != null)
                    {
                        var json = service.CallCustomMethod(request.CallCustomMethodParam);
                        return responseBuilder.ReturnRepositoryResponseOK(json);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.SaveLogError(LogLevel.Error, this.ToString(), ex, request, userInfo);
            }
            return responseBuilder.ReturnRepositoryResponseError("Api manager Error");
        }

        public ApiResponse ExecuteExport()
        {
            if (this.request.GetExportParam != null)
            {
                if (service.IsAllowedCallQueryBuilder(request.GetExportParam.EntityName, OperationAccessRightsEnum.GetExportParam))
                {
                    DataTable dataTable = null;

                    try
                    {
                        dataTable = service.GetExport_Text(request.GetExportParam);
                    }
                    catch (Exception ex)
                    {
                        Logger.SaveLogError(LogLevel.Error, this.ToString(), ex, request, userInfo);
                    }

                    if (dataTable == null)
                    {
                        return responseBuilder.ReturnRepositoryResponseError("ExecuteExport");
                    }

                    try
                    {
                        switch (this.userInfo.UserLanguage)
                        {
                            case "cs-CZ":
                                request.GetExportParam.Schema.Add(new Column() { ColumnName = "EntityCategories", LocalizationString = "Kategorie", Type_FK = DbColumnTypeEnum.String });
                                break;
                            case "en-US":
                                request.GetExportParam.Schema.Add(new Column() { ColumnName = "EntityCategories", LocalizationString = "Category", Type_FK = DbColumnTypeEnum.String });
                                break;
                        }
                        ApiExportResponse rr = responseBuilder.ReturnRepositoryExportResponseOk(dataTable);
                        //**rr.DataExportResult = new DataExportResult(rr.DataTable,
                        //**                              Models.Structures.GetExportType(this.request.GetExportParam.ExportType.ToUpper()),
                        //**                            "export",
                        //**                            request.GetExportParam.Schema,
                        //**                            request.GetExportParam.Columns,
                        //**                            this.request.GetExportParam.EntityName,
                        //**                            this.userInfo.UserLanguage);
                        return rr;
                    }
                    catch (Exception ex)
                    {
                        Logger.SaveLogError(LogLevel.Error, this.ToString(), ex, request, userInfo);
                    }
                }
                return responseBuilder.ReturnRepositoryResponseUnauthorized("");
            }
            return responseBuilder.ReturnRepositoryResponseError("ExecuteExport");
        }
    }
}

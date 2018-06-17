using Core.Models.ApiRequest.Params;

namespace Core.Models.ApiRequest
{
    public class ApiRequest
    {
        public string RequestGuid { get; set; }
        public string RequestDatetime { get; set; }
        public string UserToken { get; set; }
        public string Culture { get; set; }
        public string Device { get; set; }
        public string Version { get; set; }
        public MethodGetGridCountParam GetGridCountParam { get; set; }
        public MethodGetPageParam GetPageParam { get; set; }
        public MethodGetExportParam GetExportParam { get; set; }
        public MethodGetGridIdsParam GetGridIdsParam { get; set; }
        public MethodGetByIdParam GetByIdParam { get; set; }
        public MethodGetByIdParam GetByIdSimpleParam { get; set; }
        public MethodGetAutocompleteParam GetAutocompleteParam { get; set; }
        public MethodGetEnumerationParam GetEnumerationParam { get; set; }
        public MethodGetEntityFilteredParam GetEntityFilteredParam { get; set; }
        public MethodGetEntityListParam GetEntityListParam { get; set; }
        public MethodSaveParam SaveParam { get; set; }
        public MethodGdprDeleteDataParam GdprDeleteDataParam { get; set; }
        //public MethodSendEmailParam SendEmailParam { get; set; }
        public MethodCreateParam CreateParam { get; set; }
        public MethodLoginParam LoginParam { get; set; }
        public MethodGetMenuParam GetMenu { get; set; }
        public MethodGetEmailFoldersParam GetEmailFoldersParam { get; set; }
        public MethodGetSchemaParam GetSchema { get; set; }
        public MethodAddSignatureParam AddSignatureParam { get; set; }
        public MethodRemoveParam RemoveParam { get; set; }
        public MethodMassRemoveParam MassRemoveParam { get; set; }
        public MethodMassActionGridParam MassActionGridParam { get; set; }
        public MethodGetCalendarDataParam GetCalendarDataParam { get; set; }
        public MethodUpdateCalendarDataParam UpdateCalendarDataParam { get; set; }
        public MethodGetCategoriesByProjectionNameParam GetCategoriesByProjectionNameParam { get; set; }
        public SaveLayoutParam SaveLayoutParam { get; set; }
        public SaveGridSettingsParam SaveGridSettingsParam { get; set; }
        public GetGridSettingsParam GetGridSettingsParam { get; set; }
        public FilterSettingsParam GetFiltersParam { get; set; }
        public FilterSettingsParam SaveFiltersParam { get; set; }
        public MethodCreateEntityFromParam CreateEntityFromParam { get; set; }
        public MethodMassActionUpdateParam MassActionUpdateParam { get; set; }
        public MethodGetByIdParam GetRsReportList { get; set; }
        public MethodSetEmailIsReadParam SetEmailIsReadParam { get; set; }
        public MethodSetEmailsIsReadParam SetEmailsIsReadParam { get; set; }
        public MethodSetEmailFolderIsReadParam SetEmailFolderIsReadParam { get; set; }
        public MethodSetEmailIsFlaggedParam SetEmailIsFlaggedParam { get; set; }
        public MethodSetEmailIsFlaggedParam SetEmailIsLinkedWithVistosParam { get; set; }
        public MethodFullTextSearchParam FullTextSearchParam { get; set; }

        public MethodAddExistingManyToManyParam AddExistingManyToManyParam { get; set; }
        public MethodGetSpDataParam GetSpDataParam { get; set; }
        public MethodGetProjectionAccessRightsParam GetProjectionAccessRightsParam { get; set; }
        public MethodSaveAccessRightsParam SaveAccessRightsParam { get; set; }
        public MethodSaveAccessRightsColumnsParam SaveAccessRightsColumnsParam { get; set; }
        public MethodResetLayoutParam ResetLayoutParam { get; set; }
        public MethodGetAccessRightsColumnByProjection GetAccessRightsColumnByProjectionParam { get; set; }
        public MethodGetDiscussionByEntitParam GetDiscussionByEntitParam { get; set; }
        public MethodSaveDiscussionMessageParam SaveDiscussionMessageParam { get; set; }
        public MethodEditDiscussionMessageParam EditDiscussionMessageParam { get; set; }
        public MethodRestartAPIParam RestartAPIParam { get; set; }
        public MethodCleanCacheParam CleanCacheParam { get; set; }
        public MethodGetLocalizationParam GetLocalizationParam { get; set; }
        public MethodSaveLocalizationParam SaveLocalizationParam { get; set; }
        public MethodCallCustomMethodParam CallCustomMethodParam { get; set; }

        public MethodImportParam ImportParam { get; set; }

        public MethodMoveEmailsToFolderParam MoveEmailsToFolderParam { get; set; }
        public MethodGetMerkInfoRegNumber GetMerkInfoRegNumberParam { get; set; }
        public MethodGetMerkSuggest GetMerkSuggestParam { get; set; }

        

    }
}
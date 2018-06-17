using Core.Models;
using Core.QueryBuilder.Templates;
using Core.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Core.QueryBuilder
{
    public class TemplateQueryBuilder
    {
        private Settings settings = Settings.GetInstance;

        public TemplateQueryBuilder()
        {
        }

        public string RemoveQuery(string projectionName, UserInfo userInfo, string parentProjectionName, int? parentEntityId, string gridMode)
        {
            TemplateBase template = null;
            if (!String.IsNullOrEmpty(parentProjectionName) && parentEntityId.HasValue && parentEntityId.Value > 0)
            {
                template = new Templates.DeleteRelationEntity(userInfo, projectionName, parentProjectionName, parentEntityId, gridMode);
            }
            else
            {
                template = new Templates.Delete(userInfo, projectionName);
            }
            string retVal = template.TransformText();
            return retVal;
        }

        public string GetByIdQuery(string projectionName, UserInfo userInfo, bool simple)
        {
            string retVal = settings.GetQueryBuilderScript(userInfo.UserLanguage, userInfo.ProfileId, projectionName, "GetById" + simple.ToString());
            if (string.IsNullOrEmpty(retVal))
            {
                var template = new Templates.GetById(userInfo, projectionName, simple);
                retVal = template.TransformText();
                settings.SetQueryBuilderScript(userInfo.UserLanguage, userInfo.ProfileId, projectionName, "GetById" + simple.ToString(), retVal);
            }
            return retVal;
        }

        public string GetImportQuery(string projectionName, string pairingColumn, string[] columns, UserInfo userInfo)
        {
            var template = new Templates.Import(userInfo, projectionName, pairingColumn, columns);
            string retVal = template.TransformText();
            return retVal;
        }

        public string UpdateCalendarEntity(string entityName, UserInfo userInfo)
        {
            var template = new Templates.UpdateCalendarEntity(entityName, userInfo);
            return template.TransformText();
        }

        public string GetCalendarEntities(Dictionary<string, List<int>> showEntities, List<int> userIDs, UserInfo userInfo)
        {
            var template = new Templates.GetCalendarEntities(showEntities, userIDs, userInfo);
            return template.TransformText();
        }

        public string GetGridIdsQuery(string projectionName, UserInfo userInfo, string filter, string parentProjectionName, int? parentEntityId, string projectionRelationName, bool ignoreSwitchIsVisibleOnFilter)
        {
            var template = new Templates.GetGridIds(userInfo, projectionName, filter, parentProjectionName, parentEntityId,
                projectionRelationName, ignoreSwitchIsVisibleOnFilter);
            string retVal = template.TransformText();
            return retVal;
        }

        public string GetItemsForAutocompleteQuery(string projectionName, UserInfo userInfo, string filter)
        {
            var template = new Templates.GetItemsForAutocomplete(userInfo, projectionName, filter);
            string retVal = template.TransformText();
            return retVal;
        }

        public string NewEntityQuery(string projectionName, UserInfo userInfo)
        {
            string retVal = settings.GetQueryBuilderScript(userInfo.UserLanguage, userInfo.ProfileId, projectionName, "NewEntity");
            if (string.IsNullOrEmpty(retVal))
            {
                var template = new Templates.New(userInfo, projectionName);
                retVal = template.TransformText();
                settings.SetQueryBuilderScript(userInfo.UserLanguage, userInfo.ProfileId, projectionName, "NewEntity", retVal);
            }
            return retVal;
        }

        public string CreateFromEntityQuery(string projectionNameFrom, string projectionNameTarget, ProjectionActionType actionTypeName, UserInfo userInfo)
        {
            string retVal = settings.GetQueryBuilderScript(userInfo.UserLanguage, userInfo.ProfileId, projectionNameFrom + projectionNameTarget, "CreateFromEntity");
            if (string.IsNullOrEmpty(retVal))
            {
                var template = new Templates.CreateFrom(userInfo, projectionNameFrom, projectionNameTarget, actionTypeName);
                retVal = template.TransformText();
                settings.SetQueryBuilderScript(userInfo.UserLanguage, userInfo.ProfileId, projectionNameFrom + projectionNameTarget, "CreateFromEntity", retVal);
            }
            return retVal;
        }

        public string GetUpdateQuery(string projectionName, UserInfo userInfo)
        {
            string retVal = settings.GetQueryBuilderScript(userInfo.UserLanguage, userInfo.ProfileId, projectionName, "GetUpdate");
            if (string.IsNullOrEmpty(retVal))
            {
                var template = new Templates.Update(userInfo, projectionName);
                retVal = template.TransformText();
                settings.SetQueryBuilderScript(userInfo.UserLanguage, userInfo.ProfileId, projectionName, "GetUpdate", retVal);
            }
            return retVal;
        }

        public string GetInsertQuery(string projectionName, UserInfo userInfo)
        {
            var template = new Templates.Insert(userInfo, projectionName);
            string retVal = template.TransformText();
            return retVal;
        }

        public string GetInsertManyTomanyQuery(string projectionName, string parentProjectionName, UserInfo userInfo)
        {
            var template = new Templates.InsertEntityEntity(projectionName, parentProjectionName, userInfo);
            string retVal = template.TransformText();
            return retVal;
        }

        public string GetGridCountQuery(
            UserInfo userInfo,
            string projectionName,
            string parentProjectionName,
            int parentEntityId,
            string ProjectionRelationName
            )
        {
            var template = new Templates.GetGridCount(
                userInfo,
                projectionName,
                parentProjectionName,
                parentEntityId,
                ProjectionRelationName
                );
            string retVal = template.TransformText();
            return retVal;
        }

        public string GetPageQuery_EntityActivity(
            UserInfo userInfo,
            int draw,
            int start,
            int length,
            string sortOrderColumnName,
            string sortOrderDirection,
            string filter,
            string parentProjectionName,
            int? parentEntityId,
            string[] columns
            )
        {
            var template = new Templates.GetPage_EntityActivity(
                userInfo,
                draw,
                start,
                length,
                sortOrderColumnName,
                sortOrderDirection,
                filter,
                parentProjectionName,
                parentEntityId,
                columns
                );
            string retVal = template.TransformText();
            return retVal;
        }

        public string GetPageQuery(
            UserInfo userInfo,
            string projectionName,
            int draw,
            int start,
            int length,
            string sortOrderColumnName,
            string sortOrderDirection,
            string filter,
            string parentProjectionName,
            int? parentEntityId,
            string[] columns,
            string ProjectionRelationName,
            string gridMode
            )
        {
            var template = new Templates.GetPage(
                userInfo,
                projectionName,
                draw,
                start,
                length,
                sortOrderColumnName,
                sortOrderDirection,
                filter,
                parentProjectionName,
                parentEntityId,
                columns,
                ProjectionRelationName,
                gridMode
                );
            string retVal = template.TransformText();
            return retVal;
        }

        public string GetExportQuery(
            UserInfo userInfo,
            string projectionName,
            string sortOrderColumnName,
            string sortOrderDirection,
            string filter,
            string[] columns,
            string parentProjectionName,
            int? parentEntityId,
            string projectionRelationName,
            string gridMode
            )
        {
            var template = new Templates.GetExport(
                userInfo,
                projectionName,
                sortOrderColumnName,
                sortOrderDirection,
                filter,
                parentProjectionName,
                parentEntityId,
                columns,
                projectionRelationName,
                gridMode
                );
            string retVal = template.TransformText();
            return retVal;
        }
        public string MassUpdateQuery(
           UserInfo userInfo,
           string projectionName,
           int[] ids,
           JObject data
           )
        {
            var template = new Templates.MassUpdate(
                userInfo,
                projectionName,
                ids,
                data
                );
            string retVal = template.TransformText();
            return retVal;
        }

    }
}

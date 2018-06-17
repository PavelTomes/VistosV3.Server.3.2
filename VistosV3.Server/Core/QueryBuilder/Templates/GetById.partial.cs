using Core.Models;
using Core.VistosDb.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.QueryBuilder.Templates
{
    public partial class GetById
    {
        private bool simple { get; set; }

        public GetById(UserInfo userInfo, string projectionName, bool simple)
        {
            this.userInfo = userInfo;
            this.vwProjection = settings.GetVwProjectionList(userInfo.ProfileId).First(x => x.Projection_Name == projectionName);
            this.entityType = settings.GetCrmEntityType(this.vwProjection.DbObject_Name);
            this.columns = settings.GetVwProjectionColumnList(userInfo.ProfileId, projectionName).Where(x =>
                (x.Column_IsVisibleOnForm || x.Column_HiddenData)
                && (x.AccessRightsType_Id == 1 || x.AccessRightsType_Id == 2)
                && !SystemEnums.SysColumns.Contains(x.DbColumn_Name)
                && !x.Column_IsPrimaryKey
                ).ToList();
            this.simple = simple;
        }

        private void WriteCaptionStringForColumn(vwProjectionColumn column)
        {
            if (column != null)
            {
                List<string> renderCaptionColumnNameList = new List<string>();
                Write(GetReferenceCaption(column.DbColumnType_Id, column.ProjectionReference_Id, column.ProjectionColumn_Name, null, vwProjection.Projection_Name, column.DbColumn_Name, column.Column_ComputedExpression, out renderCaptionColumnNameList));
            }
        }

        private void WriteCategoriesForEntity()
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine($",(");
            sb.AppendLine($"SELECT");
            sb.AppendLine($" [Category].[Category_ID] as [Id]");
            sb.AppendLine($" ,isnull([Category].[Description_{userInfo.UserLanguage}],[Category].[Description]) as [Description]");
            sb.AppendLine($"FROM [crm].[Entity_Category]");
            sb.AppendLine($"INNER JOIN [crm].[Category] on [crm].[Category].[Deleted] = 0 and [crm].[Category].Category_ID = [crm].[Entity_Category].[Category_FK]");
            sb.AppendLine($"where [crm].[Entity_Category].[Deleted] = 0 and [crm].[Entity_Category].[Type] = {entityType} and [crm].[Entity_Category].[Entity_FK] = [{vwProjection.Projection_Name}].[{vwProjection.DbPrimaryColumn_Name}]");
            sb.AppendLine($"FOR JSON PATH, INCLUDE_NULL_VALUES");
            sb.AppendLine($") as [EntityCategories]");
            sb.AppendLine();

            Write(sb.ToString());
        }

        private void WriteGrids()
        {
            List<vwProjectionRelation> gridItems = settings.VwProjectionRelationList.Where(x =>
                x.ProjectionRelation_ParentProjection_FK == vwProjection.Projection_Id
                && x.ProjectionRelation_ChildProjectionName != "Participant"
                && x.ProjectionRelation_ChildProjectionProfile_Id == userInfo.ProfileId
                && ((x.ProjectionRelation_ChildProjectionAccessRight & (long)1) == (long)1)
                ).ToList();

            foreach (vwProjectionRelation gridItem in gridItems)
            {
                switch (gridItem.ProjectionRelation_Type_FK)
                {
                    case (int)DbRelationTypeEnum.ItemGrid:
                        WriteItemGrid(gridItem);
                        break;
                    case (int)DbRelationTypeEnum.ItemMasterGrid:
                        WriteItemGrid(gridItem);
                        break;
                    default:
                        break;
                }
            }
        }

        private void WriteItemGrid(vwProjectionRelation gridItem)
        {
            GetByIdItemGrid GetByIdItemGridTemplate = new Templates.GetByIdItemGrid(userInfo, gridItem, vwProjection);
            string itemSql = GetByIdItemGridTemplate.TransformText();
            Write(itemSql);
        }

        private void WriteColumnBracketedString(vwProjectionColumn column)
        {
            string val = $",[{vwProjection.Projection_Name}].[{column.DbColumn_Name}] as [{column.ProjectionColumn_Name}]";

            if (column.DbColumnType_Id == (int)DbColumnTypeEnum.SystemChart || column.DbColumnType_Id == (int)DbColumnTypeEnum.UserChart)
            {
                val = $",null as [{column.ProjectionColumn_Name}]";
            }
            if (column.DbColumnType_Id == (int)DbColumnTypeEnum.Signature)
            {
                val = $",(SELECT TOP 1 [UniqueGuid] FROM [crm].[Signature] sig WHERE sig.[Deleted] = 0 AND sig.Id = [{vwProjection.Projection_Name}].[{column.DbColumn_Name}]) as [{column.ProjectionColumn_Name}]";
            }
            if (column.DbColumnType_Id == (int)DbColumnTypeEnum.Geography)
            {
                val = $",case when [{vwProjection.Projection_Name}].[{column.DbColumn_Name}] is not null then [{vwProjection.Projection_Name}].[{column.DbColumn_Name}].Lat else null end as [{column.ProjectionColumn_Name}_Lat]";
                val = val + Environment.NewLine + $",case when [{vwProjection.Projection_Name}].[{column.DbColumn_Name}] is not null then [{vwProjection.Projection_Name}].[{column.DbColumn_Name}].Long else null end as [{column.ProjectionColumn_Name}_Long]";
            }
            if (column.DbColumnType_Id == (int)DbColumnTypeEnum.MultiEnumeration)
            {
                val = $",JSON_QUERY([{vwProjection.Projection_Name}].[{column.DbColumn_Name}]) as [{column.ProjectionColumn_Name}]";
            }
            if (!string.IsNullOrEmpty(column.Column_ComputedExpression))
            {
                val = $",{column.Column_ComputedExpression.Replace("##EntityName##", vwProjection.Projection_Name)} as [{column.ProjectionColumn_Name}]";
            }
            if (column.DbColumnType_Id == (int)DbColumnTypeEnum.Password)
            {
                val = $",null as [{column.ProjectionColumn_Name}]";
            }
            Write(val);
        }

        new private void WriteWhereFilter()
        {
            if (vwProjection.DbObjectType_Id == 1)
            {
                Write($"where [{vwProjection.Projection_Name}].[Deleted] = 0{Environment.NewLine}");
            }
            else
            {
                Write($"where 1 = 1");
            }
            
            if (vwProjection.DbObject_Name == "TimeSheet")
            {
                Write($"and [{vwProjection.Projection_Name}].[CreatedBy_FK] = @userId{Environment.NewLine}");
            }
            if (vwProjection.DbObject_Name == "Email")
            {
                Write($"and (([{vwProjection.Projection_Name}].[IsPublic] = 1) or ([{vwProjection.Projection_Name}].[EmailAccount_Folder_FK] in (SELECT eac.Id FROM [crm].[EmailAccountFolder] eac INNER JOIN [crm].[UserEmailAccount] uea on uea.Id = eac.User_EmailAccount_FK AND uea.Deleted = 0 WHERE eac.Deleted = 0 and uea.User_FK = @userId))){Environment.NewLine}");
            }

            if (entityType > 0)
            {
                //Write($"and [{vwProjection.Projection_Name}].[{vwProjection.DbPrimaryColumn_Name}] not in (SELECT Entity_FK FROM [crm].[fn_GetBannedEntityIds] (@userId, {entityType}, 1)){Environment.NewLine}");
            }
            if (!String.IsNullOrEmpty(vwProjection.Projection_ProjectionFilter))
            {
                string filter = vwProjection.Projection_ProjectionFilter.Replace("##EntityName##", vwProjection.Projection_Name);
                Write($" {filter} {Environment.NewLine}");
            }
        }

    }
}

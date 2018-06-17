using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.VistosDb.Objects;

namespace Core.QueryBuilder.Templates
{
    public partial class Update
    {
        public Update(UserInfo userInfo, string projectionName)
        {
            this.userInfo = userInfo;
            this.vwProjection = settings.GetVwProjectionList(userInfo.ProfileId).First(x => x.Projection_Name == projectionName);
            this.entityType = settings.GetCrmEntityType(this.vwProjection.DbObject_Name);
            this.columns = settings.GetVwProjectionColumnList(userInfo.ProfileId, projectionName).Where(x =>
                x.AppColumnType_Id != 1
                && !x.Column_IsReadOnly
                && !x.Column_IsPrimaryKey
                && (x.Column_IsVisibleOnForm || x.Column_HiddenData)
                && x.AccessRightsType_Id == 2
                && !string.IsNullOrEmpty(x.Column_DbColumnTypeNative)
            ).ToList();
        }

        private void WriteGrids()
        {
            List<vwProjectionRelation> gridItems = settings.VwProjectionRelationList.Where(x =>
                x.ProjectionRelation_ParentProjection_FK == this.vwProjection.Projection_Id
                && x.ProjectionRelation_ChildProjectionName != "Participant"
                && x.ProjectionRelation_ChildProjectionProfile_Id == userInfo.ProfileId
                && ((x.ProjectionRelation_ChildProjectionAccessRight & (long)6) == (long)6)
                ).ToList();

            foreach (vwProjectionRelation gridItem in gridItems)
            {
                switch (gridItem.ProjectionRelation_Type_FK)
                {
                    case (int)DbRelationTypeEnum.ItemGrid:
                    case (int)DbRelationTypeEnum.ItemMasterGrid:
                        {
                            ItemGridUpdate itemGridUpdateTemplate = new ItemGridUpdate(userInfo, gridItem, vwProjection, entityType);
                            string childVal = itemGridUpdateTemplate.TransformText();
                            Write(childVal);

                            ItemGridInsert itemGridInsertTemplate = new ItemGridInsert(userInfo, gridItem, vwProjection, entityType);
                            string childInsertVal = itemGridInsertTemplate.TransformText();
                            Write(childInsertVal);
                        }
                        break;
                    default:
                        break;
                }

            }
        }

        private void WriteUpdateColumn(vwProjectionColumn column)
        {
            string val = $",[{column.DbColumn_Name}] = json.[{column.ProjectionColumn_Name}]";
            if (!string.IsNullOrEmpty(column.Column_UpdateDefaultValue))
            {
                val = $",[{column.DbColumn_Name}] = {column.Column_UpdateDefaultValue}";
            }
            else
            {
                if (column.DbColumnType_Id == (int)DbColumnTypeEnum.Geography)
                {
                    val = $",[{column.DbColumn_Name}] = case when json.{column.ProjectionColumn_Name}_Lat <> 0 and json.{column.ProjectionColumn_Name}_Long <> 0 then geography::Point(json.{column.ProjectionColumn_Name}_Lat, json.{column.ProjectionColumn_Name}_Long, 4326) else null end";
                }
                if (column.DbColumnType_Id == (int)DbColumnTypeEnum.Signature)
                {
                    val = $",[{column.DbColumn_Name}] = (select top 1 si.[Id] from [crm].[Signature] si where si.deleted = 0 and si.UniqueGuid = json.[{column.ProjectionColumn_Name}])";
                }
                if (column.DbColumnType_Id == (int)DbColumnTypeEnum.MultiEnumeration)
                {
                    val = $",[{column.DbColumn_Name}] = JSON_QUERY(@json, '$.{column.ProjectionColumn_Name}')";
                }
                if (column.DbColumnType_Id == (int)DbColumnTypeEnum.String
                    || column.DbColumnType_Id == (int)DbColumnTypeEnum.Text
                    || column.DbColumnType_Id == (int)DbColumnTypeEnum.WidgetHtml
                    || column.DbColumnType_Id == (int)DbColumnTypeEnum.TextSimple
                    || column.DbColumnType_Id == (int)DbColumnTypeEnum.IconSelect
                    || column.DbColumnType_Id == (int)DbColumnTypeEnum.JSON
                    || column.DbColumnType_Id == (int)DbColumnTypeEnum.Color
                    || column.DbColumnType_Id == (int)DbColumnTypeEnum.EmailAddress
                    || column.DbColumnType_Id == (int)DbColumnTypeEnum.WebUrl
                    || column.DbColumnType_Id == (int)DbColumnTypeEnum.PhoneNumber
                    )
                {
                    val = $",[{column.DbColumn_Name}] = iif(len(isnull(json.[{column.ProjectionColumn_Name}],'')) > 0, json.[{column.ProjectionColumn_Name}], null)";
                }
                if (column.DbColumnType_Id == (int)DbColumnTypeEnum.Password)
                {
                    val = $",[{column.DbColumn_Name}] = iif(len(isnull(json.[{column.ProjectionColumn_Name}],'')) > 0, json.[{column.ProjectionColumn_Name}], [{vwProjection.DbObject_Schema}].[{vwProjection.DbObject_Name}].[{column.DbColumn_Name}])";
                }
                if (!string.IsNullOrEmpty(val))
                {
                    WriteLine(val);
                }
            }
        }

        private void WriteJsonColumn(vwProjectionColumn column, bool isFirst)
        {
            string val = $"[{column.ProjectionColumn_Name}] {column.Column_DbColumnTypeNative}";
            if (column.DbColumnType_Id == (int)DbColumnTypeEnum.Geography)
            {
                val = $"[{column.ProjectionColumn_Name}_Lat] float, [{column.ProjectionColumn_Name}_Long] float";
            }
            if (column.DbColumnType_Id == (int)DbColumnTypeEnum.Signature)
            {
                val = $"[{column.ProjectionColumn_Name}] [varchar](50)";
            }
            if (column.DbColumnType_Id == (int)DbColumnTypeEnum.MultiEnumeration)
            {
                val = string.Empty;
            }
            if (!string.IsNullOrEmpty(val))
            {
                WriteLine((!isFirst ? "," : string.Empty) + val);
            }
        }
    }
}

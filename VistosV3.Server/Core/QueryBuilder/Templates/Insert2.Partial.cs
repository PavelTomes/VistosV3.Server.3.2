using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.Services;
using Core.VistosDb.Objects;

namespace Core.QueryBuilder.Templates
{
    public partial class Insert2 : Insert2Base
    {
        private Settings settings = Settings.GetInstance;
        private string projectionName { get; set; }
        private int entityType { get; set; }
        private vwProjection vwProjection { get; set; }
        private List<vwProjectionColumn> columns { get; set; }
        private UserInfo userInfo { get; set; }
                private List<vwProjectionRelation> subItems { get; set; }

        public Insert2(UserInfo userInfo, string projectionName)
        {

            this.userInfo = userInfo;
            this.projectionName = projectionName;
            this.vwProjection = settings.GetVwProjectionList(userInfo.ProfileId).First(x => x.Projection_Name == projectionName);
            this.entityType = settings.GetCrmEntityType(this.vwProjection.DbObject_Name);
            this.columns = settings.GetVwProjectionColumnList(userInfo.ProfileId, projectionName).Where(x =>
                x.AppColumnType_Id != 1
                && !x.Column_IsReadOnly
                && !x.Column_IsPrimaryKey
                && (x.Column_IsVisibleOnForm || x.Column_HiddenData)
                && x.AccessRightsType_Id == 2
            ).ToList();
            this.subItems = settings.VwProjectionRelationList.Where(x =>
                x.ProjectionRelation_ParentProjection_FK == this.vwProjection.Projection_Id
                && x.ProjectionRelation_ChildProjectionName != "Participant"
                && x.ProjectionRelation_ChildProjectionProfile_Id == userInfo.ProfileId
                && ((x.ProjectionRelation_ChildProjectionAccessRight & (long)2) == (long)2)
                && x.ProjectionRelation_Type_FK == 1
                ).ToList();
        }

        private void WriteSubItem(vwProjectionRelation gridItem)
        {
            Insert2_ItemGridInsert itemGridInsertTemplate = new Insert2_ItemGridInsert(userInfo, gridItem, this.vwProjection, 0, $"Id");
            string childInsertVal = itemGridInsertTemplate.TransformText();
            Write(childInsertVal);
        }

        private void WriteInsertColumn(vwProjectionColumn column)
        {
            string val = $",[{column.DbColumn_Name}]";
            if (!string.IsNullOrEmpty(val))
            {
                WriteLine(val);
            }
        }

        private void WriteSelectColumn(vwProjectionColumn column)
        {
            if (vwProjection.NumberingSequence_NumericDbColumnId.HasValue && column.DbColumn_Id == vwProjection.NumberingSequence_NumericDbColumnId.Value)
            {
                WriteLine(",@newSequenceNumber");
            }
            else
            {

                switch (column.DbColumnType_Id)
                {
                    case (int)DbColumnTypeEnum.Bit:
                    case (int)DbColumnTypeEnum.BitIcon:
                        {
                            if (!column.Column_IsNullable)
                            {
                                WriteLine($",isnull(json.[{column.ProjectionColumn_Name}], 0)");
                            }
                            else
                            {
                                WriteLine($",json.[{column.ProjectionColumn_Name}]");
                            }
                        }
                        break;
                    case (int)DbColumnTypeEnum.MultiEnumeration:
                        {
                            WriteLine($",JSON_QUERY(@json, '$.{column.ProjectionColumn_Name}')");
                        }
                        break;
                    default:
                        if (!string.IsNullOrEmpty(column.Column_InsertDefaultValue))
                        {
                            WriteLine($",{column.Column_InsertDefaultValue}");
                        }
                        else
                        {
                            WriteLine($",json.[{column.ProjectionColumn_Name}]");
                        }
                        break;
                }
            }
        }

        private void WriteJsonColumn(vwProjectionColumn column)
        {
            string val = $",[{column.ProjectionColumn_Name}] {column.Column_DbColumnTypeNative}";
            if (column.DbColumnType_Id == (int)DbColumnTypeEnum.MultiEnumeration)
            {
                val = string.Empty;
            }
            if (!string.IsNullOrEmpty(val))
            {
                WriteLine(val);
            }
        }
    }
}

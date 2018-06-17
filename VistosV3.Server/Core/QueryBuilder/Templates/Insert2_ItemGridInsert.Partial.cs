using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.Services;
using Core.VistosDb.Objects;

namespace Core.QueryBuilder.Templates
{
    public partial class Insert2_ItemGridInsert : Insert2_ItemGridInsertBase
    {
        private Settings settings = Settings.GetInstance;
        private vwProjection entity { get; set; }
        private vwProjection parentEntity { get; set; }
        private List<vwProjectionColumn> columns { get; set; }
        private vwProjectionRelation relation { get; set; }
        private UserInfo userInfo { get; set; }
        private int parentEntityType { get; set; }
        private string childProjectionName { get; set; }
        private List<vwProjectionRelation> subItems { get; set; }
        private string parentIdVarName { get; set; }

        public Insert2_ItemGridInsert(UserInfo userInfo, vwProjectionRelation relation, vwProjection parentEntity, int parentEntityType, string parentIdVarName)
        {
            this.parentIdVarName = parentIdVarName;
            this.userInfo = userInfo;
            this.relation = relation;
            this.childProjectionName = relation.ProjectionRelation_ChildProjectionName;
            this.columns = settings.GetVwProjectionColumnList(userInfo.ProfileId, relation.ProjectionRelation_ChildProjectionName).Where(x =>
                x.AppColumnType_Id != 1
                && !x.Column_IsReadOnly
                && !x.Column_IsPrimaryKey
                && x.ProjectionColumn_Id != relation.ProjectionColumn1_Id
                && (x.Column_IsVisibleOnItemGrid || x.Column_HiddenData)
                && x.AccessRightsType_Id == 2
            ).ToList();

            this.entity = settings.GetVwProjectionList(userInfo.ProfileId).First(x => x.Projection_Id == relation.ProjectionRelation_ChildProjection_FK);
            this.parentEntity = parentEntity;
            this.parentEntityType = parentEntityType;
            this.subItems = settings.VwProjectionRelationList.Where(x =>
                x.ProjectionRelation_ParentProjection_FK == this.entity.Projection_Id
                && x.ProjectionRelation_ChildProjectionName != "Participant"
                && x.ProjectionRelation_ChildProjectionProfile_Id == userInfo.ProfileId
                && ((x.ProjectionRelation_ChildProjectionAccessRight & (long)2) == (long)2)
                && x.ProjectionRelation_Type_FK == 1
                ).ToList();
        }

        private void WriteSubItem(vwProjectionRelation gridItem)
        {
            Insert2_ItemGridInsert itemGridInsertTemplate = new Insert2_ItemGridInsert(userInfo, gridItem, this.entity, 0, $"newId{childProjectionName}");
            string childInsertVal = itemGridInsertTemplate.TransformText();
            Write(childInsertVal);
        }

        private void WriteJsonColumn(vwProjectionColumn column)
        {
            Write($",[{column.ProjectionColumn_Name}] {column.Column_DbColumnTypeNative}");
            WriteLine("");
        }

        private void WriteSelectColumn(vwProjectionColumn column)
        {
            string val = $",json{childProjectionName}.[" + column.ProjectionColumn_Name + "]";
            if (!string.IsNullOrEmpty(column.Column_InsertDefaultValue))
            {
                val = $",{column.Column_InsertDefaultValue}";
            }
            Write(val);
            WriteLine("");
        }

    }
}

using Core.Models;
using Core.Services;
using Core.VistosDb.Objects;
using System.Collections.Generic;
using System.Linq;

namespace Core.QueryBuilder.Templates
{
    public partial class ItemGridInsert : ItemGridInsertBase
    {
        private Settings settings = Settings.GetInstance;
        private vwProjection entity { get; set; }
        private vwProjection parentEntity { get; set; }
        private List<vwProjectionColumn> columns { get; set; }
        private vwProjectionRelation relation { get; set; }
        private UserInfo userInfo { get; set; }
        private int parentEntityType { get; set; }

        public ItemGridInsert(UserInfo userInfo, vwProjectionRelation relation, vwProjection parentEntity, int parentEntityType)
        {
            this.userInfo = userInfo;
            this.relation = relation;
            this.columns = settings.GetVwProjectionColumnList(userInfo.ProfileId, relation.ProjectionRelation_ChildProjectionName).Where(x =>
                x.AppColumnType_Id != 1
                && !x.Column_IsReadOnly
                && !x.Column_IsPrimaryKey
                && x.ProjectionColumn_Id != relation.ProjectionColumn1_Id
                && (x.Column_IsVisibleOnItemGrid || x.Column_HiddenData || x.Column_InsertDefaultValue != null)
                && x.AccessRightsType_Id == 2
            ).ToList();
            this.entity = settings.GetVwProjectionList(userInfo.ProfileId).First(x => x.Projection_Id == relation.ProjectionRelation_ChildProjection_FK);
            this.parentEntity = parentEntity;
            this.parentEntityType = parentEntityType;
        }

        private void WriteJsonColumn(vwProjectionColumn column)
        {
            Write($",[{column.ProjectionColumn_Name}] {column.Column_DbColumnTypeNative}");
            WriteLine("");
        }

        private void WriteSelectColumn(vwProjectionColumn column)
        {
            string val = $",json.[" + column.ProjectionColumn_Name + "]";
            if (!string.IsNullOrEmpty(column.Column_InsertDefaultValue))
            {
                val = $",{column.Column_InsertDefaultValue}";
            }
            Write(val);
            WriteLine("");
        }
    }
}

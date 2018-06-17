using Core.Models;
using Core.Services;
using Core.VistosDb.Objects;
using System.Collections.Generic;
using System.Linq;

namespace Core.QueryBuilder.Templates
{
    public partial class ItemGridUpdate : ItemGridUpdateBase
    {
        private Settings settings = Settings.GetInstance;
        private vwProjection vwProjection { get; set; }
        private vwProjection parentVwProjection { get; set; }
        private List<vwProjectionColumn> columns { get; set; }
        private vwProjectionRelation relation { get; set; }
        private UserInfo userInfo { get; set; }
        private int parentEntityType { get; set; }

        public ItemGridUpdate(UserInfo userInfo, vwProjectionRelation relation, vwProjection parentVwProjection, int parentEntityType)
        {
            
            this.userInfo = userInfo;
            this.relation = relation;
            this.columns = settings.GetVwProjectionColumnList(userInfo.ProfileId, relation.ProjectionRelation_ChildProjectionName).Where(x =>
                x.AppColumnType_Id != 1
                && !x.Column_IsReadOnly
                && !x.Column_IsPrimaryKey
                && x.ProjectionColumn_Id != relation.ProjectionColumn1_Id
                && (x.Column_IsVisibleOnItemGrid || x.Column_HiddenData)
                && x.AccessRightsType_Id == 2
            ).ToList();
            this.vwProjection = settings.GetVwProjectionList(userInfo.ProfileId).First(x => x.Projection_Id == relation.ProjectionRelation_ChildProjection_FK);
            this.parentVwProjection = parentVwProjection;
            this.parentEntityType = parentEntityType;
        }

        private void WriteUpdateColumn(vwProjectionColumn column)
        {
            string val = $",[{column.DbColumn_Name}] = json.[{column.ProjectionColumn_Name}]";
            if (!string.IsNullOrEmpty(column.Column_UpdateDefaultValue))
            {
                val = $",[{column.DbColumn_Name}] = {column.Column_UpdateDefaultValue}";
            }
            Write(val);
            WriteLine("");
        }

        private void WriteJsonColumn(vwProjectionColumn column)
        {
            Write($",[{column.ProjectionColumn_Name}] {column.Column_DbColumnTypeNative}");
            WriteLine("");
        }

    }
}

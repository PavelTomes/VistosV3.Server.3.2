using Core.Models;
using Core.VistosDb.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.QueryBuilder.Templates
{
    public partial class GetByIdItemGrid
    {
        private vwProjection parentEntity { get; set; }
        private vwProjectionRelation relation { get; set; }

        public GetByIdItemGrid(UserInfo userInfo, vwProjectionRelation relation, vwProjection parentEntity)
        {
            this.userInfo = userInfo;
            this.relation = relation;
            this.columns = settings.GetVwProjectionColumnList(userInfo.ProfileId, relation.ProjectionRelation_ChildProjectionName).Where(x => 
                (x.Column_IsVisibleOnItemGrid || (x.Column_IsPrimaryKey) || x.Column_HiddenData)
                && (x.AccessRightsType_Id == 1 || x.AccessRightsType_Id == 2)
                && !SystemEnums.SysColumns.Contains(x.DbColumn_Name)
            ).ToList();
            this.vwProjection = settings.GetVwProjectionList(userInfo.ProfileId).First(x => x.Projection_Id == relation.ProjectionRelation_ChildProjection_FK);
            this.parentEntity = parentEntity;
        }

        private void WriteCaptionStringForColumn(vwProjectionColumn column)
        {
            if (column != null)
            {
                List<string> renderCaptionColumnNameList = new List<string>();
                Write(GetReferenceCaption(column.DbColumnType_Id, column.ProjectionReference_Id, column.ProjectionColumn_Name, null, vwProjection.Projection_Name, column.DbColumn_Name, column.Column_ComputedExpression, out renderCaptionColumnNameList));
            }
        }

        private void WriteColumnBracketedString(vwProjectionColumn column)
        {
            string val = $",[{this.relation.ProjectionRelation_ChildProjectionName}].[{column.DbColumn_Name}] as [{column.ProjectionColumn_Name}]";

            if (column.Column_IsPrimaryKey && column.DbColumn_Name.ToUpper() != "ID")
            {
                val = val + Environment.NewLine + $",[{column.DbColumn_Name}] as [Id]";
            }
            if (column.DbColumnType_Id == (int)DbColumnTypeEnum.MultiEnumeration)
            {
                val = $",JSON_QUERY([{this.relation.ProjectionRelation_ChildProjectionName}].[{column.DbColumn_Name}]) as [{column.ProjectionColumn_Name}]";
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
            if (!String.IsNullOrEmpty(vwProjection.Projection_ProjectionFilter))
            {
                string filter = vwProjection.Projection_ProjectionFilter.Replace("##EntityName##", relation.ProjectionRelation_ChildProjectionName);
                Write($" {filter} {Environment.NewLine}");
            }
        }

    }
}

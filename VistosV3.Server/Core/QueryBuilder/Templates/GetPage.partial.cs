using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.VistosDb.Objects;

namespace Core.QueryBuilder.Templates
{
    public partial class GetPage
    {
        private int draw;
        private int length;
        private int start;

        public GetPage(
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
            string[] columnsVisible,
            string projectionRelationName,
            string gridMode
            )
        {
            this.userInfo = userInfo;
            this.vwProjection = settings.GetVwProjectionList(userInfo.ProfileId).First(x => x.Projection_Name == projectionName);
            this.entityType = settings.GetCrmEntityType(this.vwProjection.DbObject_Name);
            this.columns = settings.GetVwProjectionColumnList(userInfo.ProfileId, projectionName).Where(x =>
                (x.Column_VisibleOnGrid == 1 || x.Column_VisibleOnGrid == 2 || x.ProjectionColumn_Name == sortOrderColumnName)
                && !x.Column_IsPrimaryKey 
                && (x.AccessRightsType_Id == 1 || x.AccessRightsType_Id == 2)
                ).ToList();
            this.columnsForFilter = settings.GetVwProjectionColumnList(userInfo.ProfileId, projectionName).Where(x =>
                (
                    ((x.Column_IsVisibleOnFilter) && (x.AccessRightsType_Id == 1 || x.AccessRightsType_Id == 2))
                    ||
                    (projectionName == "Email" && x.DbColumn_Name == "EmailAccount_Folder_FK")
                )
                ).Distinct().ToList();

            this.draw = draw;
            this.start = start;
            this.length = length;
            this.sortOrderColumnName = sortOrderColumnName;
            this.sortOrderDirection = sortOrderDirection;
            this.FilterString = filter;
            this.parentProjectionName = parentProjectionName;
            this.parentDbObject_Id = settings.GetVwProjectionList(userInfo.ProfileId).Where(x => x.Projection_Name == parentProjectionName).Select(x => x.DbObject_Id).FirstOrDefault();
            this.parentEntityId = parentEntityId;

            this.columnsVisible = columnsVisible;
            if (!string.IsNullOrEmpty(projectionRelationName))
            {
                vwProjectionRelation vwProjectionRelation = settings.VwProjectionRelationList.Where(r => r.ProjectionRelation_Name == projectionRelationName).FirstOrDefault();
                if (vwProjectionRelation != null && vwProjectionRelation.ProjectionRelation_Id > 0)
                {
                    this.vwProjectionRelation = vwProjectionRelation;
                }
            }
            this.gridMode = gridMode;
            FillForeignTablesAndSortOrderString();
        }
    }
}

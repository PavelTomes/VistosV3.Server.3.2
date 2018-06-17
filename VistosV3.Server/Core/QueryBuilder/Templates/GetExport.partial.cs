using System.Linq;
using Core.Models;
using Core.VistosDb.Objects;

namespace Core.QueryBuilder.Templates
{
    public partial class GetExport
    {
        public GetExport(
            UserInfo userInfo,
            string projectionName,

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
                (x.Column_VisibleOnGrid == 1 || x.Column_VisibleOnGrid == 2 || x.Column_IsPrimaryKey || x.ProjectionColumn_Name == sortOrderColumnName)
                && !x.Column_IsPrimaryKey
                && (x.AccessRightsType_Id == 1 || x.AccessRightsType_Id == 2)
                ).ToList();
            this.columnsForFilter = settings.GetVwProjectionColumnList(userInfo.ProfileId, projectionName).Where(x =>
                (x.Column_IsVisibleOnFilter)
                && (x.AccessRightsType_Id == 1 || x.AccessRightsType_Id == 2)
                ).ToList();

            this.sortOrderColumnName = sortOrderColumnName;
            this.sortOrderDirection = sortOrderDirection;
            this.FilterString = filter;
            this.columnsVisible = columnsVisible;
            this.parentProjectionName = parentProjectionName;
            this.parentDbObject_Id = settings.GetVwProjectionList(userInfo.ProfileId).Where(x => x.Projection_Name == parentProjectionName).Select(x => x.DbObject_Id).FirstOrDefault();
            this.parentEntityId = parentEntityId;
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

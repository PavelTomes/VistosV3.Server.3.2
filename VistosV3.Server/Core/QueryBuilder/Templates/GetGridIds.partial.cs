using Core.Models;
using System.Linq;
using Core.VistosDb.Objects;

namespace Core.QueryBuilder.Templates
{
    public partial class GetGridIds
    {
        public GetGridIds(
          UserInfo userInfo,
          string projectionName,
          string filter,
          string parentProjectionName,
          int? parentEntityId,
          string projectionRelationName,
          bool ignoreSwitchIsVisibleOnFilter
          )
        {
            this.userInfo = userInfo;
            this.vwProjection = settings.GetVwProjectionList(userInfo.ProfileId).First(x => x.Projection_Name == projectionName);
            this.entityType = settings.GetCrmEntityType(this.vwProjection.DbObject_Name);

            this.FilterString = filter;

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
            this.columns = settings.GetVwProjectionColumnList(userInfo.ProfileId, projectionName).Where(x =>
                (x.Column_VisibleOnGrid == 1 || x.Column_VisibleOnGrid == 2 || x.Column_IsPrimaryKey || x.ProjectionColumn_Name == sortOrderColumnName)
                && (x.AccessRightsType_Id == 1 || x.AccessRightsType_Id == 2)
                ).ToList();
            if (ignoreSwitchIsVisibleOnFilter)
            {
                this.columnsForFilter = settings.GetVwProjectionColumnList(userInfo.ProfileId, projectionName).Where(x =>
                    (x.AccessRightsType_Id == 1 || x.AccessRightsType_Id == 2)
                    ).ToList();
            }
            else
            {
                this.columnsForFilter = settings.GetVwProjectionColumnList(userInfo.ProfileId, projectionName).Where(x =>
                    (x.Column_IsVisibleOnFilter)
                    && (x.AccessRightsType_Id == 1 || x.AccessRightsType_Id == 2)
                    ).ToList();
            }
            FillForeignTablesAndSortOrderString();
        }
    }
}

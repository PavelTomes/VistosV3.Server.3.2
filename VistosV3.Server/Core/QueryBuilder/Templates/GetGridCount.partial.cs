using System.Linq;
using Core.Models;
using Core.VistosDb.Objects;

namespace Core.QueryBuilder.Templates
{
    public partial class GetGridCount
    {
        public GetGridCount(
            UserInfo userInfo,
            string projectionName,
            string parentProjectionName,
            int parentEntityId,
            string projectionRelationName
            )
        {
            this.userInfo = userInfo;
            this.vwProjection = settings.GetVwProjectionList(userInfo.ProfileId).First(x => x.Projection_Name == projectionName);
            this.entityType = settings.GetCrmEntityType(this.vwProjection.DbObject_Name);
            this.columnsForFilter = settings.GetVwProjectionColumnList(userInfo.ProfileId, projectionName).Where(x =>
                (x.Column_IsVisibleOnFilter)
                && (x.AccessRightsType_Id == 1 || x.AccessRightsType_Id == 2)
                ).ToList();
            this.FilterString = "{}";
            this.parentProjectionName = parentProjectionName;
            this.parentDbObject_Id = settings.GetVwProjectionList(userInfo.ProfileId).Where(x => x.Projection_Name == parentProjectionName).Select(x => x.DbObject_Id).FirstOrDefault();
            this.parentEntityId = parentEntityId;
            this.gridMode = "Subgrid";

            if (!string.IsNullOrEmpty(projectionRelationName))
            {
                vwProjectionRelation vwProjectionRelation = settings.VwProjectionRelationList.Where(r => r.ProjectionRelation_Name == projectionRelationName).FirstOrDefault();
                if (vwProjectionRelation != null && vwProjectionRelation.ProjectionRelation_Id > 0)
                {
                    this.vwProjectionRelation = vwProjectionRelation;
                }
            }
        }
    }
}

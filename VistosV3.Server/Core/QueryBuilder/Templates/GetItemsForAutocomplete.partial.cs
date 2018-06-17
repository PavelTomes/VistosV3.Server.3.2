using System.Linq;
using Core.Models;

namespace Core.QueryBuilder.Templates
{
    public partial class GetItemsForAutocomplete
    {
        public GetItemsForAutocomplete(UserInfo userInfo, string projectionName, string filter)
        {
            this.FilterString = filter;
            this.userInfo = userInfo;
            this.vwProjection = settings.GetVwProjectionList(userInfo.ProfileId).First(x => x.Projection_Name == projectionName);
            this.entityType = settings.GetCrmEntityType(this.vwProjection.DbObject_Name);
            this.columnsForFilter = settings.GetVwProjectionColumnList(userInfo.ProfileId, projectionName).Where(x =>
                (x.Column_IsVisibleOnFilter)
                && (x.AccessRightsType_Id == 1 || x.AccessRightsType_Id == 2)
                ).ToList();
        }
    }
}

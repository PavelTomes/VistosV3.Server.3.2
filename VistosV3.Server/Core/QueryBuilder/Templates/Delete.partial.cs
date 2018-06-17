using Core.Models;
using System.Linq;

namespace Core.QueryBuilder.Templates
{
    public partial class Delete
    {
        public Delete(UserInfo userInfo, string projectionName)
        {
            this.userInfo = userInfo;
            this.vwProjection = settings.GetVwProjectionList(userInfo.ProfileId).First(x => x.Projection_Name == projectionName);
            this.entityType = settings.GetCrmEntityType(this.vwProjection.DbObject_Name);
        }
    }
}

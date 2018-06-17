using Core.Models;
using System.Linq;

namespace Core.QueryBuilder.Templates
{
    public partial class InsertEntityEntity 
    {
        public InsertEntityEntity(string projectionName, string parentProjectionName, UserInfo userInfo)
        {
            this.parentDbObject_Id = settings.GetVwProjectionList(userInfo.ProfileId).Where(x => x.Projection_Name == parentProjectionName).Select(x => x.DbObject_Id).FirstOrDefault();
            this.dbObject_Id = settings.GetVwProjectionList(userInfo.ProfileId).Where(x => x.Projection_Name == projectionName).Select(x => x.DbObject_Id).FirstOrDefault(); ;

            this.userInfo = userInfo;
        }

    }
}

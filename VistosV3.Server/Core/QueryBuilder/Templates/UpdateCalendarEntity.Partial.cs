using Core.Models;
using System;
using System.Linq;

namespace Core.QueryBuilder.Templates
{
    public partial class UpdateCalendarEntity
    {
        public UpdateCalendarEntity(string entityName, UserInfo userInfo)
        {
            this.userInfo = userInfo;
           
            this.vwProjection = settings.GetVwProjectionList(userInfo.ProfileId).FirstOrDefault(i => i.IsCalendarObject.Value && i.Projection_Name == entityName);

            if (this.vwProjection == null)
                throw new Exception($"Projection {entityName} cannot be found or is unaccessible for the user.");
        }
    }
}

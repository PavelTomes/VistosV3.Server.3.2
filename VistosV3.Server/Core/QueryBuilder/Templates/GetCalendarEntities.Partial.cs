using Core.Models;
using Core.VistosDb.Objects;
using System.Collections.Generic;
using System.Linq;

namespace Core.QueryBuilder.Templates
{
    public partial class GetCalendarEntities
    {
        private IEnumerable<vwProjection> CalendarProjections;
        private List<int> UserIDs { get; set; }
        private Dictionary<string, List<int>> EntitiesAndRoles { get; set; }

        public GetCalendarEntities(Dictionary<string, List<int>> entitiesAndRoles, List<int> userIDs, UserInfo userInfo)
        {
            this.userInfo = userInfo;
            EntitiesAndRoles = entitiesAndRoles;
            UserIDs = userIDs == null || userIDs.Count() == 0 ? new List<int> { userInfo.UserId } : userIDs;

            CalendarProjections = settings.GetVwProjectionList(userInfo.ProfileId).Where(i => i.IsCalendarObject.Value && entitiesAndRoles.ContainsKey(i.Projection_Name));
        }

        private string GetListValues(List<int> list)
        {
            return string.Join(",", list);
        }
    }
}

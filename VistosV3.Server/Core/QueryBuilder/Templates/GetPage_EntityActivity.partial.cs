using System.Linq;
using Core.Models;

namespace Core.QueryBuilder.Templates
{
    public partial class GetPage_EntityActivity
    {
        private int draw;
        private int length;
        private int start;

        public GetPage_EntityActivity(
            UserInfo userInfo,
            int draw,
            int start,
            int length,
            string sortOrderColumnName,
            string sortOrderDirection,
            string filter,
            string parentProjectionName,
            int? parentEntityId,
            string[] columnsVisible
            )
        {
            this.userInfo = userInfo;
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

            string sortOrderColumnNameCalc = sortOrderColumnName;
            if (string.IsNullOrEmpty(sortOrderColumnName) || sortOrderColumnName == "Role__AssignedTo")
            {
                sortOrderColumnNameCalc = "Created";
            }
            if (sortOrderColumnName == "Icon" || sortOrderColumnName == "ProjectionLoc")
            {
                sortOrderColumnNameCalc = "MainProjection";
            }
            this.sortOrder_Entity = $"[Entity1].[{sortOrderColumnNameCalc}] {(string.IsNullOrEmpty(sortOrderDirection) ? "ASC" : sortOrderDirection)}";
            this.sortOrder_Project = $"[Project1].[{sortOrderColumnNameCalc}] {(string.IsNullOrEmpty(sortOrderDirection) ? "ASC" : sortOrderDirection)}";
        }
    }
}

using Core.Models;
using System.Collections.Generic;
using System.Linq;
using Core.VistosDb.Objects;

namespace Core.QueryBuilder.Templates
{
    public partial class New
    {
        public New(UserInfo userInfo, string projectionName)
        {

            this.userInfo = userInfo;
            this.projectionName = projectionName;
            this.vwProjection = settings.GetVwProjectionList(userInfo.ProfileId).First(x => x.Projection_Name == projectionName);
        }

        private void WriteSelectColumn()
        {
            List<vwProjectionActionColumnMapping> columns = settings.VwProjectionActionColumnMappingList.Where(x => x.ProjectionTo_Name.Equals(this.projectionName) && x.ActionTypeName == "New").Distinct().ToList();
            foreach (vwProjectionActionColumnMapping column in columns)
            {
                WriteLine($",{column.DefaultValue} as [{column.ProjectionColumnTo_Name}]");
                List<string> renderCaptionColumnNameList = new List<string>();
                Write(GetReferenceCaption(column.ProjectionColumnTo_DbColumnType_Id, column.ProjectionColumnTo_ProjectionReference_Id, column.ProjectionColumnTo_Name, column.DefaultValue, null, null, null, out renderCaptionColumnNameList));
            }
        }
    }
}


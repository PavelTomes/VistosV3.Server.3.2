using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.VistosDb.Objects;

namespace Core.QueryBuilder.Templates
{
    public partial class MassUpdate 
    {
        private int[] ids;
        private List<String> updateColumn;

        public MassUpdate(UserInfo userInfo, string projectionName, int[] ids, JObject data)
        {
            this.userInfo = userInfo;
            this.ids = ids;
            this.vwProjection = settings.GetVwProjectionList(userInfo.ProfileId).First(x => x.Projection_Name == projectionName);
            this.entityType = settings.GetCrmEntityType(this.vwProjection.DbObject_Name);

            updateColumn = new List<string>();
            foreach (KeyValuePair<string, JToken> node in data)
            {
                updateColumn.Add(node.Key);
            }

            this.columns = settings.GetVwProjectionColumnList(userInfo.ProfileId, projectionName).Where(x =>
                updateColumn.Contains(x.DbColumn_Name)
                && x.AppColumnType_Id != 1
                && !x.Column_IsReadOnly
                && !x.Column_IsPrimaryKey
                && (x.Column_IsVisibleOnForm || x.Column_HiddenData)
                && x.AccessRightsType_Id == 2
            ).ToList();

        }

        private void WriteUpdateColumn(vwProjectionColumn column)
        {
            Write($",[{column.DbColumn_Name}] = json.[{column.DbColumn_Name}]");
            WriteLine("");
        }

        private void WriteJsonColumn(vwProjectionColumn column)
        {
            Write($"[{column.DbColumn_Name}] {column.Column_DbColumnTypeNative}");
            WriteLine("");
        }

        private void WriteUpdateIds()
        {
            if (vwProjection.DbObjectType_Id == 1)
            {
                Write($" WHERE {vwProjection.DbPrimaryColumn_Name} in ({String.Join(",", this.ids)}) {Environment.NewLine}");
                //Write($"and [{vwProjection.DbPrimaryColumn_Name}] not in (SELECT Entity_FK FROM [crm].[fn_GetBannedEntityIds] (@userId, {entityType}, 1)){Environment.NewLine}");
            }
            if (!String.IsNullOrEmpty(vwProjection.Projection_ProjectionFilter))
            {
                string filter = vwProjection.Projection_ProjectionFilter.Replace("[##EntityName##].", $"[{vwProjection.DbObject_Schema}].[{vwProjection.DbObject_Name}].");
                Write($" {filter} {Environment.NewLine}");
            }
            WriteLine("");
        }
    }
}

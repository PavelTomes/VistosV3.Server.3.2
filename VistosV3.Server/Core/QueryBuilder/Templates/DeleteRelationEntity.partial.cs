using Core.Models;
using System;
using System.Linq;

namespace Core.QueryBuilder.Templates
{
    public partial class DeleteRelationEntity
    {
        public DeleteRelationEntity(UserInfo userInfo, string projectionName, string parentProjectionName, int? parentEntityId, string gridMode)
        {
            this.userInfo = userInfo;
            this.parentProjectionName = settings.GetVwProjectionList(userInfo.ProfileId).Where(x => x.Projection_Name == parentProjectionName).Select(x => x.DbObject_Name).FirstOrDefault();
            this.parentDbObject_Id = settings.GetVwProjectionList(userInfo.ProfileId).Where(x => x.Projection_Name == parentProjectionName).Select(x => x.DbObject_Id).FirstOrDefault();
            this.parentEntityId = parentEntityId;
            this.gridMode = gridMode;
            this.projectionName = settings.GetVwProjectionList(userInfo.ProfileId).Where(x => x.Projection_Name == projectionName).Select(x => x.DbObject_Name).FirstOrDefault(); ;
            this.dbObject_Id = settings.GetVwProjectionList(userInfo.ProfileId).Where(x => x.Projection_Name == projectionName).Select(x => x.DbObject_Id).FirstOrDefault(); ;
        }

        public void WriteEntityEntityFilter()
        {
            Write($" WHERE ([DbObjectDbObject].Deleted = 0 and [DbObjectDbObject].LeftDbObject_FK = '{this.parentDbObject_Id}' and [DbObjectDbObject].LeftRecordId = {parentEntityId}  and [DbObjectDbObject].RightDbObject_FK = '{this.dbObject_Id}' and [DbObjectDbObject].[RightRecordId] = @id ){Environment.NewLine}");
            Write($"     OR([DbObjectDbObject].Deleted = 0 and [DbObjectDbObject].RightDbObject_FK = '{this.parentDbObject_Id}' and [DbObjectDbObject].RightRecordId = {parentEntityId} and [DbObjectDbObject].LeftDbObject_FK = '{this.dbObject_Id}' and [DbObjectDbObject].[LeftRecordId] = @id ) {Environment.NewLine}");
        }

    }
}

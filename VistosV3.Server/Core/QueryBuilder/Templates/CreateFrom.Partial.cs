using Core.Models;
using Core.VistosDb.Objects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.QueryBuilder.Templates
{
    public partial class CreateFrom
    {
        private string projectionNameFrom { get; set; }
        private string projectionNameTarget { get; set; }
        private ProjectionActionType actionTypeName { get; set; }
        private vwProjection projectionFrom { get; set; }

        public CreateFrom(UserInfo userInfo, string projectionNameFrom, string projectionNameTarget, ProjectionActionType actionTypeName)
        {
            this.userInfo = userInfo;
            this.projectionNameFrom = projectionNameFrom;
            this.projectionNameTarget = projectionNameTarget;
            this.actionTypeName = actionTypeName;
            this.projectionFrom = settings.GetVwProjectionList(userInfo.ProfileId).First(x => x.Projection_Name == projectionNameFrom);
        }

        private void WriteGrids()
        {
            List<vwProjectionAction> childProjectionActions = settings.VwProjectionActionList.Where(x =>

            x.ParentProjectionFrom_Name == projectionNameFrom
            && x.ParentProjectionFrom_ProfileID == userInfo.ProfileId
            && ((x.ParentProjectionFrom_AccessRight & (long)1) == (long)1)

            && x.ParentProjectionTo_Name == projectionNameTarget
            && x.ParentProjectionTo_ProfileID == userInfo.ProfileId
            && ((x.ParentProjectionTo_AccessRight & (long)2) == (long)2)

            && x.ProjectionFrom_ProfileID == userInfo.ProfileId
            && ((x.ProjectionFrom_AccessRight & (long)1) == (long)1)

            && x.ProjectionTo_ProfileID == userInfo.ProfileId
            && ((x.ProjectionTo_AccessRight & (long)2) == (long)2)

            ).Distinct().ToList();

            List<string> projectionToNames = childProjectionActions.Select(a => a.ProjectionTo_Name).Distinct().ToList();
            foreach(string projectionToName in projectionToNames)
            {
                CreateFrom_ItemGrid itemGridInsertTemplate = new CreateFrom_ItemGrid(
                    this.userInfo
                    , projectionToName
                    , childProjectionActions.Where(a => a.ProjectionTo_Name == projectionToName).ToList()
                    , this.projectionFrom);
                string childInsertVal = itemGridInsertTemplate.TransformText();
                Write(childInsertVal);
            }
        }

        private void WriteSelectColumn()
        {
            List<vwProjectionActionColumnMapping> columns = settings.VwProjectionActionColumnMappingList.Where(
                x => x.ProjectionFrom_Name == this.projectionNameFrom
                && x.ProjectionTo_Name == this.projectionNameTarget
                && x.ActionTypeName == this.actionTypeName.ToString()
            ).Distinct().ToList();

            foreach (vwProjectionActionColumnMapping column in columns)
            {
                string val = "";
                if (!string.IsNullOrEmpty(column.ProjectionColumnFrom_Name))
                {
                    val = $",[{projectionFrom.Projection_Name}].[{column.ProjectionColumnFrom_Name}] as [{column.ProjectionColumnTo_Name}]";
                }
                if (!string.IsNullOrEmpty(column.DefaultValue))
                {
                    val = $",{column.DefaultValue.Replace("##EntityName##", projectionFrom.Projection_Name)} as [{column.ProjectionColumnTo_Name}]";
                }
                List<string> renderCaptionColumnNameList = new List<string>();
                val = val + GetReferenceCaption(column.ProjectionColumnTo_DbColumnType_Id, column.ProjectionColumnTo_ProjectionReference_Id, column.ProjectionColumnTo_Name, column.DefaultValue, projectionFrom.Projection_Name, column.ProjectionColumnFrom_Name, null, out renderCaptionColumnNameList);
                WriteLine(val);
            }

            List<vwProjectionColumn> columnsWithSameName_ = (from c1 in settings.GetVwProjectionColumnList(userInfo.ProfileId, projectionNameFrom)
                                                             join c2 in settings.GetVwProjectionColumnList(userInfo.ProfileId, projectionNameTarget) on new { c1.ProjectionColumn_Name, c1.DbColumnType_Id } equals new { c2.ProjectionColumn_Name, c2.DbColumnType_Id }
                                                             where c1.Projection_Name == projectionNameFrom && c1.Profile_Id == userInfo.ProfileId
                                                                && c2.Projection_Name == projectionNameTarget && c2.Profile_Id == userInfo.ProfileId
                                                                && !string.IsNullOrEmpty(c1.Column_DbColumnTypeNative)
                                                             select c1).ToList();
    //        this.columns = settings.GetVwProjectionColumnList(userInfo.ProfileId, projectionName).Where(x =>
    //(x.Column_IsVisibleOnForm || x.Column_HiddenData)
    //&& (x.AccessRightsType_Id == 1 || x.AccessRightsType_Id == 2)
    //&& !SystemEnums.SysColumns.Contains(x.DbColumn_Name)
    //).ToList();

            columnsWithSameName_.RemoveAll(c => c.ProjectionColumn_Name == "Locked");
            columnsWithSameName_.RemoveAll(c => c.ProjectionColumn_Name == "Archived");
            columnsWithSameName_.RemoveAll(c => c.ProjectionColumn_Name == "Ext_ID");
            columnsWithSameName_.RemoveAll(c => c.ProjectionColumn_Name == "Status_FK");
            columnsWithSameName_.RemoveAll(c => c.ProjectionColumn_Name == "Version");

            List<vwProjectionColumn> columnsWithSameName = columnsWithSameName_.Where(c => !columns.Select(e => e.ProjectionColumnTo_Name).Contains(c.ProjectionColumn_Name)).ToList();

            foreach (vwProjectionColumn column in columnsWithSameName)
            {
                string val = $",[{projectionFrom.Projection_Name}].[{column.DbColumn_Name}] as [{column.ProjectionColumn_Name}]";

                if (column.DbColumnType_Id == (int)DbColumnTypeEnum.SystemChart || column.DbColumnType_Id == (int)DbColumnTypeEnum.UserChart)
                {
                    val = $",null as [{column.ProjectionColumn_Name}]";
                }
                if (column.DbColumnType_Id == (int)DbColumnTypeEnum.Signature)
                {
                    val = $",(SELECT TOP 1 [UniqueGuid] FROM [crm].[Signature] sig WHERE sig.[Deleted] = 0 AND sig.Id = [{projectionFrom.Projection_Name}].[{column.DbColumn_Name}]) as [{column.ProjectionColumn_Name}]";
                }
                if (column.DbColumnType_Id == (int)DbColumnTypeEnum.Geography)
                {
                    val = $",case when [{projectionFrom.Projection_Name}].[{column.DbColumn_Name}] is not null then [{projectionFrom.Projection_Name}].[{column.DbColumn_Name}].Lat else null end as [{column.ProjectionColumn_Name}_Lat]";
                    val = val + Environment.NewLine + $",case when [{projectionFrom.Projection_Name}].[{column.DbColumn_Name}] is not null then [{projectionFrom.Projection_Name}].[{column.DbColumn_Name}].Long else null end as [{column.ProjectionColumn_Name}_Long]";
                }
                if (column.DbColumnType_Id == (int)DbColumnTypeEnum.MultiEnumeration)
                {
                    val = $",JSON_QUERY([{projectionFrom.Projection_Name}].[{column.DbColumn_Name}]) as [{column.ProjectionColumn_Name}]";
                }
                if (!string.IsNullOrEmpty(column.Column_ComputedExpression))
                {
                    val = $",{column.Column_ComputedExpression.Replace("##EntityName##", projectionFrom.Projection_Name)} as [{column.ProjectionColumn_Name}]";
                }
                if (column.DbColumnType_Id == (int)DbColumnTypeEnum.Password)
                {
                    val = $",null as [{column.ProjectionColumn_Name}]";
                }



                List<string> renderCaptionColumnNameList = new List<string>();
                val = val + GetReferenceCaption(column.DbColumnType_Id, column.ProjectionReference_Id, column.ProjectionColumn_Name, null, projectionFrom.Projection_Name, column.DbColumn_Name, column.Column_ComputedExpression, out renderCaptionColumnNameList);
                WriteLine(val);
            }
        }
    }
}


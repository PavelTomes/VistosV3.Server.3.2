using Core.Models;
using Core.VistosDb.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Core.QueryBuilder.Templates
{
    public class CreateFrom_ItemGrid : TemplateBase
    {
        private string projectionNameTarget { get; set; }
        private vwProjection parentEntity { get; set; }
        private List<vwProjectionAction> projectionActions { get; set; }

        public CreateFrom_ItemGrid(UserInfo userInfo, string projectionNameTarget, List<vwProjectionAction> projectionActions, vwProjection parentEntity)
        {
            this.userInfo = userInfo;
            this.projectionNameTarget = projectionNameTarget;
            this.parentEntity = parentEntity;
            this.projectionActions = projectionActions;
        }

        public new string TransformText()
        {
            StringBuilder sb = new StringBuilder();
            if (projectionActions != null && projectionActions.Count > 0)
            {
                List<string> globalColumnNameList = new List<string>();

                sb.AppendLine(", ( ");
                sb.AppendLine("SELECT");
                sb.AppendLine(" [RowSelect].[Version] as [Version]");
                sb.AppendLine(" ,[RowSelect].[Deleted] as [Deleted]");
                sb.AppendLine("###GlobalColumns###");
                sb.AppendLine("FROM (");
                bool first = true;
                foreach (vwProjectionAction projectionAction in projectionActions)
                {
                    vwProjection projectionFrom = settings.GetVwProjectionList(userInfo.ProfileId).First(x => x.Projection_Name == projectionAction.ProjectionFrom_Name);

                    vwProjectionRelation relation = settings.VwProjectionRelationList.Where(x =>
                        x.ProjectionRelation_ParentProjection_FK == projectionAction.ParentProjectionFrom_ID
                        && x.ProjectionRelation_ChildProjection_FK == projectionAction.ProjectionFrom_ID
                        && x.ProjectionRelation_ChildProjectionProfile_Id == userInfo.ProfileId
                        && ((x.ProjectionRelation_ChildProjectionAccessRight & (long)1) == (long)1)
                    ).FirstOrDefault();

                    if (!first && projectionActions.Count > 1)
                    {
                        sb.AppendLine("");
                        sb.AppendLine("UNION ALL");
                        sb.AppendLine("");
                    }
                    sb.AppendLine("SELECT");
                    sb.AppendLine(" 0 as [Version]");
                    sb.AppendLine(" ,0 as [Deleted]");
                    List<string> renderCaptionColumnNameList = new List<string>();
                    sb.Append(WriteSelectColumn(projectionAction, out renderCaptionColumnNameList));
                    if (first)
                    {
                        globalColumnNameList = renderCaptionColumnNameList;
                    }
                    else
                    {
                        globalColumnNameList = globalColumnNameList.Intersect(renderCaptionColumnNameList).ToList();
                    }

                    sb.AppendLine($"FROM [{projectionFrom.DbObject_Schema}].[{projectionFrom.DbObject_Name}] as [{projectionAction.ProjectionFrom_Name}]");
                    sb.AppendLine($"where [{projectionAction.ProjectionFrom_Name}].[Deleted] = 0");
                    sb.AppendLine($" and [{relation.ProjectionRelation_ChildProjectionName}].[{relation.DbColumn1_Name}] = [{parentEntity.Projection_Name}].[{parentEntity.DbPrimaryColumn_Name}]");
                    //sb.AppendLine($"ORDER BY [{projectionAction.ProjectionFrom_Name}].[{relation.DbColumn_NameSortBy}]");

                    first = false;
                }
                sb.AppendLine(") [RowSelect]");
                sb.AppendLine("FOR JSON PATH, INCLUDE_NULL_VALUES)");
                sb.AppendLine($"as [{projectionNameTarget}]");

                StringBuilder sbGlobalColumns = new StringBuilder();
                if (globalColumnNameList != null && globalColumnNameList.Count > 0)
                {
                    foreach(string name in globalColumnNameList)
                    {
                        sbGlobalColumns.AppendLine($" ,[RowSelect].[{name}] as [{name}]");
                    }
                }
                sb.Replace("###GlobalColumns###", sbGlobalColumns.ToString());
            }
            return sb.ToString();
        }

        private string WriteSelectColumn(vwProjectionAction projectionAction, out List<string> renderCaptionColumnNameList)
        {
            renderCaptionColumnNameList = new List<string>();
            StringBuilder sb = new StringBuilder();

            List<vwProjectionActionColumnMapping> columns = settings.VwProjectionActionColumnMappingList.Where(
                            x => x.ProjectionAction_Id == projectionAction.Id
                        ).ToList();

            foreach (vwProjectionActionColumnMapping column in columns)
            {
                string val = "";
                if (!string.IsNullOrEmpty(column.ProjectionColumnFrom_Name))
                {
                    val = $",[{projectionAction.ProjectionFrom_Name}].[{column.ProjectionColumnFrom_Name}] as [{column.ProjectionColumnTo_Name}]";
                }
                if (!string.IsNullOrEmpty(column.DefaultValue))
                {
                    val = $",{column.DefaultValue.Replace("##EntityName##", projectionAction.ProjectionFrom_Name)} as [{column.ProjectionColumnTo_Name}]";
                }
                List<string> renderCaptionColumnNameListTemp = new List<string>();
                val = val + GetReferenceCaption(column.ProjectionColumnTo_DbColumnType_Id, column.ProjectionColumnTo_ProjectionReference_Id, column.ProjectionColumnTo_Name, column.DefaultValue, projectionAction.ProjectionFrom_Name, column.ProjectionColumnFrom_Name, null, out renderCaptionColumnNameListTemp);

                renderCaptionColumnNameList.Add(column.ProjectionColumnTo_Name);
                if (renderCaptionColumnNameListTemp != null && renderCaptionColumnNameListTemp.Count > 0)
                {
                    foreach (string name in renderCaptionColumnNameListTemp)
                    {
                        renderCaptionColumnNameList.Add(name);
                    }
                }
                sb.AppendLine(val);
            }

            List<vwProjectionColumn> columnsWithSameName_ = (from c1 in settings.GetVwProjectionColumnList(userInfo.ProfileId, projectionAction.ProjectionFrom_Name)
                                                             join c2 in settings.GetVwProjectionColumnList(userInfo.ProfileId, projectionNameTarget) on new { c1.ProjectionColumn_Name, c1.DbColumnType_Id } equals new { c2.ProjectionColumn_Name, c2.DbColumnType_Id }
                                                             where c1.Projection_Name == projectionAction.ProjectionFrom_Name && c1.Profile_Id == userInfo.ProfileId && c2.Projection_Name == projectionNameTarget && c2.Profile_Id == userInfo.ProfileId
                                                             select c2).ToList();

            columnsWithSameName_.RemoveAll(c => c.ProjectionColumn_Name == "Id");
            columnsWithSameName_.RemoveAll(c => c.ProjectionColumn_Name == "Locked");
            columnsWithSameName_.RemoveAll(c => c.ProjectionColumn_Name == "Archived");
            columnsWithSameName_.RemoveAll(c => c.ProjectionColumn_Name == "Parent_FK");
            columnsWithSameName_.RemoveAll(c => c.ProjectionColumn_Name == "Ext_ID");
            columnsWithSameName_.RemoveAll(c => c.ProjectionColumn_Name == "Version");

            List<vwProjectionColumn> columnsWithSameName = columnsWithSameName_.Where(c => !columns.Select(e => e.ProjectionColumnTo_Name).Contains(c.ProjectionColumn_Name)).ToList();

            foreach (vwProjectionColumn column in columnsWithSameName)
            {
                string val = "";
                val = $",[{projectionAction.ProjectionFrom_Name}].[{column.DbColumn_Name}] as [{column.ProjectionColumn_Name}]";
                List<string> renderCaptionColumnNameListTemp = new List<string>();
                val = val + GetReferenceCaption(column.DbColumnType_Id, column.ProjectionReference_Id, column.ProjectionColumn_Name, null, projectionAction.ProjectionFrom_Name, column.DbColumn_Name, column.Column_ComputedExpression, out renderCaptionColumnNameListTemp);

                renderCaptionColumnNameList.Add(column.ProjectionColumn_Name);
                if (renderCaptionColumnNameListTemp != null && renderCaptionColumnNameListTemp.Count > 0)
                {
                    foreach (string name in renderCaptionColumnNameListTemp)
                    {
                        renderCaptionColumnNameList.Add(name);
                    }
                }

                sb.AppendLine(val);
            }

            return sb.ToString();
        }
    }
}


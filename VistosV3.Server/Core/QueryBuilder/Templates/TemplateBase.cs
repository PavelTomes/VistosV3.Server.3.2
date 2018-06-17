using Core.Models;
using Core.Services;
using Core.VistosDb.Objects;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.CodeDom;

namespace Core.QueryBuilder.Templates
{
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.TextTemplating", "15.0.0.0")]
    public class TemplateBase
    {
        protected Settings settings = Settings.GetInstance;
        protected string projectionName { get; set; }
        protected int dbObject_Id { get; set; }
        protected int entityType;
        protected UserInfo userInfo;
        protected vwProjection vwProjection;
        protected List<vwProjectionColumn> columns;
        protected List<vwProjectionColumn> columnsForFilter;
        protected Dictionary<string, string> foreignTables;
        protected string sortOrder_Entity;
        protected string sortOrder_Project;

        protected string sortOrderColumnName_Default = "CaptionSort";
        protected string sortOrderDirection_Default = "ASC";

        protected string sortOrderColumnName;
        protected string sortOrderDirection;
        protected string filterString;
        protected string[] columnsVisible;
        protected List<string> columnsInFilter;
        protected string gridMode;
        protected string parentProjectionName { get; set; }
        protected int parentDbObject_Id { get; set; }
        protected int? parentEntityId { get; set; }
        protected vwProjectionRelation vwProjectionRelation { get; set; }

        protected string[] pohodaNameEntities = new string[] { "Dirrectory", "Product" };

        protected string FilterString
        {
            get
            {
                return this.filterString;
            }
            set
            {
                this.filterString = value;

                this.columnsInFilter = new List<string>();
                if (!string.IsNullOrEmpty(this.filterString))
                {
                    JToken objects = JToken.Parse(this.filterString);
                    foreach (JToken elem in objects)
                    {
                        if (elem.Type == JTokenType.Property)
                        {
                            JProperty prop = ((Newtonsoft.Json.Linq.JProperty)elem);
                            if (prop != null && !string.IsNullOrEmpty(prop.Name) && prop.Value != null && !string.IsNullOrEmpty(prop.Value.ToString()))
                            {
                                if (prop.Name == "EntityCategories" || prop.Name == "EntityCategories_FK")
                                {
                                }
                                else
                                {
                                    string propName = prop.Name.Replace("_From", "").Replace("_To", "");
                                    columnsInFilter.Add(propName);
                                }
                            }
                        }
                    }
                }
                this.columnsInFilter = this.columnsInFilter.Distinct().ToList();
            }
        }

        /// Method for filling dictonery with References for left join and creating sortOrderDirection by DbColumnTypeEnum
        protected void FillForeignTablesAndSortOrderString()
        {
            sortOrder_Entity = $"[Entity1].[{sortOrderColumnName_Default}] {sortOrderDirection_Default}";
            sortOrder_Project = $"[Project1].[{sortOrderColumnName_Default}] {sortOrderDirection_Default}";

            if (!string.IsNullOrEmpty(sortOrderColumnName) && !string.IsNullOrEmpty(sortOrderDirection))
            {
                sortOrder_Entity = $"[Entity1].[{sortOrderColumnName}] {sortOrderDirection}";
                sortOrder_Project = $"[Project1].[{sortOrderColumnName}] {sortOrderDirection}";
            }

            if (vwProjection.DbObjectType_Id == 5) //list
            {
                sortOrder_Entity = $"[Entity1].[{vwProjection.DbPrimaryColumn_Name}] DESC";
                sortOrder_Project = $"[Project1].[{vwProjection.DbPrimaryColumn_Name}] DESC";
            }

            foreignTables = new Dictionary<string, string>();

            int i = 2;
            foreach (vwProjectionColumn column in columns.Where(c => c.DbColumnType_Id == (int)DbColumnTypeEnum.Entity || c.DbColumnType_Id == (int)DbColumnTypeEnum.Enumeration || c.DbColumnType_Id == (int)DbColumnTypeEnum.EntityEnumeration).ToList())
            {
                foreignTables.Add(column.ProjectionColumn_Name, "Project" + i);
                if (!string.IsNullOrEmpty(sortOrderColumnName) && !string.IsNullOrEmpty(sortOrderDirection) && sortOrderColumnName == column.DbColumn_Name)
                {
                    switch (column.DbColumnType_Id)
                    {
                        case (int)DbColumnTypeEnum.Enumeration:
                            {
                                sortOrder_Entity = $"[Entity1].[{column.DbColumn_Name}] {sortOrderDirection}";
                                sortOrder_Project = $"[Project{i}].[Description_{userInfo.UserLanguage}] {sortOrderDirection}";
                            }
                            break;
                        case (int)DbColumnTypeEnum.Entity:
                            {
                                sortOrder_Entity = $"[Entity1].[{column.DbColumn_Name}] {sortOrderDirection}";
                                sortOrder_Project = $"[Project{i}].[CaptionSort] {sortOrderDirection}";
                            }
                            break;
                    }
                }
                i = i + 1;
            }
        }

        protected void WriteFilter()
        {
            string val = string.Empty;

            JToken objects = JToken.Parse(this.filterString);
            foreach (JToken elem in objects)
            {
                val = string.Empty;
                if (elem.Type == JTokenType.Property)
                {
                    JProperty prop = ((Newtonsoft.Json.Linq.JProperty)elem);
                    if (prop != null && !string.IsNullOrEmpty(prop.Name) && prop.Value != null && !string.IsNullOrEmpty(prop.Value.ToString()))
                    {
                        if (prop.Name == "EmailParentProjectionName"
                            && objects["EmailParentRecordId"] != null && int.Parse(objects["EmailParentRecordId"].ToString()) > 0)
                        {
                            string parentProjectionName = prop.Value.ToString();
                            int parentRecordId = int.Parse(objects["EmailParentRecordId"].ToString());
                            vwProjection vwProjectionParent = settings.GetVwProjectionList(userInfo.ProfileId).First(x => x.Projection_Name == parentProjectionName);
                            string directoryEmailFilter = string.Empty;
                            if (vwProjectionParent.DbObject_Name == "Directory")
                            {
                                directoryEmailFilter = $" or ([Project1].[{this.vwProjection.DbPrimaryColumn_Name}] in (SELECT ed1.[Email_FK] FROM [crm].[Email_Directory] ed1 WHERE ed1.[Deleted] = 0 and ed1.[Directory_FK] = {parentRecordId}))";
                            }
                            val = $"and (([Project1].[{this.vwProjection.DbPrimaryColumn_Name}] in (SELECT doe1.[Email_FK] FROM [crm].[DbObjectEmail] doe1 WHERE doe1.[Deleted] = 0 and doe1.[DbObject_FK] = {vwProjectionParent.DbObject_Id} and doe1.[RecordId] = {parentRecordId})){directoryEmailFilter})";
                        }
                        else if (prop.Name == "EntityCategories" || prop.Name == "EntityCategories_FK")
                        {
                            string categoriesVals = string.Join(", ", prop.Value.ToArray().Select(t => t.ToString()));
                            if (!string.IsNullOrEmpty(categoriesVals))
                            {
                                val = $"and [Project1].[{this.vwProjection.DbPrimaryColumn_Name}] in (SELECT ec.[Entity_FK] FROM[crm].[Entity_Category] ec WHERE ec.[Deleted] = 0 and ec.[Type] = {this.entityType} and ec.[Category_FK] in ({categoriesVals}))";
                            }
                        }
                        else if (prop.Name.StartsWith("Role__"))
                        {
                            int? roleId = null;
                            JToken[] parts = prop.Value.ToArray();
                            if (parts != null && parts.Length > 0)
                            {
                                roleId = parts.First().Value<int?>("RoleId");
                                if (roleId.HasValue && roleId.Value > 0)
                                {
                                    string userIds = string.Join(", ", parts.Select(t => t.Value<string>("UserId")));
                                    val = $@"and [Project1].[{this.vwProjection.DbPrimaryColumn_Name}] in (
					                            SELECT part1.[RecordId]
					                            FROM [crm].[Role] role1
					                            INNER JOIN [crm].[DbObjectRole] orole1 on orole1.[Deleted] = 0 and orole1.[DbObject_FK] = {this.vwProjection.DbObject_Id} and orole1.[Role_FK] = role1.[Id]
					                            INNER JOIN [crm].[Participant] part1 on part1.[Deleted] = 0 and part1.[DbObjectRole_FK] = orole1.[Id] and part1.[User_FK] in ({userIds})
					                            WHERE role1.[Deleted] = 0 and role1.[Id] = {roleId.Value}
				                            )";
                                }
                            }
                        }
                        else
                        {
                            string propName = prop.Name.Replace("_From", "").Replace("_To", "");
                            vwProjectionColumn column = columnsForFilter.Where(c => c.ProjectionColumn_Name == propName).FirstOrDefault();
                            if (column != null)
                            {
                                if (string.IsNullOrEmpty(column.Column_Filter))
                                {
                                    switch (column.DbColumnType_Id)
                                    {
                                        case (int)DbColumnTypeEnum.Bit:
                                        case (int)DbColumnTypeEnum.BitIcon:
                                            {
                                                val = $"and [Project1].[{column.DbColumn_Name}] = {((new string[] { "1", "true", "yes", "ok" }).Contains(prop.Value.ToString().ToLower()) ? "1" : "0")}";
                                            }
                                            break;
                                        case (int)DbColumnTypeEnum.Int:
                                        case (int)DbColumnTypeEnum.BigInt:
                                        case (int)DbColumnTypeEnum.Float:
                                        case (int)DbColumnTypeEnum.Money:
                                            {
                                                if (prop.Name.EndsWith("_From"))
                                                {
                                                    val = $"and [Project1].[{column.DbColumn_Name}] >= {prop.Value.ToString()}";
                                                }
                                                if (prop.Name.EndsWith("_To"))
                                                {
                                                    val = $"and [Project1].[{column.DbColumn_Name}] < {prop.Value.ToString()}";
                                                }
                                                if (!prop.Name.EndsWith("_From") && !prop.Name.EndsWith("_To"))
                                                {
                                                    val = $"and [Project1].[{column.DbColumn_Name}] = {prop.Value.ToString()}";
                                                }
                                            }
                                            break;
                                        case (int)DbColumnTypeEnum.String:
                                        case (int)DbColumnTypeEnum.Text:
                                        case (int)DbColumnTypeEnum.EmailAddress:
                                        case (int)DbColumnTypeEnum.WebUrl:
                                        case (int)DbColumnTypeEnum.PhoneNumber:
                                            {
                                                if (this.filterString.Contains(prop.Name + "_StringEquals"))
                                                {
                                                    val = $"and [Project1].[{column.DbColumn_Name}] = N'{prop.Value.ToString().Replace("'", "''")}'";
                                                }
                                                else
                                                {
                                                    val = $"and [Project1].[{column.DbColumn_Name}] like N'%{prop.Value.ToString().Replace("'", "''")}%'";
                                                }
                                                if (column.DbColumn_Name == "Pohoda_ID")
                                                {
                                                    val = $"and [Project1].[{column.DbColumn_Name}] = N'{prop.Value.ToString().Replace("'", "''")}'";
                                                }
                                            }
                                            break;
                                        case (int)DbColumnTypeEnum.Entity:
                                            {
                                                val = $"and [Project1].[{column.DbColumn_Name}] = {prop.Value.ToString()}";
                                            }
                                            break;
                                        case (int)DbColumnTypeEnum.Enumeration:
                                            {
                                                string inValue = prop.Value.ToString().Replace("[", "").Replace("]", "");
                                                if (!string.IsNullOrEmpty(inValue) && inValue.Length > 0)
                                                {
                                                    val = $"and [Project1].[{column.DbColumn_Name}] in ({inValue})";
                                                }
                                            }
                                            break;
                                        case (int)DbColumnTypeEnum.MultiEnumeration:
                                            {
                                                string inValue = prop.Value.ToString();
                                                if (!string.IsNullOrEmpty(inValue) && inValue.Length > 2) // []
                                                {
                                                    //val = $"and [Project1].[{vwProjection.DbPrimaryColumn_Name}] in (SELECT doe.[RecordId] FROM [crm].[DbColumnEnumeration] doe WHERE doe.[Deleted] = 0 AND doe.[DbColumn_FK] = {column.DbColumn_Id} AND doe.[Enumeration_FK] in ({inValue}))";
                                                    val = $"and [Project1].[{column.DbColumn_Name}] IS NOT NULL and ((SELECT count(*) FROM OPENJSON(N'{inValue}') json2 INNER JOIN OPENJSON([Project1].[{column.DbColumn_Name}]) json1 ON json1.value = json2.value) > 0)";
                                                }
                                            }
                                            break;
                                        case (int)DbColumnTypeEnum.Date:
                                        case (int)DbColumnTypeEnum.DateTime:
                                            if (prop.Value != null)
                                            {
                                                DateTime date1 = (DateTime)prop;
                                                if (prop.Name.EndsWith("_From"))
                                                {
                                                    val = $"and [Project1].[{column.DbColumn_Name}] >= convert(datetime, '{date1.ToString("s")}', 126)";
                                                }
                                                if (prop.Name.EndsWith("_To"))
                                                {
                                                    val = $"and [Project1].[{column.DbColumn_Name}] < convert(datetime, '{date1.ToString("s")}', 126)";
                                                }
                                            }
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                else
                                {
                                    val = column.Column_Filter.Replace("##Value##", prop.Value.ToString());
                                }
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(val))
                {
                    Write(val);
                    Write(Environment.NewLine);
                }
            }
        }

        protected void WriteLeftOuterJoin(WriteLeftOuterJoinType type)
        {
            foreach (KeyValuePair<string, string> pair in this.foreignTables)
            {
                string val = string.Empty;
                vwProjectionColumn column = null;
                if (type == WriteLeftOuterJoinType.ForData)
                {
                    column = columns.Where(c => c.ProjectionColumn_Name == pair.Key
                    && (
                        columnsVisible != null && columnsVisible.Contains(c.ProjectionColumn_Name)
                        ||
                        !string.IsNullOrEmpty(sortOrderColumnName) && c.ProjectionColumn_Name == sortOrderColumnName
                        //||
                        //columnsInFilter.Contains(c.ProjectionColumn_Name)
                        )
                    ).FirstOrDefault();
                }
                if (column != null)
                {
                    switch (column.DbColumnType_Id)
                    {
                        case (int)DbColumnTypeEnum.Enumeration:
                            if (this.foreignTables.ContainsKey(column.ProjectionColumn_Name))
                            {
                                val = $"LEFT OUTER JOIN [crm].[Enumeration] [{pair.Value}] on [{pair.Value}].[Id] = [Project1].[{column.ProjectionColumn_Name}]";
                            }
                            break;
                        case (int)DbColumnTypeEnum.Entity:
                            {
                                if (column.ProjectionReference_Id.HasValue)
                                {
                                    vwProjection fk = settings.GetVwProjectionList(userInfo.ProfileId).FirstOrDefault(x => x.Projection_Id == column.ProjectionReference_Id.Value);
                                    if (fk != null)
                                    {
                                        val = $"LEFT OUTER JOIN [{fk.DbObject_Schema}].[{fk.DbObject_Name}] [{pair.Value}] on [{pair.Value}].[{fk.DbPrimaryColumn_Name}] = [Project1].[{column.ProjectionColumn_Name}]";
                                    }
                                }
                            }
                            break;
                        case (int)DbColumnTypeEnum.EntityEnumeration:
                            {
                                if (column.ProjectionReference_Id.HasValue)
                                {
                                    vwProjection fk = settings.GetVwProjectionList(userInfo.ProfileId).FirstOrDefault(x => x.Projection_Id == column.ProjectionReference_Id.Value);
                                    if (fk != null)
                                    {
                                        val = $"LEFT OUTER JOIN [{fk.DbObject_Schema}].[{fk.DbObject_Name}] [{pair.Value}] on [{pair.Value}].[{fk.DbPrimaryColumn_Name}] = [Project1].[{column.ProjectionColumn_Name}]";
                                    }
                                }
                            }
                            break;
                        default:
                            val = $"[Project1].[{column.DbColumn_Name}] as [{column.DbColumn_Name}]";
                            break;
                    }
                }

                if (!string.IsNullOrEmpty(val))
                {
                    WriteLine(val);
                }
            }
        }

        protected void WriteWhereFilter()
        {
            if (vwProjection.DbObjectType_Id == 1)
            {
                WriteLine($"where [Project1].[Deleted] = 0");
            }
            else
            {
                WriteLine($"where 1 = 1");
            }

            if (vwProjection.DbObject_Name == "TimeSheet")
            {
                WriteLine($"and [Project1].[CreatedBy_FK] = @userId");
            }
            if (vwProjection.DbObject_Name == "Email")
            {
                WriteLine($"and (([Project1].[EmailAccount_Folder_FK] in (SELECT eac.Id FROM [crm].[EmailAccountFolder] eac INNER JOIN [crm].[UserEmailAccount] uea on uea.Id = eac.User_EmailAccount_FK AND uea.Deleted = 0 WHERE eac.Deleted = 0 and uea.User_FK = @userId)) or ([Project1].[IsPublic] = 1))");
            }

            if (entityType > 0)
            {
                //WriteLine($"and [Project1].[{vwProjection.DbPrimaryColumn_Name}] not in (SELECT Entity_FK FROM [crm].[fn_GetBannedEntityIds] (@userId, {entityType}, 1))");
            }
            if (vwProjectionRelation != null && parentEntityId.HasValue && parentEntityId.Value > 0 && gridMode != "ManyToMany" && gridMode != "ComputedManyToMany")
            {
                if (!string.IsNullOrEmpty(vwProjectionRelation.ProjectionRelation_Filter))
                {
                    string filter = vwProjectionRelation.ProjectionRelation_Filter.Replace("##ParentEntityId##", parentEntityId.ToString());
                    WriteLine(filter);
                }
                else if (!String.IsNullOrEmpty(vwProjectionRelation.DbColumn1_Name))
                {
                    WriteLine($"and ([Project1].[{vwProjectionRelation.DbColumn1_Name}] = {parentEntityId}{(!String.IsNullOrEmpty(vwProjectionRelation.DbColumn2_Name) ? $" or [Project1].[{vwProjectionRelation.DbColumn2_Name}] = {parentEntityId}" : string.Empty)}{(!String.IsNullOrEmpty(vwProjectionRelation.DbColumn3_Name) ? $" or [Project1].[{vwProjectionRelation.DbColumn3_Name}] = {parentEntityId}" : string.Empty)}) ");
                }
            }
            if (vwProjectionRelation != null && parentEntityId.HasValue && parentEntityId.Value > 0 && (gridMode == "ManyToMany" || gridMode == "ComputedManyToMany"))
            {
                if (!string.IsNullOrEmpty(vwProjectionRelation.ProjectionRelation_Filter))
                {
                    string filter = vwProjectionRelation.ProjectionRelation_Filter.Replace("##ParentEntityId##", parentEntityId.ToString());
                    WriteLine(filter);
                }
                else
                {
                    WriteLine($" and(   ([Project1].[{vwProjection.DbPrimaryColumn_Name}] in (SELECT eeLeft.[RightRecordId] FROM [crm].[DbObjectDbObject] eeLeft WHERE eeLeft.Deleted = 0 and eeLeft.LeftDbObject_FK = '{parentDbObject_Id}' and eeLeft.LeftRecordId = {parentEntityId} and eeLeft.RightDbObject_FK = '{vwProjection.DbObject_Id}'))");
                    WriteLine($"      or([Project1].[{vwProjection.DbPrimaryColumn_Name}] in (SELECT eeRight.[LeftRecordId] FROM [crm].[DbObjectDbObject] eeRight WHERE eeRight.Deleted = 0 and eeRight.RightDbObject_FK = '{parentDbObject_Id}' and eeRight.RightRecordId = {parentEntityId} and eeRight.LeftDbObject_FK = '{vwProjection.DbObject_Id}')))");
                }
            }
            if (!String.IsNullOrEmpty(vwProjection.Projection_ProjectionFilter))
            {
                string filter = vwProjection.Projection_ProjectionFilter.Replace("##EntityName##", "Project1");
                WriteLine(filter);
            }

        }

        public void WriteUpdateFilter()
        {

            WriteLine($" WHERE {vwProjection.DbPrimaryColumn_Name} = @id");
            //WriteLine($"and [{vwProjection.DbPrimaryColumn_Name}] not in (SELECT Entity_FK FROM [crm].[fn_GetBannedEntityIds] (@userId, {entityType}, 1))");

            if (!String.IsNullOrEmpty(vwProjection.Projection_ProjectionFilter))
            {
                string filter = vwProjection.Projection_ProjectionFilter.Replace("##EntityName##", vwProjection.DbObject_Name);
                WriteLine(filter);
            }
        }

        protected void WriteColumnForEntity1Select(vwProjectionColumn column)
        {
            string val = $",[Entity1].[{column.ProjectionColumn_Name}] as [{column.ProjectionColumn_Name}]";

            switch (column.DbColumnType_Id)
            {
                case (int)DbColumnTypeEnum.Enumeration:
                    val = val + Environment.NewLine + $",[Entity1].[{column.ProjectionColumn_Name}_Description] as [{column.ProjectionColumn_Name}_Description]";
                    break;
            }

            //if (column.Column_IsPrimaryKey && column.DbColumn_IsPrimaryKey.Value && column.DbColumn_Name.ToUpper() != "ID")
            //{
            //    val = val + Environment.NewLine + $",[Entity1].[{column.DbColumn_Name}] as [Id]";
            //}

            Write(val);
        }

        protected void WriteColumnForProject1Select(vwProjectionColumn column)
        {
            string val = string.Empty;

            switch (column.DbColumnType_Id)
            {
                case (int)DbColumnTypeEnum.Enumeration:
                    if (this.foreignTables.ContainsKey(column.ProjectionColumn_Name))
                    {
                        val = $",[{this.foreignTables[column.ProjectionColumn_Name]}].[Description_{userInfo.UserLanguage}] as [{column.ProjectionColumn_Name}]";
                        val = val + Environment.NewLine + $",[{this.foreignTables[column.ProjectionColumn_Name]}].[Description] as [{column.ProjectionColumn_Name}_Description]";
                    }
                    break;
                case (int)DbColumnTypeEnum.Entity:
                    {
                        val = $",[{this.foreignTables[column.ProjectionColumn_Name]}].[CaptionDisplay] as [{column.ProjectionColumn_Name}]";
                    }
                    break;
                case (int)DbColumnTypeEnum.EntityEnumeration:
                    {
                        val = $",[{this.foreignTables[column.ProjectionColumn_Name]}].[CaptionDisplay] as [{column.ProjectionColumn_Name}]";
                    }
                    break;
                case (int)DbColumnTypeEnum.MultiEnumeration:
                    {
                        val = $",IIF([Project1].[{column.DbColumn_Name}] IS NOT NULL, STUFF((SELECT ',' + en1.[Description_{userInfo.UserLanguage}] FROM (SELECT value as [Value] FROM OPENJSON([Project1].[{column.DbColumn_Name}])) doe INNER JOIN[crm].[Enumeration] en1 ON en1.[Deleted] = 0 AND en1.[Id] = doe.[Value] FOR XML PATH('')),1,1,''), '') as [{column.ProjectionColumn_Name}]";
                    }
                    break;
                case (int)DbColumnTypeEnum.Password:
                    {
                        val = $",null as [{column.ProjectionColumn_Name}]";
                    }
                    break;
                default:
                    val = $",[Project1].[{column.DbColumn_Name}] as [{column.ProjectionColumn_Name}]";
                    if (!string.IsNullOrEmpty(column.Column_ComputedExpression))
                    {
                        val = $",{column.Column_ComputedExpression.Replace("##EntityName##", "Project1").Replace("##ProjectionPrimaryColumnName##", vwProjection.DbPrimaryColumn_Name).Replace("##EntityDbObjectName##", vwProjection.DbObject_Name)} as [{column.ProjectionColumn_Name}]";
                    }
                    break;
            }

            //if (column.Column_IsPrimaryKey && column.DbColumn_IsPrimaryKey.Value && column.DbColumn_Name.ToUpper() != "ID")
            //{
            //    val = val + Environment.NewLine + $",[Project1].[{column.DbColumn_Name}] as [Id]{Environment.NewLine},[Project1].[CaptionSort] as [CaptionSort]";
            //}

            Write(val);
        }

        protected string GetReferenceCaption(int columnType_FK, int? ReferenceProjection_FK, string destinationProjectionColumnName, string defaultValue, string projectionName, string dbColumnName, string computedExpression, out List<string> outColumnNameList)
        {
            outColumnNameList = new List<string>();
            string val = string.Empty;
            switch (columnType_FK)
            {
                case (int)DbColumnTypeEnum.Enumeration:
                    {
                        val = val + Environment.NewLine + $",(select top 1 e.[Description_{userInfo.UserLanguage}] from [crm].[Enumeration] e where e.Deleted = 0 and e.[Id] = {(!string.IsNullOrEmpty(defaultValue) ? defaultValue : $"[{projectionName}].[{dbColumnName}]")}) as [{destinationProjectionColumnName}_Caption]";
                        outColumnNameList.Add($"{destinationProjectionColumnName}_Caption");
                        val = val + Environment.NewLine + $",(select top 1 e.[Description] from [crm].[Enumeration] e where e.Deleted = 0 and e.[Id] = {(!string.IsNullOrEmpty(defaultValue) ? defaultValue : $"[{projectionName}].[{dbColumnName}]")}) as [{destinationProjectionColumnName}_Description]";
                        outColumnNameList.Add($"{destinationProjectionColumnName}_Description");
                    }
                    break;
                case (int)DbColumnTypeEnum.Entity:
                    if (ReferenceProjection_FK.HasValue)
                    {
                        vwProjection fk = settings.GetVwProjectionList(userInfo.ProfileId).FirstOrDefault(x => x.Projection_Id == ReferenceProjection_FK.Value);
                        if (fk != null)
                        {
                            val = val + Environment.NewLine + $",(select top 1 s.[CaptionDisplay] from [{fk.DbObject_Schema}].[{fk.DbObject_Name}] s where s.{fk.DbPrimaryColumn_Name} = {(!string.IsNullOrEmpty(defaultValue) ? defaultValue.Replace("##EntityName##", projectionName) : (!string.IsNullOrEmpty(computedExpression) ? computedExpression.Replace("##EntityName##", projectionName) : $"[{projectionName}].[{dbColumnName}]"))}) as [{destinationProjectionColumnName}_Caption]";
                            outColumnNameList.Add($"{destinationProjectionColumnName}_Caption");
                            if (settings.SystemSettings.PohodaConnectorEnabled)
                            {
                                if (fk.HasPohodaIdColumn.HasValue && fk.HasPohodaIdColumn.Value)
                                {
                                    val = val + Environment.NewLine + $",(select top 1 s.[Pohoda_PohodaId] from [{fk.DbObject_Schema}].[{fk.DbObject_Name}] s where s.{fk.DbPrimaryColumn_Name} = {(!string.IsNullOrEmpty(defaultValue) ? defaultValue.Replace("##EntityName##", projectionName) : (!string.IsNullOrEmpty(computedExpression) ? computedExpression.Replace("##EntityName##", projectionName) : $"[{projectionName}].[{dbColumnName}]"))}) as [{destinationProjectionColumnName}_PohodaId]";
                                    outColumnNameList.Add($"{destinationProjectionColumnName}_PohodaId");
                                }
                                if (fk.DbObject_Schema == "dbo" && fk.DbObject_Name == "Product")
                                {
                                    val = val + Environment.NewLine + $",(select top 1 s.[MftPartNum] from [{fk.DbObject_Schema}].[{fk.DbObject_Name}] s where s.{fk.DbPrimaryColumn_Name} = {(!string.IsNullOrEmpty(defaultValue) ? defaultValue.Replace("##EntityName##", projectionName) : (!string.IsNullOrEmpty(computedExpression) ? computedExpression.Replace("##EntityName##", projectionName) : $"[{projectionName}].[{dbColumnName}]"))}) as [{destinationProjectionColumnName}_MftPartNum]";
                                    outColumnNameList.Add($"{destinationProjectionColumnName}_MftPartNum");
                                    val = val + Environment.NewLine + $",(select top 1 s.[VendorPartNum] from [{fk.DbObject_Schema}].[{fk.DbObject_Name}] s where s.{fk.DbPrimaryColumn_Name} = {(!string.IsNullOrEmpty(defaultValue) ? defaultValue.Replace("##EntityName##", projectionName) : (!string.IsNullOrEmpty(computedExpression) ? computedExpression.Replace("##EntityName##", projectionName) : $"[{projectionName}].[{dbColumnName}]"))}) as [{destinationProjectionColumnName}_VendorPartNum]";
                                    outColumnNameList.Add($"{destinationProjectionColumnName}_VendorPartNum");
                                }
                            }
                            if (fk.DbObject_Schema == "dbo" && fk.DbObject_Name == "Directory")
                            {
                                val = val + Environment.NewLine + $",(select top 1 (case s.[DirectoryType_FK] when 2 then 'Contact' when 3 then 'Contractor' when 4 then 'DirectoryBranch' else 'Company' end) from [{fk.DbObject_Schema}].[{fk.DbObject_Name}] s where s.{fk.DbPrimaryColumn_Name} = {(!string.IsNullOrEmpty(defaultValue) ? defaultValue.Replace("##EntityName##", projectionName) : (!string.IsNullOrEmpty(computedExpression) ? computedExpression.Replace("##EntityName##", projectionName) : $"[{projectionName}].[{dbColumnName}]"))}) as [{destinationProjectionColumnName}_MainProjection]";
                                outColumnNameList.Add($"{destinationProjectionColumnName}_MainProjection");
                            }
                        }
                    }
                    break;
                case (int)DbColumnTypeEnum.EntityEnumeration:
                    if (ReferenceProjection_FK.HasValue)
                    {
                        vwProjection fk = settings.GetVwProjectionList(userInfo.ProfileId).FirstOrDefault(x => x.Projection_Id == ReferenceProjection_FK.Value);
                        if (fk != null)
                        {
                            val = val + Environment.NewLine + $",(select top 1 s.[CaptionDisplay] from [{fk.DbObject_Schema}].[{fk.DbObject_Name}] s where s.{fk.DbPrimaryColumn_Name} = {(!string.IsNullOrEmpty(defaultValue) ? defaultValue : (!string.IsNullOrEmpty(computedExpression) ? computedExpression.Replace("##EntityName##", projectionName) : $"[{projectionName}].[{dbColumnName}]"))}) as [{destinationProjectionColumnName}_Caption]";
                            outColumnNameList.Add($"{destinationProjectionColumnName}_Caption");
                        }
                    }
                    break;
            }
            return val;
        }

        public virtual string TransformText()
        {
            return string.Empty;
        }


        // toto vygenerovalo VS samo, nesahat !!!!

        #region Fields
        private global::System.Text.StringBuilder generationEnvironmentField;
        private global::System.CodeDom.Compiler.CompilerErrorCollection errorsField;
        private global::System.Collections.Generic.List<int> indentLengthsField;
        private string currentIndentField = "";
        private bool endsWithNewline;
        private global::System.Collections.Generic.IDictionary<string, object> sessionField;
        #endregion
        #region Properties
        /// <summary>
        /// The string builder that generation-time code is using to assemble generated output
        /// </summary>
        protected System.Text.StringBuilder GenerationEnvironment
        {
            get
            {
                if ((this.generationEnvironmentField == null))
                {
                    this.generationEnvironmentField = new global::System.Text.StringBuilder();
                }
                return this.generationEnvironmentField;
            }
            set
            {
                this.generationEnvironmentField = value;
            }
        }
        /// <summary>
        /// The error collection for the generation process
        /// </summary>
        public System.CodeDom.Compiler.CompilerErrorCollection Errors
        {
            get
            {
                if ((this.errorsField == null))
                {
                    this.errorsField = new global::System.CodeDom.Compiler.CompilerErrorCollection();
                }
                return this.errorsField;
            }
        }
        /// <summary>
        /// A list of the lengths of each indent that was added with PushIndent
        /// </summary>
        private System.Collections.Generic.List<int> indentLengths
        {
            get
            {
                if ((this.indentLengthsField == null))
                {
                    this.indentLengthsField = new global::System.Collections.Generic.List<int>();
                }
                return this.indentLengthsField;
            }
        }
        /// <summary>
        /// Gets the current indent we use when adding lines to the output
        /// </summary>
        public string CurrentIndent
        {
            get
            {
                return this.currentIndentField;
            }
        }
        /// <summary>
        /// Current transformation session
        /// </summary>
        public virtual global::System.Collections.Generic.IDictionary<string, object> Session
        {
            get
            {
                return this.sessionField;
            }
            set
            {
                this.sessionField = value;
            }
        }
        #endregion

        #region Transform-time helpers
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void Write(string textToAppend)
        {
            if (string.IsNullOrEmpty(textToAppend))
            {
                return;
            }
            // If we're starting off, or if the previous text ended with a newline,
            // we have to append the current indent first.
            if (((this.GenerationEnvironment.Length == 0)
                        || this.endsWithNewline))
            {
                this.GenerationEnvironment.Append(this.currentIndentField);
                this.endsWithNewline = false;
            }
            // Check if the current text ends with a newline
            if (textToAppend.EndsWith(global::System.Environment.NewLine, global::System.StringComparison.CurrentCulture))
            {
                this.endsWithNewline = true;
            }
            // This is an optimization. If the current indent is "", then we don't have to do any
            // of the more complex stuff further down.
            if ((this.currentIndentField.Length == 0))
            {
                this.GenerationEnvironment.Append(textToAppend);
                return;
            }
            // Everywhere there is a newline in the text, add an indent after it
            textToAppend = textToAppend.Replace(global::System.Environment.NewLine, (global::System.Environment.NewLine + this.currentIndentField));
            // If the text ends with a newline, then we should strip off the indent added at the very end
            // because the appropriate indent will be added when the next time Write() is called
            if (this.endsWithNewline)
            {
                this.GenerationEnvironment.Append(textToAppend, 0, (textToAppend.Length - this.currentIndentField.Length));
            }
            else
            {
                this.GenerationEnvironment.Append(textToAppend);
            }
        }
        /// <summary>
        /// Write text directly into the generated output
        /// </summary>
        public void WriteLine(string textToAppend)
        {
            this.Write(textToAppend);
            this.GenerationEnvironment.AppendLine();
            this.endsWithNewline = true;
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void Write(string format, params object[] args)
        {
            this.Write(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Write formatted text directly into the generated output
        /// </summary>
        public void WriteLine(string format, params object[] args)
        {
            this.WriteLine(string.Format(global::System.Globalization.CultureInfo.CurrentCulture, format, args));
        }
        /// <summary>
        /// Raise an error
        /// </summary>
        public void Error(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Raise a warning
        /// </summary>
        public void Warning(string message)
        {
            System.CodeDom.Compiler.CompilerError error = new global::System.CodeDom.Compiler.CompilerError();
            error.ErrorText = message;
            error.IsWarning = true;
            this.Errors.Add(error);
        }
        /// <summary>
        /// Increase the indent
        /// </summary>
        public void PushIndent(string indent)
        {
            if ((indent == null))
            {
                throw new global::System.ArgumentNullException("indent");
            }
            this.currentIndentField = (this.currentIndentField + indent);
            this.indentLengths.Add(indent.Length);
        }
        /// <summary>
        /// Remove the last indent that was added with PushIndent
        /// </summary>
        public string PopIndent()
        {
            string returnValue = "";
            if ((this.indentLengths.Count > 0))
            {
                int indentLength = this.indentLengths[(this.indentLengths.Count - 1)];
                this.indentLengths.RemoveAt((this.indentLengths.Count - 1));
                if ((indentLength > 0))
                {
                    returnValue = this.currentIndentField.Substring((this.currentIndentField.Length - indentLength));
                    this.currentIndentField = this.currentIndentField.Remove((this.currentIndentField.Length - indentLength));
                }
            }
            return returnValue;
        }
        /// <summary>
        /// Remove any indentation
        /// </summary>
        public void ClearIndent()
        {
            this.indentLengths.Clear();
            this.currentIndentField = "";
        }
        #endregion
        #region ToString Helpers
        /// <summary>
        /// Utility class to produce culture-oriented representation of an object as a string.
        /// </summary>
        public class ToStringInstanceHelper
        {
            private System.IFormatProvider formatProviderField = global::System.Globalization.CultureInfo.InvariantCulture;
            /// <summary>
            /// Gets or sets format provider to be used by ToStringWithCulture method.
            /// </summary>
            public System.IFormatProvider FormatProvider
            {
                get
                {
                    return this.formatProviderField;
                }
                set
                {
                    if ((value != null))
                    {
                        this.formatProviderField = value;
                    }
                }
            }
            /// <summary>
            /// This is called from the compile/run appdomain to convert objects within an expression block to a string
            /// </summary>
            public string ToStringWithCulture(object objectToConvert)
            {
                if ((objectToConvert == null))
                {
                    throw new global::System.ArgumentNullException("objectToConvert");
                }
                System.Type t = objectToConvert.GetType();
                System.Reflection.MethodInfo method = t.GetMethod("ToString", new System.Type[] {
                            typeof(System.IFormatProvider)});
                if ((method == null))
                {
                    return objectToConvert.ToString();
                }
                else
                {
                    return ((string)(method.Invoke(objectToConvert, new object[] {
                                this.formatProviderField })));
                }
            }
        }
        private ToStringInstanceHelper toStringHelperField = new ToStringInstanceHelper();
        /// <summary>
        /// Helper to produce culture-oriented representation of an object as a string
        /// </summary>
        public ToStringInstanceHelper ToStringHelper
        {
            get
            {
                return this.toStringHelperField;
            }
        }
        #endregion
    }

    public enum WriteLeftOuterJoinType
    {
        ForRecordsFiltered,
        ForData
    }
}

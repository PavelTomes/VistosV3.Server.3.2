using System.Collections.Generic;
using System.Linq;
using Core.Models;
using Core.Services;
using Core.VistosDb.Objects;

namespace Core.QueryBuilder.Templates
{
    public partial class Insert : InsertBase
    {
        private Settings settings = Settings.GetInstance;
        private string projectionName { get; set; }
        private int entityType { get; set; }
        private vwProjection vwProjection { get; set; }
        private List<vwProjectionColumn> columns { get; set; }
        private UserInfo userInfo { get; set; }

        public Insert(UserInfo userInfo, string projectionName)
        {

            this.userInfo = userInfo;
            this.projectionName = projectionName;
            this.vwProjection = settings.GetVwProjectionList(userInfo.ProfileId).First(x => x.Projection_Name == projectionName);
            this.entityType = settings.GetCrmEntityType(this.vwProjection.DbObject_Name);
            this.columns = settings.GetVwProjectionColumnList(userInfo.ProfileId, projectionName).Where(x =>
                x.AppColumnType_Id != 1
                && !x.Column_IsReadOnly
                && !x.Column_IsPrimaryKey
                && (x.Column_IsVisibleOnForm || x.Column_HiddenData)
                && x.AccessRightsType_Id == 2
                && !string.IsNullOrEmpty(x.Column_DbColumnTypeNative)
            ).ToList();
        }

        private void WriteNewSequenceNumber()
        {
            if (vwProjection.NumberingSequence_NumericDbColumnId.HasValue)
            {
                vwProjectionColumn dateProjectionColumn = this.columns.Where(c => vwProjection.NumberingSequence_DateDbColumnId.HasValue && c.DbColumn_Id == vwProjection.NumberingSequence_DateDbColumnId.Value).FirstOrDefault();
                vwProjectionColumn issuerProjectionColumn = this.columns.Where(c => vwProjection.NumberingSequence_IssuerDbColumnId.HasValue && c.DbColumn_Id == vwProjection.NumberingSequence_IssuerDbColumnId.Value).FirstOrDefault();
                vwProjectionColumn typeProjectionColumn = this.columns.Where(c => vwProjection.NumberingSequence_TypeDbColumnId.HasValue && c.DbColumn_Id == vwProjection.NumberingSequence_TypeDbColumnId.Value).FirstOrDefault();

                WriteLine("DECLARE	@newSequenceNumber varchar(100)");
                WriteLine("DECLARE	@recordIssuerAccountId int");
                WriteLine("DECLARE	@recordDate datetime");
                WriteLine("DECLARE	@recordTypeId int");

                WriteLine("SELECT");
                if (dateProjectionColumn != null && !string.IsNullOrEmpty(dateProjectionColumn.ProjectionColumn_Name))
                {
                    WriteLine($" @recordDate = isnull(json.[{dateProjectionColumn.ProjectionColumn_Name}], getdate())");
                }
                else
                {
                    WriteLine($" @recordDate = getdate()");
                }
                if (issuerProjectionColumn != null && !string.IsNullOrEmpty(issuerProjectionColumn.ProjectionColumn_Name))
                {
                    WriteLine($" ,@recordIssuerAccountId = json.[{issuerProjectionColumn.ProjectionColumn_Name}]");
                }
                if (typeProjectionColumn != null && !string.IsNullOrEmpty(typeProjectionColumn.ProjectionColumn_Name))
                {
                    WriteLine($" ,@recordTypeId = json.[{typeProjectionColumn.ProjectionColumn_Name}]");
                }
                WriteLine("FROM OPENJSON(@json) WITH ( [Deleted] [int]");
                if (dateProjectionColumn != null && !string.IsNullOrEmpty(dateProjectionColumn.ProjectionColumn_Name))
                {
                    WriteJsonColumn(dateProjectionColumn);
                }
                if (issuerProjectionColumn != null && !string.IsNullOrEmpty(issuerProjectionColumn.ProjectionColumn_Name))
                {
                    WriteJsonColumn(issuerProjectionColumn);
                }
                if (typeProjectionColumn != null && !string.IsNullOrEmpty(typeProjectionColumn.ProjectionColumn_Name))
                {
                    WriteJsonColumn(typeProjectionColumn);
                }
                WriteLine(") AS json");

                WriteLine($"EXEC [dbo].[sp_api3_GetNewProjectionNumber] {userInfo.UserId}, @recordIssuerAccountId, @recordDate, {vwProjection.DbObject_Id}, @recordTypeId, @newSequenceNumber OUTPUT");
            }
        }

        private void WriteGrids()
        {
            List<vwProjectionRelation> gridItems = settings.VwProjectionRelationList.Where(x =>
                x.ProjectionRelation_ParentProjection_FK == this.vwProjection.Projection_Id
                && x.ProjectionRelation_ChildProjectionName != "Participant"
                && x.ProjectionRelation_ChildProjectionProfile_Id == userInfo.ProfileId
                && ((x.ProjectionRelation_ChildProjectionAccessRight & (long)6) == (long)6)
                ).ToList();

            foreach (vwProjectionRelation gridItem in gridItems)
            {
                switch (gridItem.ProjectionRelation_Type_FK)
                {
                    case (int)DbRelationTypeEnum.ItemGrid:
                    case (int)DbRelationTypeEnum.ItemMasterGrid:
                        {
                            ItemGridUpdate itemGridUpdateTemplate = new ItemGridUpdate(userInfo, gridItem, vwProjection, entityType);
                            string childVal = itemGridUpdateTemplate.TransformText();
                            Write(childVal);

                            ItemGridInsert itemGridInsertTemplate = new ItemGridInsert(userInfo, gridItem, vwProjection, entityType);
                            string childInsertVal = itemGridInsertTemplate.TransformText();
                            Write(childInsertVal);
                        }
                        break;
                    default:
                        break;
                }

            }
        }

        private void WriteInsertColumn(vwProjectionColumn column)
        {
            string val = $",[{column.DbColumn_Name}]";
            if (!string.IsNullOrEmpty(val))
            {
                WriteLine(val);
            }
        }

        private void WriteSelectColumn(vwProjectionColumn column)
        {
            if (vwProjection.NumberingSequence_NumericDbColumnId.HasValue && column.DbColumn_Id == vwProjection.NumberingSequence_NumericDbColumnId.Value)
            {
                WriteLine(",@newSequenceNumber");
            }
            else if (!string.IsNullOrEmpty(column.Column_InsertDefaultValue))
            {
                WriteLine($",{column.Column_InsertDefaultValue}");
            }
            else
            {
                switch (column.DbColumnType_Id)
                {
                    case (int)DbColumnTypeEnum.Bit:
                    case (int)DbColumnTypeEnum.BitIcon:
                        {
                            if (!column.Column_IsNullable)
                            {
                                WriteLine($",isnull(json.[{column.ProjectionColumn_Name}], 0)");
                            }
                            else
                            {
                                WriteLine($",json.[{column.ProjectionColumn_Name}]");
                            }
                        }
                        break;
                    case (int)DbColumnTypeEnum.Signature:
                        WriteLine($",(select top 1 si.[Id] from [crm].[Signature] si where si.deleted = 0 and si.UniqueGuid = json.[{column.ProjectionColumn_Name}])");
                        break;
                    case (int)DbColumnTypeEnum.Geography:
                        WriteLine($",case when json.{column.ProjectionColumn_Name}_Lat <> 0 and json.{column.ProjectionColumn_Name}_Long <> 0 then geography::Point(json.{column.ProjectionColumn_Name}_Lat, json.{column.ProjectionColumn_Name}_Long, 4326) else null end");
                        break;
                    case (int)DbColumnTypeEnum.Password:
                        WriteLine($",iif(len(isnull(json.[{column.ProjectionColumn_Name}],'')) > 0, json.[{column.ProjectionColumn_Name}], convert(varchar(40),newid()))");
                        break;
                    case (int)DbColumnTypeEnum.MultiEnumeration:
                        {
                            WriteLine($",JSON_QUERY(@json, '$.{column.ProjectionColumn_Name}')");
                        }
                        break;
                    case (int)DbColumnTypeEnum.String:
                    case (int)DbColumnTypeEnum.Text:
                    case (int)DbColumnTypeEnum.WidgetHtml:
                    case (int)DbColumnTypeEnum.TextSimple:
                    case (int)DbColumnTypeEnum.IconSelect:
                    case (int)DbColumnTypeEnum.JSON:
                    case (int)DbColumnTypeEnum.Color:
                    case (int)DbColumnTypeEnum.EmailAddress:
                    case (int)DbColumnTypeEnum.WebUrl:
                    case (int)DbColumnTypeEnum.PhoneNumber:
                        WriteLine($",iif(len(isnull(json.[{column.ProjectionColumn_Name}],'')) > 0, json.[{column.ProjectionColumn_Name}], null)");
                        break;
                    default:
                        WriteLine($",json.[{column.ProjectionColumn_Name}]");
                        break;
                }
            }
        }

        private void WriteJsonColumn(vwProjectionColumn column)
        {
            string val = $",[{column.ProjectionColumn_Name}] {column.Column_DbColumnTypeNative}";
            if (column.DbColumnType_Id == (int)DbColumnTypeEnum.Geography)
            {
                val = $",[{column.ProjectionColumn_Name}_Lat] float, [{column.ProjectionColumn_Name}_Long] float";
            }
            if (column.DbColumnType_Id == (int)DbColumnTypeEnum.Signature)
            {
                val = $",[{column.ProjectionColumn_Name}] [varchar](50)";
            }
            if (column.DbColumnType_Id == (int)DbColumnTypeEnum.MultiEnumeration)
            {
                val = string.Empty;
            }
            if (!string.IsNullOrEmpty(val))
            {
                WriteLine(val);
            }
        }


    }
}

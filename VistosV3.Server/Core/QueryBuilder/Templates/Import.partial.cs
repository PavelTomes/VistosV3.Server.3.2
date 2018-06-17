using Core.Models;
using Core.VistosDb.Objects;
using System.Collections.Generic;
using System.Linq;

namespace Core.QueryBuilder.Templates
{
    public partial class Import
    {
        private string pairingColumn { get; set; }
        private List<vwProjectionActionColumnMapping> newActionColumns { get; set; }

        public Import(UserInfo userInfo, string projectionName, string pairingColumn, string[] columns)
        {
            this.userInfo = userInfo;
            this.projectionName = projectionName;
            this.vwProjection = settings.GetVwProjectionList(userInfo.ProfileId).First(x => x.Projection_Name == projectionName);
            this.columns = settings.GetVwProjectionColumnList(userInfo.ProfileId, projectionName).Where(x =>
                x.AppColumnType_Id != 1
                && !x.Column_IsReadOnly
                && !x.Column_IsPrimaryKey
                && (x.Column_IsVisibleOnForm || x.Column_HiddenData)
                && x.AccessRightsType_Id == 2
                && (columns.Contains(x.ProjectionColumn_Name) || x.ProjectionColumn_Name == pairingColumn)
            ).ToList();
            this.pairingColumn = pairingColumn;
            this.newActionColumns = settings.VwProjectionActionColumnMappingList.Where(x => 
                x.ProjectionTo_Name.Equals(this.projectionName) 
                && x.ActionTypeName == "New"
                && !columns.Contains(x.ProjectionColumnTo_Name)
            ).ToList();
        }

        private void WriteInsertColumn(vwProjectionColumn column)
        {
            string val = $",[{column.DbColumn_Name}]";
            WriteLine(val);
        }
        private void WriteInsertColumn(vwProjectionActionColumnMapping column)
        {
            string val = $",[{column.ProjectionColumnTo_Name}]";
            WriteLine(val);
        }

        private void WriteUpdateColumn(vwProjectionColumn column)
        {
            if (column.ProjectionColumn_Name != pairingColumn)
            {
                string val = $",[{column.DbColumn_Name}] = json.[{column.ProjectionColumn_Name}]";
                switch (column.DbColumnType_Id)
                {
                    case (int)DbColumnTypeEnum.Bit:
                    case (int)DbColumnTypeEnum.BitIcon:
                        {
                            if (!column.Column_IsNullable)
                            {
                                val = $",[{column.DbColumn_Name}] = isnull(json.[{column.ProjectionColumn_Name}], 0)";
                            }
                        }
                        break;
                    default:
                        break;
                }
                if (!string.IsNullOrEmpty(column.Column_UpdateDefaultValue))
                {
                    val = $",[{column.DbColumn_Name}] = {column.Column_UpdateDefaultValue}";
                }

                Write(val);
                WriteLine("");
            }
        }
        private void WriteSelectColumn(vwProjectionActionColumnMapping column)
        {
            WriteLine($",{column.DefaultValue} as [{column.ProjectionColumnTo_Name}]");
        }

        private void WriteSelectColumn(vwProjectionColumn column)
        {
            if (vwProjection.NumberingSequence_NumericDbColumnId.HasValue  && column.DbColumn_Id == vwProjection.NumberingSequence_NumericDbColumnId.Value)
            {
                WriteLine(",@NewSequenceNumber");
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
                                Write($",isnull(json.[{column.ProjectionColumn_Name}], 0)");
                            }
                            else
                            {
                                Write($",json.[{column.ProjectionColumn_Name}]");
                            }
                        }
                        break;
                    default:
                        if (!string.IsNullOrEmpty(column.Column_InsertDefaultValue))
                        {
                            Write($",{column.Column_InsertDefaultValue}");
                        }
                        else
                        {
                            Write($",json.[{column.ProjectionColumn_Name}]");
                        }
                        break;
                }
                WriteLine("");
            }
        }

        private void WriteJsonColumn(vwProjectionColumn column)
        {
            Write($",[{column.ProjectionColumn_Name}] {column.Column_DbColumnTypeNative}");
            WriteLine("");
        }

    }
}

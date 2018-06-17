namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;


    public partial class vwProjectionColumn
    {
        public int? Column_QueueOrder { get; set; }
        public bool Column_IsNullable { get; set; }
        public string Column_LocalizationKey { get; set; }
        public string Column_InsertDefaultValue { get; set; }
        public string Column_UpdateDefaultValue { get; set; }
        public bool Column_IsReadOnly { get; set; }
        public bool Column_HiddenData { get; set; }
        public int Column_VisibleOnGrid { get; set; }
        public bool Column_IsVisibleOnForm { get; set; }
        public bool Column_IsVisibleOnFilter { get; set; }
        public bool Column_IsVisibleOnTooltip { get; set; }
        public bool Column_IsVisibleOnItemGrid { get; set; }
        public string Column_Filter { get; set; }
        public bool Column_EnablePickerCreate { get; set; }
        public bool Column_EnableGetSimpleTooltip { get; set; }
        public string Column_ConfigJson { get; set; }
        public string Column_ComputedExpression { get; set; }
        public string Column_StdFormLogic { get; set; }
        public string Column_CustomFormLogic { get; set; }
        public bool Column_IsPrimaryKey { get; set; }
        public int? Column_StringMaxLength { get; set; }
        public bool Column_IgnoreRecordHistory { get; set; }
        public bool Column_IgnoreTrackChanges { get; set; }
        public string Column_DbColumnTypeNative { get; set; }
        public int ProjectionColumn_Id { get; set; }
        public string ProjectionColumn_Name { get; set; }
        public int Projection_Id { get; set; }
        public string Projection_Name { get; set; }
        public int DbColumn_Id { get; set; }
        public string DbColumn_Name { get; set; }
        public int DbColumnType_Id { get; set; }
        public string DbColumnType_Name { get; set; }
        public int DbObjectType_Id { get; set; }
        public string DbObjectType_Name { get; set; }
        public int DbObject_Id { get; set; }
        public string DbObject_Name { get; set; }
        public int? ProjectionReference_Id { get; set; }
        public string ProjectionReference_Name { get; set; }
        public int? DbObjectReference_Id { get; set; }
        public string DbObjectReference_Name { get; set; }
        public int? EnumerationType_Id { get; set; }
        public string EnumerationType_Type { get; set; }
        public int AppColumnType_Id { get; set; }
        public string AppColumnType_Name { get; set; }
        public int? ParentDbColumn_Id { get; set; }
        public string ParentDbColumn_Name { get; set; }
        public int Profile_Id { get; set; }
        public int? AccessRightsType_Id { get; set; }
        public int Column_GdprStatus { get; set; }
    }
}

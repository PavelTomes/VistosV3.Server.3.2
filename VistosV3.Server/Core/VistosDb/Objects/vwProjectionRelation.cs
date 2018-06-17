namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;


    public partial class vwProjectionRelation
    {
        
        public int ProjectionRelation_Id { get; set; }
        public string ProjectionRelation_Name { get; set; }
        public int ProjectionRelation_Type_FK { get; set; }
        public string ProjectionRelation_Filter { get; set; }
        public int ProjectionRelation_ParentProjection_FK { get; set; }
        public string ProjectionRelation_ParentProjectionName { get; set; }
        public int ProjectionRelation_ChildProjection_FK { get; set; }
        public string ProjectionRelation_ChildProjectionName { get; set; }
        public int ProjectionRelation_ChildProjectionProfile_Id { get; set; }
        public long ProjectionRelation_ChildProjectionAccessRight { get; set; }
        public int? ProjectionColumn1_Id { get; set; }
        public string ProjectionColumn1_Name { get; set; }
        public int? ProjectionColumn1_VisibleOnGrid { get; set; }
        public Nullable<bool> ProjectionColumn1_IsVisibleOnForm { get; set; }
        public Nullable<bool> ProjectionColumn1_IsVisibleOnFilter { get; set; }
        public Nullable<bool> ProjectionColumn1_IsVisibleOnTooltip { get; set; }
        public int? DbColumn1_Id { get; set; }
        public string DbColumn1_Name { get; set; }
        public Nullable<bool> DbColumn1_IsPrimaryKey { get; set; }
        public string DbColumn1_DbColumnTypeNative { get; set; }
        public int? DbColumn1_StringMaxLength { get; set; }
        public int? DbObject1_Id { get; set; }
        public string DbObject1_Name { get; set; }
        public string DbObject1_Schema { get; set; }
        public int? ProjectionColumn2_Id { get; set; }
        public string ProjectionColumn2_Name { get; set; }
        public int? ProjectionColumn2_VisibleOnGrid { get; set; }
        public Nullable<bool> ProjectionColumn2_IsVisibleOnForm { get; set; }
        public Nullable<bool> ProjectionColumn2_IsVisibleOnFilter { get; set; }
        public Nullable<bool> ProjectionColumn2_IsVisibleOnTooltip { get; set; }
        public int? DbColumn2_Id { get; set; }
        public string DbColumn2_Name { get; set; }
        public Nullable<bool> DbColumn2_IsPrimaryKey { get; set; }
        public string DbColumn2_DbColumnTypeNative { get; set; }
        public int? DbColumn2_StringMaxLength { get; set; }
        public int? DbObject2_Id { get; set; }
        public string DbObject2_Name { get; set; }
        public string DbObject2_Schema { get; set; }
        public int? ProjectionColumn3_Id { get; set; }
        public string ProjectionColumn3_Name { get; set; }
        public int? ProjectionColumn3_VisibleOnGrid { get; set; }
        public Nullable<bool> ProjectionColumn3_IsVisibleOnForm { get; set; }
        public Nullable<bool> ProjectionColumn3_IsVisibleOnFilter { get; set; }
        public Nullable<bool> ProjectionColumn3_IsVisibleOnTooltip { get; set; }
        public int? DbColumn3_Id { get; set; }
        public string DbColumn3_Name { get; set; }
        public Nullable<bool> DbColumn3_IsPrimaryKey { get; set; }
        public string DbColumn3_DbColumnTypeNative { get; set; }
        public int? DbColumn3_StringMaxLength { get; set; }
        public int? DbObject3_Id { get; set; }
        public string DbObject3_Name { get; set; }
        public string DbObject3_Schema { get; set; }
        public string DbColumn_NameSortBy { get; set; }
    }
}

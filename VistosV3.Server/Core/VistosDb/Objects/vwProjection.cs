namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;


    public partial class vwProjection
    {
        
        public int Projection_Id { get; set; }
        public string Projection_Name { get; set; }
        public string Projection_Icon { get; set; }
        public string Projection_ProjectionFilter { get; set; }
        public bool Projection_ItemGridDragAndDropEditModeEnabled { get; set; }
        public bool Projection_ItemGridSelectionEnabled { get; set; }
        public bool Projection_FullTextEnabled { get; set; }
        public string Object_Icon { get; set; }
        public bool Object_IsActive { get; set; }
        public bool Object_CategoriesEnabled { get; set; }
        public bool Object_DocumentEnabled { get; set; }
        public bool Object_ParticipantEnabled { get; set; }
        public bool Object_TrackChangesEnabled { get; set; }
        public bool Object_DiscussionEnabled { get; set; }
        public bool Object_RecordHistoryEnabled { get; set; }
        public bool Object_NotificationEnabled { get; set; }
        public bool Object_RemindersEnabled { get; set; }
        public int Object_MethodMode_GetById_FK { get; set; }
        public int Object_MethodMode_Add_FK { get; set; }
        public int Object_MethodMode_Update_FK { get; set; }
        public int Object_MethodMode_Remove_FK { get; set; }
        public int Object_MethodMode_GetPage_FK { get; set; }
        public int Object_MethodMode_NewEntityFrom_FK { get; set; }
        public int Object_MethodMode_NewEntity_FK { get; set; }
        
        public int Profile_Id { get; set; }
        public long AccessRight { get; set; }
        public int? ProjectionPrimaryColumn_Id { get; set; }
        public string ProjectionPrimaryColumn_Name { get; set; }
        public int? DbPrimaryColumn_Id { get; set; }
        public string DbPrimaryColumn_Name { get; set; }
        public int DbObject_Id { get; set; }
        public string DbObject_Name { get; set; }
        public string DbObject_Schema { get; set; }
        public int DbObjectType_Id { get; set; }
        public string DbObjectType_Name { get; set; }
        public int AppObjectType_Id { get; set; }
        public string AppObjectType_Name { get; set; }
        public int? NumberingSequence_NumericDbColumnId { get; set; }
        public int? NumberingSequence_TypeDbColumnId { get; set; }
        public int? NumberingSequence_IssuerDbColumnId { get; set; }
        public int? NumberingSequence_DateDbColumnId { get; set; }
        public Nullable<bool> HasPohodaIdColumn { get; set; }
        public Nullable<bool> IsCalendarObject { get; set; }
        public string EventStartColumnName { get; set; }
        public string EventEndColumnName { get; set; }
        public string CalendarBackgroundColor { get; set; }
        public bool Projection_ActivityEnabled { get; set; }
    }
}

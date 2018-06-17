namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class TrackChanges
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public int User_FK { get; set; }
        public int Type_FK { get; set; }
        public int? Parent_FK { get; set; }
        public int? ProjectionColumn_FK { get; set; }
        public int? DbObject_FK { get; set; }
        public int? DbColumn_FK { get; set; }
        public int RecordId { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string OldValueCaption { get; set; }
        public string NewValueCaption { get; set; }
        public int? ReferenceId { get; set; }
    }
}

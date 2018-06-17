namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class ReminderSettings
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public int CreatedBy_FK { get; set; }
        public DateTime Created { get; set; }
        public int? ModifiedBy_FK { get; set; }
        public DateTime Modified { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public int? DbObject_FK { get; set; }
        public int? DbColumn_FK { get; set; }
        public int? TimeShift { get; set; }
        public int? Role_FK { get; set; }
        public string CaptionDisplay { get; set; }
        public string CaptionSort { get; set; }
    }
}

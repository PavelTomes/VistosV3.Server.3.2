namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class Reminder
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public int CreatedBy_FK { get; set; }
        public DateTime Created { get; set; }
        public int? ModifiedBy_FK { get; set; }
        public DateTime Modified { get; set; }
        public int RecordId { get; set; }
        public int? Role_FK { get; set; }
        public int? User_FK { get; set; }
        public DateTime ReminderTime { get; set; }
        public int? ReminderSettings_FK { get; set; }
        public bool Disabled { get; set; }
        public int? DbObject_FK { get; set; }
        public DateTime? ReminderSent { get; set; }
    }
}

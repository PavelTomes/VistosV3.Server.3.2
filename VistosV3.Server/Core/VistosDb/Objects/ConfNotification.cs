namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class ConfNotification
    {
        public int Id { get; set; }
        public int User_FK { get; set; }
        public int? Projection_FK { get; set; }
        public Nullable<System.TimeSpan> InTime { get; set; }
        public string OnDay { get; set; }
        public int? FrequencyType_FK { get; set; }
        public DateTime? LastRun { get; set; }
        public DateTime? NextRun { get; set; }
        public int? RecordId { get; set; }
        public int NotificationType_FK { get; set; }
    }
}

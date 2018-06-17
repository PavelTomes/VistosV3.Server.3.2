namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class NotificationConfiguration
    {
        public int Id { get; set; }
        public int DbObject_FK { get; set; }
        public int? User_FK { get; set; }
        public int? Profile_FK { get; set; }
        public int? Role_FK { get; set; }
        public int? RecordId { get; set; }
        public int Frequency_FK { get; set; }
        public DateTime? StartSend { get; set; }
        public DateTime? LastSend { get; set; }
        public bool IsEnabled { get; set; }
        public bool Deleted { get; set; }
        public int CreatedBy_FK { get; set; }
        public DateTime Created { get; set; }
        public int? ModifiedBy_FK { get; set; }
        public DateTime Modified { get; set; }
        public int CaptionDisplay { get; set; }
        public int CaptionSort { get; set; }
    }
}

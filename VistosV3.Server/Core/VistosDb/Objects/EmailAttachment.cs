namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmailAttachment
    {
        public int Id { get; set; }
        public int? Email_FK { get; set; }
        public bool Deleted { get; set; }
        public int CreatedBy_FK { get; set; }
        public DateTime Created { get; set; }
        public int? ModifiedBy_FK { get; set; }
        public DateTime Modified { get; set; }
        public string FileName { get; set; }
        public byte[] Data { get; set; }
        public string CID { get; set; }
        public string Type { get; set; }
        public string ContentType { get; set; }
        public long DataLength { get; set; }
        public bool StoredInDropbox { get; set; }
        public string CaptionDisplay { get; set; }
        public string CaptionSort { get; set; }
    }
}

namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class DocumentAttachment
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public int CreatedBy_FK { get; set; }
        public DateTime Created { get; set; }
        public int? ModifiedBy_FK { get; set; }
        public DateTime Modified { get; set; }
        public int Document_FK { get; set; }
        public string DocName { get; set; }
        public DateTime? UploadDate { get; set; }
        public byte[] Attachment { get; set; }
        public bool StoreInDropBox { get; set; }
        public long DataLength { get; set; }
        public string ContentType { get; set; }
        public string Icon { get; set; }
        public bool StoreInFtp { get; set; }
    }
}

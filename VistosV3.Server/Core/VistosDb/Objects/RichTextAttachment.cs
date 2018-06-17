namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class RichTextAttachment
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public int CreatedBy_FK { get; set; }
        public DateTime Created { get; set; }
        public int? ModifiedBy_FK { get; set; }
        public DateTime Modified { get; set; }
        public byte[] Content { get; set; }
        public System.Guid UniqueGuid { get; set; }
    }
}

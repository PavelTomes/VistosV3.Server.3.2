namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class DbObjectEmail
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public int CreatedBy_FK { get; set; }
        public DateTime Created { get; set; }
        public int? ModifiedBy_FK { get; set; }
        public DateTime Modified { get; set; }
        public int DbObject_FK { get; set; }
        public int Email_FK { get; set; }
        public int RecordId { get; set; }
    }
}

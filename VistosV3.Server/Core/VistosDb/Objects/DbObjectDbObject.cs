namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class DbObjectDbObject
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public int CreatedBy_FK { get; set; }
        public DateTime Created { get; set; }
        public int? ModifiedBy_FK { get; set; }
        public DateTime Modified { get; set; }
        public int LeftDbObject_FK { get; set; }
        public int LeftRecordId { get; set; }
        public int RightDbObject_FK { get; set; }
        public int RightRecordId { get; set; }
    }
}

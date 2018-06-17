namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class Enumeration
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public int CreatedBy_FK { get; set; }
        public DateTime Created { get; set; }
        public int? ModifiedBy_FK { get; set; }
        public DateTime Modified { get; set; }
        public string Description { get; set; }
        public string Description_cs_CZ { get; set; }
        public string Description_en_GB { get; set; }
        public string Description_fr_BE { get; set; }
        public string Description_en_US { get; set; }
        public int? ListOrder { get; set; }
        public int? Parent_FK { get; set; }
        public bool SystemEnum { get; set; }
        public string Description_sk_SK { get; set; }
        public int? EnumerationType_FK { get; set; }
        public string Description_de_DE { get; set; }
        public int? SearchColumn { get; set; }
        public string Description_tr_TR { get; set; }
        public string Description_nl_BE { get; set; }
        public string CaptionDisplay { get; set; }
        public string CaptionSort { get; set; }
    }
}

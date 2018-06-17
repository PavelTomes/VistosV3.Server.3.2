namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class EnumerationType
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public int CreatedBy_FK { get; set; }
        public DateTime Created { get; set; }
        public int? ModifiedBy_FK { get; set; }
        public DateTime Modified { get; set; }
        public string Type { get; set; }
        public string Type_cs_CZ { get; set; }
        public string Type_en_GB { get; set; }
        public string Type_fr_BE { get; set; }
        public string Type_en_US { get; set; }
        public string Type_sk_SK { get; set; }
        public bool Visibility { get; set; }
        public bool Custom { get; set; }
        public string Type_de_DE { get; set; }
        public string Type_nl_BE { get; set; }
        public string Type_tr_TR { get; set; }
        public string CaptionDisplay { get; set; }
        public string CaptionSort { get; set; }
    }
}

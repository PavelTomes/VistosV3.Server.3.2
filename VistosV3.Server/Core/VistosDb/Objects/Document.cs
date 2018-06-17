namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class Document
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public int CreatedBy_FK { get; set; }
        public DateTime Created { get; set; }
        public int? ModifiedBy_FK { get; set; }
        public DateTime Modified { get; set; }
        public string Name { get; set; }
        public string DocKey { get; set; }
        public string DocNo { get; set; }
        public string Message { get; set; }
        public DateTime? ActiveDate { get; set; }
        public DateTime? ExpDate { get; set; }
        public bool Is_Template { get; set; }
        public int? Type_FK { get; set; }
        public int? TemplateType_FK { get; set; }
        public int? Contact_FK { get; set; }
        public int? Account_FK { get; set; }
        public int? SearchColumn { get; set; }
        public int? Email_FK { get; set; }
        public int? PrintReport_FK { get; set; }
        public DateTime? Locked { get; set; }
        public DateTime? Archived { get; set; }
        public string CaptionDisplay { get; set; }
        public string CaptionSort { get; set; }
    }

    public partial class DocumentExt : Document
    {
        public string Icon { get; set; }
    }
}

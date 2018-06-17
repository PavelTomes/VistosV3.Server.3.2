namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class TextTemplate
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public int CreatedBy_FK { get; set; }
        public DateTime Created { get; set; }
        public int? ModifiedBy_FK { get; set; }
        public DateTime Modified { get; set; }
        public string SystemName { get; set; }
        public string Name { get; set; }
        public bool Custom { get; set; }
        public int? Language_FK { get; set; }
        public int? Type_FK { get; set; }
        public string TemplateText { get; set; }
        public string CaptionDisplay { get; set; }
        public string CaptionSort { get; set; }
        public int? Projection_FK { get; set; }
        public bool IsDefault { get; set; }
    }
}

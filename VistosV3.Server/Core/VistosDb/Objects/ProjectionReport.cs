namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProjectionReport
    {
        public int Id { get; set; }
        public string ReportName { get; set; }
        public int Projection_FK { get; set; }
        public string EntityName { get; set; }
        public string RdlReportName { get; set; }
        public string Parametrs { get; set; }
        public bool ReportOnIndexPage { get; set; }
        public bool Deleted { get; set; }
        public int CreatedBy_FK { get; set; }
        public DateTime Created { get; set; }
        public int? ModifiedBy_FK { get; set; }
        public DateTime Modified { get; set; }
        public string CaptionDisplay { get; set; }
        public string CaptionSort { get; set; }
        public int? DataProjection_FK { get; set; }
    }
}

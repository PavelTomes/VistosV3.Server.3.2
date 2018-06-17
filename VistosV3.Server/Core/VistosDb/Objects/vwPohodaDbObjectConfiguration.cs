namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;


    public partial class vwPohodaDbObjectConfiguration
    {
        
        public int Conf_Id { get; set; }
        public bool Conf_Deleted { get; set; }
        public int Conf_CreatedBy_FK { get; set; }
        public DateTime Conf_Created { get; set; }
        public int? Conf_ModifiedBy_FK { get; set; }
        public DateTime Conf_Modified { get; set; }
        public string Conf_Name { get; set; }
        public string Conf_CaptionDisplay { get; set; }
        public string Conf_CaptionSort { get; set; }
        public int DbObject_Id { get; set; }
        public string DbObject_Name { get; set; }
        public int Conf_BusinessUnit_FK { get; set; }
        public bool Conf_PohodaExportEnabled { get; set; }
        public string Conf_PohodaExportXmlFitler { get; set; }
        public DateTime? Conf_PohodaExportLastSuccess { get; set; }
        public bool Conf_PohodaImportEnabled { get; set; }
        public bool Conf_PohodaImportRepeatedly { get; set; }
    }
}

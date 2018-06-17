namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;

    
    public partial class vwProjectionColumnLocalization
    {
        public Nullable<System.Guid> Guid { get; set; }
        public int ProjectionColumn_Id { get; set; }
        public string ProjectionColumn_Name { get; set; }
        public int Projection_Id { get; set; }
        public string Projection_Name { get; set; }
        public int Language_Id { get; set; }
        public string Language_Name { get; set; }
        public string LocalizationString { get; set; }
    }
}

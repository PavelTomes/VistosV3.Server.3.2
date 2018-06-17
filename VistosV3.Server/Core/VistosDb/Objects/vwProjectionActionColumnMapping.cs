namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;


    public partial class vwProjectionActionColumnMapping
    {
        
        public int Id { get; set; }
        public int ProjectionAction_Id { get; set; }
        public int? ProjectionFrom_FK { get; set; }
        public string ProjectionFrom_Name { get; set; }
        public int ProjectionTo_FK { get; set; }
        public string ProjectionTo_Name { get; set; }
        public int ActionTypeId { get; set; }
        public string ActionTypeName { get; set; }
        public int? ProjectionColumnFrom_FK { get; set; }
        public string ProjectionColumnFrom_Name { get; set; }
        public int ProjectionColumnTo_FK { get; set; }
        public string DefaultValue { get; set; }
        public string ProjectionColumnTo_Name { get; set; }
        public int ProjectionColumnTo_DbColumnType_Id { get; set; }
        public string ProjectionColumnTo_DbColumnType_Name { get; set; }
        public int? ProjectionColumnTo_ProjectionReference_Id { get; set; }
    }
}

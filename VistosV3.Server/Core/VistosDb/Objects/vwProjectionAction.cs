namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;


    public partial class vwProjectionAction
    {
        
        public int Id { get; set; }
        public string Icon { get; set; }
        public string ProjectionActionName { get; set; }
        public int ProjectionActionType_ID { get; set; }
        public string ProjectionActionType_Name { get; set; }
        public int ProjectionActionMethodMode_ID { get; set; }
        public string ProjectionActionMethodMode_Name { get; set; }
        public int ProjectionActionResultType_ID { get; set; }
        public string ProjectionActionResultType_Name { get; set; }
        public int? ProjectionFrom_ID { get; set; }
        public string ProjectionFrom_Name { get; set; }
        public int? ProjectionFrom_ProfileID { get; set; }
        public Nullable<long> ProjectionFrom_AccessRight { get; set; }
        public int ProjectionTo_ID { get; set; }
        public string ProjectionTo_Name { get; set; }
        public string ProjectionTo_Icon { get; set; }
        public int ProjectionTo_ProfileID { get; set; }
        public long ProjectionTo_AccessRight { get; set; }
        public int? ProjectionActionParent_ID { get; set; }
        public int? ParentProjectionFrom_ID { get; set; }
        public string ParentProjectionFrom_Name { get; set; }
        public int? ParentProjectionFrom_ProfileID { get; set; }
        public Nullable<long> ParentProjectionFrom_AccessRight { get; set; }
        public int? ParentProjectionTo_ID { get; set; }
        public string ParentProjectionTo_Name { get; set; }
        public int? ParentProjectionTo_ProfileID { get; set; }
        public Nullable<long> ParentProjectionTo_AccessRight { get; set; }
        public int? ProjectionActionQueueOrder { get; set; }
    }
}

namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class ReportAccessRights
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Profile_FK { get; set; }
        public long AccessRight { get; set; }
    }
}

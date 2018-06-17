namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class BusinessUnit
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public int CreatedBy_FK { get; set; }
        public DateTime Created { get; set; }
        public int? ModifiedBy_FK { get; set; }
        public DateTime Modified { get; set; }
        public string Name { get; set; }
        public int Account_FK { get; set; }
        public bool PohodaConnectorEnabled { get; set; }
        public string PohodaApiUrl { get; set; }
        public string PohodaApiName { get; set; }
        public string PohodaApiUserName { get; set; }
        public string PohodaApiPassword { get; set; }
        public string PohodaInterval { get; set; }
        public string CaptionDisplay { get; set; }
        public string CaptionSort { get; set; }
    }
}

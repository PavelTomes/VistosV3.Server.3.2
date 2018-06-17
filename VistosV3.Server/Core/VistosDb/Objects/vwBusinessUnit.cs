namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;


    public partial class vwBusinessUnit
    {
        
        public int BusinessUnit_Id { get; set; }
        public string BusinessUnit_Name { get; set; }
        public int Account_Id { get; set; }
        public string Account_Name { get; set; }
        public string Account_RegNumber { get; set; }
        public bool BusinessUnit_PohodaConnectorEnabled { get; set; }
        public string BusinessUnit_PohodaApiUrl { get; set; }
        public string BusinessUnit_PohodaApiName { get; set; }
        public string BusinessUnit_PohodaApiUserName { get; set; }
        public string BusinessUnit_PohodaApiPassword { get; set; }
        public string BusinessUnit_PohodaInterval { get; set; }
    }
}

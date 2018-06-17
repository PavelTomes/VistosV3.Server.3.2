namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class LocalizationString
    {
        public int Id { get; set; }
        public int Language_Fk { get; set; }
        public int Area_Fk { get; set; }
        public string key { get; set; }
        public string value { get; set; }
        public bool customized { get; set; }
        public string tooltip { get; set; }
    }
}

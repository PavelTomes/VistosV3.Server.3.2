namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;


    public partial class vwLocalization
    {
        
        public int LocalizationString_ID { get; set; }
        public string LocalizationString_Key { get; set; }
        public string LocalizationString_Value { get; set; }
        public bool LocalizationString_Customized { get; set; }
        public int? LocalizationArea_ID { get; set; }
        public string LocalizationArea_Name { get; set; }
        public int? LocalizationLanguage_ID { get; set; }
        public string LocalizationLanguage_Name { get; set; }
    }
}

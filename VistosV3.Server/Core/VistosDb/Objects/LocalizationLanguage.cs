namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class LocalizationLanguage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CaptionDisplay { get; set; }
        public string CaptionSort { get; set; }
    }
}

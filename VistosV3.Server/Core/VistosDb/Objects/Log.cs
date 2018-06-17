namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class Log
    {
        public int Id { get; set; }
        public string Application { get; set; }
        public DateTime Time { get; set; }
        public string Level { get; set; }
        public string Message { get; set; }
        public string Exception { get; set; }
        public string StackTrace { get; set; }
        public string RequestGuid { get; set; }
        public string CaptionDisplay { get; set; }
        public string CaptionSort { get; set; }
    }
}

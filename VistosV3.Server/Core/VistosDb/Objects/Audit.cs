namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class Audit
    {
        public int Id { get; set; }
        public DateTime Time { get; set; }
        public int? User_FK { get; set; }
        public string Action { get; set; }
        public string Operation { get; set; }
        public string JsonRequest { get; set; }
        public string JsonResponse { get; set; }
        public string EntityName { get; set; }
        public int? EntityId { get; set; }
        public string UserToken { get; set; }
        public string IpAddress { get; set; }
        public string RequestGuid { get; set; }
        public string Device { get; set; }
        public string QueryBuilderSql { get; set; }
        public string CaptionDisplay { get; set; }
        public string CaptionSort { get; set; }
    }
}

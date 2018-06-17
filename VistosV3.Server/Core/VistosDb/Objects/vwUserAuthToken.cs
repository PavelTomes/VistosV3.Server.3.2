namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class vwUserAuthToken
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string UserLanguage { get; set; }
        public int? UserLanguageId { get; set; }
        public int? ProfileId { get; set; }
        public string Token { get; set; }
    }
}

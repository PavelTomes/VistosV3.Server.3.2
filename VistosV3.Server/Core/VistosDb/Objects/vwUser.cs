namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;


    public partial class vwUser
    {
        
        public int Id { get; set; }
        public Nullable<byte> UserType { get; set; }
        public DateTime? LoginDate { get; set; }
        public string WindowsAccount { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; }
        public string skinname { get; set; }
        public int? Contact_FK { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? DefaultEmailAccount_FK { get; set; }
        public int? DefaultEmailFolder_FK { get; set; }
        public int? LoginFailCount { get; set; }
        public DateTime? LoginBannedUntil { get; set; }
        public string NickName { get; set; }
        public int? User_Avatar_FK { get; set; }
        public string HomePageUrl { get; set; }
        public string CallerId { get; set; }
        public int? Profile_FK { get; set; }
        public string Language { get; set; }
        public string CaptionDisplay { get; set; }
        public string DefaultEmail { get; set; }
        public string ContactEmail { get; set; }
    }
}

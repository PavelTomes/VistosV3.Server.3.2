namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserEmailAccount
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public int CreatedBy_FK { get; set; }
        public DateTime Created { get; set; }
        public int? ModifiedBy_FK { get; set; }
        public DateTime Modified { get; set; }
        public string Name { get; set; }
        public string SMTP_Server { get; set; }
        public int? SMTP_Port { get; set; }
        public string SMTP_UserName { get; set; }
        public string SMTP_Password { get; set; }
        public string Email { get; set; }
        public int User_FK { get; set; }
        public string AccountName { get; set; }
        public string Inc_Server { get; set; }
        public string Inc_Username { get; set; }
        public string Inc_Password { get; set; }
        public int? Inc_Server_Port { get; set; }
        public bool SMTP_RequiresAuth { get; set; }
        public int? AccountType { get; set; }
        public int? SMTP_Encryption { get; set; }
        public int? Inc_Encryption { get; set; }
        public string Signature { get; set; }
        public int? SMTP_Authentication { get; set; }
        public bool EnableHighlightingUnjoinedEmails { get; set; }
        public string MergeEmailsOptOut { get; set; }
        public int? Inc_Authentication { get; set; }
        public bool AllEmailsArePublic { get; set; }
        public byte[] SignCertificate { get; set; }
        public string SignCertificatePassword { get; set; }
        public string CaptionDisplay { get; set; }
        public string CaptionSort { get; set; }
        public int? StoreAttachmentsInDbInDays { get; set; }
        public int? StoreBodyInDbInDays { get; set; }
    }
}

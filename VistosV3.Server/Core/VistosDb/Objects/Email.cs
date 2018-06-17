namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class Email
    {
        public int Id { get; set; }
        public bool Deleted { get; set; }
        public int CreatedBy_FK { get; set; }
        public DateTime Created { get; set; }
        public int? ModifiedBy_FK { get; set; }
        public DateTime Modified { get; set; }
        public string To { get; set; }
        public string CC { get; set; }
        public string BCC { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool Autoforwarded { get; set; }
        public DateTime? ExpirationDateTime { get; set; }
        public Nullable<byte> FlagStatus { get; set; }
        public int? Importence { get; set; }
        public int? InternetCodePage { get; set; }
        public bool ReminderSet { get; set; }
        public DateTime? ReminderDateTime { get; set; }
        public string From { get; set; }
        public DateTime? ReceivedTime { get; set; }
        public int? Type_FK { get; set; }
        public int? Status_FK { get; set; }
        public string MessageID { get; set; }
        public bool IsSignedOrEncrypted { get; set; }
        public int? EmailAccount_Folder_FK { get; set; }
        public bool IsBodyHTML { get; set; }
        public string BodyText { get; set; }
        public bool IsPublic { get; set; }
        public int? DeliverType_FK { get; set; }
        public bool SendByService { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime? DoNotSendBefore { get; set; }
        public string LanguageCode { get; set; }
        public string RfcMessageID { get; set; }
        public DateTime? Locked { get; set; }
        public DateTime? Archived { get; set; }
        public string ReplyTo { get; set; }
        public int? PreviousEmailAccount_Folder_FK { get; set; }
        public bool IsLinkedWithVistos { get; set; }
        public string CaptionDisplay { get; set; }
        public string CaptionSort { get; set; }
        public bool BodyStoredInDropbox { get; set; }
    }
}

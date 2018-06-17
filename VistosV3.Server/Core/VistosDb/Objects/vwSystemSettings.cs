namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class vwSystemSettings
    {
        public string ReportServerUrl { get; set; }
        public string ReportServerFormsPath { get; set; }
        public string ReportServerReportsPath { get; set; }
        public string ReportServerUserName { get; set; }
        public string ReportServerPassword { get; set; }
        public string VistosVersion { get; set; }
        public string VistosApiUrl { get; set; }
        public string VistosUrl { get; set; }
        public string Language { get; set; }
        public int QuantityDecimalPlaces { get; set; }
        public int DefaultCompany_FK { get; set; }
        public int DefaultCurrency_FK { get; set; }
        public string PlugInDllName { get; set; }
        public string PlugInClassNamespace { get; set; }
        public bool CategoriesEnabled { get; set; }
        public bool DocumentEnabled { get; set; }
        public bool ParticipantEnabled { get; set; }
        public bool TrackChangesEnabled { get; set; }
        public bool DiscussionEnabled { get; set; }
        public bool RecordHistoryEnabled { get; set; }
        public bool NotificationEnabled { get; set; }
        public bool RemindersEnabled { get; set; }
        public bool PohodaConnectorEnabled { get; set; }
        public int? DefaultEmailAccount_FK { get; set; }
        public int? DefaultEmailFolderDraft_FK { get; set; }
        public string DropBoxSecurityToken { get; set; }
        public bool StoreEmailBodyInDropbox { get; set; }
        public bool StoreEmailAttachmentsInDropbox { get; set; }
        public bool MerkBasicEnabled { get; set; }
        public string FtpHost { get; set; }
        public int? FtpPort { get; set; }
        public string FtpUserName { get; set; }
        public string FtpPassword { get; set; }
        public string FtpType { get; set; }
        public string FtpPrivateKey { get; set; }
        public string FtpPassPhrase { get; set; }
        public string FtpRoot { get; set; }
        public bool MerkAdvancedEnabled { get; set; }
        public string DefaultEmail { get; set; }
        public bool GdprEnabled { get; set; }
        public string LogoFileName { get; set; }
    }
}

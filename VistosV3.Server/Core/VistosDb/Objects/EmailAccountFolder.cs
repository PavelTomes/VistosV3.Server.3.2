namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;
    
    public partial class EmailAccountFolder
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public int? User_EmailAccount_FK { get; set; }
        public int FolderType { get; set; }
        public bool IsPublic { get; set; }
        public bool Deleted { get; set; }
        public bool IsSystem { get; set; }
        public int? Parent_ID { get; set; }
        public string AccountFolderName { get; set; }
        public Nullable<long> ValidityID { get; set; }
        public string CaptionDisplay { get; set; }
        public string CaptionSort { get; set; }
        public int? StoreAttachmentsInDbInDays { get; set; }
        public int? StoreBodyInDbInDays { get; set; }
    }
}

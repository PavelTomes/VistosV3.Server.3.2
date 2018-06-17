namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;


    public partial class vwParticipant
    {
        public int Role_Id { get; set; }
        public string Role_Name { get; set; }
        public int? Role_MaxParticipantsInRole { get; set; }
        public int DbObjectRole_Id { get; set; }
        public int? DbObjectRole_MaxParticipantsInRole { get; set; }
        
        public int Participant_Id { get; set; }
        public int Participant_RecordId { get; set; }
        public int DbObject_Id { get; set; }
        public string DbObject_Name { get; set; }
        public int DbObject_DbObjectType_FK { get; set; }
        public int DbObject_AppObjectType_FK { get; set; }
        public string DbObject_Schema { get; set; }
        public int User_Id { get; set; }
        public string User_UserName { get; set; }
        public int? User_Profile_FK { get; set; }
        public string User_Language { get; set; }
        public string ContactEmail { get; set; }
    }
}

namespace Core.VistosDb.Objects
{
    using System;
    using System.Collections.Generic;


    public partial class vwNumberingSequence
    {
        
        public int Id { get; set; }
        public int? NumberingSequence_IssuerAccount_FK { get; set; }
        public int? NumberingSequence_Type_FK { get; set; }
        public DateTime? NumberingSequence_StartDate { get; set; }
        public DateTime? NumberingSequence_EndDate { get; set; }
        public string NumberingSequence_SequenceFormat { get; set; }
        public string NumberingSequence_RepetitionDate { get; set; }
        public int? NumberingSequence_DbObject_FK { get; set; }
        public int? NumberingSequence_NumericDbColumn_FK { get; set; }
        public int? NumberingSequence_TypeDbColumn_FK { get; set; }
        public int? NumberingSequence_IssuerDbColumn_FK { get; set; }
        public int? NumberingSequence_DateDbColumn_FK { get; set; }
        public int NumberingSequence_RepetitionType_FK { get; set; }
        public string Projection_Name { get; set; }
        public string NumericProjectionColumn_Name { get; set; }
        public int Profile_Id { get; set; }
        public int? AccessRightsType_Id { get; set; }
    }
}

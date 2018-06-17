using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class NewTrackChangesNotificationMessage
    {
        //public String Name { get; set; }
        public Int32 TrackChanges_ID { get; set; }
        public String Subject { get; set; }
        public DateTime Created { get; set; }
        public String CreatedByContact { get; set; }
        public String OldValue { get; set; }
        public String NewValue { get; set; }
        public Int32 ToUserId { get; set; }
        public Int32 ProfileId { get; set; }
        public String EmailTo { get; set; }
        public String UseLanguage { get; set; }
        public Int32 RecordId { get; set; }
        public String ProjectionColumnName { get; set; }
        public Int32 ProjectionColumnId { get; set; }
        public String ProjectionColumnTypeName { get; set; }
        public Int32 ProjectionColumnTypeId { get; set; }

        public Int32 ProjectionId { get; set; }
        public String ProjectionName { get; set; }

        public DateTimeFormatInfo DateTimeFormatInfo { get; set; }
        public String Header { get; set; }
        public String Body { get; set; }
        public String FieldName { get; set; }
        public String OldValueCaption { get; set; }
        public String NewValueCaption { get; set; }
        //public DateTime Send { get; set; }

        public String HtmlSubject
        {
            get
            {
                return Subject;
            }
        }

        public String HtmlHeader
        {
            get
            {
                // TODO
                return Header;
                //return Header;
            }
        }

        private String htmlBody;
        public String HtmlBody
        {
            get
            {
                if (String.IsNullOrEmpty(htmlBody))
                    return (
                        (String.IsNullOrEmpty(this.OldValue)) ? "" : this.OldValue
                        + "|" + ((String.IsNullOrEmpty(this.NewValue)) ? "" : this.NewValue));

                return this.htmlBody;//String.Format("Nová zpráva {0}", Body);
            }
            set
            {
                this.htmlBody = value;
            }
        }

        public String HtmlCreated
        {
            get
            {
                return Created.ToString(DateTimeFormatInfo);
            }
        }

        public Int32 HtmlUserId
        {
            get
            {
                return ToUserId;
            }
        }

        public Int32 HtmlProjectionId
        {
            get
            {
                return ProjectionId;
            }
        }

        public Int32 HtmlRecord
        {
            get
            {
                return RecordId;
            }
        }
    }
}

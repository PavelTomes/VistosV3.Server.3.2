using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class NewDiscussionNotificationMessage
    {
        //public String Name { get; set; }
        public Int32 DiscussionMessageId { get; set; }
        public String Subject { get; set; }
        public DateTime Created { get; set; }
        public String CreatedByContact { get; set; }
        public String Text { get; set; }
        public Int32 ToUserId { get; set; }
        public Int32 ProfileId { get; set; }
        public String EmailTo { get; set; }
        public String UseLanguage { get; set; }
        public Int32 DiscussionId { get; set; }
        public Int32 RecordId { get; set; }
        public String Hierarchy_ID { get; set; }
        
        public Int32 ProjectionId { get; set; }
        public String ProjectionName { get; set; }

        public DateTimeFormatInfo DateTimeFormatInfo { get; set; }
        public String Header { get; set; }
        public String Body { get; set; }
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
                    return this.Text;

                return this.htmlBody;//String.Format("Nová zpráva {0}", Body);
            } set
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

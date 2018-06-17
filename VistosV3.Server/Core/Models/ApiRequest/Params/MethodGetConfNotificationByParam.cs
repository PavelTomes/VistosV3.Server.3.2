using System;

namespace Core.Models.ApiRequest.Params
{
    public class MethodGetConfNotificationByParam : IRequestParam
    {
        public Int32 ProjectionId { get; set; }
        public Int32 Record_ID { get; set; }
    }
}
using System;

namespace Core.Models.ApiRequest.Params
{
    public class MethodGetCalendarDataParam : IRequestParam
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public object Filter { get; set; }
    }
}
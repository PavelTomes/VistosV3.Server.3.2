using System;

namespace Core.Models.ApiRequest.Params
{
    public class MethodUpdateCalendarDataParam : IRequestParam
    {
        public string EntityName { get; set; }
        public int EntityId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
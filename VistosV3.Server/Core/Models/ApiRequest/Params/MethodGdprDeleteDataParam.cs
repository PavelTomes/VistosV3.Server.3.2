using Newtonsoft.Json.Linq;






namespace Core.Models.ApiRequest.Params
{
    public class MethodGdprDeleteDataParam : IRequestParam
    {
        public string EntityName { get; set; }
        public int EntityId { get; set; }
        public string[] Columns { get; set; }
    }
}
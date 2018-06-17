using Newtonsoft.Json.Linq;






namespace Core.Models.ApiRequest.Params
{
    public class MethodCallCustomMethodParam : IRequestParam
    {
        public string MethodName { get; set; }
        public string EntityName { get; set; }
        public int? EntityId { get; set; }
        public JObject Data { get; set; }
    }
}
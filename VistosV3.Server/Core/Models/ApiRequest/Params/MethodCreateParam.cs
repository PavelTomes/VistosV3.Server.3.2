using Newtonsoft.Json.Linq;






namespace Core.Models.ApiRequest.Params
{
    public class MethodCreateParam : IRequestParam
    {
        public string EntityName { get; set; }
        public JObject Data { get; set; }
    }
}
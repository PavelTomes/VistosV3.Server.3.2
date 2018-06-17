using Newtonsoft.Json.Linq;






namespace Core.Models.ApiRequest.Params
{
    public class MethodSaveParam : IRequestParam
    {
        public string EntityName { get; set; }
        public int EntityId { get; set; }
        public JObject Data { get; set; }
    }
}
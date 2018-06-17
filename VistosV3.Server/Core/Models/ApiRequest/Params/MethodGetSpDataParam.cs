using Newtonsoft.Json.Linq;





namespace Core.Models.ApiRequest.Params
{
    public class MethodGetSpDataParam
    {
        public string SpName { get; set; }
        public JObject SpParameters { get; set; }
    }
}
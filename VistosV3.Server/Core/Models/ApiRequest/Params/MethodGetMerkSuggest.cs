using Newtonsoft.Json.Linq;






namespace Core.Models.ApiRequest.Params
{
    public class MethodGetMerkSuggest : IRequestParam
    {
        public string RegNumber { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }
    }
}
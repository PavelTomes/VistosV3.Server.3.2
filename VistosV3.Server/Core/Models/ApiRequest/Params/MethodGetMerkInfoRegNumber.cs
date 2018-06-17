using Newtonsoft.Json.Linq;






namespace Core.Models.ApiRequest.Params
{
    public class MethodGetMerkInfoRegNumber : IRequestParam
    {
        public string RegNumber { get; set; }
        public string CountryCode { get; set; }
        public bool Advanced { get; set; }
    }
}
using Newtonsoft.Json.Linq;






namespace Core.Models.ApiRequest.Params
{
    public class MethodImportParam : IRequestParam
    {
        public string ProjectionName { get; set; }
        public string PairingColumn { get; set; }
        public string[] Columns { get; set; }
        public JArray Data { get; set; }
    }
}
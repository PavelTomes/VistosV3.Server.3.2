using Newtonsoft.Json.Linq;





namespace Core.Models.ApiRequest.Params
{
    public class MethodSaveAccessRightsParam
    {
        public JArray Data { get; set; }
        public int ForProfil { get; set; }
    }
}
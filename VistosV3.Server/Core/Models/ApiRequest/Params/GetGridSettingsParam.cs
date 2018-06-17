





namespace Core.Models.ApiRequest.Params
{
    public class GetGridSettingsParam : IRequestParam
    {
        public string ProjectionName { get; set; }
        public string GridName { get; set; }
        public GridSettingsType GridSettingsType { get; set; }
    }
}
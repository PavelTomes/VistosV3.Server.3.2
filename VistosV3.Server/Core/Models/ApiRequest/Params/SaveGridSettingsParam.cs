





namespace Core.Models.ApiRequest.Params
{
    public class SaveGridSettingsParam : IRequestParam
    {
        public string ProjectionName { get; set; }
        public string GridName { get; set; }
        public object GridSettings { get; set; }
        public GridSettingsType GridSettingsType { get; set; }
    }
}
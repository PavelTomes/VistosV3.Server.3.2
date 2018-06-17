namespace Core.Models.ApiRequest.Params
{
    public class FilterSettingsParam : IRequestParam
    {
        public string EntityName { get; set; }
        public string FilterName { get; set; }
        public string FilterType { get; set; }
        public object FilterSettings { get; set; }
    }
}
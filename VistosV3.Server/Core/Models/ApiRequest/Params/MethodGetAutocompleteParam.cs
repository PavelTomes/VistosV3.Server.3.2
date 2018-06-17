namespace Core.Models.ApiRequest.Params
{
    public class MethodGetAutocompleteParam : IRequestParam
    {
        public string EntityName { get; set; }
        public string SearchText { get; set; }
        public object Filter { get; set; }
    }
}
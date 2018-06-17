namespace Core.Models.ApiRequest.Params
{

    public class MethodGetEnumerationParam : IRequestParam
    {
        public string EnumerationType { get; set; }
        public int? ParentValue { get; set; }
        public string FilterText { get; set; }
    }
}
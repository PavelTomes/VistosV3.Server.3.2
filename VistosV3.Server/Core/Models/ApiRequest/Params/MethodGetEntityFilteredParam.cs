





namespace Core.Models.ApiRequest.Params
{
    public class MethodGetEntityFilteredParam : IRequestParam
    {
        public string EntityName { get; set; }
        public object Filter { get; set; }
    }
}






namespace Core.Models.ApiRequest.Params
{

    public class SaveLayoutParam : IRequestParam
    {
        public string EntityName { get; set; }
        public object Layout { get; set; }
        public string Mode { get; set; }
    }
}
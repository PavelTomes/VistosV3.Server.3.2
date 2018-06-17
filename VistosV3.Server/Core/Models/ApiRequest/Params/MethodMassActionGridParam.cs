





namespace Core.Models.ApiRequest.Params
{
    public class MethodMassActionGridParam : IRequestParam
    {
        public string EntityName { get; set; }
        public string ActionName { get; set; }
        public object EntityId { get; set; }
        public int? Value { get; set; }
    }
}
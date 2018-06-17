





namespace Core.Models.ApiRequest.Params
{
    public class MethodRemoveParam : IRequestParam
    {
        public string EntityName { get; set; }
        public int EntityId { get; set; }
        public string ParentEntityName { get; set; }
        public int? ParentEntityId { get; set; }
        public string GridMode { get; set; }
    }
}
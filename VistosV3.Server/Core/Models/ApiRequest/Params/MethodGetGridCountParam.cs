namespace Core.Models.ApiRequest.Params
{
    public class MethodGetGridCountParam : IRequestParam
    {
        public string EntityName { get; set; }
        public string ParentEntityName { get; set; }
        public int? ParentEntityId { get; set; }
        public string ProjectionRelationName { get; set; }
    }
}
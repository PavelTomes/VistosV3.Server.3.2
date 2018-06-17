namespace Core.Models.ApiRequest.Params
{
    public class MethodGetGridIdsParam : IRequestParam
    {
        public string EntityName { get; set; }
        public object Filter { get; set; }
        public string ParentEntityName { get; set; }
        public int? ParentEntityId { get; set; }
        public string ProjectionRelationName { get; set; }
        public bool IgnoreVisibleOnFilter { get; set; }
    }

}
namespace Core.Models.ApiRequest.Params
{
    public class MethodGetByIdParam : IRequestParam
    {
        // TODO: EntityName refaktorovat na ProjectionName (Spučasně i na straně klienta!!!)
        public string EntityName { get; set; }
        public int EntityId { get; set; }
    }
}
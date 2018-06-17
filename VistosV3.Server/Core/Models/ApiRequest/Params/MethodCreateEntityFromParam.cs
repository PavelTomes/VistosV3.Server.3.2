using Newtonsoft.Json.Linq;

namespace Core.Models.ApiRequest.Params
{
    public class MethodCreateEntityFromParam : IRequestParam
    {
        public string EntityNameFrom { get; set; }
        public string EntityNameTarget { get; set; }
        public int EntityIdFrom { get; set; }
        public ProjectionActionType ActionTypeName { get; set; }
        public ProjectionMethodMode MethodMode { get; set; }
        public ProjectionActionResultType ResultType { get; set; }
        public JObject ExtData { get; set; }
    }
}
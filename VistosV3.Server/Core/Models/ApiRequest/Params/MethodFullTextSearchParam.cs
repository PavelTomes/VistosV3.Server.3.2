using Newtonsoft.Json.Linq;






namespace Core.Models.ApiRequest.Params
{

    public class MethodFullTextSearchParam : IRequestParam
    {
        public string Text { get; set; }
        public bool IncludeDiscussionMessage { get; set; }
        public JArray DbObjectIdArrayJson { get; set; }

    }
}

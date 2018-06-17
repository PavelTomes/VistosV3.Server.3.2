




namespace Core.Models.ApiRequest.Params
{
    public class MethodEditDiscussionMessageParam
    {
        public string HierarchyID { get; set; }
        public int EntityFk { get; set; }
        public string EntityName { get; set; }
        public string Text { get; set; }
        public bool IsSystemMessage { get; set; }
    }
}






namespace Core.Models.ApiRequest.Params
{
    public class SortOrderParam : IRequestParam
    {
        public string EntityName { get; set; }
        public string ColumnName { get; set; }
        public string Direction { get; set; }
    }

}
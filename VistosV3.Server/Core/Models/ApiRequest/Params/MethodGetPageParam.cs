using System.Collections.Generic;

namespace Core.Models.ApiRequest.Params
{
    public class MethodGetPageParam : IRequestParam
    {
        public string EntityName { get; set; }
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public List<SortOrderParam> SortOrder { get; set; }
        public object Filter { get; set; }
        public string Fulltext { get; set; }
        public string ParentEntityName { get; set; }
        public int? ParentEntityId { get; set; }
        public string ProjectionRelationName { get; set; }
        public Column[] Columns { get; set; }
        public string GridMode { get; set; }
    }
}
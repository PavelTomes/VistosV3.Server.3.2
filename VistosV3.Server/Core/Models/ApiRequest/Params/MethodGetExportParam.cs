using System.Collections.Generic;

namespace Core.Models.ApiRequest.Params
{
    public class MethodGetExportParam : MethodGetPageParam
    {
        public string ExportType { get; set; }
        public List<Column> Schema { get; set; }
    }
}
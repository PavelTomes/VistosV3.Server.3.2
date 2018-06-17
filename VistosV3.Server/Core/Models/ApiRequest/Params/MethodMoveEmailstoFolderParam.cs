using Newtonsoft.Json.Linq;





namespace Core.Models.ApiRequest.Params
{
    public class MethodMoveEmailsToFolderParam
    {
        public int FolderId { get; set; }
        public int[] EmailIds { get; set; }
    }
}

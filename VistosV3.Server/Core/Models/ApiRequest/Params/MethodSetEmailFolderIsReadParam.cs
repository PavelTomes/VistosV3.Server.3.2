





namespace Core.Models.ApiRequest.Params
{
    public class MethodSetEmailFolderIsReadParam : IRequestParam
    {
        public int FolderId { get; set; }
        public bool IsRead { get; set; }
    }
}
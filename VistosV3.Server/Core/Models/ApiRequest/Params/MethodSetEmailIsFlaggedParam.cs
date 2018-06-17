





namespace Core.Models.ApiRequest.Params
{
    public class MethodSetEmailIsFlaggedParam : IRequestParam
    {
        public int EmailId { get; set; }
        public bool IsFlagged { get; set; }
    }
}
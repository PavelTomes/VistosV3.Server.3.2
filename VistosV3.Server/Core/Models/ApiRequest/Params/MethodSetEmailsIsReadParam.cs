





namespace Core.Models.ApiRequest.Params
{
    public class MethodSetEmailsIsReadParam : IRequestParam
    {
        public int[] EmailsId { get; set; }
        public bool IsRead { get; set; }
    }
}
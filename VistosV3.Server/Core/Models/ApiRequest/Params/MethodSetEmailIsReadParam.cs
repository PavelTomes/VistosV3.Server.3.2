





namespace Core.Models.ApiRequest.Params
{
    public class MethodSetEmailIsReadParam : IRequestParam
    {
        public int EmailId { get; set; }
        public bool IsRead { get; set; }
    }
}
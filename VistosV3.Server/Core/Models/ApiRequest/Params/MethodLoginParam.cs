





namespace Core.Models.ApiRequest.Params
{
    public class MethodLoginParam : IRequestParam
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
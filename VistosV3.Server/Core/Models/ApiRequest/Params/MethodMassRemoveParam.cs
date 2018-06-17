





namespace Core.Models.ApiRequest.Params
{
    public class MethodMassRemoveParam : IRequestParam
    {
        public string EntityName { get; set; }
        public int[] EntityIds { get; set; }
    }
}
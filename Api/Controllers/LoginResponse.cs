namespace Goc.Api.Controllers
{
    public class LoginResponse
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
        
        public bool IsAuthenticated { get; set; }

        public string Message { get; set; }
    }
}
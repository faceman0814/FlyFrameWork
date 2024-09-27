namespace FlyFramework.WebHost.Models
{
    public class LoginDto
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsApiLogin { get; set; }
    }
}

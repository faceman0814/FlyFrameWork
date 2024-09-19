using FlyFramework.Domain.Entities;

namespace FlyFramework.Application.UserService.Dtos
{
    public class UserDto : Entity<string>
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsActive { get; set; }
    }
}

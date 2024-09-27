using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Application.UserService.Dtos
{
    public class UserLoginDto
    {
        public string Avatar { get; set; }
        public string UserName { get; set; }
        public string NickName { get; set; }
        public List<string> Roles { get; set; }
        public List<string> Permissions { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTimeOffset Expires { get; set; }
    }
}

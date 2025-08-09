using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.UserModule.Authorization
{
    public class UserAuthority
    {
        public const string User = "User";
        public const string User_Node = "User.Node";
        public const string User_Create = User_Node + ".Create";
        public const string User_Update = User_Node + ".Update";
        public const string User_Delete = User_Node + ".Delete";

        public const string User_Node_Name = "用户节点";
        public const string User_Create_Name = "创建用户";
        public const string User_Update_Name = "修改用户";
        public const string User_Delete_Name = "删除用户";
    }
}

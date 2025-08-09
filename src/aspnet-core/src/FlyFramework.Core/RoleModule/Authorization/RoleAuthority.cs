using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.RoleModule.Authorization
{
    public class RoleAuthority
    {
        public const string Role = "Role";
        public const string Role_Node = "Role.Node";
        public const string Role_Create = Role_Node + ".Create";
        public const string Role_Update = Role_Node + ".Update";
        public const string Role_Delete = Role_Node + ".Delete";

        public const string Role_Node_Name = "角色节点";
        public const string Role_Create_Name = "创建角色";
        public const string Role_Update_Name = "修改角色";
        public const string Role_Delete_Name = "删除角色";
    }
}

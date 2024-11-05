using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.UserModule.Authority
{
    public static class UserAuthority
    {
        private static string User = "User.";
        public static string User_Node = User + "Node";
        public static string User_Create = User_Node + ".Create";
        public static string User_Update = User_Node + ".Update";
        public static string User_Delete = User_Node + ".Delete";
    }
}

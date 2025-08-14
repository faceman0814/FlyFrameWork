using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.OrgUnitModule.Authorization
{
    public class OrgUnitPermissions
    {
        public const string OrgUnitManager = "OrgUnitManager";
        public const string OrgUnit_Node = "OrgUnit.Node";
        public const string OrgUnit_Create = OrgUnit_Node + ".Create";
        public const string OrgUnit_Update = OrgUnit_Node + ".Update";
        public const string OrgUnit_Delete = OrgUnit_Node + ".Delete";

        public const string OrgUnitManager_Name = "组织单元管理";
        public const string OrgUnit_Node_Name = "组织单元节点";
        public const string OrgUnit_Create_Name = "创建组织单元";
        public const string OrgUnit_Update_Name = "修改组织单元";
        public const string OrgUnit_Delete_Name = "删除组织单元";
    }
}

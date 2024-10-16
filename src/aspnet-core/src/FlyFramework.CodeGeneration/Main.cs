using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlyFramework
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Main_Load(object sender, EventArgs e)
        {

        }

        private void CodeGeneration(object sender, EventArgs e)
        {
            //获取项目路径
            var projectPath = this.projectPath.Text;
            //获取实体名称
            var entityName = this.entityName.Text;
            //获取实体描述
            var entityDescription = this.entityDescription.Text;
            //获取表格数据
            int rowCount = this.entityTable.Rows.Count - 1; // 减去末尾的空行
            List<Entity> entityList = new List<Entity>();
            for (int i = 0; i < rowCount; i++)
            {
                var entity = new Entity();
                entity.Name = this.entityTable.Rows[i].Cells[0].Value.ToString();
                entity.Name = char.ToUpper(entity.Name[0]) + entity.Name.Substring(1);
                entity.Type = this.entityTable.Rows[i].Cells[1].Value.ToString();
                entity.Length = int.Parse(this.entityTable.Rows[i].Cells[2].Value.ToString());
                entity.Description = this.entityTable.Rows[i].Cells[3].Value.ToString();
                entityList.Add(entity);
            }
            //生成实体类代码
            CodeGenerator.GenerateEntityClass(projectPath, entityName, entityList, entityDescription);
        }
    }
}

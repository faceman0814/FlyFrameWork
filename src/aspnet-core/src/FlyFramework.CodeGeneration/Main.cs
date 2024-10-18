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
            //获取实体名称
            var entityName = char.ToUpper(this.entityName.Text[0]) + this.entityName.Text.Substring(1);
            //获取项目路径
            var projectPath = this.projectPath.Text;
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
            entityList.Add(new Entity() { Name = "TenantId", Type = "string", Description = "租户Id" });

            //生成Core层代码
            var corePath = projectPath + $"\\src\\aspnet-core\\src\\FlyFrameWork.Core\\{entityName}Module";
            // 如果目录不存在，则创建目录
            if (!Directory.Exists(corePath))
            {
                Directory.CreateDirectory(corePath);
            }
            //生成实体类代码
            CodeGenerator.GenerateEntityClass(corePath, entityName, entityList, entityDescription);
            //生成管理器接口类代码
            CodeGenerator.GenerateIManagerClass(corePath + "\\DomainService", entityName);
            //生成管理器类代码
            CodeGenerator.GenerateManagerClass(corePath + "\\DomainService", entityName);

            //生成Application层代码
            var applicationPath = projectPath + $"\\src\\aspnet-core\\src\\FlyFrameWork.Application\\{entityName}Module";

            // 如果目录不存在，则创建目录
            if (!Directory.Exists(applicationPath))
            {
                Directory.CreateDirectory(applicationPath);
            }
            //生成服务接口类代码
            CodeGenerator.GenerateIAppServiceClass(applicationPath, entityName);
            //生成服务类代码
            CodeGenerator.GenerateAppServiceClass(applicationPath, entityName);

            var dtoPath = applicationPath + $"\\Dtos";
            // 如果目录不存在，则创建目录
            if (!Directory.Exists(dtoPath))
            {
                Directory.CreateDirectory(dtoPath);
            }
            //创建Dto类代码
            CodeGenerator.GenerateDtoClass(dtoPath, entityName);
            var mapperPath = applicationPath + $"\\Mappers";
            // 如果目录不存在，则创建目录
            if (!Directory.Exists(mapperPath))
            {
                Directory.CreateDirectory(mapperPath);
            }
            //创建Mapper类代码
            CodeGenerator.GenerateMapperClass(mapperPath, entityName);
        }
    }
}

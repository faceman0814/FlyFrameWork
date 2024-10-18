using Microsoft.CSharp;

using System.CodeDom;
using System.CodeDom.Compiler;
using System.Text.RegularExpressions;

namespace FlyFramework
{
    public class CodeGenerator
    {
        /// <summary>
        /// 根据字段生成实体类代码
        /// </summary>
        /// <param name="outputPath"> 输出路径 </param>
        /// <param name="entityName"> 实体类名称 </param>
        /// <param name="fields"> 字段字典 </param>
        public static void GenerateEntityClass(string outputPath, string entityName, List<Entity> fields, string description = null)
        {
            // 如果目录不存在，则创建目录
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            // 创建一个新的CodeCompileUnit来存放代码模型
            CodeCompileUnit compileUnit = new CodeCompileUnit();

            // 创建命名空间
            CodeNamespace myNamespace = new CodeNamespace($"FlyFramework.{entityName}Module");
            compileUnit.Namespaces.Add(myNamespace);

            // 创建类
            CodeTypeDeclaration myClass = new CodeTypeDeclaration(entityName)
            {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.Public
            };
            myNamespace.Types.Add(myClass);

            myClass.BaseTypes.Add($"FullAuditedEntity<string>");
            myClass.BaseTypes.Add($"IMustHaveTenant");

            // 添加类注释
            myClass.Comments.Add(new CodeCommentStatement("<summary>", true));
            myClass.Comments.Add(new CodeCommentStatement(description, true));
            myClass.Comments.Add(new CodeCommentStatement("</summary>", true));

            // 为每个字段添加属性
            foreach (var field in fields)
            {
                // 使用CodeSnippetTypeMember直接插入自动属性代码，包括描述
                string propertyCode = $@"        /// <summary>
        /// {field.Description}
        /// </summary>
        {(field.Length != null ? $"[MaxLength({field.Length})]" : "")}
        public {field.Type} {field.Name} {{ get; set; }}
";
                CodeSnippetTypeMember snippet = new CodeSnippetTypeMember(propertyCode);

                // 将自动属性添加到类中
                myClass.Members.Add(snippet);
            }

            // 使用CSharpCodeProvider生成代码
            CSharpCodeProvider provider = new CSharpCodeProvider();
            using (StreamWriter sw = new StreamWriter(Path.Combine(outputPath, $"{entityName}.cs"), false))
            {
                // 手动编写 'using' 指令
                sw.WriteLine("using System;");
                sw.WriteLine("using System.ComponentModel.DataAnnotations;");
                sw.WriteLine("using FlyFramework.Entities;");
                sw.WriteLine();

                // 生成命名空间和类的代码
                provider.GenerateCodeFromCompileUnit(compileUnit, sw, new CodeGeneratorOptions() { BracingStyle = "C" });
            }
        }

        /// <summary>
        /// 创建领域服务接口类
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="entityName"></param>
        /// <param name="fields"></param>
        public static void GenerateIManagerClass(string outputPath, string entityName)
        {
            //创建DomainService文件夹
            outputPath = Path.Combine(outputPath, "DomainService");

            // 如果目录不存在，则创建目录
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }
            //#region 创建IManager接口
            //// 创建一个新的CodeCompileUnit来存放代码模型
            //CodeCompileUnit iManager = new CodeCompileUnit();

            //// 创建命名空间
            //CodeNamespace iManagerNamespace = new CodeNamespace($"FlyFramework.{entityName}Module.DomainService");
            //iManager.Namespaces.Add(iManagerNamespace);

            //// 创建接口
            //CodeTypeDeclaration myInterface = new CodeTypeDeclaration("I" + entityName + "Manager")
            //{
            //    IsInterface = true,
            //    TypeAttributes = System.Reflection.TypeAttributes.Public
            //};
            //iManagerNamespace.Types.Add(myInterface);

            //// 添加基类和接口
            //myInterface.BaseTypes.Add($"IGuidDomainService<{entityName}>");
            //#endregion

            //// 使用CSharpCodeProvider生成代码
            //CSharpCodeProvider provider = new CSharpCodeProvider();
            //using (StreamWriter sw = new StreamWriter(Path.Combine(outputPath, $"I{entityName}Manager.cs"), false))
            //{
            //    // 手动编写 'using' 指令
            //    sw.WriteLine("using FlyFramework.Domains;");
            //    sw.WriteLine();

            //    // 生成命名空间和类的代码
            //    provider.GenerateCodeFromCompileUnit(iManager, sw, new CodeGeneratorOptions() { BracingStyle = "C" });

            //    // Debug output
            //    Console.WriteLine($"Is Interface: {myInterface.IsInterface}");
            //}

            // 接口文件完整路径
            string interfaceFilePath = Path.Combine(outputPath, $"I{entityName}Manager.cs");

            // 打开文件准备写入
            using (StreamWriter sw = new StreamWriter(interfaceFilePath, false))
            {
                // 写入 'using' 指令
                sw.WriteLine("using FlyFramework.Domains;");
                sw.WriteLine();

                // 写入命名空间
                sw.WriteLine($"namespace FlyFramework.{entityName}Module.DomainService");
                sw.WriteLine("{");

                // 写入接口定义
                sw.WriteLine($"    public interface I{entityName}Manager : IGuidDomainService<{entityName}>");
                sw.WriteLine("    {");
                // 在这里可以添加更多接口方法定义
                sw.WriteLine("    }");

                sw.WriteLine("}");
            }
        }

        /// <summary>
        /// 创建领域服务类
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="entityName"></param>
        public static void GenerateManagerClass(string outputPath, string entityName)
        {
            //创建DomainService文件夹
            outputPath = Path.Combine(outputPath, "DomainService");

            // 如果目录不存在，则创建目录
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            #region 创建Manager类
            // 创建一个新的CodeCompileUnit来存放代码模型
            CodeCompileUnit manager = new CodeCompileUnit();

            // 创建命名空间
            CodeNamespace managerNamespace = new CodeNamespace($"FlyFramework.{entityName}Module.DomainService");
            manager.Namespaces.Add(managerNamespace);

            // 创建类
            CodeTypeDeclaration myClass = new CodeTypeDeclaration(entityName + "Manager")
            {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.Public
            };
            managerNamespace.Types.Add(myClass);

            // 添加基类和接口
            myClass.BaseTypes.Add($"GuidDomainService<{entityName}>");
            myClass.BaseTypes.Add($"I{entityName}Manager");

            // 添加构造函数
            CodeConstructor constructor = new CodeConstructor
            {
                Attributes = MemberAttributes.Public
            };
            constructor.Parameters.Add(new CodeParameterDeclarationExpression("IServiceProvider", "serviceProvider"));
            constructor.BaseConstructorArgs.Add(new CodeVariableReferenceExpression("serviceProvider"));
            myClass.Members.Add(constructor);

            // 添加 GetIncludeQuery 方法
            CodeMemberMethod getIncludeQuery = new CodeMemberMethod
            {
                Name = "GetIncludeQuery",
                ReturnType = new CodeTypeReference($"IQueryable<{entityName}>"),
                Attributes = MemberAttributes.Public | MemberAttributes.Override
            };
            getIncludeQuery.Statements.Add(new CodeThrowExceptionStatement(new CodeObjectCreateExpression("NotImplementedException")));
            myClass.Members.Add(getIncludeQuery);

            // 添加 ValidateOnCreateOrUpdate 方法
            CodeMemberMethod validateOnCreateOrUpdate = new CodeMemberMethod
            {
                Name = "ValidateOnCreateOrUpdate",
                ReturnType = new CodeTypeReference("Task"),
                Attributes = MemberAttributes.Public | MemberAttributes.Override
            };
            validateOnCreateOrUpdate.Parameters.Add(new CodeParameterDeclarationExpression(entityName, "entity"));
            validateOnCreateOrUpdate.Statements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("Task"), "CompletedTask")));
            myClass.Members.Add(validateOnCreateOrUpdate);

            // 添加 ValidateOnDelete 方法
            CodeMemberMethod validateOnDelete = new CodeMemberMethod
            {
                Name = "ValidateOnDelete",
                ReturnType = new CodeTypeReference("Task"),
                Attributes = MemberAttributes.Public | MemberAttributes.Override
            };
            validateOnDelete.Parameters.Add(new CodeParameterDeclarationExpression(entityName, "entity"));
            validateOnDelete.Statements.Add(new CodeMethodReturnStatement(new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("Task"), "CompletedTask")));
            myClass.Members.Add(validateOnDelete);

            #endregion

            // 使用CSharpCodeProvider生成代码
            CSharpCodeProvider provider = new CSharpCodeProvider();
            using (StreamWriter sw = new StreamWriter(Path.Combine(outputPath, $"{entityName}Manager.cs"), false))
            {
                // 手动编写 'using' 指令
                sw.WriteLine("using FlyFramework.Domains;");
                sw.WriteLine("using Microsoft.EntityFrameworkCore;");
                sw.WriteLine("using System;");
                sw.WriteLine("using System.Linq;");
                sw.WriteLine("using System.Threading.Tasks;");
                sw.WriteLine();

                // 生成命名空间和类的代码
                provider.GenerateCodeFromCompileUnit(manager, sw, new CodeGeneratorOptions() { BracingStyle = "C" });
            }
        }
    }
}

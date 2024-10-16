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
            // 实体名称首字母大写
            entityName = char.ToUpper(entityName[0]) + entityName.Substring(1);

            // 创建输出目录
            outputPath = Path.Combine(outputPath, $"{entityName}Module");

            // 如果目录不存在，则创建目录
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

            // 创建一个新的CodeCompileUnit来存放代码模型
            CodeCompileUnit compileUnit = new CodeCompileUnit();

            // 创建命名空间
            CodeNamespace myNamespace = new CodeNamespace($"FlyFramework.{entityName}");
            compileUnit.Namespaces.Add(myNamespace);

            // 创建类
            CodeTypeDeclaration myClass = new CodeTypeDeclaration(entityName)
            {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.Public
            };
            myNamespace.Types.Add(myClass);

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
        [MaxLength({field.Length})]
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
                sw.WriteLine();

                // 生成命名空间和类的代码
                provider.GenerateCodeFromCompileUnit(compileUnit, sw, new CodeGeneratorOptions() { BracingStyle = "C" });
            }
        }
    }
}

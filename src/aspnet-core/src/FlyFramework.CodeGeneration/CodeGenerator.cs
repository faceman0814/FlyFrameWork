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
        /// 创建管理服务接口类
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="entityName"></param>
        /// <param name="fields"></param>
        public static void GenerateIManagerClass(string outputPath, string entityName)
        {
            // 如果目录不存在，则创建目录
            if (!Directory.Exists(outputPath))
            {
                Directory.CreateDirectory(outputPath);
            }

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
        /// 创建管理服务类
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="entityName"></param>
        public static void GenerateManagerClass(string outputPath, string entityName)
        {
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
                sw.WriteLine("using System;");
                sw.WriteLine("using System.Linq;");
                sw.WriteLine("using System.Threading.Tasks;");
                sw.WriteLine();

                // 生成命名空间和类的代码
                provider.GenerateCodeFromCompileUnit(manager, sw, new CodeGeneratorOptions() { BracingStyle = "C" });
            }
        }

        /// <summary>
        /// 生成应用层接口类代码
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="entityName"></param>
        public static void GenerateIAppServiceClass(string outputPath, string entityName)
        {
            #region 创建IAppService接口
            // 接口文件完整路径
            string interfaceFilePath = Path.Combine(outputPath, $"I{entityName}AppService.cs");

            // 打开文件准备写入
            using (StreamWriter sw = new StreamWriter(interfaceFilePath, false))
            {
                // 写入 'using' 指令
                sw.WriteLine("using FlyFramework.ApplicationServices;");
                sw.WriteLine();

                // 写入命名空间
                sw.WriteLine($"namespace FlyFramework.{entityName}Module");
                sw.WriteLine("{");

                // 写入接口定义
                sw.WriteLine($"    public interface I{entityName}AppService : IApplicationService");
                sw.WriteLine("    {");
                // 在这里可以添加更多接口方法定义
                sw.WriteLine("    }");

                sw.WriteLine("}");
            }

            #endregion
        }

        /// <summary>
        /// 生成应用层类代码
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="entityName"></param>
        public static void GenerateAppServiceClass(string outputPath, string entityName)
        {
            // AppService文件完整路径
            string appServiceFilePath = Path.Combine(outputPath, $"{entityName}AppService.cs");

            var lowerEntityName = char.ToLower(entityName[0]) + entityName.Substring(1);
            // 打开文件准备写入
            using (StreamWriter sw = new StreamWriter(appServiceFilePath, false))
            {
                // 写入 'using' 指令
                sw.WriteLine("using FlyFramework.ApplicationServices;");
                sw.WriteLine("using FlyFramework.Extentions.Object;");
                sw.WriteLine("using FlyFramework.LazyModule.LazyDefinition;");
                sw.WriteLine($"using FlyFramework.{entityName}Module.DomainService;");
                sw.WriteLine($"using FlyFramework.{entityName}Module.Dtos;");
                sw.WriteLine($"using System.Threading.Tasks;");
                sw.WriteLine();

                // 开始命名空间和类定义
                sw.WriteLine($"namespace FlyFramework.{entityName}Module");
                sw.WriteLine("{");
                sw.WriteLine($"    public class {entityName}AppService : ApplicationService, I{entityName}AppService");
                sw.WriteLine("    {");
                sw.WriteLine($"        private readonly I{entityName}Manager _{lowerEntityName}Manager;");
                sw.WriteLine();
                sw.WriteLine($"        public {entityName}AppService(IFlyFrameworkLazy flyFrameworkLazy)");
                sw.WriteLine("        {");
                sw.WriteLine($"             _{lowerEntityName}Manager = flyFrameworkLazy.LazyGetRequiredService<I{entityName}Manager>().Value;");
                sw.WriteLine("        }");
                sw.WriteLine();

                // 方法 CreateOrUpdate
                sw.WriteLine($"        public async Task CreateOrUpdate(CreateOrUpdate{entityName}Input input)");
                sw.WriteLine("        {");
                sw.WriteLine($"            if (input.{entityName}.Id.HasValue())");
                sw.WriteLine("            {");
                sw.WriteLine($"                await Update(input.{entityName});");
                sw.WriteLine("            }");
                sw.WriteLine("            else");
                sw.WriteLine("            {");
                sw.WriteLine($"                await Create(input.{entityName});");
                sw.WriteLine("            }");
                sw.WriteLine("        }");
                sw.WriteLine();
                sw.WriteLine("        #region 私有方法");
                sw.WriteLine($"        private async Task Create({entityName}EditDto input)");
                sw.WriteLine("        {");
                sw.WriteLine($"            var entity = ObjectMapper.Map<{entityName}>(input);");
                sw.WriteLine($"            await _{lowerEntityName}Manager.Create(entity);");
                sw.WriteLine("        }");
                sw.WriteLine();
                sw.WriteLine($"        private async Task Update({entityName}EditDto input)");
                sw.WriteLine("        {");
                sw.WriteLine($"            var entity = await _{lowerEntityName}Manager.FindById(input.Id);");
                sw.WriteLine($"            entity = ObjectMapper.Map<{entityName}>(input);");
                sw.WriteLine($"            await _{lowerEntityName}Manager.Update(entity);");
                sw.WriteLine("        }");
                sw.WriteLine("        #endregion");
                sw.WriteLine("    }");
                sw.WriteLine("}");
            }
        }

        /// <summary>
        /// 生成Dto类代码
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="entityName"></param>
        /// <param name="fields"></param>
        public static void GenerateDtoClass(string outputPath, string entityName)
        {
            #region 创建CreateOrUpdateEntityInput
            // 创建一个新的CodeCompileUnit来存放代码模型
            CodeCompileUnit createOrUpdateEntityInput = new CodeCompileUnit();
            // 创建命名空间
            CodeNamespace createOrUpdateEntityInputNamespace = new CodeNamespace($"FlyFramework.{entityName}Module.Dtos");
            createOrUpdateEntityInput.Namespaces.Add(createOrUpdateEntityInputNamespace);

            // 创建CreateOrUpdateEntityInput类
            CodeTypeDeclaration createOrUpdateEntityInputClass = new CodeTypeDeclaration("CreateOrUpdate" + entityName + "Input")
            {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.Public
            };
            createOrUpdateEntityInputNamespace.Types.Add(createOrUpdateEntityInputClass);

            string propertyCode = $@"        /// <summary>
        /// {entityName}编辑Dto
        /// </summary>
        [Required]
        public {entityName}EditDto {entityName} {{ get; set; }}";
            CodeSnippetTypeMember snippet = new CodeSnippetTypeMember(propertyCode);

            // 将自动属性添加到类中
            createOrUpdateEntityInputClass.Members.Add(snippet);

            // 使用CSharpCodeProvider生成代码
            CSharpCodeProvider provider = new CSharpCodeProvider();
            using (StreamWriter sw = new StreamWriter(Path.Combine(outputPath, $"CreateOrUpdate{entityName}Input.cs"), false))
            {
                // 手动编写 'using' 指令
                sw.WriteLine("using System.ComponentModel.DataAnnotations;");
                sw.WriteLine();

                // 生成命名空间和类的代码
                provider.GenerateCodeFromCompileUnit(createOrUpdateEntityInput, sw, new CodeGeneratorOptions() { BracingStyle = "C" });
            }
            #endregion

            #region 创建EntityEditDto
            // 创建一个新的CodeCompileUnit来存放代码模型
            CodeCompileUnit entityEditDto = new CodeCompileUnit();
            // 创建命名空间
            CodeNamespace entityEditDtoNamespace = new CodeNamespace($"FlyFramework.{entityName}Module.Dtos");
            entityEditDto.Namespaces.Add(entityEditDtoNamespace);

            // 创建CreateOrUpdateEntityInput类
            CodeTypeDeclaration entityEditDtoClass = new CodeTypeDeclaration(entityName + "EditDto")
            {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.Public
            };
            entityEditDtoNamespace.Types.Add(entityEditDtoClass);

            string entityEditPropertyCodeDto = $@"        /// <summary>
        /// 主键
        /// </summary>
        public string Id {{ get; set; }}";
            CodeSnippetTypeMember entityEditSnippet = new CodeSnippetTypeMember(entityEditPropertyCodeDto);

            // 将自动属性添加到类中
            entityEditDtoClass.Members.Add(entityEditSnippet);

            // 使用CSharpCodeProvider生成代码
            CSharpCodeProvider entityEditProvider = new CSharpCodeProvider();
            using (StreamWriter sw = new StreamWriter(Path.Combine(outputPath, $"{entityName}EditDto.cs"), false))
            {
                // 生成命名空间和类的代码
                entityEditProvider.GenerateCodeFromCompileUnit(entityEditDto, sw, new CodeGeneratorOptions() { BracingStyle = "C" });
            }
            #endregion

            #region 创建EntityForEditOutput
            // 创建一个新的CodeCompileUnit来存放代码模型
            CodeCompileUnit entityForEditOutput = new CodeCompileUnit();
            // 创建命名空间
            CodeNamespace entityForEditOutputNamespace = new CodeNamespace($"FlyFramework.{entityName}Module.Dtos");
            entityForEditOutput.Namespaces.Add(entityForEditOutputNamespace);

            // 创建CreateOrUpdateEntityInput类
            CodeTypeDeclaration entityForEditOutputClass = new CodeTypeDeclaration(entityName + "ForEditOutput")
            {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.Public
            };
            entityForEditOutputNamespace.Types.Add(entityForEditOutputClass);

            // 将自动属性添加到类中
            createOrUpdateEntityInputClass.Members.Add(snippet);
            string entityForEditPropertyCodeOutput = $@"        /// <summary>
        /// {entityName}编辑Dto
        /// </summary>
        [Required]
        public {entityName}EditDto {entityName} {{ get; set; }}";
            CodeSnippetTypeMember entityForEditSnippet = new CodeSnippetTypeMember(entityForEditPropertyCodeOutput);

            // 将自动属性添加到类中
            entityForEditOutputClass.Members.Add(entityForEditSnippet);

            // 使用CSharpCodeProvider生成代码
            CSharpCodeProvider entityForEditProvider = new CSharpCodeProvider();
            using (StreamWriter sw = new StreamWriter(Path.Combine(outputPath, $"{entityName}ForEditOutput.cs"), false))
            {
                // 手动编写 'using' 指令
                sw.WriteLine("using System.ComponentModel.DataAnnotations;");
                sw.WriteLine();

                // 生成命名空间和类的代码
                entityForEditProvider.GenerateCodeFromCompileUnit(entityForEditOutput, sw, new CodeGeneratorOptions() { BracingStyle = "C" });
            }
            #endregion

            #region 创建EntityListDto
            // 创建一个新的CodeCompileUnit来存放代码模型
            CodeCompileUnit entityListDto = new CodeCompileUnit();
            // 创建命名空间
            CodeNamespace entityListDtoNamespace = new CodeNamespace($"FlyFramework.{entityName}Module.Dtos");
            entityListDto.Namespaces.Add(entityListDtoNamespace);

            // 创建EntityListDtoClass类
            CodeTypeDeclaration entityListDtoClass = new CodeTypeDeclaration(entityName + "ListDto")
            {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.Public
            };
            entityListDtoNamespace.Types.Add(entityListDtoClass);

            string entityListPropertyCodeDto = $@"        /// <summary>
        /// 主键
        /// </summary>
        public string Id {{ get; set; }}";
            CodeSnippetTypeMember entityListSnippet = new CodeSnippetTypeMember(entityListPropertyCodeDto);

            // 将自动属性添加到类中
            entityListDtoClass.Members.Add(entityListSnippet);

            // 使用CSharpCodeProvider生成代码
            CSharpCodeProvider entityListProvider = new CSharpCodeProvider();
            using (StreamWriter sw = new StreamWriter(Path.Combine(outputPath, $"{entityName}ListDto.cs"), false))
            {
                // 生成命名空间和类的代码
                entityListProvider.GenerateCodeFromCompileUnit(entityListDto, sw, new CodeGeneratorOptions() { BracingStyle = "C" });
            }
            #endregion
        }

        /// <summary>
        /// 生成映射类代码
        /// </summary>
        /// <param name="outputPath"></param>
        /// <param name="entityName"></param>
        public static void GenerateMapperClass(string outputPath, string entityName)
        {
            #region 创建EntityMapper
            // 创建一个新的CodeCompileUnit来存放代码模型
            CodeCompileUnit entityMapper = new CodeCompileUnit();
            // 创建命名空间
            CodeNamespace entityMapperNamespace = new CodeNamespace($"FlyFramework.{entityName}Module.Mappers");
            entityMapper.Namespaces.Add(entityMapperNamespace);

            // 创建EntityMapper类
            CodeTypeDeclaration entityMapperClass = new CodeTypeDeclaration(entityName + "Mapper")
            {
                IsClass = true,
                TypeAttributes = System.Reflection.TypeAttributes.Public
            };
            entityMapperNamespace.Types.Add(entityMapperClass);
            // 定义方法
            CodeMemberMethod method = new CodeMemberMethod
            {
                Name = "CreateMappings",
                Attributes = MemberAttributes.Public | MemberAttributes.Static
            };
            method.Parameters.Add(new CodeParameterDeclarationExpression("IMapperConfigurationExpression", "configuration"));
            entityMapperClass.Members.Add(method);
            // 添加方法内的代码
            method.Statements.Add(new CodeSnippetExpression(
                $"configuration.CreateMap<List<{entityName}>, List<{entityName}ListDto>>().ReverseMap()"));
            method.Statements.Add(new CodeSnippetExpression(
                $"configuration.CreateMap<{entityName}, {entityName}EditDto>().ReverseMap().IgnoreNullSourceProperties()"));

            // 生成代码文件
            using (StreamWriter sw = new StreamWriter(Path.Combine(outputPath, $"{entityName}Mapper.cs")))
            {
                // 手动编写 'using' 指令
                sw.WriteLine("using AutoMapper;");
                sw.WriteLine($"using FlyFramework.{entityName}Module.Dtos;");
                sw.WriteLine("using System.Collections.Generic;");
                sw.WriteLine();
                CSharpCodeProvider provider = new CSharpCodeProvider();
                provider.GenerateCodeFromCompileUnit(entityMapper, sw, new CodeGeneratorOptions());
            }
            #endregion
        }
    }
}

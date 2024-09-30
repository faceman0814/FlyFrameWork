using FlyFramework.Authorizations.JwtBearer;
using FlyFramework.FlyFrameworkModules;
using FlyFramework.FlyFrameworkModules.Extensions;
using FlyFramework.FlyFrameworkModules.Modules;
using FlyFramework.Identitys;
using FlyFramework.UserModule;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json;

using System;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework
{
    [DependOn(typeof(FlyFrameworkEntityFrameworkCoreModule))]
    public class FlyFrameworkWebCoreModule : FlyFrameworkBaseModule
    {
        public override void Initialize(ServiceConfigerContext context)
        {
            var configuration = context.GetConfiguration();
            var services = context.Services;

            //Identity必须在JWT之前注册
            AddIdentity(services);

            AddJWT(services, configuration);

        }

        public void AddIdentity(IServiceCollection services)
        {
            services.AddIdentity<User, Role>()
        .AddEntityFrameworkStores<FlyFrameworkDbContext>()
         .AddDefaultTokenProviders();

            services.AddIdentityServer()
             .AddAspNetIdentity<User>()
             .AddDeveloperSigningCredential()
             //扩展在每次启动时，为令牌签名创建了一个临时密钥。在生成环境需要一个持久化的密钥
             .AddInMemoryClients(IdentityConfig.GetClients())             //验证方式
             .AddInMemoryApiResources(IdentityConfig.GetApiResources())
             .AddInMemoryIdentityResources(IdentityConfig.GetIdentityResources())     //创建接口返回格式
             .AddInMemoryApiScopes(IdentityConfig.ApiScopes)
             .AddInMemoryPersistedGrants()
             .AddInMemoryCaching()
             .AddTestUsers(IdentityConfig.GetUsers());

        }

        public void AddJWT(IServiceCollection services, IConfiguration configuration)
        {
            //将身份验证服务添加到管道中
            var jwtBearer = configuration.GetSection("JwtBearer").Get<TokenAuthConfiguration>();
            jwtBearer.AccessTokenExpiration = FlyFrameworkConst.AccessTokenExpiration;
            jwtBearer.RefreshTokenExpiration = FlyFrameworkConst.RefreshTokenExpiration;
            jwtBearer.SecurityKey = new SymmetricSecurityKey(
              Encoding.ASCII.GetBytes(configuration["JwtBearer:SecretKey"])
          );
            jwtBearer.SigningCredentials = new SigningCredentials(
                jwtBearer.SecurityKey,
                SecurityAlgorithms.HmacSha256
            );

            // 将 jwtBearer 作为参数传递
            services.AddSingleton(jwtBearer);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "BearerCookie";
                options.ExpireTimeSpan = jwtBearer.AccessTokenExpiration;
                options.SlidingExpiration = false;
                options.LogoutPath = "/Home/Index";
                options.Events = new CookieAuthenticationEvents
                {
                    OnSigningOut = async context =>
                    {
                        context.Response.Cookies.Delete("access-token");
                        await Task.CompletedTask;
                    }
                };
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    //验证Audience
                    ValidateAudience = true,
                    ValidAudience = jwtBearer.Audience,
                    //验证Issuer
                    ValidateIssuer = true,
                    ValidIssuer = jwtBearer.Issuer,
                    //验证签发时间
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromMinutes(5),
                    // 验证签名
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = jwtBearer.SecurityKey,
                };
                options.Events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        var payload = JsonConvert.SerializeObject(new { Code = "401", Message = "很抱歉，您无权访问该接口" });
                        context.Response.ContentType = "application/json";
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.WriteAsync(payload);
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}

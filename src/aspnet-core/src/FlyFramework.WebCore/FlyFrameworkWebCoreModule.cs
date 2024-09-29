using Autofac;

using FlyFramework.Authorizations.JwtBearer;
using FlyFramework.Controllers;
using FlyFramework.DynamicWebAPI;
using FlyFramework.FlyFrameworkModules;
using FlyFramework.FlyFrameworkModules.Extensions;
using FlyFramework.FlyFrameworkModules.Modules;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Newtonsoft.Json;

using System;
using System.Text;
using System.Threading.Tasks;
namespace FlyFramework
{
    [DependOn(
       typeof(FlyFrameworkApplicationModule),
       typeof(FlyFrameworkEntityFrameworkCoreModule)
   )]
    public class FlyFrameworkWebCoreModule : FlyFrameworkBaseModule
    {
        public override void PreInitialize(ServiceConfigerContext context)
        {
            //配置动态webapi
            context.Services.AddMvc(options => { })
                   .AddRazorPagesOptions((options) => { })
                   .AddRazorRuntimeCompilation()
                   .AddDynamicWebApi(context.GetConfiguration());

            //services.AddSingleton<IJWTTokenManager, JWTTokenManager>();
            //context.Services.AddSingleton<IJwtBearerModel, JwtBearerModel>();

            var configuration = context.GetConfiguration();

            //将身份验证服务添加到管道中
            var jwtBearer = configuration.GetSection("JwtBearer").Get<TokenAuthConfiguration>();
            jwtBearer.AccessTokenExpiration = FlyFrameworkConst.AccessTokenExpiration;
            jwtBearer.RefreshTokenExpiration = FlyFrameworkConst.RefreshTokenExpiration;
            jwtBearer.SecurityKey = new SymmetricSecurityKey(
              Encoding.ASCII.GetBytes(configuration["JwtBearer:SecurityKey"])
          );
            jwtBearer.SigningCredentials = new SigningCredentials(
                jwtBearer.SecurityKey,
                SecurityAlgorithms.HmacSha256
            );

            // 将 jwtBearer 作为参数传递
            context.Services.AddSingleton<ITokenAuthConfiguration>(jwtBearer);

            context.Services.Configure<CookiePolicyOptions>(options =>
            {
                // 此设置确保所有cookie都将标记为Secure，只通过HTTPS传输
                options.Secure = CookieSecurePolicy.Always;

                // 设置MinimumSameSitePolicy为None允许跨站请求携带cookie
                options.MinimumSameSitePolicy = SameSiteMode.None;

                // 设置HttpOnly为Always增加保护，防止通过客户端脚本访问cookie
                options.HttpOnly = HttpOnlyPolicy.Always;

            });

            context.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Cookie.Name = "BearerCookie";
                options.ExpireTimeSpan = FlyFrameworkConst.AccessTokenExpiration;
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

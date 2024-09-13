using IdentityModel.Client;

using IdentityServer4.Models;

using Microsoft.AspNetCore.Mvc;

namespace FlyFramework.WebHost.Controllers
{
    [ApiController]
    public class TestAPPController : Controller
    {
        /// <summary>
        /// 控制器API测试11
        /// </summary>
        /// <returns></returns>
        [HttpGet("api/test")]
        public string Test()
        {
            return "Hello World!";
        }
        [HttpGet("api/cilentGetTest")]
        public async Task<string> CilentGetTest()
        {
            #region  模拟客户端请求api接口
            var client = new HttpClient();
            //var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5100/connect/token");  //要请求的验证的地址
            //模拟客户端验证
            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = "http://localhost:5134/connect/token",    //identity自带的获取验证令牌的路由
                ClientId = "client",       //对应server端定义的配置
                ClientSecret = "secret",
                Scope = "api"
            });

            if (tokenResponse.IsError)
            {
                return tokenResponse.Error;
            }

            client.SetBearerToken(tokenResponse.AccessToken);  //要将Token发送到API，通常使用HTTP Authorization标头。 这是使用SetBearerToken扩展方法完成的：

            var response = await client.GetAsync("http://localhost:5134/api/Book/Gethelle");  //需要请求接口/controller的地址
            if (!response.IsSuccessStatusCode)
            {
                return response.StatusCode.ToString();
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            #endregion

        }
    }
}

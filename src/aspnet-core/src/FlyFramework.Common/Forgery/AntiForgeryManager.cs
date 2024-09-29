using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlyFramework.Forgery
{
    public class AntiForgeryManager : IAntiForgeryManager
    {
        protected AntiForgeryOptions Options { get; }

        protected HttpContext HttpContext => _httpContextAccessor.HttpContext!;

        private readonly IAntiforgery _antiforgery;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AntiForgeryManager(
            IAntiforgery antiforgery,
            IHttpContextAccessor httpContextAccessor,
            IOptions<AntiForgeryOptions> options)
        {
            _antiforgery = antiforgery;
            _httpContextAccessor = httpContextAccessor;
            Options = options.Value;
        }

        public virtual void SetCookie()
        {
            HttpContext.Response.Cookies.Append(
                Options.TokenCookie.Name!,
                GenerateToken(),
                Options.TokenCookie.Build(HttpContext)
            );
        }

        public virtual string GenerateToken()
        {
            return _antiforgery.GetAndStoreTokens(_httpContextAccessor.HttpContext!).RequestToken!;
        }
    }
}

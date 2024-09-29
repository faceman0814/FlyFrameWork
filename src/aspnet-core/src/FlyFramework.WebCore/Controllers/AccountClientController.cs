using FlyFramework.Authorizations;
using FlyFramework.Authorizations.JwtBearer;
using FlyFramework.Extentions;
using FlyFramework.Extentions.Object;
using FlyFramework.Models;
using FlyFramework.UserModule;
using FlyFramework.UserModule.DomainService;
using FlyFramework.UserSessions;
using FlyFramework.Utilities.Redis;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FlyFramework.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountClientController : Controller
    {
        readonly IOptions<IdentityOptions> _identityOptions;
        readonly TokenAuthConfiguration _tokenAuthConfiguration;
        readonly ICacheManager _cacheManager;
        readonly UserClaimsPrincipalFactory<User, Role> _claimsPrincipalFactory;
        readonly SignInManager<User> _signInManager;
        readonly IUserManager _userManager;
        readonly ILogInManager _logInManager;

        public AccountClientController(
           UserClaimsPrincipalFactory<User, Role> userClaimsPrincipalFactory,
           SignInManager<User> signInManager,
           IUserManager userManager,
           ILogInManager logInManager,
           IOptions<IdentityOptions> identityOptions,
           ICacheManager cacheManager,
           TokenAuthConfiguration tokenAuthConfiguration)
        {
            _claimsPrincipalFactory = userClaimsPrincipalFactory;
            _signInManager = signInManager;
            _userManager = userManager;
            _logInManager = logInManager;
            _identityOptions = identityOptions;
            _cacheManager = cacheManager;
            _tokenAuthConfiguration = tokenAuthConfiguration;
        }
        [HttpPost]
        public async Task<AuthenticateResultModel> Login(AccountLoginDto input)
        {
            var result = await _signInManager.PasswordSignInAsync(input.UserName, input.Password, false, false);
            if (result.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(input.UserName);
                var claimsIdentity = await _logInManager.LoginAsync(user);
                // 客户端token标识，用于区分是否来源同一浏览器
                var clientTokenTag = Guid.NewGuid().ToString();

                // 创建refresh token
                var refreshTokenClaims = await CreateJwtClaims(
                    claimsIdentity,
                    user,
                    tokenType: TokenType.RefreshToken,
                    clientType: input.ClientType,
                    clientTokenTag: clientTokenTag
                    );
                var refreshToken = CreateRefreshToken(refreshTokenClaims);

                // 创建access token
                var accessTokenClaims = await CreateJwtClaims(
                    claimsIdentity,
                    user,
                    refreshTokenKey: refreshToken.key,
                    clientType: input.ClientType,
                    clientTokenTag: clientTokenTag);
                var accessToken = CreateAccessToken(accessTokenClaims);

                // 第三方登录-登录记录
                //if (!string.IsNullOrEmpty(input.ProviderId))
                //{
                //    var externalAuthProvider = await _externalAuthProviderManager
                //        .FindByIdAsync(input.ProviderId);

                //    await _userManager.AddLoginAsync(
                //        loginResult.User,
                //        new UserLoginInfo(
                //            externalAuthProvider.Id,
                //            input.ProviderCode,
                //            externalAuthProvider.Name
                //            )
                //        );
                //}

                if (input.IsApiLogin)
                {
                    Response.Cookies.Append("access-token", accessToken, new CookieOptions()
                    {
                        Expires = DateTimeOffset.Now.Add(_tokenAuthConfiguration.AccessTokenExpiration)
                    }
                    );
                }
                return new AuthenticateResultModel
                {
                    AccessToken = accessToken,
                    EncryptedAccessToken = GetEncrpyedAccessToken(accessToken),
                    Expires = DateTimeOffset.Now.Add(_tokenAuthConfiguration.AccessTokenExpiration),
                    ExpireInSeconds = (int)_tokenAuthConfiguration.AccessTokenExpiration.TotalSeconds,
                    RefreshToken = refreshToken.token,
                    RefreshTokenExpireInSeconds = (int)_tokenAuthConfiguration.RefreshTokenExpiration.TotalSeconds,
                    UserId = user.Id,
                    ReturnUrl = input.ReturnUrl,
                    Password = input.RememberMe ? SimpleStringCipher.Instance.Encrypt(input.Password) : null,
                    UserName = user.UserName,
                    NickName = user.FullName,
                    Roles = new List<string>()
                     {
                         "admin"
                     },
                    Permissions = new List<string>()
                     {
                         "*:*:*"
                     },
                    Avatar = "https://avatars.githubusercontent.com/u/44761321",
                };
            }
            return null;
        }

        [HttpPost]
        public async Task<AuthenticateResultModel> RefreshToken(string refreshToken)
        {
            var claims = _logInManager.GetClaims(refreshToken);
            var userName = claims.FirstOrDefault(x => x.Type == UserClaimTypes.UserName)?.Value;
            var user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
                return await Login(new AccountLoginDto()
                {
                    IsRefresh = true,
                    UserName = user.UserName,
                    Password = SimpleStringCipher.Instance.Decrypt(user.Password),
                });
            }
            return null;
        }
        #region 创建Token
        /// <summary>
        /// 创建jwt token
        /// </summary>
        /// <param name="claims">认证信息</param>
        /// <param name="expiration">过期时间</param>
        /// <returns></returns>
        string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            return CreateToken(claims, expiration ?? FlyFrameworkConst.AccessTokenExpiration);
        }

        /// <summary>
        /// 创建刷新Token
        /// </summary>
        /// <param name="claims">认证信息</param>
        /// <returns></returns>
        (string token, string key) CreateRefreshToken(IEnumerable<Claim> claims)
        {
            var claimsList = claims.ToList();
            return (CreateToken(claimsList, _tokenAuthConfiguration.RefreshTokenExpiration),
                claimsList.First(c => c.Type == FlyFrameworkConst.TokenValidityKey).Value);
        }

        /// <summary>
        /// 创建Token
        /// </summary>
        /// <param name="claims">认证信息</param>
        /// <param name="expiration">过期时间</param>
        /// <returns></returns>
        string CreateToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            var now = DateTime.Now;

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _tokenAuthConfiguration.Issuer,
                audience: _tokenAuthConfiguration.Audience,
                claims: claims,
                notBefore: now,
                expires: expiration == null ? null : now.Add(expiration.Value),
                signingCredentials: _tokenAuthConfiguration.SigningCredentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }


        /// <summary>
        /// 添加JwtClaims
        /// </summary>
        /// <param name="identity">认证信息</param>
        /// <param name="user">登录的用户</param>
        /// <param name="expiration">过期时间</param>
        /// <param name="tokenType">token类型</param>
        /// <param name="refreshTokenKey">refreshToken秘钥</param>
        /// <param name="clientType">客户端类型</param>
        /// <param name="clientTokenTag">token标记</param>
        /// <returns></returns>
        async Task<IEnumerable<Claim>> CreateJwtClaims(
            ClaimsIdentity identity,
            User user,
            TimeSpan? expiration = null,
            TokenType tokenType = TokenType.AccessToken,
            string refreshTokenKey = null,
            string clientType = null,
            string clientTokenTag = null)
        {
            var tokenValidityKey = Guid.NewGuid().ToString();
            var claims = identity.Claims.ToList();

            var nameIdClaim = claims.First(c => c.Type == _identityOptions.Value.ClaimsIdentity.UserIdClaimType);

            if (!string.IsNullOrWhiteSpace(clientTokenTag))
            {
                claims.Add(new Claim(FlyFrameworkConst.ClientTokenTag, clientTokenTag));
            }

            if (!string.IsNullOrWhiteSpace(clientType))
            {
                claims.Add(new Claim(FlyFrameworkConst.ClientType, clientType));
            }

            if (_identityOptions.Value.ClaimsIdentity.UserIdClaimType != JwtRegisteredClaimNames.Sub)
            {
                claims.Add(new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value));
            }

            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
                new Claim(FlyFrameworkConst.TokenValidityKey, tokenValidityKey),
                new Claim(FlyFrameworkConst.UserIdentifier, user.Id.ToString()),
                new Claim(UserClaimTypes.TenantId, user.TenantId.ToString()),
                new Claim(FlyFrameworkConst.TokenType, tokenType.To<int>().ToString()),
            });

            if (!string.IsNullOrEmpty(refreshTokenKey))
            {
                claims.Add(new Claim(FlyFrameworkConst.RefreshTokenValidityKey, refreshTokenKey));
            }

            if (!expiration.HasValue)
            {
                expiration = tokenType == TokenType.AccessToken
                    ? _tokenAuthConfiguration.AccessTokenExpiration
                    : _tokenAuthConfiguration.RefreshTokenExpiration;
            }

            if (tokenType == TokenType.RefreshToken)
            {
                return claims;
            }
            var expirationDate = DateTime.Now.Add(expiration.Value);

            await _cacheManager.SetCacheAsync<string>($"{user.Id}-{tokenType}", tokenValidityKey, name: FlyFrameworkConst.TokenValidityKey);

            // 创建token验证缓存 滑动过期
            await _cacheManager.SetCacheAsync<string>(tokenValidityKey, "", expirationDate, FlyFrameworkConst.TokenValidityKey);
            return claims;
        }


        /// <summary>
        /// 编码jwt token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        string GetEncrpyedAccessToken(string accessToken)
        {
            return SimpleStringCipher.Instance.Encrypt(accessToken, FlyFrameworkConst.DefaultPassPhrase);
        }

        #endregion
    }
}

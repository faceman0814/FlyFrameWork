using System.ComponentModel.DataAnnotations;

namespace FlyFramework.Models.Options
{
	public class JwtOptions
	{
		[Required]
		[StringLength(512, MinimumLength = 32)]
		public string SecretKey { get; set; } = string.Empty;

		[Required]
		public string Issuer { get; set; } = string.Empty;

		[Required]
		public string Audience { get; set; } = string.Empty;
	}
}


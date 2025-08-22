using System.ComponentModel.DataAnnotations;

namespace FlyFramework.Models.Options
{
	public class MinioOptions
	{
		public bool Enable { get; set; }

		[Required(AllowEmptyStrings = false)]
		public string Endpoint { get; set; } = string.Empty;

		[Required]
		public string AccessKey { get; set; } = string.Empty;

		[Required]
		public string SecretKey { get; set; } = string.Empty;

		[Required]
		public string BucketName { get; set; } = string.Empty;

		public bool Secure { get; set; }
	}
}


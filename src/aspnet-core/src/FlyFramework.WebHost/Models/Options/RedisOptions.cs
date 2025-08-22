using System.ComponentModel.DataAnnotations;

namespace FlyFramework.Models.Options
{
	public class RedisOptions
	{
		public bool Enable { get; set; }

		[Required(AllowEmptyStrings = false)]
		public string Host { get; set; } = string.Empty;

		[Range(1, 65535)]
		public int Port { get; set; } = 6379;

		public string Password { get; set; } = string.Empty;

		[Range(0, 63)]
		public int Db { get; set; }

		public bool SSL { get; set; }

		[Required]
		public string PreName { get; set; } = "FlyFrameWork-";
	}
}


using System.ComponentModel.DataAnnotations;

namespace FlyFramework.Models.Options
{
	public class RabbitMqOptions
	{
		public bool Enable { get; set; }

		[Required]
		public string HostName { get; set; } = string.Empty;

		[Range(1, 65535)]
		public int Port { get; set; } = 5672;

		[Required]
		public string UserName { get; set; } = string.Empty;

		[Required]
		public string Password { get; set; } = string.Empty;
	}
}


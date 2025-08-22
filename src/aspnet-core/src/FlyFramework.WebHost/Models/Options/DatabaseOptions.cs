using System.ComponentModel.DataAnnotations;

namespace FlyFramework.Models.Options
{
	public class DatabaseOptions
	{
		[Required]
		public string DatabaseType { get; set; } = "Postgre";

		[Required]
		public string Default { get; set; } = string.Empty;
	}
}


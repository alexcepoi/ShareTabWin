using System;
using System.Windows.Controls;

namespace ShareTabWin.Helpers
{
	/// <summary>
	/// Validation Rule for a Tcp port. Valid ports are between 1024 and 65536 exclusively.
	/// </summary>
	class PortValidationRule : ValidationRule
	{
		public int MinPort { get; set; }
		public int MaxPort { get; set; }
		public PortValidationRule ()
		{
			MinPort = 1024;
			MaxPort = 65536;
		}
		public override ValidationResult Validate (object value, System.Globalization.CultureInfo cultureInfo)
		{
			int port;

			// Is an integer?
			if (!int.TryParse ((string) value, out port))
			{
				return new ValidationResult (false, "Not an integer value");
			}
			
			// Is in range?
			if (!(port > MinPort && port < MaxPort))
			{
				return new ValidationResult (false, String.Format (
					"Out of range ({0} - {1})",
					MinPort, MaxPort));
			}
			return new ValidationResult (true, null);
		}
	}
}

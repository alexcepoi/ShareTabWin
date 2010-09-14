using System.Windows;

namespace ShareTabWin.Helpers
{
	/// <summary>
	/// Hepler class with DependencyProperties needed for styling
	/// </summary>
	public static class CustomProps
	{
		/// <summary>
		/// Custom CornerRadius property.
		/// </summary>
		public static readonly DependencyProperty MyCornerRadiusProperty = DependencyProperty.RegisterAttached (
			"MyCornerRadius", typeof (CornerRadius), typeof (CustomProps), new UIPropertyMetadata (new CornerRadius (2)));

		public static CornerRadius GetMyCornerRadius (DependencyObject target)
		{
			return (CornerRadius) target.GetValue (MyCornerRadiusProperty);
		}

		public static void SetMyCornerRadius (DependencyObject target, CornerRadius value)
		{
			target.SetValue (MyCornerRadiusProperty, value);
		}
	}
}

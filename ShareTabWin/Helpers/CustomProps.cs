using System.Windows;

namespace ShareTabWin.Helpers
{
	public static class CustomProps
	{
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

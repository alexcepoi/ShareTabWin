using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ShareTabWin.Helpers
{
	public static class ValidationHelpers
	{
		/// <summary>
		/// Inspects the DependencyObject and all its children's validation status. 
		/// If it's invalid, makes it receive the focus.
		/// </summary>
		/// <param name="node">The DependencyObject to be inspected</param>
		/// <returns>True if the element and all its children are valid, false otherwise.</returns>
		public static bool IsValid (this DependencyObject node)
		{
			// Check if dependency object was passed
			if (node != null)
			{
				// Check if dependency object is valid.
				// NOTE: Validation.GetHasError works for controls that have validation rules attached 
				bool isValid = !Validation.GetHasError (node);
				if (!isValid)
				{
					// If the dependency object is invalid, and it can receive the focus,
					// set the focus
					if (node is IInputElement) Keyboard.Focus ((IInputElement) node);
					return false;
				}
			}

			// If this dependency object is valid, check all child dependency objects
			foreach (object subnode in LogicalTreeHelper.GetChildren (node))
			{
				if (subnode is DependencyObject)
				{
					// If a child dependency object is invalid, return false immediately,
					// otherwise keep checking
					if (IsValid ((DependencyObject) subnode) == false) return false;
				}
			}

			// All dependency objects are valid
			return true;
		}
	}
}

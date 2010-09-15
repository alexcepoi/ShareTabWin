using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using System.Diagnostics;
using Skybound.Gecko;
using System.Collections;
using ShareTabWin.Helpers;

namespace ShareTabWin
{
	/// <summary>
	/// A standard browser window exposing normal functionality: navigation commands
	/// and a html renderer, as well as a sketch popup that allows one to doodle on
	/// top of the renderer's content.
	/// </summary>
	public partial class BrowserWindow : AvalonDock.DocumentContent
	{
		/// <summary>
		/// Gets the navigation bar DockPanel.
		/// </summary>
		protected DockPanel NavBar { get { return navBar; } }

		public BrowserWindow()
		{
			InitializeComponent();
			doodleCanvas.Strokes.StrokesChanged += sketch_StrokesChanged;
		}

		/// <summary>
		/// Method that executes whenever the GeckoFx WinForms control gets its handle created.
		/// This needs to be used for initialization purposes instead of the constructor
		/// in the case of WPF interop like here.
		/// </summary>
		protected virtual void renderer_HandleCreated(object sender, EventArgs e) {}

		#region Navigation Commands
		/// <summary>
		/// Triggers a reload on the renderer.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Refresh_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			renderer.Reload();
		}

		/// <summary>
		/// Stops the current navigation being performed by the renderer.
		/// </summary>
		private void Stop_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			renderer.Stop();
			CommandManager.InvalidateRequerySuggested();
		}

		/// <summary>
		/// Indicates whether the Stop command can be executed. Sets 
		/// <code>e.CanExecute</code> to <code>true</code> when the renderer is busy.
		/// </summary>
		private void Stop_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			if (renderer != null)
				e.CanExecute = renderer.IsBusy;
			else
				e.CanExecute = false;
		}

		/// <summary>
		/// Navigates to the homepage.
		/// </summary>
		private void Home_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (renderer != null)
				renderer.Navigate(Tab.HomePage);
		}

		/// <summary>
		/// Triggers a history Back command on the renderer.
		/// </summary>
		private void GoBack_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			renderer.GoBack();
		}

		/// <summary>
		/// Indicates whether the renderer is able to go back in history
		/// by setting <code>e.CanExecute</code> to <code>renderer.CanGoBack</code>
		/// </summary>
		private void GoBack_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			if (renderer != null)
				e.CanExecute = renderer.CanGoBack;
			else
				e.CanExecute = false;
		}

		/// <summary>
		/// Triggers a history Forward command on the renderer
		/// </summary>
		private void GoForward_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			renderer.GoForward();
		}

		/// <summary>
		/// Indicates whether the renderer is able to go forward in 
		/// history by setting <code>e.CanExecute</code> to <code>
		/// renderer.CanGoForward</code>
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void GoForward_CanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			if (renderer != null)
				e.CanExecute = renderer.CanGoForward;
			else
				e.CanExecute = false;
		}

		/// <summary>
		/// Directs the renderer to the address inside the address bar.
		/// </summary>
		private void Go_Click(object sender, RoutedEventArgs e)
		{
			if (renderer != null)
				renderer.Navigate(addressBar.Text);
		}
		#endregion

		#region Renderer integration
		/// <summary>
		/// Handles the KeyDown event on the address bar. Handles the following keys:
		/// <list type="bullet">
		/// <item><term>Enter</term><description>Navigates to the address inside the address bar.</description></item>
		/// <item><term>Escape</term><description>Resets the changes to the address bar and focuses the renderer.</description></item>
		/// </list>
		/// </summary>
		private void addressBar_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.Key)
			{
				case Key.Enter:
					renderer.Navigate((e.Source as ComboBox).Text);
					break;
				case Key.Escape:
					addressBar.Text = renderer.Url.AbsoluteUri;
					renderer.Focus();
					break;
			}
		}

		/// <summary>
		/// Displays the status progress bar when the renderer is navigating.
		/// </summary>
		protected virtual void browser_Navigating(object sender, GeckoNavigatingEventArgs e)
		{
			CommandManager.InvalidateRequerySuggested();
			progress.Visibility = Visibility.Visible;
			progress.Value = 0;
		}

		/// <summary>
		/// Updates the address bar when the renderer has navigated somewhere.
		/// </summary>
		protected virtual void browser_Navigated(object sender, GeckoNavigatedEventArgs e)
		{
			addressBar.Text = e.Uri.AbsoluteUri;
		}

		/// <summary>
		/// Hides the progress bar when there is nothing more to load.
		/// </summary>
		private void browser_DocumentCompleted(object sender, EventArgs e)
		{
			CommandManager.InvalidateRequerySuggested();

			progress.Visibility = Visibility.Hidden;
		}

		/// <summary>
		/// Updates the status bar text when requested by the renderer.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void browser_StatusTextChanged(object sender, EventArgs e)
		{
			if (renderer.StatusText != "" && renderer.StatusText != null)
				status.Content = renderer.StatusText;
			else
				status.Content = "Done";
		}

		/// <summary>
		/// Updates the progress bar whenever progress changes.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void browser_ProgressChanged(object sender, GeckoProgressEventArgs e)
		{
			// TODO: progressbar needs more love
			progress.Value = e.CurrentProgress;
			progress.Maximum = e.MaximumProgress;
		}

		/// <summary>
		/// Updates the availability of the Back and Forward commands whenever needed.
		/// </summary>
		private void browser_RequeryCommands(object sender, EventArgs e)
		{
			CommandManager.InvalidateRequerySuggested();
		}
		#endregion

		/// <summary>
		/// Method that executes whenever the DomMouseMove event occurs inside the renderer
		/// active document. It is the only way to capture MouseMove behaviour over the
		/// GeckoFx control.
		/// </summary>
		protected virtual void renderer_DomMouseMove (object sender, GeckoDomMouseEventArgs e) { }

		/// <summary>
		/// Method that executes whenever the DocumentTitle attribute of the renderer
		/// is updated (like by Javascript or by a page navigation).
		/// </summary>
		protected virtual void renderer_DocumentTitleChanged (object sender, EventArgs e) { }

		/// <summary>
		/// Method that executes whenever the strokes collection in the ink canvas resets.
		/// This resets the event handlers.
		/// </summary>
		protected virtual void sketch_StrokesReplaced (object sender, InkCanvasStrokesReplacedEventArgs e)
		{
			e.PreviousStrokes.StrokesChanged -= sketch_StrokesChanged;
			e.NewStrokes.StrokesChanged += sketch_StrokesChanged;
		}

		/// <summary>
		/// Method that executes whenever the drawing inside the canvas changes.
		/// </summary>
		protected virtual void sketch_StrokesChanged (object sender, System.Windows.Ink.StrokeCollectionChangedEventArgs e)
		{
		}

		/// <summary>
		/// Clears the ink canvas.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void ClearSketch (object sender, ExecutedRoutedEventArgs e)
		{
			doodleCanvas.Strokes.Clear ();
		}

		protected virtual void renderer_DomMouseUp (object sender, GeckoDomMouseEventArgs e) { }

		protected virtual void renderer_DomKeyUp (object sender, GeckoDomKeyEventArgs e) { }
	}
}

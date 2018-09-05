using DPUruNet;
using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FingerPrintWPF
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : ModernWindow
	{
	    public HwndSource hwndSource;
		private const int DEVICE_CHANGE = 0x219;

		public MainWindow()
		{
			InitializeComponent();
		}

		protected override void OnSourceInitialized(EventArgs e)
		{
			base.OnSourceInitialized(e);
			var hwndSource = PresentationSource.FromVisual(this) as HwndSource;
			if (hwndSource != null)
			{
				hwndSource.AddHook(WndProc);
			}
		}

		/// <summary>
		/// WndProc matches the HwndSourceHook delegate signature so it can be passed to AddHook() as a callback. This is the same as overriding a Windows.Form's WncProc method.
		/// </summary>
		/// <param name="hwnd">The window handle</param>
		/// <param name="msg">The message ID</param>
		/// <param name="wParam">The message's wParam value, historically used in the win32 api for handles and integers</param>
		/// <param name="lParam">The message's lParam value, historically used in the win32 api to pass pointers</param>
		/// <param name="handled">A value that indicates whether the message was handled</param>
		/// <returns></returns>
		private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{

			if (msg == DEVICE_CHANGE)
			{

			}

			return IntPtr.Zero;
		}





	}
}

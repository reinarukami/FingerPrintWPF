using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace FingerPrintWPF.Content
{
	/// <summary>
	/// Interaction logic for FaceVerification.xaml
	/// </summary>
	public partial class FaceVerification : UserControl
	{
		public HwndSource hwndSource;
		private const int DEVICE_CHANGE = 0x219;

		public FaceVerification()
		{
			InitializeComponent();
			hwndSource = PresentationSource.FromVisual(this) as HwndSource;
			if (hwndSource != null)
			{
				hwndSource.AddHook(WndProc);
			}
		}

		private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
		{
			if (msg == DEVICE_CHANGE)
			{
				
			}

			return IntPtr.Zero;
		}
	}
}

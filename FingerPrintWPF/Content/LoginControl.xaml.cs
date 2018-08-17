using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FingerPrintWPF.Content
{
	/// <summary>
	/// Interaction logic for LoginControl.xaml
	/// </summary>
	public partial class LoginControl : UserControl
	{
	
		public LoginControl()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			string uname = UserName.Text;
			string pword = Password.Password;

			if(uname == "admin" && pword == "admin")
			{
				ModernDialog.ShowMessage("Username and Password Match", "Success" , MessageBoxButton.OK);

				System.Windows.IInputElement target = FirstFloor.ModernUI.Windows.Navigation.NavigationHelper.FindFrame("_top", this);
				System.Windows.Input.NavigationCommands.GoToPage.Execute("/Content/FingerPrintControl.xaml", target);
			}

			else
			{
				ModernDialog.ShowMessage("Username and Password did not match, Please Try Again", "Failed", MessageBoxButton.OK);
			}

		}
	}
}

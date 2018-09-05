using DPUruNet;
using FirstFloor.ModernUI.Windows;
using FirstFloor.ModernUI.Windows.Controls;
using FirstFloor.ModernUI.Windows.Navigation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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

namespace FingerPrintWPF
{
	/// <summary>
	/// Interaction logic for FingerPrint.xaml
	/// </summary>
	public partial class FingerPrint : UserControl
	{
		private const int PROBABILITY_ONE = 0x7fffffff;
		public Reader currentReader;
		public Fmd FingerPrints;
		public int count;
		public Fmd MainPrint;

		public FingerPrint()
		{
			InitializeComponent();

			//Static find and Set Device
			var _readers = ReaderCollection.GetReaders();
			currentReader = _readers[0];

			FingerPrints = null;
			count = 0;

			//Check Device Reader
			if (!OpenReader())
			{
				Application.Current.Dispatcher.Invoke((Action)delegate
				{
					ModernDialog.ShowMessage("Fingerprint capture device not found please try again", "Device Not Found", MessageBoxButton.OK);
				});

				System.Windows.IInputElement target = FirstFloor.ModernUI.Windows.Navigation.NavigationHelper.FindFrame("_top", this);
				System.Windows.Input.NavigationCommands.GoToPage.Execute("/Content/LoginControl.xaml", target);
			}

			if (!StartCaptureAsync(this.OnCaptured) && OpenReader())
			{
				System.Windows.IInputElement target = FirstFloor.ModernUI.Windows.Navigation.NavigationHelper.FindFrame("_top", this);
				System.Windows.Input.NavigationCommands.GoToPage.Execute("/Content/LoginControl.xaml", target);

			}
		}

		private void OnCaptured(CaptureResult captureResult)
		{
			try
			{
				// Check capture quality and throw an error if bad.
				if (!CheckCaptureResult(captureResult)) return;

				// Gets the base fingerprint for to verify inputted fingerprint
				// string will be replaced of saved xml string of fingerprint
				MainPrint = Fmd.DeserializeXml("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Fid><Bytes>Rk1SACAyMAABHAAz/v8AAAFlAYgAxQDFAQAAAFYqQHIAg3FjQH8AoAhgQDoAYxdeQIYASWdcQLQAoqJbgGMBQodbgGIASBFaQK0AiqtZQIMA1H1ZgCoAbXVYQG0BWI5XQKMA7I1WQMEBJThTQMEA/pFSgLYBC5FRgEgAdRdRgGcA6iNQQE8A1H1PgE8A7YFPQGIBUy9OQJ4AsWJOgFYBUItNQJgAR15NQKoA4Y9NQKQApqNMQJMAeGdMQHIA+IVMQKQBCY1MQJIBEIxHgEsBXI1CQJgBPTNBgFcBYS9BQKUAnlI/QK8BCDU9gNAAvZg7QJgAxqA7QNQBBJU7QI8A4YU4gGMAyX83QJMA3Yk1AJUAzZYxAEsBV5AqAAA=</Bytes><Format>1769473</Format><Version>1.0.0</Version></Fid>");

				foreach (Fid.Fiv fiv in captureResult.Data.Views)
				{
					Application.Current.Dispatcher.Invoke(() =>
					{
						//FingerImage.Source = new BitmapImage(new Uri(@"\myserver\folder1\Customer Data\sample.png"));

						FingerImage.Source = new BitmapImage(new Uri(@"\Assets\Images\blotocol.png"));
					});

				}

				DataResult<Fmd> resultConversion = FeatureExtraction.CreateFmdFromFid(captureResult.Data, Constants.Formats.Fmd.ANSI);
				if (resultConversion.ResultCode != Constants.ResultCode.DP_SUCCESS)
				{
					throw new Exception(resultConversion.ResultCode.ToString());
				}

				FingerPrints = resultConversion.Data;

				CompareResult compareResult = Comparison.Compare(FingerPrints, 0, MainPrint, 0);

				//If compareResult has Errors
				if (compareResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
				{
					throw new Exception(compareResult.ResultCode.ToString());
				}

				//If Fingerprint captured
				else
				{
					Application.Current.Dispatcher.Invoke(() =>
					{

						FingerProgress.Value = 4;
						StatusLabel.Text = "Comparison resulted in a dissimilarity score of " + compareResult.Score.ToString() + (compareResult.Score < (PROBABILITY_ONE / 100000) ? " (fingerprints matched)" : "(fingerprints did not match)");

					});

					if (compareResult.Score == 0)
					{
						
						Application.Current.Dispatcher.Invoke((Action)delegate {
							ModernDialog.ShowMessage("Comparison resulted in a dissimilarity score of " + compareResult.Score.ToString() + (compareResult.Score < (PROBABILITY_ONE / 100000) ? "(fingerprints matched)" : "(fingerprints did not match)"), "Success", MessageBoxButton.OK);

							//Stop Device from capturing fingerprint
							currentReader.Dispose();

							//Go to the next Form
							System.Windows.IInputElement target = FirstFloor.ModernUI.Windows.Navigation.NavigationHelper.FindFrame("_top", this);
							System.Windows.Input.NavigationCommands.GoToPage.Execute("/Content/FaceVerificationControl.xaml", target);

						});


					}
					else
					{
						Application.Current.Dispatcher.Invoke((Action)delegate
						{
							ModernDialog.ShowMessage("Comparison resulted in a dissimilarity score of " + compareResult.Score.ToString() + (compareResult.Score < (PROBABILITY_ONE / 100000) ? "(fingerprints matched)" : "(fingerprints did not match)"), "Failed", MessageBoxButton.OK);
						});
					}
				}

			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}

		public bool CheckCaptureResult(CaptureResult captureResult)
		{
			if (captureResult.Data == null)
			{
				if (captureResult.ResultCode != Constants.ResultCode.DP_SUCCESS)
				{
					throw new Exception(captureResult.ResultCode.ToString());
				}

				// Send message if quality shows fake finger
				if ((captureResult.Quality != Constants.CaptureQuality.DP_QUALITY_CANCELED))
				{
					throw new Exception("Quality - " + captureResult.Quality);
				}
				return false;
			}

			return true;
		}

		public bool OpenReader()
		{
			try
			{
	
				Constants.ResultCode result = Constants.ResultCode.DP_DEVICE_FAILURE;
				// Open reader
				result = currentReader.Open(Constants.CapturePriority.DP_PRIORITY_COOPERATIVE);

				if (result != Constants.ResultCode.DP_SUCCESS)
				{
					MessageBox.Show("Error:  " + result);
					return false;
				}
				return true;
			}
			catch
			{
				return false;
			}

	
		}

		public bool StartCaptureAsync(Reader.CaptureCallback OnCaptured)
		{
			try
			{
				// Activate capture handler
				currentReader.On_Captured += new Reader.CaptureCallback(OnCaptured);

				// Call capture
				if (!CaptureFingerAsync())
				{
					return false;
				}

				return true;
			}
			catch
			{
				return false;
			}

		}

		private bool CaptureFingerAsync()
		{
			try
			{

				Constants.ResultCode captureResult = currentReader.CaptureAsync(Constants.Formats.Fid.ANSI, Constants.CaptureProcessing.DP_IMG_PROC_DEFAULT, currentReader.Capabilities.Resolutions[0]);
				if (captureResult != Constants.ResultCode.DP_SUCCESS)
				{
					throw new Exception("" + captureResult);
				}

				return true;
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error:  " + ex.Message);
				return false;
			}
		}

		public BitmapImage CreateBitmap(byte[] bytes, int width, int height)
		{
			byte[] rgbBytes = new byte[bytes.Length * 3];

			for (int i = 0; i <= bytes.Length - 1; i++)
			{
				rgbBytes[(i * 3)] = bytes[i];
				rgbBytes[(i * 3) + 1] = bytes[i];
				rgbBytes[(i * 3) + 2] = bytes[i];
			}
			Bitmap bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

			BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

			for (int i = 0; i <= bmp.Height - 1; i++)
			{
				IntPtr p = new IntPtr(data.Scan0.ToInt64() + data.Stride * i);
				System.Runtime.InteropServices.Marshal.Copy(rgbBytes, i * bmp.Width * 3, p, bmp.Width * 3);
			}

			bmp.UnlockBits(data);

			using (var memory = new MemoryStream())
			{
				bmp.Save(memory, ImageFormat.Png);
				memory.Position = 0;

				var bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.StreamSource = memory;
				bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
				bitmapImage.EndInit();
				bitmapImage.Freeze();

				return bitmapImage;
			}

		}

	}
}

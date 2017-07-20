using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FtpClient.ViewModel;
using ListBox = System.Windows.Controls.ListBox;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace FtpClient
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private MainViewModel viewModel;

		public MainWindow()
		{
			InitializeComponent();
			viewModel = new MainViewModel();
			DataContext = (MainViewModel)viewModel;
		}

		private void OnItemDoubleClick(object sender, MouseButtonEventArgs e)
		{
			viewModel.MoveToFolder(((ListBox)sender).SelectedValue as string);
		}

		private void OnChangeSavingLocation(object sender, RoutedEventArgs e)
		{
			FolderBrowserDialog dialog = new FolderBrowserDialog();
			var result = dialog.ShowDialog();

			if (result == System.Windows.Forms.DialogResult.OK)
			{
				viewModel.SavingPath = dialog.SelectedPath;
			}
		}
	}
}
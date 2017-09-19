using System;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Net;
using System.IO;
using System.Text;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FtpClient.ViewModel
{
	public class MainViewModel : ViewModelBase
	{
		#region observables

		private string _baseUriValue;

		public string BaseUriValue
		{
			get { return _baseUriValue; }
			set
			{
				_baseUriValue = value;
				ConnectToServerCommand.RaiseCanExecuteChanged();
				RaisePropertyChanged(() => BaseUriValue);
			}
		}

		private string _username;

		public string Username
		{
			get { return _username; }
			set
			{
				_username = value;
				ConnectToServerCommand.RaiseCanExecuteChanged();
				RaisePropertyChanged(() => Username);
			}
		}

		private string _password;

		public string Password
		{
			get { return _password; }
			set
			{
				_password = value;
				ConnectToServerCommand.RaiseCanExecuteChanged();
				RaisePropertyChanged(() => Password);
			}
		}

		private ObservableCollection<string> _itemsToDisplay;

		public ObservableCollection<string> ItemsToDisplay
		{
			get { return _itemsToDisplay; }
			set
			{
				_itemsToDisplay = value;
				RaisePropertyChanged(() => ItemsToDisplay);
			}
		}

		private string _selectedFile;

		public string SelectedFile
		{
			get { return _selectedFile; }
			set
			{
				_selectedFile = value;
				DownloadFileCommand.RaiseCanExecuteChanged();
				RaisePropertyChanged(() => SelectedFile);
			}
		}

		private string _output;

		public string Output
		{
			get { return _output; }
			set
			{
				_output = value;
				RaisePropertyChanged(() => Output);
			}
		}

		private string _savingPath;

		public string SavingPath
		{
			get { return _savingPath; }
			set
			{
				_savingPath = value;
				DownloadFileCommand.RaiseCanExecuteChanged();
				RaisePropertyChanged(() => SavingPath);
			}
		}

		#endregion observables

		private List<string> paths = new List<string>();

		public MainViewModel()
		{
			ConnectToServerCommand = new RelayCommand(ConnectToServerCommandExecute, CanConnectToServerCommandExecute);
			DownloadFileCommand = new RelayCommand(DownloadFileCommandExecute, CanDownloadFileCommandExecute);

			ItemsToDisplay = new ObservableCollection<string>();
			Username = "";
			Password = "";
			BaseUriValue = "";
		}

		#region Commands

		public RelayCommand DownloadFileCommand { get; set; }

		private bool CanDownloadFileCommandExecute()
		{
			return !string.IsNullOrWhiteSpace(SavingPath) && !string.IsNullOrWhiteSpace(SelectedFile);
		}

		private void DownloadFileCommandExecute()
		{
			DownloadFtpFile();
		}

		public RelayCommand ConnectToServerCommand { get; set; }

		private void ConnectToServerCommandExecute()
		{
			ExecuteFtpList(BaseUriValue);
		}

		private bool CanConnectToServerCommandExecute()
		{
			return !string.IsNullOrWhiteSpace(_baseUriValue) && !string.IsNullOrWhiteSpace(_username) &&
				   !string.IsNullOrWhiteSpace(_password);
		}

		#endregion Commands

		private async Task<bool> DownloadFtpFile()
		{
			var serverUri = new Uri(GetSelectedFilePath());
			if (serverUri.Scheme != Uri.UriSchemeFtp)
			{
				return false;
			}

			WebClient request = new WebClient();

			request.Credentials = new NetworkCredential(Username, Password);
			WriteToOutput($"Starting download of {SelectedFile}");
			try
			{
				request.DownloadDataAsync(serverUri);
				//request.DownloadDataCompleted += Request_DownloadDataCompleted;
				request.DownloadProgressChanged += (sender, e) => Request_DownloadProgressChanged(sender, e, HttpUtility.UrlDecode(serverUri.Segments.Last()));
				request.DownloadDataCompleted += (sender, e) => Request_DownloadDataCompleted(sender, e, HttpUtility.UrlDecode(serverUri.Segments.Last()));
			}
			catch (Exception e)
			{
				WriteToOutput(e.Message);
			}
			return true;
		}

		private void Request_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e, string fileName)
		{
			WriteToOutput($"progress of {fileName}: {e.BytesReceived}/{e.TotalBytesToReceive}");
		}

		private void WriteToOutput(string message)
		{
			Output += $"\r\n {message}";
		}

		private void Request_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e, string fileName)
		{
			if (e.Error != null)
			{
				WriteToOutput(e.Error.Message);
				return;
			}

			var newFileData = e.Result;

			using (var fs = new FileStream($@"{SavingPath}\{fileName}", FileMode.Create, FileAccess.Write))
			{
				fs.Write(newFileData, 0, newFileData.Length);
			}

			WriteToOutput("Download done.");
		}

		private string GetSelectedFilePath()
		{
			StringBuilder builder = new StringBuilder(BaseUriValue);
			for (int i = 0; i < paths.Count; i++)
			{
				if (string.Equals(paths[i], SelectedFile))
				{
					continue;
				}

				builder.Append($"/{paths[i]}");
			}

			builder.Append($"/{SelectedFile}");

			return builder.ToString();
		}

		private void ExecuteFtpList(string uri)
		{
			ItemsToDisplay.Clear();
			Uri serverUri = new Uri(uri, UriKind.Absolute);
			if (serverUri.Scheme != Uri.UriSchemeFtp)
			{
				Output = "wrong uri";
				return;
			}

			FtpWebRequest request = (FtpWebRequest)WebRequest.Create(serverUri);
			request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
			request.Credentials = new NetworkCredential(Username, Password);
			WebResponse response;

			try
			{
				response = request.GetResponse();
			}
			catch (Exception e)
			{
				MoveFtpPathBack(2);
				ExecuteFtpList(BuildFtpPath());
				return;
			}
			var s = response.GetResponseStream();

			StreamReader sr = new StreamReader(s, Encoding.UTF8);
			var result = sr.ReadToEnd();
			var items = result.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
			ItemsToDisplay.Add("..");
			foreach (var item in items)
			{
				var split = item.Split('/');
				ItemsToDisplay.Add(split.ElementAt(split.Length - 1));
			}
		}

		public void MoveToFolder(string nextPath)
		{
			if (string.Equals("..", nextPath) && paths.Count >= 1)
			{
				MoveFtpPathBack(1);
			}
			else
			{
				MoveFtpPathForward(nextPath);
			}

			ExecuteFtpList(BuildFtpPath());
		}

		private string BuildFtpPath()
		{
			StringBuilder builder = new StringBuilder(BaseUriValue);

			foreach (var path in paths)
			{
				builder.Append($"/{path}");
			}

			return builder.ToString();
		}

		private void MoveFtpPathBack(int step)
		{
			for (int i = 0; i < step; i++)
			{
				paths.RemoveAt(paths.Count - 1);
			}
		}

		private void MoveFtpPathForward(string nextPath)
		{
			if (paths.Count > 0 && string.Equals(nextPath, paths.ElementAt(paths.Count - 1)))
			{
				return;
			}

			paths.Add(nextPath);

			WriteToOutput(BuildFtpPath());
		}
	}
}
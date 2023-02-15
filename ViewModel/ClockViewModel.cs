using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using VewModelSample.Model;
using VewModelSample.ViewModel;
using VewModelSample.ViewModel.Command;
using MessageBox = System.Windows.MessageBox;
using System.IO;
using System.Windows.Media;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
using VewModelSample.View;

namespace VewModelSample.ViewModel
{
    public class ClockViewModel : INotifyPropertyChanged
    {
        // PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        private Model.ClockModel clockModel = null;
        private ViewModel.ViewLogViewModel viewLog = null;

        public ClockViewModel()
        {
            clockModel = ClockModel.Instance;
            viewLog = ViewLogViewModel.Instance;
        }

        public DateTime Standard
        {
            get { return clockModel.Standard; }
            set { clockModel.Standard = value; OnPropertyChanged("SetTime"); }
        }

        public String BackgroundFilepath
        {
            get { return clockModel.BackgroundFilepath; }
            set { clockModel.BackgroundFilepath = value; OnPropertyChanged("BackgroundFilepath"); }
        }

        public String StandardChangeViewFormat
        {
            get { return clockModel.StandardChangeViewFormat; }
            set { clockModel.StandardChangeViewFormat = value; OnPropertyChanged("StandardChangeViewFormat"); }
        }

        public ICommand ChangeBackground => new RelayCommand<object>(changeBackground, null);
        private void changeBackground(object obj)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JPG files(*.jpg)|*.jpg|JPEG files (*.jpeg)|*.jpeg|PNG files (*.png)|*.png|All files (*.*)|*.*";
            
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (Path.GetExtension(openFileDialog.FileName).ToLower() == ".jpg" || Path.GetExtension(openFileDialog.FileName) == ".jpeg" || Path.GetExtension(openFileDialog.FileName) == ".png")
                {
                    BackgroundFilepath = openFileDialog.FileName;
                    viewLog.AddData("BackgroundChange", Standard.ToString(StandardChangeViewFormat), "배경 변경 -> " + openFileDialog.FileName);
                    MessageBox.Show("배경을 성공적으로 변경하였습니다..", "성공", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("지원하는 파일 형식이 아닙니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }
        public ICommand TCPServer => new RelayCommand<object>(tcpServer, null);
        private void tcpServer(object e) // datagrid
        {
            TCP_Server serverView = new TCP_Server();
            serverView.Show();
        }
        public ICommand TCPClient => new RelayCommand<object>(tcpClient, null);
        private void tcpClient(object e) // datagrid
        {
            TCP_Client clientView = new TCP_Client();
            clientView.Show();
        }

        public static readonly ICommand CloseCommand = new RelayCommand<object>(o => ((Window)o).Close());
    }
}

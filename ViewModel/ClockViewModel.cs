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

            DateFormat = "yyyy'년' M'월' d'일' dddd";
            TimeFormat = "tt h:mm:ss";

            if (StandardChangeIndex == 0)
            {
                StandardChangeView = DateTime.Now.ToString(StandardChangeViewFormat);
            }
            else if (StandardChangeIndex == 1)
            {
                StandardChangeView = DateTime.UtcNow.ToString(StandardChangeViewFormat);
            }
            else if (StandardChangeIndex == 2)
            {
                StandardChangeView = DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).ToString(StandardChangeViewFormat);
            }

            TemporaryDateFormat = DateFormat;
            TemporaryTimeFormat = TimeFormat;

            Thread RefreshThread = new Thread(Refresh);
            RefreshThread.IsBackground = true;
            RefreshThread.Start();
            RefreshThread.Name = nameof(Refresh);
        }
        public String StandardChangeFormat
        {
            get { return clockModel.StandardChangeFormat; }
            set { clockModel.StandardChangeFormat = value; OnPropertyChanged("StandardChangeFormat"); }
        }

        public String TemporaryTimeFormat
        {
            get { return clockModel.TemporaryTimeFormat; }
            set { clockModel.TemporaryTimeFormat = value; OnPropertyChanged("TemporaryTimeText"); }
        }

        public String TemporaryDateFormat
        {
            get { return clockModel.TemporaryDateFormat; }
            set { clockModel.TemporaryDateFormat = value; OnPropertyChanged("TemporaryDateText"); }
        }
        public int TemporaryDateIndex
        {
            get { return clockModel.TemporaryDateIndex; }
            set { 
                
                clockModel.TemporaryDateIndex = value;

                if (clockModel.TemporaryDateIndex == 0)
                {
                    TemporaryDateFormat = "yyyy'년' M'월' d'일' dddd";
                }
                else if (clockModel.TemporaryDateIndex == 1)
                {
                    TemporaryDateFormat = "yyyy'년' M'월' d'일'";
                }
                else if (clockModel.TemporaryDateIndex == 2)
                {
                    TemporaryDateFormat = "yy'년' M'월' d'일' dddd";
                }
                else if (clockModel.TemporaryDateIndex == 3)
                {   
                    TemporaryDateFormat = "yy'년' M'월' d'일'";
                }

                OnPropertyChanged("TemporaryDateIndex");
            }
        }
        public int StandardChangeIndex
        {
            get { return clockModel.StandardChangeIndex; }
            set
            {
                clockModel.StandardChangeIndex = value;

                if (clockModel.StandardChangeIndex == 0)
                {
                    StandardChangeFormat = "KST(UTC+9)";
                    StandardChangeView = DateTime.Now.ToString(StandardChangeViewFormat);
                }
                else if (clockModel.StandardChangeIndex == 1)
                {
                    StandardChangeFormat = "UTC(UTC+0)";
                    StandardChangeView = DateTime.UtcNow.ToString(StandardChangeViewFormat);
                }
                else if (clockModel.StandardChangeIndex == 2)
                {
                    StandardChangeFormat = "PST(UTC-8)";
                    StandardChangeView = DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).ToString(StandardChangeViewFormat);
                }

                OnPropertyChanged("StandardChangeIndex");
            }
        }

        public String StandardChangeView
        {
            get { return clockModel.StandardChangeView; }
            set { clockModel.StandardChangeView = value; OnPropertyChanged("StandardChangeView"); }
        }
        public String StandardChangeViewFormat
        {
            get { return clockModel.StandardChangeViewFormat; }
            set { clockModel.StandardChangeViewFormat = value; OnPropertyChanged("StandardChangeViewFormat"); }
        }
        public String DateFormat
        {
            get { return clockModel.DateFormat; }
            set
            {
                clockModel.DateFormat = value;
                OnPropertyChanged("DateFormat");
            }
        }

        public String TimeFormat
        {
            get { return clockModel.TimeFormat; }
            set
            {
                clockModel.TimeFormat = value;
                OnPropertyChanged("SetTimeFormat");
            }
        }

        public String Kind
        {
            get { return clockModel.Kind; }
            set { clockModel.Kind = value; OnPropertyChanged("Kind"); }
        }

        public String ViewCurrentDate
        {
            get { return clockModel.ViewCurrentDate; } 
            set { clockModel.ViewCurrentDate = value; OnPropertyChanged("ViewCurrentDate"); }
        }

        public String ViewCurrentTime
        {
            get { return clockModel.ViewCurrentTime; }
            set { clockModel.ViewCurrentTime = value; OnPropertyChanged("ViewCurrentTime"); }
        }

        public String ViewCurrentKind
        {
            get { return clockModel.ViewCurrentKind; }
            set { clockModel.ViewCurrentKind = value; OnPropertyChanged("ViewCurrentKind"); }
        }

        public int TimeMode
        {
            get { return clockModel.TimeMode; }
            set { clockModel.TimeMode = value; OnPropertyChanged("TimeMode"); }
        }

        public DateTime Standard
        {
            get { return clockModel.Standard; }
            set { clockModel.Standard = value; OnPropertyChanged("SetTime"); }
        }
        public int SecAngle
        {
            get { return clockModel.SecAngle; }
            set { clockModel.SecAngle = value; OnPropertyChanged("SecAngle"); }
        }
        public int MinAngle
        {
            get { return clockModel.MinAngle; }
            set { clockModel.MinAngle = value; OnPropertyChanged("MinAngle"); }
        }
        public double HourAngle
        {
            get { return clockModel.HourAngle; }
            set { clockModel.HourAngle = value; OnPropertyChanged("HourAngle"); }
        }

        public String ViewTemporaryDate
        {
            get { return clockModel.ViewTemporaryDate; }
            set { clockModel.ViewTemporaryDate = value; OnPropertyChanged("ViewTemporaryDate"); }
        }
        public String ViewTemporaryTime
        {
            get { return clockModel.ViewTemporaryTime; }
            set{ clockModel.ViewTemporaryTime = value; OnPropertyChanged("ViewTemporaryTime"); }
        }
        public String BackgroundFilepath
        {
            get { return clockModel.BackgroundFilepath; }
            set { clockModel.BackgroundFilepath = value; OnPropertyChanged("BackgroundFilepath"); }
        }
        // Refresh Thread
        void Refresh(Object obj)
        {
            while (true)
            {
                if (TimeMode == 0)
                {
                    Standard = DateTime.Now;
                }
                else if (TimeMode == 1)
                {
                    Thread.Sleep(1000);
                    Standard = Standard.AddSeconds(1);
                }
                else if (TimeMode == 2)
                {
                    Standard = DateTime.UtcNow;
                }

                SecAngle = Standard.Second * 6;
                MinAngle = Standard.Minute * 6;
                HourAngle = (Standard.Hour * 30) + (Standard.Minute * 0.5);

                // Clock
                ViewCurrentDate = Standard.ToString(DateFormat);
                ViewCurrentTime = Standard.ToString(TimeFormat);
                ViewCurrentKind = Kind;

                ViewTemporaryDate = Standard.ToString(TemporaryDateFormat);
                ViewTemporaryTime = Standard.ToString(TemporaryTimeFormat);
            }
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

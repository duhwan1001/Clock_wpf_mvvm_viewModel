using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using VewModelSample.Model;
using VewModelSample.View;
using VewModelSample.ViewModel.Command;

namespace VewModelSample.ViewModel
{
    internal class StandardChangeViewModel : INotifyPropertyChanged
    {
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

        public StandardChangeViewModel()
        {
            clockModel = ClockModel.Instance;
            viewLog = ViewLogViewModel.Instance;
            

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

        public DateTime Standard
        {
            get { return clockModel.Standard; }
            set { clockModel.Standard = value; OnPropertyChanged("SetTime"); }
        }

        public String Kind
        {
            get { return clockModel.Kind; }
            set { clockModel.Kind = value; OnPropertyChanged("Kind"); }
        }

        public int TimeMode
        {
            get { return clockModel.TimeMode; }
            set { clockModel.TimeMode = value; OnPropertyChanged("TimeMode"); }
        }

        public String StandardChangeFormat
        {
            get { return clockModel.StandardChangeFormat; }
            set { clockModel.StandardChangeFormat = value; OnPropertyChanged("StandardChangeFormat"); }
        }
        public String BackgroundFilepath
        {
            get { return clockModel.BackgroundFilepath; }
            set { clockModel.BackgroundFilepath = value; OnPropertyChanged("BackgroundFilepath"); }
        }
        // 표준시 변경 확인 버튼
        public ICommand StandardChangeConfirm => new RelayCommand<object>(standardChangeConfirm, null);

        private void standardChangeConfirm(object e)
        {
            String function = "Standard Change";
            String now = Standard.ToString(StandardChangeViewFormat);
            String beforeKind = Kind;

            if (StandardChangeIndex == 0)
            {
                TimeMode = 0;
            }
            else if (StandardChangeIndex == 1)
            {
                TimeMode = 2;
            }
            else if (StandardChangeIndex == 2)
            {
                TimeMode = 1;
                Standard = Standard.Subtract(new TimeSpan(8, 0, 0));
            }

            Kind = StandardChangeFormat;
            String RecordText = beforeKind + "에서 변경한 기준시 : " + Kind;

            viewLog.AddData(function, now, RecordText);

            if (MessageBox.Show("선택한 표준시로 변경하였습니다.", "성공", MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                Application.Current.Windows.OfType<StandardChange>().First().Close();
            }
        }
    }
}

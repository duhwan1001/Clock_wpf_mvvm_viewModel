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
    internal class TimeFormatChangeViewModel : INotifyPropertyChanged
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
        private ViewModel.ViewLogViewModel viewLogViewModel = null;


        public TimeFormatChangeViewModel()
        {
            clockModel = ClockModel.Instance;
            viewLogViewModel = ViewLogViewModel.Instance;

            TemporaryDateFormat = DateFormat;
            TemporaryTimeFormat = TimeFormat;
        }
        public String TemporaryDateFormat
        {
            get
            {
                return clockModel.TemporaryDateFormat;
            }
            set
            {
                clockModel.TemporaryDateFormat = value;
                OnPropertyChanged("TemporaryDateText");
            }
        }

        public String TemporaryTimeFormat
        {
            get
            {
                return clockModel.TemporaryTimeFormat;
            }
            set
            {
                clockModel.TemporaryTimeFormat = value;
                OnPropertyChanged("TemporaryTimeText");
            }
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
        public DateTime Standard
        {
            get { return clockModel.Standard; }
            set { clockModel.Standard = value; OnPropertyChanged("SetTime"); }
        }
        public String StandardChangeViewFormat
        {
            get { return clockModel.StandardChangeViewFormat; }
            set { clockModel.StandardChangeViewFormat = value; OnPropertyChanged("StandardChangeViewFormat"); }
        }
        public String BackgroundFilepath
        {
            get { return clockModel.BackgroundFilepath; }
            set { clockModel.BackgroundFilepath = value; OnPropertyChanged("BackgroundFilepath"); }
        }

        public int TemporaryDateIndex
        {
            get { return clockModel.TemporaryDateIndex; }
            set
            {

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
                ViewTemporaryDate = Standard.ToString(TemporaryDateFormat);
                OnPropertyChanged("TemporaryDateIndex");
            }
        }
        public int TemporaryTimeIndex
        {
            get { return clockModel.TemporaryTimeIndex; }
            set
            {
                clockModel.TemporaryTimeIndex = value;

                if (clockModel.TemporaryTimeIndex == 0)
                {
                    TemporaryTimeFormat = "tt h:mm:ss";
                }
                else if (clockModel.TemporaryTimeIndex == 1)
                {
                    TemporaryTimeFormat = "tt hh:mm:ss";
                }
                else if (clockModel.TemporaryTimeIndex == 2)
                {
                    TemporaryTimeFormat = "H:mm:ss";
                }
                else if (clockModel.TemporaryTimeIndex == 3)
                {
                    TemporaryTimeFormat = "HH:mm:ss";
                }
                ViewTemporaryTime = Standard.ToString(TemporaryTimeFormat);
                OnPropertyChanged("TemporaryTimeIndex");
            }
        }

        public String ViewTemporaryTime
        { 
            get { return clockModel.ViewTemporaryTime; }
            set { clockModel.ViewTemporaryTime = value; OnPropertyChanged("ViewTemporaryTime"); }
        }

        public String ViewTemporaryDate
        {
            get { return clockModel.ViewTemporaryDate; }
            set { clockModel.ViewTemporaryDate = value; OnPropertyChanged("ViewTemporaryDate"); }
        }
        /// <summary>
        /// /
        /// </summary>

        // 타임 포맷 변경 확인 버튼
        public ICommand TimeFormatChangeConfirm => new RelayCommand<object>(timeFormatChangeConfirm, null);

        private void timeFormatChangeConfirm(object e)
        {
            String beforeChangeDate = DateFormat;
            String beforeChangeTime = TimeFormat;

            DateFormat = TemporaryDateFormat;
            TimeFormat = TemporaryTimeFormat;


            String function = "Format Change";
            String now = Standard.ToString(StandardChangeViewFormat);

            String RecordText = "날짜 포맷 변경 : " + beforeChangeDate + " => " + DateFormat + "\n" + "타임 포맷 변경 : " + beforeChangeTime + " => " + TimeFormat;

            viewLogViewModel.AddData(function, now, RecordText);

            if (MessageBox.Show("선택한 포맷으로 변경 하였습니다.", "성공", MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                Application.Current.Windows.OfType<TimeFormatChange>().First().Close();
            }
        }
    }
}

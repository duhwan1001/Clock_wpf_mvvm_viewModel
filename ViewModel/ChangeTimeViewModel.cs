using System;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VewModelSample.Model;
using VewModelSample.View;
using VewModelSample.ViewModel.Command;
using MessageBox = System.Windows.MessageBox;

namespace VewModelSample.ViewModel
{
    public class ChangeTimeViewModel : INotifyPropertyChanged
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

        private bool IsNumeric(string source)
        {
            Regex regex = new Regex("[^0-9.-]+");

            return !regex.IsMatch(source);
        }

        private Model.ClockModel clockModel = null;
        private ViewModel.ViewLogViewModel viewLog = null;

        public ChangeTimeViewModel()
        {
            clockModel = ClockModel.Instance;
            viewLog = ViewLogViewModel.Instance;
        }

        /// <summary>
        /// 
        /// </summary>
        public String BackgroundFilepath
        {
            get { return clockModel.BackgroundFilepath; }
            set { clockModel.BackgroundFilepath = value; OnPropertyChanged("BackgroundFilepath"); }
        }
        public Double SetHourAngle
        {
            get { return clockModel.SetHourAngle; }
            set { clockModel.SetHourAngle = value; OnPropertyChanged("SetHourAngle"); }
        }

        public int SetMinAngle
        {
            get { return clockModel.SetMinAngle; }
            set { clockModel.SetMinAngle = value; OnPropertyChanged("SetMinAngle"); }
        }

        public int SetSecAngle
        {
            get { return clockModel.SetSecAngle; }
            set { clockModel.SetSecAngle = value; OnPropertyChanged("SetSecAngle"); }
        }
        public int TimeSelectIndex
        {
            get { return clockModel.TimeSelectIndex; }
            set
            {
                clockModel.TimeSelectIndex = value;

                if (clockModel.TimeSelectIndex == 0)
                {
                    TimeSelectFormat = "오전";
                }
                else if (clockModel.TimeSelectIndex == 1)
                {
                    TimeSelectFormat = "오후";
                }
                OnPropertyChanged("TimeSelectIndex");
            }
        }
        public String SetHour
        {
            get { return clockModel.SetHour; }
            set
            {
                clockModel.SetHour = value;
                try
                {
                    if (!(int.Parse(SetHour) > 12))
                    {
                        SetHourAngle = int.Parse(SetHour) * 30;
                    }
                    OnPropertyChanged("SetHour");
                }
                catch { }
            }
        }

        public String SetMin
        {
            get { return clockModel.SetMin; }
            set
            {
                try
                {
                    clockModel.SetMin = value;
                    if (!(int.Parse(SetMin) > 60))
                    {
                        SetMinAngle = int.Parse(value) * 6;
                    }
                    OnPropertyChanged("SetMin");
                }
                catch { }
            }
        }

        public String SetSec
        {
            get { return clockModel.SetSec; }
            set
            {
                try
                {
                    clockModel.SetSec = value;
                    if (!(int.Parse(SetSec) > 60))
                    {
                        SetSecAngle = int.Parse(value) * 6;
                    }
                    OnPropertyChanged("SetSec");
                }
                catch { }
            }
        }

        public String SelectedDate
        {
            get { return clockModel.SelectedDate; }
            set { clockModel.SelectedDate = value; OnPropertyChanged("SelectedDate"); }
        }

        public String TimeSelectFormat
        {
            get { return clockModel.TimeSelectFormat; }
            set { clockModel.TimeSelectFormat = value; OnPropertyChanged("TimeSelectFormat"); }
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

        public String StandardChangeViewFormat
        {
            get { return clockModel.StandardChangeViewFormat; }
            set { clockModel.StandardChangeViewFormat = value; OnPropertyChanged("StandardChangeViewFormat"); }
        }

        public void UpdateHour(object sender, RoutedEventArgs e)
        {
            TextBox textbox = ((TextBox)sender);
            SetHourAngle = (int.Parse(SetHour) * 30) + (int.Parse(SetMin) * 0.5);
        }
        public void UpdateMin(object sender, RoutedEventArgs e)
        {
            TextBox textbox = ((TextBox)sender);
            SetMinAngle = int.Parse(SetMin) * 6;
        }
        public void UpdateSec(object sender, RoutedEventArgs e)
        {
            TextBox textbox = ((TextBox)sender);
            SetSecAngle = int.Parse(SetSec) * 6;
        }

        public ICommand ChangeTimeConfirm => new RelayCommand<object>(changeTimeConfirm, null);

        private void changeTimeConfirm(object e)
        {
            if (SelectedDate == null)
            {
                MessageBox.Show("날짜를 지정하세요", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SetHour == "" || SetMin == "" || SetSec == "")
            {
                MessageBox.Show("시간을 입력하세요", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (IsNumeric(SetHour) == false || IsNumeric(SetMin) == false || IsNumeric(SetSec) == false)
            {
                MessageBox.Show("숫자만 입력 가능합니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (int.Parse(SetHour) < 0 || int.Parse(SetMin) < 0 || int.Parse(SetSec) < 0)
            {
                MessageBox.Show("양수를 입력하세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int hour = int.Parse(SetHour);
            int min = int.Parse(SetMin);
            int sec = int.Parse(SetSec);

            if (min == 60)
            {
                min = 0;
            }

            if (sec == 60)
            {
                sec = 0;
            }

            if (hour > 12 || min > 60 || sec > 60)
            {
                MessageBox.Show("정확한 시간을 입력하세요", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (TimeSelectFormat == "오후" && hour != 12)
            {
                hour += 12;
            }
            if (TimeSelectFormat == "오전" && hour == 12)
            {
                hour -= 12;
            }

            TimeMode = 1;

            string selDate = SelectedDate;
            DateTime SetDateTime = Convert.ToDateTime(selDate);

            int year = SetDateTime.Year;
            int month = SetDateTime.Month;
            int day = SetDateTime.Day;

            DateTime timeToUse = new DateTime(year, month, day, hour, min, sec);

            // log
            String now = Standard.ToString(StandardChangeViewFormat);

            Standard = timeToUse;
            String afterChangeTime = timeToUse.ToString(StandardChangeViewFormat);

            String RecordText = now + "에서 변경한 시간 : " + afterChangeTime;

            viewLog.AddData("ChangeTime", now, RecordText);

            if (MessageBox.Show("시간 설정 완료.", "성공", MessageBoxButton.OK, MessageBoxImage.Information) == MessageBoxResult.OK)
            {
                Application.Current.Windows.OfType<ChangeTime>().First().Close();
            }

            SetSecAngle = 0;
            SetMinAngle = 0;
            SetHourAngle = 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Input;
using VewModelSample.Model;
using VewModelSample.ViewModel.Command;
using System.Windows;
using VewModelSample.UtilClass;
using System.Text.RegularExpressions;

namespace VewModelSample.ViewModel
{
    internal class AlarmViewModel : INotifyPropertyChanged
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

        private static AlarmViewModel _instance = new AlarmViewModel();
        public static AlarmViewModel Instance
        {
            get { return _instance; }
        }

        private bool IsNumeric(string source)
        {
            Regex regex = new Regex("[^0-9.-]+");

            return !regex.IsMatch(source);
        }

        private Model.ClockModel clockModel = null;
        private ViewModel.ViewLogViewModel viewLog = null;

        public AlarmViewModel()
        {
            clockModel = ClockModel.Instance;
            viewLog = ViewLogViewModel.Instance;
        }

        // ----------------------------------------------------------------------------------

        public ObservableCollection<ClockModel.alarmData> alarmDatas
        {
            get
            {
                if (clockModel._alarmDatas == null)
                {
                    clockModel._alarmDatas = new ObservableCollection<ClockModel.alarmData>();
                }
                return clockModel._alarmDatas;
            }
            set
            {
                clockModel._alarmDatas = value;
                OnPropertyChanged("AlarmDatas");
            }
        }

        public int AlarmSequence
        {
            get { return alarmDatas.Count; }
        }

        public void AddAlarm(String targetTime)
        {
            ClockModel.alarmData alarmGrid = new ClockModel.alarmData();
            alarmGrid.alarmSequence = AlarmSequence + 1;
            alarmGrid.targetTime = targetTime;

            alarmDatas.Add(alarmGrid);
        }
        public String BackgroundFilepath
        {
            get { return clockModel.BackgroundFilepath; }
            set { clockModel.BackgroundFilepath = value; OnPropertyChanged("BackgroundFilepath"); }
        }
        public String GetHour
        {
            get { return clockModel.GetHour; } 
            set { clockModel.GetHour = value; OnPropertyChanged("GetHour"); }
        }
        public String GetMin
        {
            get { return clockModel.GetMin; }
            set { clockModel.GetMin = value; OnPropertyChanged("GetMin"); }
        }

        public String GetSec
        {
            get { return clockModel.GetSec; }
            set { clockModel.GetSec = value; OnPropertyChanged("GetSec"); }
        }

        public String TimeSelectFormat
        {
            get { return clockModel.TimeSelectFormat; }
            set { clockModel.TimeSelectFormat = value; OnPropertyChanged("TimeSelectFormat"); }
        }

        public String SelectedDate
        {
            get { return clockModel.SelectedDate; }
            set { clockModel.SelectedDate = value; OnPropertyChanged("SelectedDate"); }
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

        public int AlarmSelectedIndex
        {
            get { return clockModel.AlarmSelectedIndex; }
            set { clockModel.AlarmSelectedIndex = value; OnPropertyChanged("AlarmSelectedIndex"); }
        }
        public int AlarmThreadSeq
        {
            get { return clockModel.alarmThreadSeq += 1; }
            //set { clockModel.alarmThreadSeq = value; OnPropertyChanged("AlarmThreadSeq"); }
        }

        public List<Thread> ThreadList
        {
            get
            {
                if (clockModel.threadList == null)
                {
                    clockModel.threadList = new List<Thread>();
                }
                return clockModel.threadList;
            }
            set
            {
                clockModel.threadList = value;
                OnPropertyChanged("ThreadList");
            }
        }

        // 알람 추가 버튼
        public Thread alarmThread = null;
        
        public ICommand AddAlarmConfirm => new RelayCommand<object>(addAlarmConfirm, null);
        private void addAlarmConfirm(object e)
        {
            if (SelectedDate == null)
            {
                MessageBox.Show("날짜를 지정하세요", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (GetHour == null || GetMin == null || GetSec == null)
            {
                MessageBox.Show("시간을 입력하세요", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (IsNumeric(GetHour) == false || IsNumeric(GetMin) == false || IsNumeric(GetSec) == false)
            {
                MessageBox.Show("숫자만 입력 가능합니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (int.Parse(GetHour) < 0 || int.Parse(GetMin) < 0 || int.Parse(GetSec) < 0)
            {
                MessageBox.Show("양수를 입력하세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int hour = int.Parse(GetHour);
            int min = int.Parse(GetMin);
            int sec = int.Parse(GetSec);

            if (hour > 12 || min > 60 || sec > 60)
            {
                MessageBox.Show("12시가 초과되었거나 60이 초과하였습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
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

            DateTime SetDateTime = Convert.ToDateTime(SelectedDate);

            int year = SetDateTime.Year;
            int month = SetDateTime.Month;
            int day = SetDateTime.Day;

            DateTime timeToUse = new DateTime(year, month, day, hour, min, sec);

            if (timeToUse < Standard)
            {
                MessageBox.Show("이미 지난 시간은 알람으로 설정할 수 없습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            for (int i = 0; i < alarmDatas.Count; i++)
            {
                if (alarmDatas[i].targetTime.Equals(timeToUse.ToString(StandardChangeViewFormat)))
                {
                    MessageBox.Show("이미 같은 시각으로 등록된 알람이 있습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            // log
            String targetTime = timeToUse.ToString(StandardChangeViewFormat);
            String RecordText = "등록한 알람 : " + targetTime;

            viewLog.AddData("SetAlarm", Standard.ToString(StandardChangeViewFormat), RecordText);
            AddAlarm(targetTime);

            alarmThread = new Thread(waitingAlarm);
            alarmThread.IsBackground = true;
            alarmThread.Name = (AlarmThreadSeq).ToString();

            string[] arr = new string[2];

            arr[0] = alarmThread.Name;
            arr[1] = timeToUse.ToString(StandardChangeViewFormat);

            alarmThread.Start(arr);

            ThreadList.Add(alarmThread);

            MessageBox.Show("알람이 추가 되었습니다.", "성공", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Alarm Thread
        public void waitingAlarm(Object obj)
        {
            string[] datas = (string[])obj;
            while (true)
            {
                if (Standard.ToString(StandardChangeViewFormat).Equals(datas[1]))
                {
                    MessageBox.Show(datas[1] + "로 설정된 " + datas[0] + "번 알람", "알람");
                    viewLog.AddData("AlarmRinging", Standard.ToString(StandardChangeViewFormat), datas[1] + "로 설정된 " + datas[0] + "번 알람");
                    DispatcherService.Invoke(() =>
                    {
                        alarmDatas.RemoveAt(int.Parse(datas[0]) - 1);
                    });
                    break;
                }
            }
        }

        public ICommand RemoveRow => new RelayCommand<object>(removeRow, null);

        private void removeRow(object parameter)
        {
            if (MessageBox.Show("선택한 알람을 삭제하시겠습니까?", "경고", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                int index = AlarmSelectedIndex;
                if (index > -1 && index < alarmDatas.Count)
                {
                    alarmDatas.RemoveAt(index);
                    ThreadList[index].Abort();
                    ThreadList.RemoveAt(index);
                }
                MessageBox.Show("선택한 알람을 삭제하였습니다.", "성공", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                return;
            }
        }


    }
}

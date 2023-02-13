using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using VewModelSample.Model;
using VewModelSample.ViewModel.Command;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;

namespace VewModelSample.ViewModel
{
    internal class StopwatchViewModel : INotifyPropertyChanged
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

        private static StopwatchViewModel _instance = new StopwatchViewModel();
        public static StopwatchViewModel Instance
        {
            get { return _instance; }
        }

        private Model.ClockModel clockModel = null;
        private ViewModel.ViewLogViewModel viewLogViewModel = null;

        public StopwatchViewModel()
        {
            viewLogViewModel = ViewLogViewModel.Instance;
            clockModel = ClockModel.Instance;
        }

        public int StopWatchSeq
        {
            get { return swDatas.Count + 1; }
        }

        ObservableCollection<ClockModel.swData> _swDatas = null;
        public ObservableCollection<ClockModel.swData> swDatas
        {
            get
            {
                if (_swDatas == null)
                {
                    _swDatas = new ObservableCollection<ClockModel.swData>();
                }
                return _swDatas;
            }
            set
            {
                _swDatas = value;
                OnPropertyChanged("AlarmDatas");
            }
        }

        public String StopWatch
        {
            get { return clockModel.StopWatch; }
            set
            {
                clockModel.StopWatch = value;
                OnPropertyChanged("StopWatch");
            }
        }
        public String SwLeftText
        {
            get { return clockModel.SwLeftText; }
            set { clockModel.SwLeftText = value; OnPropertyChanged("SwLeftText"); }
        }
        public String SwRightText
        {
            get { return clockModel.SwRightText; }
            set { clockModel.SwRightText = value; OnPropertyChanged("SwRightText"); }
        }
        public Boolean SwLeftButtonTF
        {
            get { return clockModel.SwLeftButtonTF; }
            set { clockModel.SwLeftButtonTF = value; OnPropertyChanged("SwLeftButtonTF"); }
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
        public void AddSwRecord()
        {
            ClockModel.swData swData = new ClockModel.swData();
            swData.stopWatchSeq = StopWatchSeq;
            swData.saveTime = StopWatch;

            swDatas.Add(swData);
        }

        public ICommand StartStopWatch => new RelayCommand<object>(setStopWatch, null);

        int stopwatchFlag = 0;
        public Stopwatch sw = new Stopwatch();
        private void setStopWatch(object e)
        {
            if (stopwatchFlag == 0)
            {
                FirstStartSW();
                stopwatchFlag = 1;
                SwRightText = "정지";
                SwLeftButtonTF = true;
                viewLogViewModel.AddData("Stopwatch", Standard.ToString(StandardChangeViewFormat), "스톱워치 시작");
            }
            else if (stopwatchFlag == 1)
            {
                PauseSW();
                SwRightText = "시작";
                SwLeftText = "초기화";
                RecordButtonFlag = 1;
                stopwatchFlag = 2;
            }
            else if (stopwatchFlag == 2)
            {
                RestartSW();
                SwRightText = "정지";
                SwLeftText = "기록";
                stopwatchFlag = 1;
            }

        }

        public int ResetFlag = 0;
        Thread SWThread = null;
        public void FirstStartSW()
        {
            SWThread = new Thread(startStopWatch);
            SWThread.IsBackground = true;
            SWThread.Name = nameof(SWThread);
            SWThread.Start();
        }

        public void startStopWatch(Object obj)
        {
            sw.Start();
            while (true)
            {
                StopWatch = sw.Elapsed.ToString("hh\\:mm\\:ss\\.ff");
            }
        }

        public void PauseSW()
        {
            sw.Stop();
        }

        public void RestartSW()
        {
            sw.Start();
        }
        public void ResetSW()
        {
            sw.Reset();
            SwLeftText = "기록";
            SwLeftButtonTF = false;
            stopwatchFlag = 0;
            RecordButtonFlag = 0;
            swDatas.Clear();
            SWThread.Abort();
        }

        public ICommand RecordStopwatch => new RelayCommand<object>(recordStopwatch, null);

        public int RecordButtonFlag = 0;
        private void recordStopwatch(object e)
        {
            if (RecordButtonFlag == 0)
            {
                AddSwRecord();
            }
            else if (RecordButtonFlag == 1)
            {
                ResetSW();
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using VewModelSample.ViewModel;
using VewModelSample.ViewModel.Command;

namespace VewModelSample.Model
{
    public class ClockModel : INotifyPropertyChanged
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

        private static ClockModel _instance = new ClockModel();
        public static ClockModel Instance
        {
            get { return _instance; }
        }

        #region Common ----------------------------------------------------------------------------------
        private int timeMode = 0;
        public int TimeMode
        {
            get { return timeMode; }
            set { timeMode = value; OnPropertyChanged("TimeMode"); }
        }

        private DateTime standard;
        public DateTime Standard
        {
            get { return standard; }
            set { standard = value; OnPropertyChanged("Standard"); }
        }

        private string backgroundFilepath = "images/Background/whiteAndGrayGradient.jpg";
        public String BackgroundFilepath
        {
            get { return backgroundFilepath; }
            set { backgroundFilepath = value; OnPropertyChanged("BackgroundFilepath"); }
        }
        #endregion

        #region ClockView ----------------------------------------------------------------------------------
        private double hourAngle;
        public double HourAngle
        {
            get { return hourAngle; }
            set { hourAngle = value; OnPropertyChanged("HourAngle"); }
        }

        private int minAngle;
        public int MinAngle
        {
            get { return minAngle; }
            set { minAngle = value; OnPropertyChanged("MinAngle"); }
        }

        private int secAngle;
        public int SecAngle
        {
            get { return secAngle; }
            set { secAngle = value; OnPropertyChanged("SecAngle"); }
        }
        #endregion

        #region ChangeTime ----------------------------------------------------------------------------------
        private int timeSelectIndex = 0;
        public int TimeSelectIndex
        {
            get { return timeSelectIndex; }
            set
            {
                timeSelectIndex = value;

                if (timeSelectIndex == 0)
                {
                    TimeSelectFormat = "오전";
                }
                else if (timeSelectIndex == 1)
                {
                    TimeSelectFormat = "오후";
                }
                OnPropertyChanged("TimeSelectIndex");
            }
        }

        private String timeSelectFormat;
        public String TimeSelectFormat
        {
            get { return timeSelectFormat; }
            set { timeSelectFormat = value; OnPropertyChanged("TimeSelectFormat"); }
        }

        private String selectedDate;
        public String SelectedDate
        {
            get
            {
                return selectedDate;
            }
            set
            {
                selectedDate = value;
                OnPropertyChanged("SelectedDate");
            }
        }

        private String setHour;
        public String SetHour
        {
            get { return setHour; }
            set
            {
                setHour = value;
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
        private String setMin;
        public String SetMin
        {
            get { return setMin; }
            set
            {
                try
                {
                    setMin = value;
                    if (!(int.Parse(SetMin) > 60))
                    {
                        SetMinAngle = int.Parse(value) * 6;
                    }
                    OnPropertyChanged("SetMin");
                }
                catch { }
            }
        }
        private String setSec;
        public String SetSec
        {
            get { return setSec; }
            set
            {
                try
                {
                    setSec = value;
                    if (!(int.Parse(SetSec) > 60))
                    {
                        SetSecAngle = int.Parse(value) * 6;
                    }
                    OnPropertyChanged("SetSec");
                }
                catch { }
            }
        }

        private String getHour;
        public String GetHour
        {
            get
            {
                return getHour;
            }
            set
            {
                getHour = value;
                OnPropertyChanged("GetHour");
            }
        }
        private String getMin;
        public String GetMin
        {
            get
            {
                return getMin;
            }
            set
            {
                getMin = value;
                OnPropertyChanged("GetMin");
            }
        }
        private String getSec;
        public String GetSec
        {
            get
            {
                return getSec;
            }
            set
            {
                getSec = value;
                OnPropertyChanged("GetSec");
            }
        }
        private Double setHourAngle;
        public Double SetHourAngle
        {
            get { return setHourAngle; }
            set { setHourAngle = value; OnPropertyChanged("SetHourAngle"); }
        }

        private int setMinAngle;
        public int SetMinAngle
        {
            get { return setMinAngle; }
            set { setMinAngle = value; OnPropertyChanged("SetMinAngle"); }
        }

        private int setSecAngle;
        public int SetSecAngle
        {
            get { return setSecAngle; }
            set { setSecAngle = value; OnPropertyChanged("SetSecAngle"); }
        }
        #endregion

        #region TimeFormatChange ----------------------------------------------------------------------------------
        private int temporaryDateIndex;
        public int TemporaryDateIndex
        {
            get { return temporaryDateIndex; }
            set
            {
                temporaryDateIndex = value;

                if (temporaryDateIndex == 0)
                {
                    TemporaryDateFormat = "yyyy'년' M'월' d'일' dddd";
                }
                else if (temporaryDateIndex == 1)
                {
                    TemporaryDateFormat = "yyyy'년' M'월' d'일'";
                }
                else if (temporaryDateIndex == 2)
                {
                    TemporaryDateFormat = "yy'년' M'월' d'일' dddd";
                }
                else if (temporaryDateIndex == 3)
                {
                    TemporaryDateFormat = "yy'년' M'월' d'일'";
                }

                OnPropertyChanged("TemporaryDateIndex");
            }
        }
        private int temporaryTimeIndex;
        public int TemporaryTimeIndex
        {
            get { return temporaryTimeIndex; }
            set
            {
                temporaryTimeIndex = value;

                if (temporaryTimeIndex == 0)
                {
                    TemporaryTimeFormat = "tt h:mm:ss";
                }
                else if (temporaryTimeIndex == 1)
                {
                    TemporaryTimeFormat = "tt hh:mm:ss";
                }
                else if (temporaryTimeIndex == 2)
                {
                    TemporaryTimeFormat = "H:mm:ss";
                }
                else if (temporaryTimeIndex == 3)
                {
                    TemporaryTimeFormat = "HH:mm:ss";
                }

                OnPropertyChanged("TemporaryTimeIndex");
            }
        }
        private String temporaryDateFormat;
        public String TemporaryDateFormat
        {
            get
            {
                return temporaryDateFormat;
            }
            set
            {
                temporaryDateFormat = value;
                OnPropertyChanged("TemporaryDateText");
            }
        }

        private String temporaryTimeFormat;
        public String TemporaryTimeFormat
        {
            get
            {
                return temporaryTimeFormat;
            }
            set
            {
                temporaryTimeFormat = value;
                OnPropertyChanged("TemporaryTimeText");
            }
        }
        private String viewTemporaryTime;
        public String ViewTemporaryTime
        {
            get
            {
                return viewTemporaryTime;
            }
            set
            {
                viewTemporaryTime = value;
                OnPropertyChanged("ViewTemporaryTime");
            }
        }
        private String viewTemporaryDate;
        public String ViewTemporaryDate
        {
            get
            {
                return viewTemporaryDate;
            }
            set
            {
                viewTemporaryDate = value;
                OnPropertyChanged("ViewTemporaryDate");
            }
        }
        #endregion

        # region ClockMain ----------------------------------------------------------------------------------

        private String dateFormat;
        public String DateFormat
        {
            get { return dateFormat; }
            set
            {
                dateFormat = value;
                OnPropertyChanged("DateFormat");
            }
        }


        private String timeFormat;
        public String TimeFormat
        {
            get { return timeFormat; }
            set
            {
                timeFormat = value;
                OnPropertyChanged("SetTimeFormat");
            }
        }

        private String kind = "KST(UTC+9)";
        public String Kind
        {
            get { return kind; }
            set { kind = value; OnPropertyChanged("Kind"); }
        }

        private String viewCurrentDate;
        public String ViewCurrentDate
        {
            get
            {
                return viewCurrentDate;
            }
            set
            {
                viewCurrentDate = value;
                OnPropertyChanged("ViewCurrentDate");
            }
        }

        private String viewCurrentTime;
        public String ViewCurrentTime
        {
            get
            {
                return viewCurrentTime;
            }
            set
            {
                viewCurrentTime = value;
                OnPropertyChanged("ViewCurrentTime");
            }
        }

        private String viewCurrentKind;
        public String ViewCurrentKind
        {
            get
            {
                return viewCurrentKind;
            }
            set
            {
                viewCurrentKind = value;
                OnPropertyChanged("ViewCurrentKind");
            }
        }
        #endregion

        #region StandardChange ----------------------------------------------------------------------------------
        private int standardChangeIndex;
        public int StandardChangeIndex
        {
            get { return standardChangeIndex; }
            set
            {
                standardChangeIndex = value;

                if (standardChangeIndex == 0)
                {
                    StandardChangeFormat = "KST(UTC+9)";
                    StandardChangeView = DateTime.Now.ToString(StandardChangeViewFormat);
                }
                else if (standardChangeIndex == 1)
                {
                    StandardChangeFormat = "UTC(UTC+0)";
                    StandardChangeView = DateTime.UtcNow.ToString(StandardChangeViewFormat);
                }
                else if (standardChangeIndex == 2)
                {
                    StandardChangeFormat = "PST(UTC-8)";
                    StandardChangeView = DateTime.UtcNow.Subtract(new TimeSpan(8, 0, 0)).ToString(StandardChangeViewFormat);
                }

                OnPropertyChanged("StandardChangeIndex");
            }
        }

        private String standardChangeFormat;
        public String StandardChangeFormat
        {
            get { return standardChangeFormat; }
            set { standardChangeFormat = value; OnPropertyChanged("StandardChangeFormat"); }
        }

        private String standardChangeViewFormat = "yyyy'년' M'월' d'일' dddd tt h:mm:ss";
        public String StandardChangeViewFormat
        {
            get { return standardChangeViewFormat; }
            set { standardChangeViewFormat = value; OnPropertyChanged("StandardChangeViewFormat"); }
        }

        private String standardChangeView;
        public String StandardChangeView
        {
            get { return standardChangeView; }
            set { standardChangeView = value; OnPropertyChanged("StandardChangeView"); }
        }
        #endregion

        #region SelAlarm ----------------------------------------------------------------------------------
        public int alarmThreadSeq = 0;
        public int AlarmThreadSeq
        {
            get { return alarmThreadSeq += 1; }
            //set { clockModel.alarmThreadSeq = value; OnPropertyChanged("AlarmThreadSeq"); }
        }
        public class alarmData
        {
            public int alarmSequence { get; set; }
            public String targetTime { get; set; }
        }

        public int alarmSelectedIndex;
        public int AlarmSelectedIndex
        {
            get { return alarmSelectedIndex; }
            set { alarmSelectedIndex = value; OnPropertyChanged("AlarmSelectedIndex"); }
        }

        public List<Thread> threadList = null;
        #endregion
        #region StopWatch ----------------------------------------------------------------------------------
        private String stopWatch = "00:00:00:00";
        public String StopWatch
        {
            get { return stopWatch; }
            set
            {
                stopWatch = value;
                OnPropertyChanged("StopWatch");
            }
        }

        private String swLeftText = "기록";
        public String SwLeftText
        {
            get { return swLeftText; }
            set { swLeftText = value; OnPropertyChanged("SwLeftText"); }
        }

        private String swRightText = "시작";
        public String SwRightText
        {
            get { return swRightText; }
            set { swRightText = value; OnPropertyChanged("SwRightText"); }
        }


        private Boolean swLeftButtonTF = false;
        public Boolean SwLeftButtonTF
        {
            get { return swLeftButtonTF; }
            set { swLeftButtonTF = value; OnPropertyChanged("SwLeftButtonTF"); }
        }

        //public int stopWatchSeq = 0;

        public class swData
        {
            public int stopWatchSeq { get; set; }
            public String saveTime { get; set; }
        }

        public ObservableCollection<ClockModel.alarmData> _alarmDatas = null;

        #endregion
        #region log ----------------------------------------------------------------------------------

        public class dataGridData
        {
            public int dataGridSequence { get; set; }
            public String dataGridFunction { get; set; }
            public String dataGridAddedTime { get; set; }
            public String dataGridSimpleRecordText { get; set; }
        }
        
        public ObservableCollection<ClockModel.dataGridData> _logDatas = null;
        #endregion

        #region TCP comm -------------------------------------------------------------------

        // Server ------------------------

        public String serverIpAddr = "";
        public String serverPort = "5000";

        public String serverSendData;

        public String serverButtonText = "서버 시작";

        // 버튼 비활성화 활성화 true/false
        public Boolean serverButtonTF = true;
        // 텍스트 박스 활성화 true/false
        public Boolean serverTextBoxTF = true;

        public class serverDataGrid
        {
            public int dataGridSequence { get; set; }
            public String dataGridFunction { get; set; }
            public String dataGridAddedTime { get; set; }
            public String dataGridSimpleRecordText { get; set; }
        }

        public ObservableCollection<ClockModel.serverDataGrid> _serverLogDatas = null;

        // Client ------------------------

        public String clientIpAddr = "127.0.0.1";
        public String clientPort = "5000";

        public String clientSendData;

        // 버튼 비활성화 활성화 true/false
        public Boolean clientButtonTF = false;
        // 텍스트 박스 활성화 true/false
        public Boolean clientTextBoxTF = true;

        public Uri clientFrameBind;

        public class clientDataGrid
        {
            public int dataGridSequence { get; set; }
            public String dataGridFunction { get; set; }
            public String dataGridAddedTime { get; set; }
            public String dataGridSimpleRecordText { get; set; }
        }

        public ObservableCollection<ClockModel.clientDataGrid> _clientLogDatas = null;

        // Common ------------------------

        // 시간 변경
        public String tcp_ChangeHour;
        public String tcp_ChangeMin;
        public String tcp_ChangeSec;

        // 타임 포맷 변경
        public int tcp_TimeFormat = 0;

        // 표준시 변경
        public int tcp_Standard = 0 ;
        // 0 : KST
        // 1 : UTC
        // 2 : EST

        // 알람 추가
        public String tcp_AlarmHour;
        public String tcp_AlarmMin;
        public String tcp_AlarmSec;

        // 스톱 워치
        public int tcp_StopWatchFlag;

        //public enum tcp_StopWatchFlag : int
        //{
        //    start = 0, // 0 : 시작
        //    stop,      // 1 : 정지
        //    reset,     // 2 : 리셋
        //    lap        // 3 : 기록
        //}

        #endregion
    }
}

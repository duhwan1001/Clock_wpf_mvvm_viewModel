using System;
using System.Net.Sockets;
using System.Threading;
using System.Windows.Input;
using VewModelSample.ViewModel.Command;
using System.Net;
using System.Windows;
using System.ComponentModel;
using VewModelSample.Model;
using VewModelSample.View;
using VewModelSample.UtilClass;
using static VewModelSample.Model.ClockModel;
using System.Collections.ObjectModel;

namespace VewModelSample.ViewModel
{
    public class TCP_Server_ViewModel : INotifyPropertyChanged
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

        // 모델 싱글톤
        private Model.ClockModel clockModel = null;
        private ViewModel.AlarmViewModel alarmViewModel = null;
        private ViewModel.StopwatchViewModel sw = null;

        // 생성자
        public TCP_Server_ViewModel()
        {
            clockModel = ClockModel.Instance;
            alarmViewModel = ViewModel.AlarmViewModel.Instance;
            sw = ViewModel.StopwatchViewModel.Instance;
        }

        // 모델에 선언해줘야 하는 부분
        public String ServerIPAddr
        {
            get { return clockModel.serverIpAddr; }
            set { clockModel.serverIpAddr = value; OnPropertyChanged("IPAddr"); }
        }
        public String ServerPort
        {
            get { return clockModel.serverPort; }
            set { clockModel.serverPort = value; OnPropertyChanged("Port"); }
        }
        public String ServerSendData
        {
            get { return clockModel.serverSendData; }
            set { clockModel.serverSendData = value; OnPropertyChanged("ServerSendData"); }
        }
        public Boolean ServerButtonTF
        {
            get { return clockModel.serverButtonTF; }
            set { clockModel.serverButtonTF = value; OnPropertyChanged("ServerButtonTF"); }
        }

        public Boolean ServerTextBoxTF
        {
            get { return clockModel.serverTextBoxTF; }
            set { clockModel.serverTextBoxTF = value; OnPropertyChanged("ServerTextBoxTF"); }
        }

        public String ServerButtonText
        {
            get { return clockModel.serverButtonText; }
            set { clockModel.serverButtonText = value; OnPropertyChanged("ServerButtonText"); }
        }

        public ObservableCollection<ClockModel.serverDataGrid> ServerLogDatas
        {
            get
            {
                if (clockModel._serverLogDatas == null)
                {
                    clockModel._serverLogDatas = new ObservableCollection<ClockModel.serverDataGrid>();
                }
                return clockModel._serverLogDatas;
            }
            set
            {
                clockModel._serverLogDatas = value;
                OnPropertyChanged("ServerLogDatas");
            }
        }

        public void AddServerLog(String function, String AddedTime, String RecordText)
        {
            ClockModel.serverDataGrid serverDataGrid = new ClockModel.serverDataGrid();
            serverDataGrid.dataGridSequence = LogSequence;
            serverDataGrid.dataGridFunction = function;
            serverDataGrid.dataGridAddedTime = AddedTime;
            serverDataGrid.dataGridSimpleRecordText = RecordText;

            DispatcherService.BeginInvoke((Action)delegate // <--- HERE
            {
                ServerLogDatas.Add(serverDataGrid);            
            });
        }

        public int LogSequence
        {
            get { return ServerLogDatas.Count + 1; }
        }
        // 모델 선언 End

        #region StandardChange ---------------------------------------------------------
        public DateTime Standard
        {
            get { return clockModel.Standard; }
            set { clockModel.Standard = value; OnPropertyChanged("Standard"); }
        }
        public String StandardChangeViewFormat
        {
            get { return clockModel.StandardChangeViewFormat; }
            set { clockModel.StandardChangeViewFormat = value; OnPropertyChanged("StandardChangeViewFormat"); }
        }
        public String Kind
        {
            get { return clockModel.Kind; }
            set { clockModel.Kind = value; OnPropertyChanged("Kind"); }
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
        public String StandardChangeView
        {
            get { return clockModel.StandardChangeView; }
            set { clockModel.StandardChangeView = value; OnPropertyChanged("StandardChangeView"); }
        }
        #endregion

        #region TimeFormatChange ----------------------------
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

        public String TimeFormat
        {
            get { return clockModel.TimeFormat; }
            set
            {
                clockModel.TimeFormat = value;
                OnPropertyChanged("SetTimeFormat");
            }
        }
        #endregion

        #region AlarmSet -------------------------------------
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
        public int AlarmThreadSeq
        {
            get { return clockModel.alarmThreadSeq += 1; }
        }
        #endregion

        // 연결 시도 버튼에 매핑
        public ICommand TryConnect => new RelayCommand<object>(tryConnect, null);
        private void tryConnect(object e)
        {
            Thread thread1 = new Thread(connect);
            thread1.IsBackground = true;
            thread1.Name = nameof(connect);
            thread1.Start();
            MessageBox.Show("서버가 시작되었습니다.", "서버 시작", MessageBoxButton.OK, MessageBoxImage.Warning);
            ServerButtonText = "서버 실행 중";
            ServerButtonTF = false;
        }
        
        TcpListener tcpListener = null;
        TcpClient tcpClient = null;
        NetworkStream stream = null;
        private void connect()
        {
            AddServerLog("Server", Standard.ToString(StandardChangeViewFormat), "Server Start");
            tcpListener = new TcpListener(IPAddress.Any, int.Parse(ServerPort));

            tcpListener.Start();
            // 서버 준비
            try
            {
                tcpClient = tcpListener.AcceptTcpClient();
            }
            catch (Exception ex)
            {
                MessageBox.Show(" " + ex,"Error");
                return;
            }
            // 클라이언트 연결됨

            IPEndPoint ClientIP = tcpClient.Client.RemoteEndPoint as IPEndPoint;
            string ClientIPStr = ClientIP.Address.ToString();

            byte[] buffer = new byte[1024 * 4];
            string data = string.Empty;

            while (tcpClient.Connected)
            {
                data = string.Empty;

                stream = tcpClient.GetStream();

                int bytesRead = 0;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    
                    AnalyzePacket(stream, buffer, ClientIPStr);
                    
                }

                // Shutdown and end connection
                tcpListener.Stop();
                stream.Close();
                tcpClient.Close();

                MessageBox.Show("서버가 종료되었습니다.", "서버 종료", MessageBoxButton.OK, MessageBoxImage.Warning);
                AddServerLog("Server", Standard.ToString(StandardChangeViewFormat), "Server Terminate");
                ServerButtonText = "서버 시작";
                ServerButtonTF = true;
            }
        }
        private void AnalyzePacket(NetworkStream stream, byte[] buffer, string ClientIP)
        {
            Packet packet = (Packet)Packet.Deserialize(buffer);

            if (packet == null)
                return;

            string func_name = string.Empty;
            if (packet.packet_Type == (int)PacketType.ChangeTime)
            {
                func_name = "시간 변경";
            }
            else if (packet.packet_Type == (int)PacketType.ChangeTimeFormat)
            {
                func_name = "시간 포맷 변경";
            }
            else if (packet.packet_Type == (int)PacketType.ChangeStandard)
            {
                func_name = "표준시 변경";
            }
            else if (packet.packet_Type == (int)PacketType.SetAlarm)
            {
                func_name = "알람 설정";
            }
            else if (packet.packet_Type == (int)PacketType.SetStopWatch)
            {
                func_name = "스톱워치 설정";
            }
            else if (packet.packet_Type == (int)PacketType.ConnTerminate)
            {
                func_name = "연결 종료";
            }

            if (MessageBox.Show("[ " + ClientIP + " ] 에서 [ " + func_name + " ] 을 수정하려 합니다.", "클라이언트 접근", MessageBoxButton.OKCancel, MessageBoxImage.Warning) == MessageBoxResult.Cancel)
            {
                tcpListener.Stop();
                return;
            }

            switch ((int)packet.packet_Type)
            {
                case (int)PacketType.ChangeTime:
                    {
                        //// 받은 패킷을 deserialize 시킴
                        TCP_Properties.ChangeTimeValues ChangeTimeValues = (TCP_Properties.ChangeTimeValues)Packet.Deserialize(buffer);

                        //// 전송할 패킷을 serialize 시킴
                        TCP_Properties.Result result = new TCP_Properties.Result();
                        result.packet_Type = (int)PacketType.ChangeTime;
                        try
                        {

                            int year = Standard.Year;
                            int month = Standard.Month;
                            int day = Standard.Day;
                            int hour = int.Parse(ChangeTimeValues.Packet_ChangeHour);
                            int min = int.Parse(ChangeTimeValues.Packet_ChangeMin);
                            int sec = int.Parse(ChangeTimeValues.Packet_ChangeSec);

                            DateTime timeToUse = new DateTime(year, month, day, hour, min, sec);

                            TimeMode = 1;

                            String now = Standard.ToString(StandardChangeViewFormat);

                            Standard = timeToUse;
                            String afterChangeTime = timeToUse.ToString(StandardChangeViewFormat);

                            String RecordText = ClientIP + "에서 변경 : " + now + "에서 변경한 시간 : " + afterChangeTime;

                            AddServerLog("ChangeTime", now, RecordText);

                            result.result = true;
                            result.reason = "시간 변경 성공";

                            MessageBox.Show("시간 설정 완료.", "성공", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        catch(Exception ex)
                        {
                            result.result = false;
                            result.reason = "시간 변경 실패";
                            AddServerLog("ChangeTime", Standard.ToString(StandardChangeViewFormat), ClientIP + "에서 시간 변경 실패");
                            MessageBox.Show("" + ex, "error");
                        }

                        Array.Clear(buffer, 0, buffer.Length);
                        Packet.Serialize(result).CopyTo(buffer, 0);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    break;
                case (int)PacketType.ChangeTimeFormat:
                    {
                        //// 받은 패킷을 deserialize 시킴
                        TCP_Properties.ChangeTimeFormatValues CTFIndex = (TCP_Properties.ChangeTimeFormatValues)Packet.Deserialize(buffer);

                        //// 전송할 패킷을 serialize 시킴
                        TCP_Properties.Result result = new TCP_Properties.Result();
                        result.packet_Type = (int)PacketType.ChangeTimeFormat;
                        try
                        {

                            int TimeIndex = 0;
                            if(CTFIndex.Packet_TimeFormat == 0)
                            {
                                TimeIndex = 0;
                            }
                            else if(CTFIndex.Packet_TimeFormat == 1)
                            {
                                TimeIndex = 3;
                            }

                            TemporaryTimeIndex = TimeIndex;

                            String beforeChangeTime = TimeFormat;

                            TimeFormat = TemporaryTimeFormat;

                            String function = "Format Change";
                            String now = Standard.ToString(StandardChangeViewFormat);

                            String RecordText = ClientIP + "에서 변경 : " + beforeChangeTime + " => " + TimeFormat;

                            AddServerLog(function, now, RecordText);

                            MessageBox.Show("선택한 포맷으로 변경 하였습니다.", "성공", MessageBoxButton.OK, MessageBoxImage.Information);

                            result.result = true;
                            result.reason = "타임포맷 변경 성공";
                        }
                        catch
                        {
                            result.result = false;
                            result.reason = "타임포맷 변경 실패";
                            AddServerLog("ChangeTimeFormat", Standard.ToString(StandardChangeViewFormat), ClientIP + "에서 타임포맷 변경 실패");
                        }

                        Array.Clear(buffer, 0, buffer.Length);
                        Packet.Serialize(result).CopyTo(buffer, 0);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    break;
                case (int)PacketType.ChangeStandard:
                    {
                        //// 받은 패킷을 deserialize 시킴
                        TCP_Properties.ChangeStandardValues standard = (TCP_Properties.ChangeStandardValues)Packet.Deserialize(buffer);

                        //// 전송할 패킷을 serialize 시킴
                        TCP_Properties.Result result = new TCP_Properties.Result();
                        result.packet_Type = (int)PacketType.ChangeStandard;
                        try
                        {
                            StandardChangeIndex = standard.Packet_Standard;

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
                            String RecordText = ClientIP + "에서 변경 : " + beforeKind + "에서 변경한 기준시 : " + Kind;

                            AddServerLog(function, now, RecordText);

                            result.result = true;
                            result.reason = "표준시 변경 성공";
                        }
                        catch
                        {
                            result.result = false;
                            result.reason = "표준시 변경 실패";
                            AddServerLog("ChangeStandard", Standard.ToString(StandardChangeViewFormat), ClientIP + "에서 표준시 변경 실패");
                        }

                        Array.Clear(buffer, 0, buffer.Length);
                        Packet.Serialize(result).CopyTo(buffer, 0);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    break;
                case (int)PacketType.SetAlarm:
                    {
                        //// 받은 패킷을 deserialize 시킴
                        TCP_Properties.AddAlarmValues AlarmValues= (TCP_Properties.AddAlarmValues)Packet.Deserialize(buffer);

                        //// 전송할 패킷을 serialize 시킴
                        TCP_Properties.Result result = new TCP_Properties.Result();
                        result.packet_Type = (int)PacketType.SetAlarm;
                        try
                        {
                            int hour = int.Parse(AlarmValues.Packet_AlarmHour);
                            int min = int.Parse(AlarmValues.Packet_AlarmMin);
                            int sec = 0;

                            DateTime SetDateTime = Convert.ToDateTime(Standard);

                            int year = SetDateTime.Year;
                            int month = SetDateTime.Month;
                            int day = SetDateTime.Day;

                            DateTime timeToUse = new DateTime(year, month, day, hour, min, sec);

                            for (int i = 0; i < alarmDatas.Count; i++)
                            {
                                if (alarmDatas[i].targetTime.Equals(timeToUse.ToString(StandardChangeViewFormat)))
                                {
                                    MessageBox.Show("이미 같은 시각으로 등록된 알람이 있습니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                                    result.result = false;
                                    result.reason = "알람 추가 실패";
                                    return;
                                }
                            }

                            // log
                            String targetTime = timeToUse.ToString(StandardChangeViewFormat);

                            String RecordText = ClientIP + "에서 변경 : " + "등록한 알람 => " + targetTime;
                            AddServerLog("SetAlarm", Standard.ToString(StandardChangeViewFormat), RecordText);

                            alarmViewModel.AddAlarm(targetTime);
                                                        
                            alarmViewModel.alarmThread = new Thread(alarmViewModel.waitingAlarm); // 알람 삭제한 뒤에 또 추가하려고 하면 터짐 : 스레드 점유 문제였음. Dispatcher.BeginInvoke로 해결
                            alarmViewModel.alarmThread.IsBackground = true;
                            alarmViewModel.alarmThread.Name = (AlarmThreadSeq).ToString();

                            string[] arr = new string[2];

                            arr[0] = alarmViewModel.alarmThread.Name;
                            arr[1] = timeToUse.ToString(StandardChangeViewFormat);

                            alarmViewModel.alarmThread.Start(arr);

                            alarmViewModel.ThreadList.Add(alarmViewModel.alarmThread);

                            result.result = true;
                            result.reason = "알람 추가 성공";
                        }
                        catch
                        {
                            result.result = false;
                            result.reason = "알람 추가 실패";
                            AddServerLog("AddAlarm", Standard.ToString(StandardChangeViewFormat), ClientIP + "에서 알람 추가 실패");
                        }

                        Array.Clear(buffer, 0, buffer.Length);
                        Packet.Serialize(result).CopyTo(buffer, 0);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    break;
                case (int)PacketType.SetStopWatch:
                    {
                        //// 받은 패킷을 deserialize 시킴
                        TCP_Properties.StopWatchValues stopwatchValues = (TCP_Properties.StopWatchValues)Packet.Deserialize(buffer);

                        //// 전송할 패킷을 serialize 시킴
                        TCP_Properties.Result result = new TCP_Properties.Result();
                        result.packet_Type = (int)PacketType.SetStopWatch;
                        try
                        {
                            int stopwatchFlag = stopwatchValues.Packet_StopWatchFlag;

                            if (stopwatchFlag == 0)
                            {
                                sw.FirstStartSW();
                                AddServerLog("StopWatch", Standard.ToString(StandardChangeViewFormat), "스톱워치 시작 성공");
                                result.reason = "스톱워치 시작 성공";
                            }
                            else if (stopwatchFlag == 1)
                            {
                                sw.PauseSW();
                                AddServerLog("StopWatch", Standard.ToString(StandardChangeViewFormat), "스톱워치 정지 성공");
                                result.reason = "스톱워치 정지 성공";
                            }
                            else if (stopwatchFlag == 2)
                            {
                                sw.ResetSW();
                                AddServerLog("StopWatch", Standard.ToString(StandardChangeViewFormat), "스톱워치 초기화 성공");
                                result.reason = "스톱워치 초기화 성공";
                            }
                            else if (stopwatchFlag == 3)
                            {
                                sw.AddSwRecord();
                                AddServerLog("StopWatch", Standard.ToString(StandardChangeViewFormat), "스톱워치 기록 성공");
                                result.reason = "스톱워치 기록 성공";
                            }

                            result.result = true;
                        }
                        catch
                        {
                            result.result = false;
                            result.reason = "스톱워치 제어 실패";
                            AddServerLog("StopWatch", Standard.ToString(StandardChangeViewFormat), ClientIP + "에서 스톱워치 제어 실패");
                        }

                        Array.Clear(buffer, 0, buffer.Length);
                        Packet.Serialize(result).CopyTo(buffer, 0);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    break;
            }
        }

        // 보내기 버튼에 매핑
        public ICommand ServerTerminate => new RelayCommand<object>(serverTerminate, null);
        private void serverTerminate(object e)
        {
            if(tcpListener != null)
            {
                tcpListener.Stop();
            }
            if(tcpClient != null)
            {
                tcpClient.Close();
            }
            if(stream != null)
            {
                stream.Close();
            }
            MessageBox.Show("서버가 종료되었습니다.", "서버 종료", MessageBoxButton.OK, MessageBoxImage.Warning);
            AddServerLog("Server", Standard.ToString(StandardChangeViewFormat), "Server Terminate");
            ServerButtonText = "서버 시작";
            ServerButtonTF = true;
        }
        public ICommand ServerViewLog => new RelayCommand<object>(serverViewLog, null);
        private void serverViewLog(object e)
        {
            ViewLog viewLog = new ViewLog();
            viewLog.Show();
        }
    }
}

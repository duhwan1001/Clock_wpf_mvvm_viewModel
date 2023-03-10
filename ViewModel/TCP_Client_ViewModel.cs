using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using VewModelSample.Model;
using VewModelSample.UtilClass;
using VewModelSample.View;
using VewModelSample.ViewModel.Command;

namespace VewModelSample.ViewModel
{
    public class TCP_Client_ViewModel : INotifyPropertyChanged
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

        // 생성자
        public TCP_Client_ViewModel()
        {
            clockModel = ClockModel.Instance;
        }

        StreamReader streamReader;
        StreamWriter streamWriter;

        // 모델에 선언해줘야 되는 부분
        // Client -------------
        public String ClientIPAddr
        {
            get { return clockModel.clientIpAddr; }
            set { clockModel.clientIpAddr = value; OnPropertyChanged("IPAddr"); }
        }

        public String ClientPort
        {
            get { return clockModel.clientPort; }
            set { clockModel.clientPort = value; OnPropertyChanged("Port"); }
        }

        public String ClientSendData
        {
            get { return clockModel.clientSendData; }
            set { clockModel.clientSendData = value; OnPropertyChanged("ClientSendData"); }
        }

        public Boolean ClientButtonTF
        {
            get { return clockModel.clientButtonTF; }
            set { clockModel.clientButtonTF = value; OnPropertyChanged("ClientButtonTF"); }
        }

        public Boolean ClientTextBoxTF
        {
            get { return clockModel.clientTextBoxTF; }
            set { clockModel.clientTextBoxTF = value; OnPropertyChanged("ClientTextBoxTF"); }
        }

        public Uri ClientFrameBind
        {
            get { return clockModel.clientFrameBind; }
            set { clockModel.clientFrameBind = value; OnPropertyChanged("ClientFrameBind"); }
        }

        // Common --------------------
        public string TCP_ChangeHour
        {
            get { return clockModel.tcp_ChangeHour; }
            set { clockModel.tcp_ChangeHour = value; OnPropertyChanged("TCP_ChangeHour"); }
        }

        public string TCP_ChangeMin
        {
            get { return clockModel.tcp_ChangeMin; }
            set { clockModel.tcp_ChangeMin = value; OnPropertyChanged("TCP_ChangeMin"); }
        }

        public string TCP_ChangeSec
        {
            get { return clockModel.tcp_ChangeSec; }
            set { clockModel.tcp_ChangeSec = value; OnPropertyChanged("TCP_ChangeSec"); }
        }

        public int TCP_TimeFormat
        {
            get { return clockModel.tcp_TimeFormat; }
            set { clockModel.tcp_TimeFormat = value; OnPropertyChanged("TCP_TimeFormat"); }
        }

        public int TCP_Standard
        {
            get { return clockModel.tcp_Standard; }
            set { clockModel.tcp_Standard = value; OnPropertyChanged("TCP_Standard"); }
        }

        public string TCP_AlarmHour
        {
            get { return clockModel.tcp_AlarmHour; }
            set { clockModel.tcp_AlarmHour = value; OnPropertyChanged("TCP_AlarmHour"); }
        }

        public string TCP_AlarmMin
        {
            get { return clockModel.tcp_AlarmMin; }
            set { clockModel.tcp_AlarmMin = value; OnPropertyChanged("TCP_AlarmMin"); }
        }

        public string TCP_AlarmSec
        {
            get { return clockModel.tcp_AlarmSec; }
            set { clockModel.tcp_AlarmSec = value; OnPropertyChanged("TCP_AlarmSec"); }
        }

        public int TCP_StopWatchFlag
        {
            get { return clockModel.tcp_StopWatchFlag; }
            set { clockModel.tcp_StopWatchFlag = value; OnPropertyChanged("TCP_StopWatchFlag"); }
        }
        // 모델 선언 end

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

        public int LogSequence
        {
            get { return ClientLogDatas.Count + 1; }
        }

        public ObservableCollection<ClockModel.clientDataGrid> ClientLogDatas
        {
            get
            {
                if (clockModel._clientLogDatas == null)
                {
                    clockModel._clientLogDatas = new ObservableCollection<ClockModel.clientDataGrid>();
                }
                return clockModel._clientLogDatas;
            }
            set
            {
                clockModel._clientLogDatas = value;
                OnPropertyChanged("ClientLogDatas");
            }
        }

        public void AddClientLog(String function, String AddedTime, String RecordText)
        {
            ClockModel.clientDataGrid clientDataGrid = new ClockModel.clientDataGrid();
            clientDataGrid.dataGridSequence = LogSequence;
            clientDataGrid.dataGridFunction = function;
            clientDataGrid.dataGridAddedTime = AddedTime;
            clientDataGrid.dataGridSimpleRecordText = RecordText;

            DispatcherService.BeginInvoke((Action)delegate // <--- HERE
            {
                ClientLogDatas.Add(clientDataGrid);
            });
        }

        // 연결 시도 버튼에 매핑
        public ICommand TryConnect => new RelayCommand<object>(tryConnect, null);
        private void tryConnect(object e)
        {
            Thread thread1 = new Thread(connect);
            thread1.IsBackground = true;
            thread1.Start();
        }

        TcpClient tcpClient = null;
        private void connect()
        {
            tcpClient = new TcpClient();
            IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(ClientIPAddr), int.Parse(ClientPort));
            tcpClient.Connect(ipEnd);
            // 서버 연결됨
                     
            ClientButtonTF = true;
            ClientTextBoxTF = false;

            streamReader = new StreamReader(tcpClient.GetStream());
            streamWriter = new StreamWriter(tcpClient.GetStream());
            streamWriter.AutoFlush = true;
            
            while (tcpClient.Connected)
            {
                // receiveData - 수신 데이터
                string receiveData = streamReader.ReadLine();
                                
            }

        }
        
        // 시간 변경
        public ICommand ClientChangeTime => new RelayCommand<object>(clientChangeTime, null);
        private void clientChangeTime(object e)
        {
            // 시계 시간 설정 뷰
            Uri uri = new Uri("ClientFrame/ChangeTimeFrame.xaml", UriKind.Relative);
            ClientFrameBind = uri;
        }

        private bool IsNumeric(string source)
        {
            Regex regex = new Regex("[^0-9.-]+");

            return !regex.IsMatch(source);
        }

        public ICommand SendChangeTime => new RelayCommand<object>(sendChangeTime, null);
        private void sendChangeTime(object e)
        {
            if (TCP_ChangeHour == "" || TCP_ChangeMin == "" || TCP_ChangeSec == "")
            {
                System.Windows.MessageBox.Show("시간을 입력하세요", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (IsNumeric(TCP_ChangeHour) == false || IsNumeric(TCP_ChangeMin) == false || IsNumeric(TCP_ChangeSec) == false)
            {
                System.Windows.MessageBox.Show("숫자만 입력 가능합니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (int.Parse(TCP_ChangeHour) < 0 || int.Parse(TCP_ChangeMin) < 0 || int.Parse(TCP_ChangeSec) < 0)
            {
                System.Windows.MessageBox.Show("양수를 입력하세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (int.Parse(TCP_ChangeHour) > 12 || int.Parse(TCP_ChangeMin) > 60 || int.Parse(TCP_ChangeSec) > 60)
            {
                System.Windows.MessageBox.Show("정확한 시간을 입력하세요", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                byte[] buffer = new byte[1024 * 4];

                // 서버 연결
                tcpClient = new TcpClient();
                IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(ClientIPAddr), int.Parse(ClientPort));
                tcpClient.Connect(ipEnd);

                NetworkStream stream = tcpClient.GetStream();

                // send packet
                TCP_Properties.ChangeTimeValues changeTime = new TCP_Properties.ChangeTimeValues();
                changeTime.packet_Type = (int)PacketType.ChangeTime;
                changeTime.Packet_ChangeHour = TCP_ChangeHour;
                changeTime.Packet_ChangeMin = TCP_ChangeMin;
                changeTime.Packet_ChangeSec = TCP_ChangeSec;

                Packet.Serialize(changeTime).CopyTo(buffer, 0);

                stream.Write(buffer, 0, buffer.Length);

                // receive packet
                Array.Clear(buffer, 0, buffer.Length);

                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                TCP_Properties.Result result = (TCP_Properties.Result)Packet.Deserialize(buffer);

                if (result.result)
                {
                    System.Windows.Forms.MessageBox.Show(result.reason, "클라이언트 확인", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AddClientLog("ChangeTime", Standard.ToString(StandardChangeViewFormat), ClientIPAddr + "의 시간 변경 완료 =>" + TCP_ChangeHour + "시" + TCP_ChangeMin + "분" + TCP_ChangeSec + "초");
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(result.reason, "클라이언트 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AddClientLog("ChangeTime", Standard.ToString(StandardChangeViewFormat), "에러 발생");
                }

                // close socket
                stream.Close();
                tcpClient.Close();

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // 타임 포맷
        public ICommand ClientCTF => new RelayCommand<object>(clientCTF, null);
        private void clientCTF(object e)
        {
            // 타임 포맷 뷰
            Uri uri = new Uri("ClientFrame/ChangeTimeFormatFrame.xaml", UriKind.Relative);
            ClientFrameBind = uri;
        }
        
        public ICommand SendTimeFormat => new RelayCommand<object>(sendTimeFormat, null);
        private void sendTimeFormat(object e)
        {
            try
            {
                byte[] buffer = new byte[1024 * 4];

                // 서버 연결
                tcpClient = new TcpClient();
                IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(ClientIPAddr), int.Parse(ClientPort));
                tcpClient.Connect(ipEnd);

                NetworkStream stream = tcpClient.GetStream();

                // send packet
                TCP_Properties.ChangeTimeFormatValues standard = new TCP_Properties.ChangeTimeFormatValues();
                standard.packet_Type = (int)PacketType.ChangeTimeFormat;
                standard.Packet_TimeFormat = TCP_TimeFormat;

                Packet.Serialize(standard).CopyTo(buffer, 0);

                stream.Write(buffer, 0, buffer.Length);

                // receive packet
                Array.Clear(buffer, 0, buffer.Length);

                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                TCP_Properties.Result result = (TCP_Properties.Result)Packet.Deserialize(buffer);

                if (result.result)
                {
                    System.Windows.Forms.MessageBox.Show(result.reason, "클라이언트 확인", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AddClientLog("ChangeTimeFormat", Standard.ToString(StandardChangeViewFormat), ClientIPAddr + "의 타임 포맷 변경 완료");
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(result.reason, "클라이언트 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AddClientLog("ChangeTimeFormat", Standard.ToString(StandardChangeViewFormat), "에러 발생");
                }

                // close socket
                stream.Close();
                tcpClient.Close();

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 표준시 변경
        public ICommand ClientStandardChange => new RelayCommand<object>(clientStandardChange, null);
        private void clientStandardChange(object e)
        {
            // 표준시 뷰
            Uri uri = new Uri("ClientFrame/ChangeStandardFrame.xaml", UriKind.Relative);
            ClientFrameBind = uri;
        }

        public ICommand SendStandard => new RelayCommand<object>(sendStandard, null);
        private void sendStandard(object e)
        {
            try
            {
                byte[] buffer = new byte[1024 * 4];

                // 서버 연결
                tcpClient = new TcpClient();
                IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(ClientIPAddr), int.Parse(ClientPort));
                tcpClient.Connect(ipEnd);

                NetworkStream stream = tcpClient.GetStream();

                // send packet
                TCP_Properties.ChangeStandardValues standard = new TCP_Properties.ChangeStandardValues();
                standard.packet_Type = (int)PacketType.ChangeStandard;
                standard.Packet_Standard = TCP_Standard;

                Packet.Serialize(standard).CopyTo(buffer, 0);

                stream.Write(buffer, 0, buffer.Length);

                // receive packet
                Array.Clear(buffer, 0, buffer.Length);

                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                TCP_Properties.Result result = (TCP_Properties.Result)Packet.Deserialize(buffer);

                if (result.result)
                {
                    System.Windows.Forms.MessageBox.Show(result.reason, "클라이언트 확인", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AddClientLog("ChangeStandard", Standard.ToString(StandardChangeViewFormat), ClientIPAddr + "의 표준시 변경 완료");
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(result.reason, "클라이언트 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AddClientLog("ChangeStandard", Standard.ToString(StandardChangeViewFormat), "에러 발생");
                }

                // close socket
                stream.Close();
                tcpClient.Close();

            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        

        // 알람
        public ICommand ClientSetAlarm => new RelayCommand<object>(clientSetAlarm, null);
        private void clientSetAlarm(object e)
        {
            // 알람 설정 뷰
            Uri uri = new Uri("ClientFrame/SetAlarmFrame.xaml", UriKind.Relative);
            ClientFrameBind = uri;
        }

        public ICommand SendAlarm => new RelayCommand<object>(sendAlarm, null);
        private void sendAlarm(object e)
        {
            if (TCP_AlarmHour == "" || TCP_AlarmMin == "")
            {
                System.Windows.MessageBox.Show("시간을 입력하세요", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (IsNumeric(TCP_AlarmHour) == false || IsNumeric(TCP_AlarmMin) == false)
            {
                System.Windows.MessageBox.Show("숫자만 입력 가능합니다.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (int.Parse(TCP_AlarmHour) < 0 || int.Parse(TCP_AlarmMin) < 0)
            {
                System.Windows.MessageBox.Show("양수를 입력하세요.", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (int.Parse(TCP_AlarmHour) > 24 || int.Parse(TCP_AlarmMin) > 60)
            {
                System.Windows.MessageBox.Show("정확한 시간을 입력하세요", "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                byte[] buffer = new byte[1024 * 4];

                // 서버 연결
                tcpClient = new TcpClient();
                IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(ClientIPAddr), int.Parse(ClientPort));
                tcpClient.Connect(ipEnd);

                NetworkStream stream = tcpClient.GetStream();

                // send packet
                TCP_Properties.AddAlarmValues standard = new TCP_Properties.AddAlarmValues();
                standard.packet_Type = (int)PacketType.SetAlarm;
                standard.Packet_AlarmHour = TCP_AlarmHour;
                standard.Packet_AlarmMin = TCP_AlarmMin;

                Packet.Serialize(standard).CopyTo(buffer, 0);

                stream.Write(buffer, 0, buffer.Length);

                // receive packet
                Array.Clear(buffer, 0, buffer.Length);

                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                TCP_Properties.Result result = (TCP_Properties.Result)Packet.Deserialize(buffer);

                if (result.result)
                {
                    System.Windows.Forms.MessageBox.Show(result.reason, "클라이언트 확인", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AddClientLog("AddAlarm", Standard.ToString(StandardChangeViewFormat), ClientIPAddr + "의 알람 추가 완료 => " + TCP_AlarmHour + "시" + TCP_AlarmMin + "분");
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(result.reason, "클라이언트 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AddClientLog("AddAlarm", Standard.ToString(StandardChangeViewFormat), "에러 발생");
                }

                // close socket
                stream.Close();
                tcpClient.Close();

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // 스톱워치
        public ICommand ClientStopwatch => new RelayCommand<object>(clientStopwatch, null);
        private void clientStopwatch(object e)
        {
            // 스톱워치 뷰
            Uri uri = new Uri("ClientFrame/SetStopwatchFrame.xaml", UriKind.Relative);
            ClientFrameBind = uri;
        }

        public ICommand ClientSWStart => new RelayCommand<object>(clientSWStart, null);
        private void clientSWStart(object e)
        {
            StopwatchProcess(0);
        }
        public ICommand ClientSWPause=> new RelayCommand<object>(clientSWPause, null);
        private void clientSWPause(object e)
        {
            StopwatchProcess(1);
        }
        public ICommand ClientSWReset=> new RelayCommand<object>(clientSWReset, null);
        private void clientSWReset(object e)
        {
            StopwatchProcess(2);
        }
        public ICommand ClientSWRecord => new RelayCommand<object>(clientSWRecord, null);
        private void clientSWRecord(object e)
        {
            StopwatchProcess(3);
        }

        public void StopwatchProcess(int function)
        {
            try
            {
                byte[] buffer = new byte[1024 * 4];

                // 서버 연결
                tcpClient = new TcpClient();
                IPEndPoint ipEnd = new IPEndPoint(IPAddress.Parse(ClientIPAddr), int.Parse(ClientPort));
                tcpClient.Connect(ipEnd);

                NetworkStream stream = tcpClient.GetStream();

                // send packet
                TCP_Properties.StopWatchValues standard = new TCP_Properties.StopWatchValues();
                standard.packet_Type = (int)PacketType.SetStopWatch;
                standard.Packet_StopWatchFlag = function;
                // 0 : start, 1 : pause, 2 : reset, 3 : record

                Packet.Serialize(standard).CopyTo(buffer, 0);

                stream.Write(buffer, 0, buffer.Length);

                // receive packet
                Array.Clear(buffer, 0, buffer.Length);

                int bytesRead = stream.Read(buffer, 0, buffer.Length);  
                TCP_Properties.Result result = (TCP_Properties.Result)Packet.Deserialize(buffer);

                if (result.result)
                {
                    System.Windows.Forms.MessageBox.Show(result.reason, "클라이언트 확인", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AddClientLog("StopWatch", Standard.ToString(StandardChangeViewFormat), "스톱워치 제어 성공");
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show(result.reason, "클라이언트 오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    AddClientLog("StopWatch", Standard.ToString(StandardChangeViewFormat), "에러 발생");
                }

                // close socket
                stream.Close();
                tcpClient.Close();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "오류", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        // 연결 종료
        public ICommand ClientTerminate => new RelayCommand<object>(clientTerminate, null);
        private void clientTerminate(object e)
        {
            // 연결 종료
            tcpClient.Close();
            ClientButtonTF = false;
            ClientTextBoxTF = true;
        }

        // 로그 확인
        public ICommand ClientViewLog => new RelayCommand<object>(clientViewLog, null);
        private void clientViewLog(object e)
        {
            ViewLog viewLog = new ViewLog();
            viewLog.Show();
        }


    }

}

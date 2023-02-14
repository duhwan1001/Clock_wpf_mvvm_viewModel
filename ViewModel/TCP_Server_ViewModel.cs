using System;
using System.IO;
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
using System.Windows.Controls;

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
        private ViewModel.ViewLogViewModel viewLog = null;

        // 생성자
        public TCP_Server_ViewModel()
        {
            clockModel = ClockModel.Instance;
            viewLog = ViewModel.ViewLogViewModel.Instance;
        }

        StreamReader streamReader;
        StreamWriter streamWriter;

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
        // 모델 선언 End

        #region StandardChange ---------------------------------------------------------
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


        // 연결 시도 버튼에 매핑
        public ICommand TryConnect => new RelayCommand<object>(tryConnect, null);
        private void tryConnect(object e)
        {
            Thread thread1 = new Thread(connect);
            thread1.IsBackground = true;
            thread1.Start();
        }

        private void connect()
        {
            //TcpListener tcpListener = new TcpListener(IPAddress.Parse(ServerIPAddr), int.Parse(ServerPort));            
            TcpListener tcpListener = new TcpListener(IPAddress.Any, int.Parse(ServerPort));

            tcpListener.Start();
            // 서버 준비

            TcpClient tcpClient = tcpListener.AcceptTcpClient();
            // 클라이언트 연결됨
            
            if(MessageBox.Show("어디서 접근했는지 보여주고 수락하지 않는다면 리턴") == MessageBoxResult.Cancel)
            {
                return;
            }

            byte[] buffer = new byte[1024 * 4];
            string data = string.Empty;

            while (tcpClient.Connected)
            {
                data = string.Empty;

                NetworkStream stream = tcpClient.GetStream();

                int bytesRead = 0;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    AnalyzePacket(stream, buffer);
                }

                // Shutdown and end connection
                stream.Close();
                tcpClient.Close();
            }
        }
        private void AnalyzePacket(NetworkStream stream, byte[] buffer)
        {
            Packet packet = (Packet)Packet.Deserialize(buffer);

            if (packet == null)
                return;

            switch ((int)packet.packet_Type)
            {
                case (int)PacketType.ChangeTime:
                    {

                    }
                    break;
                case (int)PacketType.ChangeTimeFormat:
                    {

                    }
                    break;
                case (int)PacketType.ChangeStandard:
                    {
                        //// 받은 패킷을 MemberRegister class 로 deserialize 시킴
                        TCP_Properties.ChangeStandardValues standard = (TCP_Properties.ChangeStandardValues)Packet.Deserialize(buffer);

                        //// 전송할 패킷을 LoginResult class 로 serialize 시킴
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
                            String RecordText = beforeKind + "에서 변경한 기준시 : " + Kind;

                            viewLog.AddData(function, now, RecordText);

                            result.result = true;
                            result.reason = "표준시 변경 성공";
                        }
                        catch
                        {
                            result.result = false;
                            result.reason = "표준시 변경 실패";
                        }

                        Array.Clear(buffer, 0, buffer.Length);
                        Packet.Serialize(result).CopyTo(buffer, 0);
                        stream.Write(buffer, 0, buffer.Length);
                    }
                    break;
                case (int)PacketType.SetAlarm:
                    {


                        //Array.Clear(buffer, 0, buffer.Length);
                        //Packet.Serialize(mrResult).CopyTo(buffer, 0);
                        //stream.Write(buffer, 0, buffer.Length);

                        //setLog("");
                    }
                    break;
                case (int)PacketType.SetStopWatch:
                    {


                        //Array.Clear(buffer, 0, buffer.Length);
                        //Packet.Serialize(mrResult).CopyTo(buffer, 0);
                        //stream.Write(buffer, 0, buffer.Length);

                        //setLog("");
                    }
                    break;
            }
        }

        // 보내기 버튼에 매핑
        public ICommand ServerTerminate => new RelayCommand<object>(serverTerminate, null);
        private void serverTerminate(object e)
        {
            streamWriter.WriteLine(ServerSendData);
        }
        public ICommand ServerViewLog => new RelayCommand<object>(serverViewLog, null);
        private void serverViewLog(object e)
        {
            ViewLog viewLog = new ViewLog();
            viewLog.Show();
        }
    }
}

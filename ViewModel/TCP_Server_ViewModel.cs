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

        // 생성자
        public TCP_Server_ViewModel()
        {
            clockModel = ClockModel.Instance;
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
            TcpListener tcpListener = new TcpListener(IPAddress.Parse(ServerIPAddr), int.Parse(ServerPort));

            tcpListener.Start();
            // 서버 준비

            TcpClient tcpClient = tcpListener.AcceptTcpClient();
            // 클라이언트 연결됨
            
            if(MessageBox.Show("어디서 접근했는지 보여주고 수락하지 않는다면 리턴") == MessageBoxResult.Cancel)
            {
                return;
            }

            streamReader = new StreamReader(tcpClient.GetStream());
            streamWriter = new StreamWriter(tcpClient.GetStream());
            streamWriter.AutoFlush = true;

            while(tcpClient.Connected)
            {
                string receiveData = streamReader.ReadLine();

                // 무슨 기능 제어할건지 먼저 플래그 받음
                int setflag = int.Parse(receiveData);
                if(setflag == 0)
                {
                    // 시계 시간 설정
                    SetClock(receiveData);
                }
                else if(setflag == 1)
                {
                    // 타임 포맷 변경
                    ChangeTimeFormat(receiveData);
                }                
                else if(setflag == 2)
                {
                    // 표준시 변경
                    StandardChange(receiveData);
                }
                else if(setflag == 3)
                {
                    // 알람 추가        
                    AddAlarm(receiveData);
                }
                else if(setflag == 4)
                {
                    // 스톱워치 설정
                    SetStopwatch(receiveData);
                }
                else if(setflag == 5)
                {
                    // 연결 종료
                    tcpClient.Close();
                }
            }
        }

        private void SetClock(string data)
        {
            // 시계 시간 설정하는 로직
        }

        private void ChangeTimeFormat(string data)
        {
            // 타임 포맷 설정하는 로직
        }

        private void StandardChange(string data)
        {
            // 표준시 바꾸는 로직
        }

        private void AddAlarm(string data)
        {
            // 알람 추가하는 로직
        }

        private void SetStopwatch(string data)
        {
            // 스톱워치 세팅하는 로직
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

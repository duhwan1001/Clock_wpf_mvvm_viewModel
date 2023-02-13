using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using VewModelSample.Model;
using VewModelSample.ViewModel.Command;
using VewModelSample.View;

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

        // 모델 선언 end

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
                     
            if (MessageBox.Show("어디서 접근했는지 보여주고 수락하지 않는다면 리턴") == MessageBoxResult.Cancel)
            {
                return;
            }

            ClientButtonTF = true;
            ClientTextBoxTF = false;

            streamReader = new StreamReader(tcpClient.GetStream());
            streamWriter = new StreamWriter(tcpClient.GetStream());
            streamWriter.AutoFlush = true;
            
            while (tcpClient.Connected)
            {
                string receiveData = streamReader.ReadLine();
                // receiveData - 수신 데이터
                                
            }

        }

        // 시간 변경
        public ICommand ClientChangeTime => new RelayCommand<object>(clientChangeTime, null);
        private void clientChangeTime(object e)
        {
            // 시계 시간 설정하는 로직
            streamWriter.WriteLine(ClientSendData);
        }

        // 타임 포맷
        public ICommand ClientCTF => new RelayCommand<object>(clientCTF, null);
        private void clientCTF(object e)
        {
            // 타임 포맷 설정하는 로직
            streamWriter.WriteLine(ClientSendData);
        }

        // 표준시 변경
        public ICommand ClientStandardChange => new RelayCommand<object>(clientStandardChange, null);
        private void clientStandardChange(object e)
        {
            // 표준시 바꾸는 로직
            streamWriter.WriteLine(ClientSendData);
        }

        // 알람
        public ICommand ClientSetAlarm => new RelayCommand<object>(clientSetAlarm, null);
        private void clientSetAlarm(object e)
        {
            // 알람 추가하는 로직
            streamWriter.WriteLine(ClientSendData);
        }

        // 스톱워치
        public ICommand ClientStopwatch => new RelayCommand<object>(clientStopwatch, null);
        private void clientStopwatch(object e)
        {
            // 스톱워치 세팅하는 로직
            streamWriter.WriteLine(ClientSendData);
        }

        // 연결 종료
        public ICommand ClientTerminate => new RelayCommand<object>(clientTerminate, null);
        private void clientTerminate(object e)
        {
            // 연결 종료 로직
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

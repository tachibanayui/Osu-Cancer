using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Windows.Threading;
using System.IO;
using System.ComponentModel;

namespace Osu_Bancher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static ManualResetEvent doneAccept = new ManualResetEvent(false);
        private string ServerData = AppDomain.CurrentDomain.BaseDirectory;
        private Cons cons = new Cons();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = cons;

            cons.OnlineUser = 0;
            cons.ServerStatus = "Offline";
            WriteOutupt("Hello! This is Osu!Bancer :D. Validating Files", OutputFlag.Info);
            if (!Directory.Exists(ServerData + "ServerData"))
            {
                Directory.CreateDirectory(ServerData + "ServerData");
                Directory.CreateDirectory(ServerData + @"ServerData\UserCredential");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            WriteOutupt("Server Start!", OutputFlag.Info);
            if(string.IsNullOrEmpty(txbIpAddress.Text) || string.IsNullOrEmpty(txbPort.Text))
            {
                WriteOutupt("Please enter IpAddress and Port to host a private server!", OutputFlag.Error);
                WriteOutupt("Server Stopped!", OutputFlag.Exception);
                return;
            }

            WriteOutupt("Summary: Socket Type:" + "Server EndPoint: " + txbIpAddress.Text + ":" + txbPort.Text, OutputFlag.Info);
            Thread thd = new Thread(OpenServer);
            thd.Start();
        }

        private void OpenServer()
        {
            string IpAddress = "", port = "";
            Dispatcher.Invoke(() => { IpAddress = txbIpAddress.Text; });
            Dispatcher.Invoke(() => { port = txbPort.Text; });
            IPEndPoint serverIp;
            try { serverIp = new IPEndPoint(IPAddress.Parse(IpAddress), int.Parse(port)); }
            catch (Exception e)
            {
                WriteOutupt("Unable to Parse Ip and Port \n" + e, OutputFlag.Error);
                WriteOutupt("Server Stopped!", OutputFlag.Exception);
                return;
            }
            
            Socket listener = new Socket(serverIp.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try { listener.Bind(serverIp); }
            catch (Exception e)
            {
                WriteOutupt("Unable to bind Server Endpoint" + e, OutputFlag.Exception);
                WriteOutupt("Server Stopped", OutputFlag.Exception);
                return;
            }

            listener.Listen(10);

            WriteOutupt("Server Online! Waiting Client", OutputFlag.Info);
            cons.ServerStatus = "Online";
            while (true)
            {
                doneAccept.Reset();
                listener.BeginAccept(AcceptCallBack, listener);
                doneAccept.WaitOne();
            }
        }
        private void AcceptCallBack(IAsyncResult ar)
        {
            doneAccept.Set();
            Dispatcher.Invoke(() => { WriteOutupt("Someone Connected, Pending Login!", OutputFlag.Info); });
            Socket sck = ((Socket)ar.AsyncState).EndAccept(ar);

            SocketState state = new SocketState() { currentSocket = sck };
            sck.BeginReceive(state.buffer, 0, 1024, 0, LoginCallBack, state);
        }
        private void LoginCallBack(IAsyncResult ar)
        {
            SocketState state = (SocketState)ar.AsyncState;
            Socket socket = state.currentSocket;
            int bufferSize = 0;
            try { bufferSize = socket.EndReceive(ar); }
            catch(Exception e)
            {
                WriteOutupt("Failed to Receive Data from user!, Client will be disconnected \n" + e, OutputFlag.Exception);
                state.currentSocket.Dispose();
                return;
            }
            

            string receivedData = Encoding.Default.GetString(state.buffer, 0, bufferSize);
            Dispatcher.Invoke(() => { WriteOutupt("Client Logging In! Receive data: \n" + receivedData, OutputFlag.Info); });

            string auth = AuthenticatingUser(receivedData);
            if(auth.Substring(0,5) == "Wrong")
            {
                WriteOutupt("Login Failed. Extra Information: " + auth + "Prompted to relogin", OutputFlag.Error);
                byte[] sendData = Encoding.Default.GetBytes(auth);
                socket.BeginSend(sendData, 0, sendData.Length, 0, ReLoginCallBack, socket);
            }
            else
            {
                WriteOutupt("Login Success", OutputFlag.Info);
                SocketState socketState = new SocketState();
                socketState.buffer = new byte[1024];
                socketState.currentSocket = socket;
                byte[] byteSend = Encoding.Default.GetBytes(auth);
                socket.BeginSend(byteSend, 0, byteSend.Length, 0, WaitRequestCallback, socketState);
            }

        }
        private void ReLoginCallBack(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;
            SocketState state = new SocketState();
            state.buffer = new byte[1024];
            state.currentSocket = socket;

            try { socket.BeginReceive(state.buffer, 0, state.buffer.Length, 0, LoginCallBack, state); }
            catch (Exception e)
            {
                WriteOutupt("Client Disconnected! Extra Information: \n" + e, OutputFlag.Exception);
            }
        }
        private string AuthenticatingUser(string data)
        {
            if(string.IsNullOrEmpty(data))
            {
                WriteOutupt("Client send a invalid data!", OutputFlag.Error);
                return "WrongUsername And Password";
            }
            string username = data.Split(':')[0];
            string password = data.Split(':')[1];
            string userdataFolder = ServerData + @"ServerData\UserCredential\" + username + @"\";
            if (Directory.Exists(userdataFolder))
            {
                FileStream fs = new FileStream(userdataFolder + @"\pass.txt", FileMode.Open);
                byte[] buffer = new byte[new FileInfo(userdataFolder + @"\pass.txt").Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();
                string userData = Encoding.Default.GetString(buffer);
                if(userData == password)
                {
                    FileStream fss = new FileStream(userdataFolder + @"\UserInfo.cfg", FileMode.Open);
                    byte[] sendbuffer = new byte[new FileInfo(userdataFolder + @"\UserInfo.cfg").Length];
                    fss.Read(sendbuffer, 0, sendbuffer.Length);
                    fss.Close();
                    return Encoding.Default.GetString(sendbuffer);
                }
                else
                {
                    return "WrongPassword";
                }
            }
            else
            {
                return "WrongUsername";
            }
        }
        private void WriteOutupt(string text, OutputFlag flag)
        {
            Dispatcher.Invoke(() =>
            {
                txbOutput.Text += DateTime.Now.ToShortTimeString() + " | " + flag.ToString() + " | " + text + "\n";
                ScrollOutput.ScrollToBottom();
            });
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Environment.Exit(0);
        }
        private void WaitRequestCallback(IAsyncResult ar)
        {

        }
    }

    public enum OutputFlag
    {
        Info, Warning, Error, Exception
    }

    public class SocketState
    {
        public Socket currentSocket;
        public byte[] buffer;
        public SocketState()
        {
            buffer = new byte[1024];
        }
    }
    public class Cons : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChange(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        private string serverStatus;
        public string ServerStatus
        {
            get { return serverStatus; }
            set
            {
                if(serverStatus != value)
                {
                    serverStatus = value;
                    NotifyPropertyChange("ServerStatus");
                }
            }
        }
        private int onlineUser;
        public int OnlineUser
        {
            get { return onlineUser; }
            set
            {
                if(onlineUser != value)
                {
                    onlineUser = value;
                    NotifyPropertyChange("OnlineUser");
                }
            }
        }
        public static string GetSettingValueFromFile(string orginalFile, string settingName)
        {
            try
            {
                return orginalFile.Substring(orginalFile.IndexOf(settingName)).Split('\r')[0].Split(':')[1];
            }
            catch
            {
                return "Can Parsing Setting From The Resources!";
            }
        }
    }

}


using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

namespace OneSub.Client
{
    public partial class MainWindow : Window
    {
        static Mutex mutex = new();
        const string outputFile = "log.txt";

        int Port { get; set; }

        public MainWindow(int port)
        {
            Port = port;
            InitializeComponent();

            Task.Run(Listen);
        }

        private void Listen()
        {
            using var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var localIP = new IPEndPoint(IPAddress.Loopback, Port);

            udpSocket.Bind(localIP);

            byte[] data = new byte[256];

            EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);

            while (true)
            {
                var result = udpSocket.ReceiveFrom(data, SocketFlags.None, ref remoteIp);
                string message = Encoding.UTF8.GetString(data, 0, result);

                mutex.WaitOne();
                File.AppendAllText(outputFile, $"{Port}: {message}\n");
                mutex.ReleaseMutex();

                if (message.StartsWith("[Currency]"))
                {
                    using var reader = new StringReader(message);
                    reader.ReadLine();
                    string euro = reader.ReadLine() ?? "0 0";
                    string usd = reader.ReadLine() ?? "0 0";
                    string[] euroarr = euro.Split(' ');
                    string[] usdarr = usd.Split(' ');

                    Dispatcher.Invoke(() => EuroSellBox.Text = euroarr[0]);
                    Dispatcher.Invoke(() => EuroBuyBox.Text = euroarr[1]);
                    Dispatcher.Invoke(() => UsdSellBox.Text = usdarr[0]);
                    Dispatcher.Invoke(() => UsdBuyBox.Text = usdarr[1]);
                }   
                else if (message.StartsWith("[Weather]"))
                {
                    using var reader = new StringReader(message);
                    string[] weatherArr = new string[5];
                    for (int i = 0; i < 5; i++)
                    {
                        weatherArr[i] = reader.ReadLine();
                    }
                    string[] temperature = weatherArr[1].Split(' ');
                    string[] feelsLike = weatherArr[2].Split(' ');
                    string[] pressure = weatherArr[3].Split(' ');
                    string[] humidity = weatherArr[4].Split(' ');

                    Dispatcher.Invoke(() => TemperatureBox.Text = temperature[1]);
                    Dispatcher.Invoke(() => FeelsLikeBox.Text = feelsLike[2]);
                    Dispatcher.Invoke(() => PressureBox.Text = pressure[1]);
                    Dispatcher.Invoke(() => HumidityBox.Text = humidity[1]);
                }
                else if (message.StartsWith("[Stocks]"))
                {
                    Debug.WriteLine(message);
                    using var reader = new StringReader(message);
                    string[] stocksArr = new string[3];
                    for (int i = 0; i < 3; i++)
                    {
                        stocksArr[i] = reader.ReadLine();
                    }

                    string[] appleArr = stocksArr[1].Split(' ');
                    string[] zoomArr = stocksArr[2].Split(' ');

                    Dispatcher.Invoke(() => AppleBox.Text = appleArr[1]);
                    Dispatcher.Invoke(() => AppleAverageTrades.Text = appleArr[2]);
                    Dispatcher.Invoke(() => ZoomBox.Text = zoomArr[1]);
                    Dispatcher.Invoke(() => ZoomAverageTrades.Text = zoomArr[2]);
                }  
                //else if (!message.StartsWith('['))
                  //  Dispatcher.Invoke(() => ServerMessageBox.Text += message);
            }
        }

        private void CurrecncyCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SendMessage($"{Port} Currency_On", 7999);
        }

        private void WeatherCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SendMessage($"{Port} Weather_On", 7999);
        }

        private void StocksCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            SendMessage($"{Port} Stocks_On", 7999);
        }

        private void CurrecncyCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
         
            SendMessage($"{Port} Currency_Off", 7999);
        }

        private void WeatherCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
         
            SendMessage($"{Port} Weather_Off", 7999);
        }

        private void StocksCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            SendMessage($"{Port} Stocks_Off", 7999);
        }

        private static void SendMessage(string message, int port)
        {
            var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            mutex.WaitOne();
            File.AppendAllText("log.txt", message + '\n');
            mutex.ReleaseMutex();
            byte[] data = Encoding.UTF8.GetBytes(message);
            EndPoint remotePoint = new IPEndPoint(IPAddress.Loopback, port);
            udpSocket.SendTo(data, SocketFlags.None, remotePoint);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SendMessage($"{Port} Currency_Off", 7999);
            SendMessage($"{Port} Weather_Off", 7999);
            SendMessage($"{Port} Stocks_Off", 7999);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace OneSub.Client
{
    public partial class MainWindow : Window
    {
        int Port { get; set; }
        bool currency;
        bool weather;
        bool stocks;
        public MainWindow(int port)
        {
            Port = port;
            InitializeComponent();

            Task.Run(Listen);
        }

        private void Listen()
        {
            while (true)
            {
                using var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                var localIP = new IPEndPoint(IPAddress.Loopback, Port);

                udpSocket.Bind(localIP);

                byte[] data = new byte[256];

                EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);

                var result = udpSocket.ReceiveFrom(data, SocketFlags.None, ref remoteIp);
                string message = Encoding.UTF8.GetString(data, 0, result);

                if (message.StartsWith("[Currency]") && currency)
                    Dispatcher.Invoke(() => MsgBox.Text += message);
                else if (message.StartsWith("[Weather]") && weather)
                    Dispatcher.Invoke(() => MsgBox.Text += message);
                else if (message.StartsWith("[Stocks]") && stocks)
                    Dispatcher.Invoke(() => MsgBox.Text += message);
                else if (!message.StartsWith('['))
                    Dispatcher.Invoke(() => MsgBox.Text += message);
            }
        }

        private void CurrecncyCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            currency = true;
        }

        private void WeatherCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            weather = true;
        }

        private void StocksCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            stocks = true;
        }

        private void CurrecncyCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            currency = false;
        }

        private void WeatherCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            weather = false;
        }

        private void StocksCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            stocks = false;
        }
    }
}

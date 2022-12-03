using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Printing;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;

namespace OneSub.Server;

public partial class MainWindow : Window
{
    private int newPort = 8000;

    class RequiredInfo
    {
        public bool currency, weather, stocks;
    }

    Dictionary<int, RequiredInfo> Infos = new();

    Timer currencyTimer = new(1000);
    Timer weatherTimer = new(1000);
    Timer stocksTimer = new(15000);


    public MainWindow()
    {
        InitializeComponent();

        if (File.Exists("log.txt"))
            File.Delete("log.txt");

        currencyTimer.Elapsed += async (s, e) => await SendCurrencyInfo();
        weatherTimer.Elapsed += async (s, e) => await SendWeatherInfo();
        stocksTimer.Elapsed += async (s, e) => await SendStocksInfo();

        Task.Run(Listen);
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Infos.Add(newPort, new RequiredInfo());
        Client.MainWindow newWnd = new(newPort++);
        newWnd.Show();
    }

    private void Listen()
    {
        using var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        var localIP = new IPEndPoint(IPAddress.Loopback, 7999);

        udpSocket.Bind(localIP);

        byte[] data = new byte[256];

        EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);

        while (true)
        {
            var result = udpSocket.ReceiveFrom(data, SocketFlags.None, ref remoteIp);
            string message = Encoding.UTF8.GetString(data, 0, result);

            

            string[] dataArr = message.Split();
            int port = int.Parse(dataArr[0]);
            string action = dataArr[1];

            if (action is "Currency_On")
                Infos[port].currency = true;
            else if (action is "Currency_Off")
                Infos[port].currency = false;
            else if (action is "Weather_On")
                Infos[port].weather = true;
            else if (action is "Weather_Off")
                Infos[port].weather = false;
            else if (action is "Stocks_On")
                Infos[port].stocks = true;
            else if (action is "Stocks_Off")
                Infos[port].stocks = false;

        }
    }

    private static void SendMessage(string message, int port)
    {
        var udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        byte[] data = Encoding.UTF8.GetBytes(message);
        EndPoint remotePoint = new IPEndPoint(IPAddress.Loopback, port);
        udpSocket.SendTo(data, SocketFlags.None, remotePoint);
    }

    private async Task SendCurrencyInfo()
    {
        using HttpClient httpClient = new HttpClient();
        var response = await httpClient.GetAsync("https://api.privatbank.ua/p24api/pubinfo?exchange&coursid=5");

        using (var responseStreamReader = new StreamReader(response.Content.ReadAsStream()))
        {
            var responseString = responseStreamReader.ReadToEnd();
            JArray currencyInfo = JArray.Parse(responseString);

            string outString = string.Join("\n", currencyInfo.Select(curr => GetDelta(double.Parse(curr["buy"]?.ToString() ?? "0", CultureInfo.InvariantCulture), 0.2).ToString() + " " + GetDelta(double.Parse(curr["sale"]?.ToString() ?? "0", CultureInfo.InvariantCulture), 0.2).ToString()));
            SendMessageToAllClients("[Currency]\n" + outString + "\n", port => Infos[port].currency);
        }
    }

    private async Task SendWeatherInfo()
    { 

        HttpClient httpClient = new HttpClient();
  
        var response = await httpClient.GetAsync("https://api.openweathermap.org/data/2.5/forecast?lat=49.85&lon=23.99&units=metric&appid=3817dc2be2f056779006459f56dda912");

        using (var reader = new StreamReader(response.Content.ReadAsStream()))
        {
            var str = reader.ReadToEnd();
            var obj = JObject.Parse(str);
            
            string outString = "Temperature: " + obj?["list"]?.First()["main"]?["temp"] + "\nFeels like: " + obj?["list"]?.First()["main"]?["feels_like"] + "\nPressure: " + obj?["list"]?.First()["main"]?["pressure"] + "\nHumidity: " + obj?["list"]?.First()["main"]?["humidity"];
            SendMessageToAllClients("[Weather]\n" + outString + "\n" ?? "[Weather] Can`t get info about weather\n", port => Infos[port].weather);
        }
    }

    private async Task SendStocksInfo()
    {
        HttpClient httpClient = new HttpClient();
        var response = await httpClient.GetAsync("https://api.twelvedata.com/time_series?apikey=562c558816244c41b7681a6b16dc50b5&interval=1min&dp=1&type=stock&symbol=AAPL,ZM&format=JSON");
        response.EnsureSuccessStatusCode();
        using (var reader = new StreamReader(response.Content.ReadAsStream()))
        {
            var str = reader.ReadToEnd();
            var obj = JObject.Parse(str);
            try
            {
                double stockAppl = GetDelta(double.Parse(obj?["AAPL"]?["values"]?.First()["open"]?.ToString() ?? "0", CultureInfo.InvariantCulture), 1);

                int volumeAppl = (int)GetDelta(double.Parse(obj?["AAPL"]?["values"]?.First()["volume"]?.ToString() ?? "0", CultureInfo.InvariantCulture), 5);

                double stockZm = GetDelta(double.Parse(obj?["ZM"]?["values"]?.First()["open"]?.ToString() ?? "0", CultureInfo.InvariantCulture), 1);

                int volumeZm = (int)GetDelta(double.Parse(obj?["ZM"]?["values"]?.First()["volume"]?.ToString() ?? "0"), 5);

                string outString = "Apple: " + stockAppl.ToString() + "$ " + volumeAppl.ToString() + "\n"
                    + "Zoom: " + stockZm.ToString() + "$ " + volumeZm.ToString() + "\n";

                SendMessageToAllClients("[Stocks]\n" + outString + "\n" ?? "[Stocks] Can`t get info about stocks\n", port => Infos[port].stocks);
            }
            catch (Exception)
            {
                SendMessageToAllClients("[Stocks] Can`t get info about stocks\n", port => Infos[port].stocks);
            }
        }
    }

    private void SendMessageToAllClients(string message, Predicate<int> pred)
    {
        for (int port = 8000; port < newPort; port++)
            if(pred(port))
                SendMessage(message, port);
    }

    private void Button_Click_2(object sender, RoutedEventArgs e)
    {
        currencyTimer.Start();
        weatherTimer.Start();
        stocksTimer.Start();
    }

    private void Button_Click_3(object sender, RoutedEventArgs e)
    {
        currencyTimer.Stop();
        weatherTimer.Stop();
        stocksTimer.Stop();
    }

    private static int GetFromString(string str) => str switch
    {
        "Every second" => 1000,
        "Every 15 seconds" => 15000,
        "Every 30 seconds" => 30000
        
    };

    private void SelectionChanged1(object sender, SelectionChangedEventArgs e)
    {
        currencyTimer.Interval = GetFromString(CurrencyComboBox.SelectedValue as string ?? " ");
    }

    private static double GetDelta(double num, double percent)
    {
        return Math.Round(num + new Random().NextDouble() * num / (100 / percent) - num / (100 / percent), 2);
    }
}

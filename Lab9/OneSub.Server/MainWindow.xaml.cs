using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Printing;
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

    Timer currencyTimer = new(1000);
    Timer weatherTimer = new(1000);
    Timer stocksTimer = new(1000);


    public MainWindow()
    {
        InitializeComponent();


        currencyTimer.Elapsed += async (s, e) => await SendCurrencyInfo();
        weatherTimer.Elapsed += async (s, e) => await SendWeatherInfo();
        stocksTimer.Elapsed += async (s, e) => await SendStocksInfo();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        Client.MainWindow newWnd = new(newPort++);
        newWnd.Show();
    }

    private void Button_Click_1(object sender, RoutedEventArgs e)
    {
        SendMessageToAllClients(CustomTextBox.Text + '\n');
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

        Debug.WriteLine("Here");

        using (var responseStreamReader = new StreamReader(response.Content.ReadAsStream()))
        {
            var responseString = responseStreamReader.ReadToEnd();
            JArray currencyInfo = JArray.Parse(responseString);
            try
            {
                string outString = string.Join("\n", currencyInfo.Select(curr => curr["ccy"]?.ToString() + " - " + GetDelta(double.Parse(curr["buy"]?.ToString()), 0.2) + " ; " + GetDelta(double.Parse(curr["sale"]?.ToString()),0.2)));
                SendMessageToAllClients("[Currency]\n" + outString + "\n" ?? "[Currency] Can`t get info about currencies\n");
            }
            catch (Exception)
            {
                SendMessageToAllClients("[Currency] Can`t get info about currencies\n");
            }
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
            string outString = "Tempeture: " + obj?["list"]?.First()["main"]?["temp"] + "\nFeels like: " + obj?["list"]?.First()["main"]?["feels_like"] + "\nPressure: " + obj?["list"]?.First()["main"]?["pressure"] + "\nHumidity: " + obj?["list"]?.First()["main"]?["humidity"];
            SendMessageToAllClients("[Weather]\n" + outString + "\n" ?? "[Weather] Can`t get info about weather\n");
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
                double stockAppl = GetDelta(double.Parse(obj?["AAPL"]?["values"]?.First()["open"]?.ToString()), 1);
                int volumeAppl = (int)GetDelta(double.Parse(obj?["AAPL"]?["values"]?.First()["volume"]?.ToString()), 5);
                double stockZm = GetDelta(double.Parse(obj?["ZM"]?["values"]?.First()["open"]?.ToString()), 1);
                int volumeZm = (int)GetDelta(double.Parse(obj?["ZM"]?["values"]?.First()["volume"]?.ToString()), 5);
                string outString = "Apple: " + stockAppl.ToString() + "$, traded in one minute:" + volumeAppl.ToString() + "\n"
                    + "Zoom:  " + stockZm.ToString() + "$, traded in one minute:" + volumeZm.ToString() + "\n";
                SendMessageToAllClients("[Stocks]\n" + outString + "\n" ?? "[Stocks] Can`t get info about stocks\n");
            }
            catch (Exception)
            {
                SendMessageToAllClients("[Stocks] Can`t get info about stocks\n");
            }
        }
    }

    private void SendMessageToAllClients(string message)
    {
        for (int port = 8000; port < newPort; port++)
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
        "Щосекунди" => 1000,
        "Щохвилини" => 60000,
        "Щогодини" => 3600000,
        _ => throw new NotImplementedException()
    };

    private void SelectionChanged1(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        currencyTimer.Interval = GetFromString(CurrencyComboBox.SelectedValue as string);
    }

    private static double GetDelta(double num, double percent)
    {
        return Math.Round(num + new Random().NextDouble() * num / (100 / percent) - num / (100 / percent), 2);
    }
}

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


        currencyTimer.Elapsed += async (s, e) => { await SendCurrencyInfo(); }; //TODO: change to variable
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

            string ourString = string.Join("\n", currencyInfo.Select(curr => curr["ccy"]?.ToString() + " - " + curr["buy"]?.ToString() + " ; " + curr["sale"]?.ToString()));

            SendMessageToAllClients("[Currency]\n" + ourString + "\n" ?? "[Currency] Can`t get info about currencies\n");
        }

        
    }
    
    private async Task SendWeatherInfo()
    {
        string apiKey = "084581ec-6d83-11ed-bc36-0242ac130002-0845825a-6d83-11ed-bc36-0242ac130002";

        HttpClient httpClient = new HttpClient();

        httpClient.DefaultRequestHeaders.Add("Authorization", apiKey);

        var response = await httpClient.GetAsync("https://api.stormglass.io/v2/weather/point?lat=49&lng=24&params=airTemperature");

        using (var reader = new StreamReader(response.Content.ReadAsStream()))
        {
            var str = reader.ReadToEnd();

            Debug.WriteLine(str);

            //var obj = JObject.Parse(str)["hours"];

            //SendMessageToAllClients("[Weather]\nLviv - " + obj?.First()["airTemperature"]?["noaa"] + "°C ; " + obj?.ElementAt(19)["airTemperature"]?["noaa"] + "°C\n");

            SendMessageToAllClients("[Weather]\nLviv - 1.5°C ; 0.5°C\n");
        }
    }

    //TODO
    private async Task SendStocksInfo()
    {
        throw new NotImplementedException();
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
        //stocksTimer.Start();
    }

    private void Button_Click_3(object sender, RoutedEventArgs e)
    {
        currencyTimer.Stop();
        weatherTimer.Stop();
        //stocksTimer.Stop();
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
}

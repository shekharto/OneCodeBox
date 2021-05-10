using System;
using System.Device.Gpio;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.Devices.Client.Exceptions;
using Newtonsoft.Json;

namespace ConsoleApp1
{
    class Program
    {
        //private static DeviceClient s_deviceClient;
        //private readonly static string s_connectionString01 = "HostName=HubflyIoTHubConnect.azure-devices.net;DeviceId=RaspberryPi;SharedAccessKey=b9g+mmjAV8SqBlv8o/TChP0WBFCL5wi8/pDccXzBoys=";

        private const string IotHubUri = "RemoteIotHome.azure-devices.net";
        private const string DeviceKey = "xLmltL7DX/O1zLpjMLqbpiHCiOlu6AYxNjxn1LUl/gs=";
        private const string DeviceId = "IotHomeSP";
        private const int Pin = 2;
         
        private static CancellationToken _ct;
        private static DeviceClient s_deviceClient;
        private readonly static string s_connectionString01 = "HostName=RemoteIotHome.azure-devices.net;DeviceId=IotHomeSP;SharedAccessKey=xLmltL7DX/O1zLpjMLqbpiHCiOlu6AYxNjxn1LUl/gs=";

        static void Main(string[] args)
        {

            Console.WriteLine("Hello From Raspberry Pi...");
          //  s_deviceClient = DeviceClient.CreateFromConnectionString(s_connectionString01, TransportType.Mqtt);
          //  SendDeviceToCloudMessagesAsync(s_deviceClient);

            Console.ReadKey();
        }

        private static async Task Run()
        {
            using var deviceClient = DeviceClient.Create(IotHubUri, new DeviceAuthenticationWithRegistrySymmetricKey(DeviceId, DeviceKey));
            using var controller = new GpioController();
            controller.OpenPin(Pin, PinMode.Output);

            Console.WriteLine($"{DateTime.Now} > Connected to the best cloud on the planet.");
            Console.WriteLine($"Azure IoT Hub: {IotHubUri}");
            Console.WriteLine($"Device ID: {DeviceId}");

            Console.WriteLine($"{DateTime.Now} > GPIO pin enabled for use: {Pin}");

            while (!_ct.IsCancellationRequested)
            {
                Console.WriteLine($"{DateTime.Now} > Waiting new message from Azure...");
                var receivedMessage = await deviceClient.ReceiveAsync(_ct);
                if (receivedMessage == null) continue;

                var msg = Encoding.ASCII.GetString(receivedMessage.GetBytes());
                Console.WriteLine($"{DateTime.Now} > Received message: {msg}");

                switch (msg)
                {
                    case "on":
                        Console.WriteLine($"{DateTime.Now} > Turn on the light.");
                        controller.Write(Pin, PinValue.High);
                        break;
                    case "off":
                        Console.WriteLine($"{DateTime.Now} > Turn off the light.");
                        controller.Write(Pin, PinValue.Low);
                        break;
                    default:
                        Console.WriteLine($"Unknown command: {msg}");
                        break;
                }

                await deviceClient.CompleteAsync(receivedMessage, _ct);
            }
        }

        private static async void SendDeviceToCloudMessagesAsync(DeviceClient s_deviceClient)
        {
              var gpio = new GpioController();
            if (gpio == null)
            {
                Console.WriteLine($"{DateTime.Now} - There is no GPIO controller on this device.");
                return;
            }

            gpio.OpenPin(Pin, PinMode.Output);
            gpio.Write(Pin, PinValue.High);
            // string messageString = "on";
            //  messageString = JsonConvert.SerializeObject(telemetryDataPoint);
            gpio.Write(Pin, PinValue.Low);
            // var message = new Message(Encoding.ASCII.GetBytes("on"));
            // await s_deviceClient.SendEventAsync(message);

            Console.WriteLine($"{DateTime.Now} > Connected to the best cloud on the planet.");




        }
    }
}

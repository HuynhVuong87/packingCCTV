using System;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Management;
using System.Linq;

namespace PackingCCTV
{
    static class Program
    {
        static bool _continue = true;
        static SerialPort port = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);
        static string currentPath = AppDomain.CurrentDomain.BaseDirectory;
        static System.Media.SoundPlayer player;
     
        [STAThread]
        static void Main()
        {            

            // pause program execution to review results...
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

            //rtsp://demo:admin123@nguyenchanh.cameraddns.net:1554/Streaming/Channels/104/
            //port.Handshake = Handshake.None;
            //string message;
            //StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;
            //port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            //try
            //{
            //    port.Open();
                
            //}
            //catch (IOException ex)
            //{
            //    MessageBox.Show("Chưa kết nối máy quét mã vạch (port COM3)");
            //    return;
            //}
            
            //while (_continue)
            //{
            //    message = Console.ReadLine();

            //    if (stringComparer.Equals("quit", message))
            //    {
            //        _continue = false;
            //    }
            //    else
            //    {
            //        port.WriteLine(message);
            //    }
            //}
            //port.Close();
            //port.Dispose();
        }

        //private static void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        //{
        //    string dataReceived = port.ReadExisting().Trim();
        //    captureCCTV("rtsp://admin:abcd1234@192.168.1.235:554/Streaming/Channels/401/", dataReceived);
        //}

        private static void sendPhoto(string IdOrder, string imageName)
        {
            string output = string.Empty;
            string error = string.Empty;

            System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + currentPath + "ChromeShopeeSendPhoto.exe " + IdOrder + " " + imageName);
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            processStartInfo.UseShellExecute = false;

            System.Diagnostics.Process process = System.Diagnostics.Process.Start(processStartInfo);
            using (StreamReader streamReader = process.StandardOutput)
            {
                output = streamReader.ReadToEnd();
            }

            using (StreamReader streamReader = process.StandardError)
            {
                error = streamReader.ReadToEnd();
            }

            Console.WriteLine("The following output was detected:");
            Console.WriteLine(output);
            if (!string.IsNullOrEmpty(error))
            Console.WriteLine(process.ExitCode);
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine(error);
            }

            switch (process.ExitCode)
            {
                case 0:
                    player = new System.Media.SoundPlayer(currentPath + "audio\\sentPhoto.wav");
                    player.Play();
                    break;
                default:
                    player = new System.Media.SoundPlayer(currentPath + "audio\\eSentPhoto.wav");
                    player.Play();
                    switch (process.ExitCode)
                    {
                        case 1:
                            player = new System.Media.SoundPlayer(currentPath + "audio\\e1.wav");
                            player.Play();
                            break;
                        case 2:
                            player = new System.Media.SoundPlayer(currentPath + "audio\\e2.wav");
                            player.Play();
                            break;
                        case 3:
                            player = new System.Media.SoundPlayer(currentPath + "audio\\e3.wav");
                            player.Play();
                            break;
                        case 4:
                            player = new System.Media.SoundPlayer(currentPath + "audio\\e4.wav");
                            player.Play();
                            break;
                    }
                    break;
            }
            
        }

       
    }
}

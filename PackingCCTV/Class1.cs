using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.IO.Ports;
using System.IO;

namespace PackingCCTV
{
    public class SerialPortManager
    {
        public SerialPort port;
        static int check = 0;
        static bool _continue = true;
        static string currentPath = AppDomain.CurrentDomain.BaseDirectory;
        static System.Media.SoundPlayer player;
        public SerialPortManager(string name)
        {
            this.port = new SerialPort(name, 9600, Parity.None, 8, StopBits.One);
        }
        public void Open()
        {
            Close();
            this.port.DataReceived += new SerialDataReceivedEventHandler(port_DataReceived);
            this.port.Open();
        }
        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string dataReceived = this.port.ReadExisting().Trim();
            captureCCTV("rtsp://admin:abcd1234@192.168.1.235:554/Streaming/Channels/401/", dataReceived);
        }
        private static void captureCCTV(string rtsp, string IdOrder)
        {
            string output = string.Empty;
            string error = string.Empty;
            string imgPath = currentPath + "CCTVPhotos\\" + IdOrder + "--" + DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss") + ".jpg";
            System.Diagnostics.ProcessStartInfo processStartInfo = new System.Diagnostics.ProcessStartInfo("cmd", "/c " + currentPath + "ffmpeg.exe -rtsp_transport tcp -i rtsp://admin:abcd1234@192.168.1.235:554/Streaming/Channels/401/ -y -xerror -vframes 1 " + imgPath);
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Minimized;
            processStartInfo.UseShellExecute = false;
            processStartInfo.CreateNoWindow = true;

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
            {
                Console.WriteLine(error);
                if (error.Contains("Error"))
                {
                    if(check < 5)
                    {
                        check++;
                        captureCCTV(rtsp, IdOrder);
                    }
                    else
                    {
                        player = new System.Media.SoundPlayer(currentPath + "audio\\retakePhoto.wav");
                        player.Play();
                    }
                    
                }
                else
                {
                    player = new System.Media.SoundPlayer(currentPath + "audio\\cameraShutter.wav");
                    player.Play();
                    //sendPhoto(IdOrder, imgPath);
                }

            }
            else
            {
                //co loi khong xac dinh
            }
        }
        public void Close()
        {
            if (this.port != null)
            {
                if (port.IsOpen)
                {
                    port.Close();
                }
            }
        }

        
    }
}

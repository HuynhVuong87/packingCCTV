using System;
using System.Windows.Forms;
using System.Management;
using System.IO.Ports;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Win32;

namespace PackingCCTV
{
    public partial class Form1 : Form
    {
        public SerialPortManager serialPortManager;
        RegistryKey rk = Registry.CurrentUser.OpenSubKey
            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.DataSource = getListPort();
            rk.SetValue("AutoTakePhoto", Application.ExecutablePath);
        }

        private void btnOpenSerialPort_Click(object sender, EventArgs e)
        {
            if(comboBox1.SelectedValue != null)
            {
                serialPortManager = new SerialPortManager(comboBox1.SelectedValue.ToString().Split(' ').First());
                serialPortManager.Open();

                btnOpenSerialPort.Enabled = !serialPortManager.port.IsOpen;
                
            }
            
            Console.WriteLine(comboBox1.SelectedValue);
        }


        public static List<string> getListPort()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE Caption like '%(COM%'"))
            {
                var portnames = SerialPort.GetPortNames();
                var ports = searcher.Get().Cast<ManagementBaseObject>().ToList().Select(p => p["Caption"].ToString());

                var portList = portnames.Select(n => n + " - " + ports.FirstOrDefault(s => s.Contains(n))).ToList();
                return portList;
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            serialPortManager.Close();
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
                notifyIcon1.Visible = true;
            }
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }
        
    }
}

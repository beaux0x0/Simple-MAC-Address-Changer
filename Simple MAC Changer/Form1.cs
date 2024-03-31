using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Simple_MAC_Changer
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();

        [DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse);

        public Form1()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.None;
            Region = System.Drawing.Region.FromHrgn(CreateRoundRectRgn(0, 0, Width, Height, 20, 20));
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0x112, 0xf012, 0);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void chngBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Starting...");
            Log("The change process has begun.");
            ChangeMacAddress();
            Log("The amendment process is complete.");
            MessageBox.Show("Operation Successfully Completed.");
        }
        private void Log(string message)
        {
            string logFilePath = "changelog.txt";
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"[{DateTime.Now}] {message}");
            }
        }

        private void ChangeMacAddress()
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "wmic.exe",
                Arguments = "nic where physicaladapter=true get deviceid",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                process.WaitForExit();

                var output = process.StandardOutput.ReadToEnd();
                var matches = Regex.Matches(output, @"\d+");
                foreach (Match match in matches)
                {
                    var deviceId = match.Value;
                    var macAddress = GenerateRandomMac();
                    SetMacAddress(deviceId, macAddress);
                    DisablePowerSavingMode(deviceId);
                    Log($"MAC address changed: Device ID: {deviceId}, New MAC: {macAddress}");
                }
            }

            ResetNetworkAdapters();
        }

        private string GenerateRandomMac()
        {
            var chars = "0123456789ABCDEF";
            var random = new Random();
            var mac = "";
            for (int i = 0; i < 12; i++)
            {
                mac += chars[random.Next(chars.Length)];
            }
            // :
            return mac;
        }

        private void SetMacAddress(string deviceId, string macAddress)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "reg.exe",
                Arguments = $"add HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Class\\{{4D36E972-E325-11CE-BFC1-08002bE10318}}\\{deviceId} /v NetworkAddress /t REG_SZ /d {macAddress} /f",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                process.WaitForExit();
            }
        }

        private void DisablePowerSavingMode(string deviceId)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "reg.exe",
                Arguments = $"add HKEY_LOCAL_MACHINE\\SYSTEM\\CurrentControlSet\\Control\\Class\\{{4D36E972-E325-11CE-BFC1-08002bE10318}}\\{deviceId} /v PnPCapabilities /t REG_DWORD /d 24 /f",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                process.WaitForExit();
            }
        }

        private void ResetNetworkAdapters()
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = "netsh.exe",
                Arguments = "interface reset",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = Process.Start(processInfo))
            {
                process.WaitForExit();
            }
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0x112, 0xf012, 0);
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }
    }
}

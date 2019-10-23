using System;
using System.IO;
using System.ServiceProcess;
using System.Timers;

namespace WindowsServiceToRunAppAsAdmin
{
    public partial class ServiceToRunApp : ServiceBase
    {
        Timer timer = new Timer();
        public ServiceToRunApp()
        {
            InitializeComponent();
        }
        protected override void OnStart(string[] args)
        {
            WriteToFile("Service is started at " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Interval = 60000; //number in mili-seconds  
            timer.Enabled = true;
        }
        protected override void OnStop()
        {
            WriteToFile("Service is stopped at " + DateTime.Now);
        }
        private void OnElapsedTime(object source, ElapsedEventArgs e)
        {
            WriteToFile("Service is recall at " + DateTime.Now);

            string applicationName = "cmd.exe";
            ApplicationLoader.PROCESS_INFORMATION procInfo;
            bool isAppCreated = ApplicationLoader.StartProcessAndBypassUAC(applicationName, "c:\\windows\\system32\\", out procInfo);

            WriteToFile("App has been launched in Admin Privilege is " + isAppCreated);

            timer.Enabled = false;
        }
        public void WriteToFile(string Message)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "\\Logs";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ServiceLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }
    }
}

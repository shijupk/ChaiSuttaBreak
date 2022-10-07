using System;
using System.Drawing;
using System.Windows.Forms;
namespace ChaiSuttaBreak
{
    class TrayContext : ApplicationContext
    {
        /// <summary>
        /// Interval between timer ticks (in ms) to refresh Windows idle timers. Shouldn't be too small to avoid resources consumption. Must be less then Windows screensaver/sleep timer.
        /// Default = 10 000 ms (10 seconds).
        /// </summary>
        const int RefreshInterval = 10000;
        /// <summary>
        /// ExecutionMode defines how blocking is made. See details at https://msdn.microsoft.com/en-us/library/aa373208.aspx?f=255&MSPPError=-2147217396
        /// </summary>
        const EXECUTION_STATE ExecutionMode = EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_AWAYMODE_REQUIRED;

        // PRIVATE VARIABLES
        private NotifyIcon TrayIcon;
        private Timer RefreshTimer;
        private bool IsRunning = true;
        readonly Image GreenIcon = Properties.Resource1.Green;
        readonly Image OrangeIcon = Properties.Resource1.Orange;
        readonly Image ResumeIcon = Properties.Resource1.start;
        readonly Image SuspendIcon = Properties.Resource1.pause;
        readonly Image ExitIcon = Properties.Resource1.quit;



        public TrayContext()
        {
            // Initialize application
            Application.ApplicationExit += this.OnApplicationExit;
            InitializeComponent();
            TrayIcon.Visible = true;


            // Set timer to tick to refresh idle timers
            RefreshTimer = new Timer() { Interval = RefreshInterval, Enabled = true };
            RefreshTimer.Tick += RefreshTimer_Tick;

        }


        private void InitializeComponent()
        {
            // Initialize Tray icon
            TrayIcon = new NotifyIcon();
            TrayIcon.BalloonTipIcon = ToolTipIcon.None;
            TrayIcon.BalloonTipText = "Feel free Sneak out, I will Keep this PC active!";
            TrayIcon.BalloonTipTitle = "Chai Sutta Break";
            TrayIcon.Text = "It's time for Chai Sutta ?";
            TrayIcon.Icon = Properties.Resource1.ChaiSutta;
            TrayIcon.DoubleClick += TrayIcon_DoubleClick;
            TrayIcon.MouseClick += TrayIcon_Click;

            UpdateTrayMenu();
        }


        /// <summary>
		/// Changes the tray menu to reflect the current status
		/// </summary>
		public void UpdateTrayMenu()
        {
            var activeMenuItem = new ToolStripMenuItem();
            var ResumeMenuItem = new ToolStripMenuItem();
            var ExitMenuItem = new ToolStripMenuItem();
            var toolStripStatusLabel = new ToolStripLabel();


            if (IsRunning)
            {
                toolStripStatusLabel.Text = "   Active";
                toolStripStatusLabel.Image = GreenIcon;

                ResumeMenuItem.Text = "Suspend";
                ResumeMenuItem.Image = SuspendIcon;
            }
            else
            {
                toolStripStatusLabel.Text = "   Suspended";
                toolStripStatusLabel.Image = OrangeIcon;

                ResumeMenuItem.Text = "Resume";
                ResumeMenuItem.Image = ResumeIcon;

            }

            ResumeMenuItem.Click += this.StartStopMenuItem_Click;
            ExitMenuItem.Text = "Exit                       ";
            ExitMenuItem.Image = ExitIcon;
            ExitMenuItem.Click += this.CloseMenuItem_Click;

            var contextMenuStrip = new ContextMenuStrip();


            contextMenuStrip.Items.Add(toolStripStatusLabel);
            contextMenuStrip.Items.Add(new ToolStripSeparator());
            contextMenuStrip.Items.Add(ResumeMenuItem);
            contextMenuStrip.Items.Add(ExitMenuItem);

            TrayIcon.ContextMenuStrip = contextMenuStrip;

        }


        private void OnApplicationExit(object sender, EventArgs e)
        {
            // Clean up things on exit
            TrayIcon.Visible = false;
            RefreshTimer.Enabled = false;
            // Clean up continuous state, if required
            if (ExecutionMode.HasFlag(EXECUTION_STATE.ES_CONTINUOUS)) WindowsUtility.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        }

        private void StartStopMenuItem_Click(object sender, EventArgs e)
        {
            if (IsRunning)
            {
                IsRunning = false;
                RefreshTimer.Stop();
            }
            else
            {
                IsRunning = true;
                RefreshTimer.Start();
            }

            UpdateTrayMenu();
        }
        private void TrayIcon_DoubleClick(object sender, EventArgs e) { TrayIcon.ShowBalloonTip(10000); }
        private void TrayIcon_Click(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Left)
                TrayIcon.ShowBalloonTip(10000);
        }
        private void CloseMenuItem_Click(object sender, EventArgs e) { Application.Exit(); }
        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            WindowsUtility.SetThreadExecutionState(ExecutionMode);
        }
    }
}

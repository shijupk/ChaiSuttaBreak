using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace ChaiSuttaBreak
{
    internal class TrayContext : ApplicationContext
    {
        /// <summary>
        /// Interval between timer ticks (in ms) to refresh Windows idle timers. Shouldn't be too small to avoid resources consumption. Must be less then Windows screensaver/sleep timer.
        /// Default = 10 000 ms (10 seconds).
        /// </summary>
        private const int RefreshInterval = 10000;
        /// <summary>
        /// ExecutionMode defines how blocking is made. See details at https://msdn.microsoft.com/en-us/library/aa373208.aspx?f=255&MSPPError=-2147217396
        /// </summary>
        private const EXECUTION_STATE ExecutionMode = EXECUTION_STATE.ES_CONTINUOUS | EXECUTION_STATE.ES_DISPLAY_REQUIRED | EXECUTION_STATE.ES_SYSTEM_REQUIRED | EXECUTION_STATE.ES_AWAYMODE_REQUIRED;

        // PRIVATE VARIABLES
        private NotifyIcon _trayIcon;
        private Timer _refreshTimer;
        private bool _isRunning = true;
        private readonly Image _greenIcon = Properties.IconResources.Green;
        private readonly Image _orangeIcon = Properties.IconResources.Orange;
        private readonly Image _resumeIcon = Properties.IconResources.start;
        private readonly Image _suspendIcon = Properties.IconResources.pause;
        private readonly Image _exitIcon = Properties.IconResources.quit;



        public TrayContext()
        {
            // Initialize application
            Application.ApplicationExit += this.OnApplicationExit;
            InitializeComponent();
            _trayIcon.Visible = true;


            // Set timer to tick to refresh idle timers
            _refreshTimer = new Timer() { Interval = RefreshInterval, Enabled = true };
            _refreshTimer.Tick += RefreshTimer_Tick;

        }


        private void InitializeComponent()
        {
            // Initialize Tray icon
            _trayIcon = new NotifyIcon();
            _trayIcon.BalloonTipIcon = ToolTipIcon.None;
            _trayIcon.BalloonTipText = "Feel free Sneak out, I will Keep this PC active!";
            _trayIcon.BalloonTipTitle = "Chai Sutta Break";
            _trayIcon.Text = "It's time for Chai Sutta ?";
            _trayIcon.Icon = Properties.IconResources.ChaiSutta;
            _trayIcon.DoubleClick += TrayIcon_DoubleClick;

            UpdateTrayMenu();
            GlobalKeyboardHook.Instance.Hook(new List<System.Windows.Input.Key> { System.Windows.Input.Key.LeftShift, System.Windows.Input.Key.B },
            () =>
            {
                Console.WriteLine("A-B");
                ToggleAction();
            }, out var message);



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


            if (_isRunning)
            {
                toolStripStatusLabel.Text = "   Active";
                toolStripStatusLabel.Image = _greenIcon;

                ResumeMenuItem.Text = "Suspend";
                ResumeMenuItem.Image = _suspendIcon;
            }
            else
            {
                toolStripStatusLabel.Text = "   Suspended";
                toolStripStatusLabel.Image = _orangeIcon;

                ResumeMenuItem.Text = "Resume";
                ResumeMenuItem.Image = _resumeIcon;

            }

            ResumeMenuItem.Click += this.StartStopMenuItem_Click;
            ExitMenuItem.Text = "Exit                       ";
            ExitMenuItem.Image = _exitIcon;
            ExitMenuItem.Click += this.CloseMenuItem_Click;

            var contextMenuStrip = new ContextMenuStrip();


            contextMenuStrip.Items.Add(toolStripStatusLabel);
            contextMenuStrip.Items.Add(new ToolStripSeparator());
            contextMenuStrip.Items.Add(ResumeMenuItem);
            contextMenuStrip.Items.Add(ExitMenuItem);

            _trayIcon.ContextMenuStrip = contextMenuStrip;

        }


        private void OnApplicationExit(object sender, EventArgs e)
        {
            // Clean up things on exit
            _trayIcon.Visible = false;
            _refreshTimer.Enabled = false;
            // Clean up continuous state, if required
            if (ExecutionMode.HasFlag(EXECUTION_STATE.ES_CONTINUOUS)) WindowsUtility.SetThreadExecutionState(EXECUTION_STATE.ES_CONTINUOUS);
        }

        private void StartStopMenuItem_Click(object sender, EventArgs e)
        {
            ToggleAction();
        }

        private void ToggleAction()
        {
            if (_isRunning)
            {
                _isRunning = false;
                _refreshTimer.Stop();
                _trayIcon.ShowBalloonTip(10000,"Chai Sutta Break","Break mode suspended!",ToolTipIcon.Info);
            }
            else
            {
                _isRunning = true;
                _refreshTimer.Start();
                _trayIcon.ShowBalloonTip(10000, "Chai Sutta Break", "Break mode resumed!", ToolTipIcon.Info);

            }

            UpdateTrayMenu();
        }
        private void TrayIcon_DoubleClick(object sender, EventArgs e) { _trayIcon.ShowBalloonTip(10000); }

        private void CloseMenuItem_Click(object sender, EventArgs e) { Application.Exit(); }
        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            SendKeys.Send("{SCROLLLOCK}");
            SendKeys.Send("{NUMLOCK}");
            WindowsUtility.SetThreadExecutionState(ExecutionMode);
            SendKeys.Send("{SCROLLLOCK}");
            SendKeys.Send("{NUMLOCK}");

        }
    }
}

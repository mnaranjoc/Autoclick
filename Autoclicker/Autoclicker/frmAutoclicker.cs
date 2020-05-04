using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace Autoclicker
{
    public partial class frmAutoclicker : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern bool GetCursorPos(out Point lpPoint);

        int x, y;
        [DllImport("User32.Dll")]
        public static extern long SetCursorPos(int x, int y);

        [DllImport("User32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, uint dwExtraInfo);

        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
        const uint KEYEVENTF_EXTENDEDKEY = 0x0001;
        const int VK_DELETE = 0x2E;
        const int VK_F5 = 0x74;

        public static bool actionsCaptured, captureModeEnabled;
        public static int actionIndex;
        
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // All actions have been captured and user wants to start executing the actions.
            if (actionsCaptured)
            {
                if (e.KeyCode == Keys.F4)
                {
                    lblStatus.Text = "Running";
                    timer1.Interval = 1000;
                    timer1.Start();
                }
                else if (e.KeyCode == Keys.F5)
                {
                    lblStatus.Text = "Stopped";
                    timer1.Stop();
                    actionIndex = 0;
                }
            }
            else
            {
                // User starts capturing the actions.
                if (captureModeEnabled)
                {
                    // Cursor coordenates
                    if (e.KeyCode == Keys.C || e.KeyCode == Keys.R || e.KeyCode == Keys.D)
                    {
                        Point newPoint = new Point();
                        GetCursorPos(out newPoint);
                        int x = newPoint.X;
                        int y = newPoint.Y;

                        if (e.KeyCode == Keys.C)
                        {
                            actionsListBox.Items.Add(string.Format("click,{0},{1}", x, y));
                        }
                        else if (e.KeyCode == Keys.R)
                        {
                            actionsListBox.Items.Add(string.Format("rclick,{0},{1}", x, y));
                        }
                        else if (e.KeyCode == Keys.D)
                        {
                            actionsListBox.Items.Add(string.Format("delete,{0},{1}", x, y));
                        }
                    }
                    else if (e.KeyCode == Keys.F)
                    {
                        actionsListBox.Items.Add("refresh");
                    }
                    else if (e.KeyCode == Keys.D1 ||
                             e.KeyCode == Keys.D2 ||
                             e.KeyCode == Keys.D3 ||
                             e.KeyCode == Keys.D4 ||
                             e.KeyCode == Keys.D5 ||
                             e.KeyCode == Keys.D6 ||
                             e.KeyCode == Keys.D7 ||
                             e.KeyCode == Keys.D8 ||
                             e.KeyCode == Keys.D9)
                    {
                        actionsListBox.Items.Add(string.Format("{0}", e.KeyCode));
                    }
                    else if (e.KeyCode == Keys.F3)
                    {
                        captureModeEnabled = false;
                        actionsCaptured = true;
                        lblStatus.Text = "Actions captured";
                    }
                }
                // Capture mode not yet enabled.
                else
                {
                    if (e.KeyCode == Keys.F1)
                    {
                        captureModeEnabled = true;
                        lblStatus.Text = "Capturing actions";
                    }
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.executeAction();

            this.updateActionIndex();
        }

        private void updateActionIndex()
        {
            actionIndex++;

            if (actionIndex >= actionsListBox.Items.Count)
            {
                actionIndex = 0;
            }
        }

        private void executeAction()
        {
            string action = actionsListBox.Items[actionIndex].ToString();

            test.Text = action;

            if (action.StartsWith("D"))
            {
                int seconds;

                int.TryParse(action.Substring(1, action.Length - 1), out seconds);

                Thread.Sleep(1000 * seconds);
            }
            else if (action.CompareTo("refresh") == 0)
            {
                keybd_event((byte)VK_F5, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
            }
            else
            {
                string[] newPosition = action.Split(',');
                
                int.TryParse(newPosition[1], out x);
                int.TryParse(newPosition[2], out y);
                // move mouse to coordenates
                SetCursorPos(x, y);
                
                if (newPosition[0].CompareTo("rclick") == 0)
                {
                    mouse_event(MOUSEEVENTF_RIGHTDOWN, x, y, 0, 0);
                    mouse_event(MOUSEEVENTF_RIGHTUP, x, y, 0, 0);
                }
                else //(newPosition[0].CompareTo("click") == 0)
                {
                    mouse_event(MOUSEEVENTF_LEFTDOWN, x, y, 0, 0);
                    mouse_event(MOUSEEVENTF_LEFTUP, x, y, 0, 0);
                }

                if (newPosition[0].CompareTo("delete") == 0)
                {
                    keybd_event((byte)VK_DELETE, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
                }

                Thread.Sleep(1800);
            }

            actionsListBox.SelectedIndex = actionIndex;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (frmImportDialog dialog = new frmImportDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string textToImport = dialog.textToImport;
                    string[] actions = textToImport.Split('|');

                    actionsListBox.Items.Clear();

                    foreach (string action in actions)
                    {
                        actionsListBox.Items.Add(action);
                    }
                }
            }
        }

        private void frmAutoclicker_Load(object sender, EventArgs e)
        {
            this.TopMost = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string actions = string.Empty;

            foreach (string action in actionsListBox.Items)
            {
                actions += string.Format("{0}|", action);
            }

            if (actions.Length > 0)
            {
                actions = actions.Substring(0, actions.Length - 1);
            }

            using (frmExportDialog dialog = new frmExportDialog())
            {
                dialog.actionsToExport = actions;
                dialog.ShowDialog();
            }
        }

        public frmAutoclicker()
        {
            InitializeComponent();
        }
    }
}

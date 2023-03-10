using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace AutoAcceptDota2
{
    public partial class Form1 : Form
    {//test upload 
        readonly Point pixel_at_accept_button = new Point { X = 810, Y = 526 };
        readonly Point pixel_at_accept_button1 = new Point { X = 833, Y = 523 };
        readonly Color accept_button_color_when_mouse_not_hover = Color.FromArgb(255, 50, 76, 67);
        readonly Color accept_button_color_when_mouse_hover = Color.FromArgb(255, 56, 146, 92);
        bool RunStatus = false;
        int j = 0;
        /// <summary>
        /// Получает позицию курсора в координатах х и у
        /// </summary>
        /// <param name="lpPoint"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        static extern bool GetCursorPos(ref Point lpPoint);

        [DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
        public static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        private void button2_Click(object sender, EventArgs e)
        {
            string startup_path = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\Dota 2 Helper.exe";
            File.Copy(Application.ExecutablePath, startup_path, true);
            MessageBox.Show("Успешно");            
        }

        public Form1()
        {
            InitializeComponent();
            //button2.Visible = false;
            label3.Visible = false;
            label4.Visible = false;
            timer1.Interval = 200;
            notifyIcon1.Text = "Dota 2 Helper (Неактивно)";
            notifyIcon1.BalloonTipTitle = "Dota 2 Helper (Неактивно)";
            button1.Text = "Включить";
            
            //WindowState = FormWindowState.Minimized;
        }
        /// <summary>
        /// Выключение и отключение активности программы
        /// </summary>
        private void Status()
        {
            if (!RunStatus)
            {
                button1.Text = "Выключить";
                label2.Text = "Статус: активно";
                RunStatus = true;
                timer1.Enabled = true;
                timer1.Start();
                notifyIcon1.Text = "Dota 2 Helper (Активно)";
                notifyIcon1.ContextMenuStrip = contextMenuStrip2;
            }
            else
            {
                button1.Text = "Включить";
                label2.Text = "Статус: неактивно";
                RunStatus = false;
                timer1.Stop();
                timer1.Enabled = false;
                notifyIcon1.Text = "Dota 2 Helper (Неактивно)";
                notifyIcon1.ContextMenuStrip = contextMenuStrip1;
            }
        }
        //Осоновная и единнственная кнопка в форме
        private void button1_Click(object sender, EventArgs e)
        {
            Status();
        }
        Bitmap screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
        /// <summary>
        /// Получить цвет пикселя в позиции <paramref name="location"/>
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Color GetColorAt(Point location)
        {
            using (Graphics gdest = Graphics.FromImage(screenPixel))
            {
                using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
                {
                    IntPtr hSrcDC = gsrc.GetHdc();
                    IntPtr hDC = gdest.GetHdc();
                    int retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, location.X, location.Y, (int)CopyPixelOperation.SourceCopy);
                    gdest.ReleaseHdc();
                    gsrc.ReleaseHdc();
                }
            }
            return screenPixel.GetPixel(0, 0);
        }
        
        /// <summary>
        /// Интервал поиска кнопки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                Process dota2 = Process.GetProcessesByName("dota2").FirstOrDefault();
                if (dota2 != null)
                {
                    Point cursor = new Point();
                    GetCursorPos(ref cursor);
                    Color currentColorAtCursorPos = GetColorAt(cursor);
                    //label3.Text = currentColorAtCursorPos.ToString();
                    //label4.Text = cursor.ToString();
                    Color color_accbutt_mouse_not_hover = GetColorAt(pixel_at_accept_button);
                    Color color_accbutt_mouse_hover = GetColorAt(pixel_at_accept_button1);
                    

                    if ((color_accbutt_mouse_not_hover.A == accept_button_color_when_mouse_not_hover.A &&
                        color_accbutt_mouse_not_hover.R == accept_button_color_when_mouse_not_hover.R &&
                        color_accbutt_mouse_not_hover.G == accept_button_color_when_mouse_not_hover.G &&
                        color_accbutt_mouse_not_hover.B == accept_button_color_when_mouse_not_hover.B) || 
                        (color_accbutt_mouse_hover.A == accept_button_color_when_mouse_hover.A &&
                        color_accbutt_mouse_hover.R == accept_button_color_when_mouse_hover.R &&
                        color_accbutt_mouse_hover.G == accept_button_color_when_mouse_hover.G &&
                        color_accbutt_mouse_hover.B == accept_button_color_when_mouse_hover.B))
                    {
                        
                        Console.WriteLine("Accept button FIND. ACCEPT HIM " + j);
                        LeftMouseClick(pixel_at_accept_button.X, pixel_at_accept_button.Y);
                        j++;
                        Thread.Sleep(500);
                    }
                }
            }
            catch { }
        }
        /// <summary>
        /// Сделать клик левой кнопкой мыши в позиции <paramref name="xpos"/> и <paramref name="ypos"/>
        /// </summary>
        /// <param name="xpos"></param>
        /// <param name="ypos"></param>
        public static void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        private void закрытьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void включитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Status();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            WindowState = FormWindowState.Normal;
        }
        private void закрытьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void выключитьToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Status();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            { 
                this.Hide(); 
            }
            else
            { 
                this.Show();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Hide();
        }
    }
}
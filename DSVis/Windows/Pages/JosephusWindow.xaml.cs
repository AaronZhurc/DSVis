using DSVis.DataStruct;
using DSVis.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DSVis.Windows.Pages {
    /// <summary>
    /// JosephusWindow.xaml 的交互逻辑
    /// </summary>
    public partial class JosephusWindow : Page {
        int m_num, m_start, m_gap, maxCircle15, maxCircle17;
        Circular list = null;
        List<int> result = new List<int>();
        int m_count;
        public delegate void DataConfirm();
        public DataConfirm dataConfirm;
        public delegate void DataClean();
        public DataClean dataClean;
        public JosephusWindow() {
            InitializeComponent();
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
            double R;
            if(this.Height<= this.Width - 170) {
                R = this.Height / 2;
            } else {
                R = (this.Width - 170) / 2;
            }
            maxCircle15 = (int)(Math.PI / Math.Asin(17 / R));
            maxCircle17 = (int)(Math.PI / Math.Asin(17 / R));
            if (MainWindowInfo.fileFlag == true) {
                this.IsEnabled = true;
            } else {
                SetJosephusForm form = new SetJosephusForm();
                form.Show();
                form.sendMessage = Recevie;
                form.windowClosed = FormClosed;
                form.getMaxCircle(maxCircle15);
                this.IsEnabled = false;
            }
            textOut.Text = "清除出去的编号\n";
        }

        private void btnSet_Click(object sender, RoutedEventArgs e) {
            double R;
            if (this.Height <= this.Width - 170) {
                R = this.Height / 2;
            } else {
                R = (this.Width - 170) / 2;
            }
            maxCircle15 = (int)(Math.PI / Math.Asin(15 / R));
            maxCircle17 = (int)(Math.PI / Math.Asin(17 / R));
            SetJosephusForm form = new SetJosephusForm();
            form.Show();
            form.sendMessage = Recevie;
            form.windowClosed = FormClosed;
            form.getMaxCircle(maxCircle15);
            this.IsEnabled = false;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e) {
            if (m_count < m_num) {
                btnSet.IsEnabled = false;
                btnNext.Content = "下一步";
                Ellipse ellipse = FindName("v" + result[m_count]) as Ellipse;
                ellipse.Fill = new SolidColorBrush(Colors.PaleVioletRed);
                if (result[m_count] < 26) {
                    if (result[m_count] == 0) {
                        textOut.Text += ('A').ToString() + " ";
                    } else {
                        textOut.Text += ((char)('A' + result[m_count])).ToString() + " ";
                    }
                } else {
                    int n = result[m_count] / 26;
                    if ((result[m_count] % 26) == 0) {
                        textOut.Text += 'A' + n.ToString() + " ";
                    } else {
                        textOut.Text += ((char)('A' + (result[m_count] % 26))) + n.ToString() + " ";
                    }
                }
                m_count++;
            } else {
                btnNext.Content = "重新演示";
                m_count = 0;
                for(int i = 0; i < m_num; i++) {
                    Ellipse ellipse = FindName("v" + i) as Ellipse;
                    ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                }
                textOut.Text = "清除出去的编号\n";
                btnSet.IsEnabled = true;
            }
        }

        public void Recevie(int num, int start, int gap) {
            dataConfirm();
            list = new Circular();
            for (int i = 0; i < m_num; i++) {
                MainCanvas.Children.Remove((UIElement)this.FindName("v" + i));
                this.UnregisterName("v" + i);
                MainCanvas.Children.Remove((UIElement)this.FindName("t" + i));
                this.UnregisterName("t" + i);
            }
            m_num = num;
            m_start = start;
            m_gap = gap;
            m_count = 0;
            for(int i = 1; i < num; i++) {
                Circular c = new Circular(i);
                list.AddNode(c);
            }
            result = list.JosephusStart(start, gap, num);
            this.IsEnabled = true;
            btnNext.IsEnabled = true;
            btnSet.IsEnabled = false;
            DrawCir();
        }

        public void windowchanged() {
            double R;
            if (this.Height <= this.Width - 170) {
                R = this.Height / 2;
            } else {
                R = (this.Width - 170) / 2;
            }
            maxCircle15 = (int)(Math.PI / Math.Asin(15 / R));
            maxCircle17 = (int)(Math.PI / Math.Asin(17 / R));
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
            CtrlBg.Height = this.Height;
            MainCanvas.Height = this.Height;
            MainCanvas.Width = this.Width - 170;
            double t = 2 * Math.PI / m_num;
            double r;
            if (m_num >= maxCircle17) {
                r = 15 / Math.Sin(t / 2);
            } else {
                r = 17 / Math.Sin(t / 2);
            }
            for (int i = 0; i < m_num; i++) {
                Ellipse ellipse = this.FindName("v" + i) as Ellipse;
                ellipse.SetValue(Canvas.LeftProperty, MainCanvas.Width / 2 - Math.Sin(-t * i) * r - 15);
                ellipse.SetValue(Canvas.TopProperty, MainCanvas.Height / 2 - Math.Cos(-t * i) * r - 15);
                TextBlock text = this.FindName("t" + i) as TextBlock;
                Size sizeText = text.DesiredSize;
                text.SetValue(Canvas.LeftProperty, MainCanvas.Width / 2 - Math.Sin(-t * i) * r - sizeText.Width / 2);
                text.SetValue(Canvas.TopProperty, MainCanvas.Height / 2 - Math.Cos(-t * i) * r - sizeText.Height / 2);
            }
        }

        public void DrawCir() {
            double t = 2 * Math.PI / m_num;
            double r;
            if (m_num >= maxCircle17) {
                r = 15 / Math.Sin(t / 2);
            } else {
                r = 17 / Math.Sin(t / 2);
            }
            for(int i = 0; i < m_num; i++) {
                Ellipse ellipse = new Ellipse();
                ellipse.Width = 30;
                ellipse.Height = 30;
                ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                ellipse.SetValue(Canvas.LeftProperty, MainCanvas.ActualWidth / 2 - Math.Sin(-t * i) * r - 15);
                ellipse.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - Math.Cos(-t * i) * r - 15);
                ellipse.Name = "v" + i.ToString();
                this.RegisterName("v" + i, ellipse);
                TextBlock text = new TextBlock();
                if (i < 26) {
                    if (i == 0) {
                        text.Text = ('A').ToString();
                    } else {
                        text.Text = ((char)('A' +  i)).ToString();
                    }
                } else {
                    int n = i / 26;
                    if ((i % 26) == 0) {
                        text.Text = 'A' + n.ToString();
                    } else {
                        text.Text = ((char)('A' + (i % 26))) + n.ToString();
                    }
                }
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size sizeText = text.DesiredSize;
                text.SetValue(Canvas.LeftProperty, MainCanvas.ActualWidth / 2 - Math.Sin(-t * i) * r - sizeText.Width / 2);
                text.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - Math.Cos(-t * i) * r - sizeText.Height / 2);
                text.HorizontalAlignment = HorizontalAlignment.Center;
                text.VerticalAlignment = VerticalAlignment.Center;
                text.TextAlignment = TextAlignment.Center;
                text.Name = "t" + i.ToString();
                this.RegisterName("t" + i, text);
                MainCanvas.Children.Add(ellipse);
                MainCanvas.Children.Add(text);
            }
        }
        public void FormClosed() {
            this.IsEnabled = true;
        }

        public void Filesave() {
            FileStream fs = new FileStream(MainWindowInfo.fileLocation, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            JObject jObject = new JObject();
            jObject["type"] = "Josephus";
            jObject["num"] = m_num;
            jObject["start"] = m_start;
            jObject["gap"] = m_gap;
            sw.Write(jObject);
            sw.Close();
            fs.Close();
        }
        public void Fileopen() {
            Dispatcher.Invoke(new Action(() => {
                Height = ActualHeight;
                Width = ActualWidth;
            }), DispatcherPriority.Loaded);//等待窗口加载完毕
            FileStream fs = new FileStream(MainWindowInfo.fileLocation, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            JObject jObject = (JObject)JsonConvert.DeserializeObject(sr.ReadToEnd());
            Recevie(int.Parse(jObject["num"].ToString()), int.Parse(jObject["start"].ToString()), int.Parse(jObject["gap"].ToString()));
            sr.Close();
            fs.Close();
        }
    }
}

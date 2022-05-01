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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DSVis.Windows.Pages {
    /// <summary>
    /// NumConverWindow.xaml 的交互逻辑
    /// </summary>
    public partial class NumConverWindow : Page {
        private int m_flagCtrl;
        bool m_flagAdnrews;
        int m_num, m_sys, m_quot;
        String m_result;
        int m_countDraw;
        Stack<int> stack = new Stack<int>();
        public delegate void DataConfirm();
        public DataConfirm dataConfirm;
        public delegate void DataClean();
        public DataClean dataClean;
        public NumConverWindow() {
            InitializeComponent();
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
            if (MainWindowInfo.fileFlag == true) {
                this.IsEnabled = true;
            } else {
                SetNumSysForm form = new SetNumSysForm();
                form.Show();
                form.sendMessage = Recevie;
                form.windowClosed = FormClosed;
                this.IsEnabled = false;
            }
        }

        private void btnSet_Click(object sender, RoutedEventArgs e) {
            SetNumSysForm form = new SetNumSysForm();
            form.Show();
            form.sendMessage = Recevie;
            form.windowClosed = FormClosed;
            this.IsEnabled = false;
        }

        public void Recevie(int num,int sys) {
            dataConfirm();
            m_num = num;
            m_quot = num;
            m_sys = sys;
            textNum.Text = num.ToString();
            textResult.Text = "";
            textQuotus.Text = "";
            textDividend.Text = "";
            textRemainder.Text = "";
            m_countDraw = 0;
            if (sys == 120) {
                textSys.Text = "12(安德鲁斯)";
                m_flagAdnrews = true;
                m_sys = 12;
            } else {
                textSys.Text = sys.ToString();
                m_flagAdnrews = false;
            }
            btnNext.IsEnabled = true;
            m_flagCtrl = 1;
        }
        public void FormClosed() {
            this.IsEnabled = true;
        }
        private void btnNext_Click(object sender, RoutedEventArgs e) {
            if (m_flagCtrl == 1) {
                if (m_sys != 1) {
                    if (m_quot >= m_sys) {
                        textDividend.Text = m_quot.ToString();
                        stack.Push(m_quot % m_sys);
                        textRemainder.Text = stack.Peek().ToString();
                        m_quot = m_quot / m_sys;
                        textQuotus.Text = m_quot.ToString();
                    } else {
                        textDividend.Text = "";
                        textRemainder.Text = "";
                        stack.Push(m_quot);
                        m_flagCtrl = 2;
                    }
                } else {
                    for(int i = 0; i < m_num; i++) {
                        stack.Push(1);
                    }
                    m_flagCtrl = 2;
                }
                Clear();
                Draw();
            } else if (m_flagCtrl == 2) {
                if (m_sys != 1) {
                    if (stack.Count != 0) {
                        int quot = stack.Pop();
                        if (quot >= 0 && quot < 10) {
                            textResult.Text += quot;
                        } else if (quot >= 10) {
                            if (m_flagAdnrews == true) {
                                if (quot == 10) {
                                    textResult.Text += 'X';
                                } else if (quot == 11) {
                                    textResult.Text += 'E';
                                }
                            } else {
                                int a = quot - 10;
                                textResult.Text += (char)('A' + a);
                            }
                        }
                    } else {
                        m_flagCtrl = 3;
                        btnNext.IsEnabled = false;
                    }
                } else {
                    stack.Clear();
                    for(int i = 0; i < m_num; i++) {
                        textResult.Text += 1;
                        if ((i + 1) % 5 == 0 && (i + 1) != m_num) {
                            textResult.Text += ',';
                        }
                    }
                    m_flagCtrl = 3;
                    btnNext.IsEnabled = false;
                }
                Clear();
                Draw();
            }
        }
        private void Clear() {
            for (int i = 0; i < m_countDraw; i++) {
                MainCanvas.Children.Remove((UIElement)this.FindName("r" + i));
                this.UnregisterName("r" + i);
                MainCanvas.Children.Remove((UIElement)this.FindName("t" + i));
                this.UnregisterName("t" + i);
            }
        }
        private void Draw() {
            for (int i = stack.Count - 1; i >= 0; i--) {
                Point point = MainCanvas.TranslatePoint(new Point(), MainCanvas);
                Rectangle rect = new Rectangle();
                rect.Width = 120;
                rect.Height = 20;
                rect.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                rect.SetValue(Canvas.LeftProperty, point.X);
                rect.SetValue(Canvas.TopProperty, point.Y + MainCanvas.ActualHeight - rect.Height * (i + 1));
                rect.Name = "r" + i;
                this.RegisterName("r" + i, rect);

                TextBlock text = new TextBlock();
                text.Text = stack.ElementAt(stack.Count - i - 1).ToString();
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size sizeText = text.DesiredSize;
                text.SetValue(Canvas.LeftProperty, point.X + 50 - sizeText.Width / 2);
                text.SetValue(Canvas.TopProperty, point.Y + MainCanvas.ActualHeight - rect.Height * (i + 1) + rect.Height / 2 - sizeText.Height / 2);
                text.Name = "t" + i;
                this.RegisterName("t" + i, text);

                MainCanvas.Children.Add(rect);
                MainCanvas.Children.Add(text);
            }
            m_countDraw = stack.Count;
        }
        public void Filesave() {
            FileStream fs = new FileStream(MainWindowInfo.fileLocation, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            JObject jObject = new JObject();
            jObject["type"] = "NumConver";
            jObject["num"] = m_num;
            jObject["sys"] = m_sys;
            sw.Write(jObject);
            sw.Close();
            fs.Close();
        }
        public void Fileopen() {
            try {
                Dispatcher.Invoke(new Action(() => {
                    Height = ActualHeight;
                    Width = ActualWidth;
                }), DispatcherPriority.Loaded);//等待窗口加载完毕
                FileStream fs = new FileStream(MainWindowInfo.fileLocation, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                JObject jObject = (JObject)JsonConvert.DeserializeObject(sr.ReadToEnd());
                Recevie((int)jObject["num"], (int)jObject["sys"]);
                sr.Close();
                fs.Close();
            } catch {
                MessageBox.Show("请检查文件是否完整");
            }
        }

        public void windowchanged() {
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
            CtrlBg.Height = this.Height;
            MainCanvas.Height = this.Height - 40;
            MainCanvas.Width = 120;
            Point point = MainCanvas.TranslatePoint(new Point(), MainCanvas);
            for (int i = 0; i < m_countDraw; i++) {
                Rectangle rect = this.FindName("r" + i) as Rectangle;
                rect.SetValue(Canvas.LeftProperty, point.X);
                rect.SetValue(Canvas.TopProperty, point.Y + MainCanvas.Height - rect.Height * (i + 1));
                TextBlock text = this.FindName("t" + i) as TextBlock;
                Size sizeText = text.DesiredSize;
                text.SetValue(Canvas.LeftProperty, point.Y + MainCanvas.Height + 50 - sizeText.Width / 2);
                text.SetValue(Canvas.TopProperty, point.Y + MainCanvas.Height - rect.Height * (i + 1) + rect.Height / 2 - sizeText.Height / 2);
            }
        }
    }
}

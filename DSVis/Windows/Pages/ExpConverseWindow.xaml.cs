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
    /// ExpConverseWindow.xaml 的交互逻辑
    /// </summary>
    public partial class ExpConverseWindow : Page {
        private int m_flagCtrl;//1 转换模式 2 计算模式
        private String m_exp,m_suffix,m_infix;
        Stack<char> stackTrans = new Stack<char>();
        Stack<int> stackCalcu = new Stack<int>();
        int m_count, m_countDraw;
        public delegate void DataConfirm();
        public DataConfirm dataConfirm;
        public delegate void DataClean();
        public DataClean dataClean;
        public ExpConverseWindow() {
            InitializeComponent();
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
            if (MainWindowInfo.fileFlag == true) {
                this.IsEnabled = true;
            } else {
                SetExpForm form = new SetExpForm();
                form.Show();
                form.sendMessage = Recevie;
                form.windowClosed = FormClosed;
                this.IsEnabled = false;
            }
        }

        private void btnSet_Click(object sender, RoutedEventArgs e) {
            SetExpForm form = new SetExpForm();
            form.Show();
            form.sendMessage = Recevie;
            form.windowClosed = FormClosed;
            this.IsEnabled = false;
        }

        public void Recevie(String value) {
            Clear();
            m_flagCtrl = 1;
            dataConfirm();
            stackTrans.Clear();
            m_suffix = "";
            textResult.Text = "";
            m_count = 0;
            m_countDraw = 0;
            m_infix = value;
            Run ri0 = textInfix.Inlines.FirstInline as Run;
            Run ri1 = textInfix.Inlines.LastInline as Run;
            ri0.Text = "";
            ri1.Text = m_infix;
            Run rs0 = textSuffix.Inlines.FirstInline as Run;
            Run rs1 = textSuffix.Inlines.LastInline as Run;
            rs0.Text = "";
            rs1.Text = "";
            m_exp = value + "#";
            stackTrans.Push('#');
            Draw();
            btnNext.IsEnabled = true;
        }
        public void FormClosed() {
            this.IsEnabled = true;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e) {
            if (m_flagCtrl == 1) {
                try {
                    char w = m_exp[m_count];
                    if (stackTrans.Peek() != '#' || w != '#') {
                        if (IsNum(w)) {
                            Run rs = textSuffix.Inlines.LastInline as Run;
                            rs.Text += w;
                            m_suffix += w;
                            m_count++;
                        } else {
                            if (stackTrans.Peek() == '(' && w == ')' || stackTrans.Peek() == '（' && w == '）') {
                                stackTrans.Pop();
                                m_count++;
                            } else {
                                if (stackTrans.Peek() == '(' || stackTrans.Peek() == '（' || GetPriority(stackTrans.Peek()) < GetPriority(w)) {
                                    stackTrans.Push(w);
                                    m_count++;
                                } else {
                                    char c = stackTrans.Pop();
                                    Run rs = textSuffix.Inlines.LastInline as Run;
                                    rs.Text += c;
                                    m_suffix += c;
                                }
                            }
                        }
                        Run ri0 = textInfix.Inlines.FirstInline as Run;
                        Run ri1 = textInfix.Inlines.LastInline as Run;
                        ri0.Text = m_infix.Substring(0, m_count);
                        ri1.Text = m_infix.Substring(m_count);
                    } else if (stackTrans.Peek() == '#' && w == '#') {
                        m_count = 0;
                        m_flagCtrl = 2;
                        btnNext.Content = "计算";
                    }
                } catch {
                    MessageBox.Show("该中缀表达式不合法");
                    btnNext.IsEnabled = false;
                }
            } else if(m_flagCtrl == 2) {
                int c = 0;
                if (m_count < m_suffix.Length) {
                    if (IsNum(m_suffix[m_count])) {
                        stackCalcu.Push(int.Parse(m_suffix[m_count].ToString()));
                    } else {
                        int a = int.Parse(stackCalcu.Pop().ToString());
                        int b = int.Parse(stackCalcu.Pop().ToString());
                        switch (m_suffix[m_count]) {
                            case '+':
                                c = a + b;
                                break;
                            case '-':
                                c = a - b;
                                break;
                            case '*':
                                c = a * b;
                                break;
                            case '/':
                                c = b / a;
                                break;
                        }
                        stackCalcu.Push(c);
                    }
                    m_count++;
                } else {
                    textResult.Text = stackCalcu.Peek().ToString();
                    m_flagCtrl = 3;
                }
                Run rs0 = textSuffix.Inlines.FirstInline as Run;
                Run rs1 = textSuffix.Inlines.LastInline as Run;
                rs0.Text = m_suffix.Substring(0, m_count);
                rs1.Text = m_suffix.Substring(m_count);
            } else {
                btnNext.Content = "重新演示";
                m_suffix = "";
                m_count = 0;
                m_countDraw = 0;
                Run ri0 = textInfix.Inlines.FirstInline as Run;
                Run ri1 = textInfix.Inlines.LastInline as Run;
                ri0.Text = "";
                ri1.Text = m_infix;
                Run rs0 = textSuffix.Inlines.FirstInline as Run;
                Run rs1 = textSuffix.Inlines.LastInline as Run;
                rs0.Text = "";
                rs1.Text = "";
                textResult.Text = "";
                stackTrans.Clear();
                stackTrans.Push('#');
                stackCalcu.Clear();
                m_flagCtrl = 1;
            }
            Clear();
            Draw();
        }
        private bool IsNum(char c) {
            if (c >= '0' && c <= '9') {
                return true;
            } else {
                return false;
            }
        }
        private int GetPriority(char c) {
            switch (c) {
                case '#':
                    return 1;
                case ')':
                case '）':
                    return 2;
                case '+':
                case '-':
                    return 3;
                case '*':
                case '/':
                    return 4;
                case '(':
                case '（':
                    return 5;
                default:
                    return 0;
            }
        }
        private void Clear() {
            for(int i = 0; i < m_countDraw; i++) {
                MainCanvas.Children.Remove((UIElement)this.FindName("r" + i));
                this.UnregisterName("r" + i);
                MainCanvas.Children.Remove((UIElement)this.FindName("t" + i));
                this.UnregisterName("t" + i);
            }
        }
        private void Draw() {
            if (m_flagCtrl == 1) {
                for (int i = stackTrans.Count - 1; i >= 0; i--) {
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
                    text.Text = stackTrans.ElementAt(stackTrans.Count - i - 1).ToString();
                    text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size sizeText = text.DesiredSize;
                    text.SetValue(Canvas.LeftProperty, point.X + 50 - sizeText.Width / 2);
                    text.SetValue(Canvas.TopProperty, point.Y + MainCanvas.ActualHeight - rect.Height * (i + 1) + rect.Height / 2 - sizeText.Height / 2);
                    text.Name = "t" + i;
                    this.RegisterName("t" + i, text);

                    MainCanvas.Children.Add(rect);
                    MainCanvas.Children.Add(text);
                }
                m_countDraw = stackTrans.Count;
            } else if (m_flagCtrl == 2) {
                for (int i = stackCalcu.Count - 1; i >= 0; i--) {
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
                    text.Text = stackCalcu.ElementAt(stackCalcu.Count - i - 1).ToString();
                    text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size sizeText = text.DesiredSize;
                    text.SetValue(Canvas.LeftProperty, point.X + 50 - sizeText.Width / 2);
                    text.SetValue(Canvas.TopProperty, point.Y + MainCanvas.ActualHeight - rect.Height * (i + 1) + rect.Height / 2 - sizeText.Height / 2);
                    text.Name = "t" + i;
                    this.RegisterName("t" + i, text);

                    MainCanvas.Children.Add(rect);
                    MainCanvas.Children.Add(text);
                }
                m_countDraw = stackCalcu.Count;
            } else {
                m_countDraw = 0;
            }
        }
        public void Filesave() {
            FileStream fs = new FileStream(MainWindowInfo.fileLocation, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            JObject jObject = new JObject();
            jObject["type"] = "ExpConverse";
            jObject["expression"] = m_infix;
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
                Recevie(jObject["expression"].ToString());
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
                text.SetValue(Canvas.LeftProperty, point.X + 50 - sizeText.Width / 2);
                text.SetValue(Canvas.TopProperty, point.Y + MainCanvas.Height - rect.Height * (i + 1) + rect.Height / 2 - sizeText.Height / 2);
            }
        }
    }
}

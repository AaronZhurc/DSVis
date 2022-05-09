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
    /// KMPWindow.xaml 的交互逻辑
    /// </summary>
    public partial class KMPWindow : Page {
        private int m_flagCtrl;//0 初始 1 计算next数组 2 比对主串
        private int m_count, m_indexi, m_indexj, m_countSame;
        public delegate void DataConfirm();
        public DataConfirm dataConfirm;
        public delegate void DataClean();
        public DataClean dataClean;
        private String mainStr, pattenStr;
        private int[] next;
        private int[] nextbp;
        private bool m_indexFlag = false;
        private List<(int, int)> listSame = new List<(int, int)>();

        public KMPWindow() {
            InitializeComponent();
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
            if (MainWindowInfo.fileFlag == true) {
                this.IsEnabled = true;
            } else {
                SetStringForm form = new SetStringForm();
                form.Show();
                form.sendMessage = Recevie;
                form.windowClosed = FormClosed;
                this.IsEnabled = false;
            }
        }

        private void btnString_Click(object sender, RoutedEventArgs e) {
            SetStringForm form = new SetStringForm();
            form.Show();
            form.sendMessage = Recevie;
            form.windowClosed = FormClosed;
            this.IsEnabled = false;
        }
        public void Recevie(string mainStr, string pattenStr) {
            Clear();
            dataConfirm();
            this.mainStr = mainStr;
            this.pattenStr = pattenStr;
            next = new int[pattenStr.Length+1];
            nextbp = new int[pattenStr.Length + 1];
            next[0] = -1;
            nextbp[0] = -1;
            m_flagCtrl = 0;
            m_countSame = 0;
            btnNext.IsEnabled = true;
            radiobtnNext.IsEnabled = true;
            radiobtnNextval.IsEnabled = true;
            m_count = 0;
            listSame.Clear();
            Draw();
        }
        public void FormClosed() {
            this.IsEnabled = true;
        }
        public void windowchanged() {
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
            CtrlBg.Height = this.Height;
            MainCanvas.Height = this.Height;
            MainCanvas.Width = this.Width - 170;
            for (int i = 0; i < pattenStr.Length; i++) {
                Ellipse ellipse = (Ellipse)this.FindName("v" + i);
                TextBlock text = (TextBlock)this.FindName("t" + i);
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size sizeText = text.DesiredSize;
                ellipse.SetValue(Canvas.LeftProperty, 150 + 35 * ((m_indexi - m_indexj) + i) - ellipse.Width / 2);
                ellipse.SetValue(Canvas.TopProperty, MainCanvas.Height / 2 - ellipse.Height / 2);
                text.SetValue(Canvas.LeftProperty, 150 + 35 * ((m_indexi - m_indexj) + i) - sizeText.Width / 2);
                text.SetValue(Canvas.TopProperty, MainCanvas.Height / 2 - sizeText.Height / 2);
                if (this.FindName("vd" + i) != null) {
                    Ellipse ed = (Ellipse)this.FindName("vd" + i);
                    TextBlock td = (TextBlock)this.FindName("td" + i);
                    td.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size sizeTd = td.DesiredSize;
                    if (radiobtnNext.IsChecked == true) {
                        ed.SetValue(Canvas.LeftProperty, 150 + 35 * (m_count + 1 - next[m_count + 1] + i) - ellipse.Width / 2);
                        ed.SetValue(Canvas.TopProperty, MainCanvas.Height / 2 - ellipse.Height / 2 + 40);
                        td.SetValue(Canvas.LeftProperty, 150 + 35 * (m_count + 1 - next[m_count + 1] + i) - sizeTd.Width / 2);
                        td.SetValue(Canvas.TopProperty, MainCanvas.Height / 2 - sizeTd.Height / 2 + 40);
                    } else {
                        ed.SetValue(Canvas.LeftProperty, 150 + 35 * (m_count + 1 - nextbp[m_count + 1] + i) - ellipse.Width / 2);
                        ed.SetValue(Canvas.TopProperty, MainCanvas.Height / 2 - ellipse.Height / 2 + 40);
                        td.SetValue(Canvas.LeftProperty, 150 + 35 * (m_count + 1 - nextbp[m_count + 1] + i) - sizeTd.Width / 2);
                        td.SetValue(Canvas.TopProperty, MainCanvas.Height / 2 - sizeTd.Height / 2 + 40);
                    }
                }
                if (this.FindName("vn" + i) != null) {
                    Ellipse en = (Ellipse)this.FindName("vn" + i);
                    TextBlock tn = (TextBlock)this.FindName("tn" + i);
                    en.SetValue(Canvas.LeftProperty, 150 + 35 * ((m_indexi - m_indexj) + i) - en.Width / 2 + 10);
                    en.SetValue(Canvas.TopProperty, MainCanvas.Height / 2 - en.Height / 2 + 5);
                    tn.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size sizeTn = tn.DesiredSize;
                    tn.SetValue(Canvas.LeftProperty, 150 + 35 * ((m_indexi - m_indexj) + i) - sizeTn.Width / 2 + 10);
                    tn.SetValue(Canvas.TopProperty, MainCanvas.Height / 2 - sizeTn.Height / 2 + 5);
                    text.SetValue(Canvas.LeftProperty, 150 + 35 * ((m_indexi - m_indexj) + i) - sizeText.Width / 2 - 5);
                    text.SetValue(Canvas.TopProperty, MainCanvas.Height / 2 - sizeText.Height / 2 - 5);
                }
            }
            for (int i = 0; i < mainStr.Length; i++) {
                if (this.FindName("vm" + i) != null) {
                    Ellipse ellipse = (Ellipse)this.FindName("vm" + i);
                    TextBlock text = (TextBlock)this.FindName("tm" + i);
                    ellipse.SetValue(Canvas.LeftProperty, 150 + 35 * i - ellipse.Width / 2);
                    ellipse.SetValue(Canvas.TopProperty, MainCanvas.Height / 2 - ellipse.Height / 2 - 40);

                    text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size sizeText = text.DesiredSize;
                    text.SetValue(Canvas.LeftProperty, 150 + 35 * i - sizeText.Width / 2);
                    text.SetValue(Canvas.TopProperty, MainCanvas.Height / 2 - sizeText.Height / 2 - 40);
                }
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e) {
            if (m_flagCtrl == 0) {
                m_flagCtrl = 1;
                m_count++;
                if (radiobtnNext.IsChecked == true) {
                    SetNext();
                    radiobtnNextval.IsEnabled = false;
                } else if (radiobtnNextval.IsChecked == true) {
                    SetNext();
                    SetNextval();
                    radiobtnNext.IsEnabled = false;
                }
                Draw();
            } else if (m_flagCtrl == 1 && m_count < pattenStr.Length - 1) {
                m_count++;
                Draw();
            } else if (m_flagCtrl == 1 && m_count == pattenStr.Length - 1) {
                m_flagCtrl = 2;
                m_count = 0;
                Draw();
            } else if (m_flagCtrl == 2 && m_count == 0 && m_indexFlag == false) {
                m_count++;
                m_indexi = 1;
                m_indexj = 1;
                Index();
                Draw();
            } else if (m_flagCtrl == 2 && m_indexFlag == false) {
                Index();
                Draw();
            } else if (m_indexFlag == true) {
                Clear();
                btnNext.Content = "重新排序";
                m_count = 0;
                m_flagCtrl = 0;
                m_indexFlag = false;
                m_countSame = 0;
                Draw();
                radiobtnNext.IsEnabled = true;
                radiobtnNextval.IsEnabled = true;
                listSame.Clear();
            } 
        }
        private void Clear() {
            if (pattenStr != null) {
                for (int i = 0; i < pattenStr.Length; i++) {
                    MainCanvas.Children.Remove((UIElement)this.FindName("v" + i));
                    this.UnregisterName("v" + i);
                    MainCanvas.Children.Remove((UIElement)this.FindName("t" + i));
                    this.UnregisterName("t" + i);
                    if (this.FindName("vd" + i) != null) {
                        MainCanvas.Children.Remove((UIElement)this.FindName("vd" + i));
                        this.UnregisterName("vd" + i);
                        MainCanvas.Children.Remove((UIElement)this.FindName("td" + i));
                        this.UnregisterName("td" + i);
                    }
                    if (this.FindName("vn" + i) != null) {
                        MainCanvas.Children.Remove((UIElement)this.FindName("vn" + i));
                        this.UnregisterName("vn" + i);
                        MainCanvas.Children.Remove((UIElement)this.FindName("tn" + i));
                        this.UnregisterName("tn" + i);
                    }
                }
            }
            if (mainStr != null) {
                for (int i = 0; i < mainStr.Length; i++) {
                    if (this.FindName("vm" + i) != null) {
                        MainCanvas.Children.Remove((UIElement)this.FindName("vm" + i));
                        this.UnregisterName("vm" + i);
                        MainCanvas.Children.Remove((UIElement)this.FindName("tm" + i));
                        this.UnregisterName("tm" + i);
                    }
                }
            }
        }
        private void Draw() {
            if (m_flagCtrl == 0) {
                for (int i = 0; i < pattenStr.Length; i++) {
                    Ellipse ellipse = new Ellipse();
                    TextBlock text = new TextBlock();
                    ellipse.Width = 30;
                    ellipse.Height = 30;
                    ellipse.Name = "v" + i;
                    this.RegisterName("v" + i, ellipse);
                    ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    ellipse.SetValue(Canvas.LeftProperty, (150 + 35 * i - ellipse.Width / 2));
                    ellipse.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - ellipse.Height / 2);

                    text.Text = pattenStr[i].ToString();
                    text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size sizeText = text.DesiredSize;
                    if (i == 0) {
                        text.SetValue(Canvas.LeftProperty, (150 + 35 * i - sizeText.Width / 2 - 5));
                        text.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - sizeText.Height / 2 - 5);
                    } else {
                        text.SetValue(Canvas.LeftProperty, (150 + 35 * i - sizeText.Width / 2));
                        text.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - sizeText.Height / 2);//出现next数组位置-5
                    }
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    text.TextAlignment = TextAlignment.Center;
                    text.Name = "t" + i;
                    this.RegisterName("t" + i, text);

                    MainCanvas.Children.Add(ellipse);
                    MainCanvas.Children.Add(text);
                }
                Ellipse ellnext = new Ellipse();
                TextBlock tnext = new TextBlock();
                ellnext.Width = 20;
                ellnext.Height = 20;
                ellnext.Name = "vn0";
                this.RegisterName("vn0", ellnext);
                ellnext.Fill = new SolidColorBrush(Colors.DarkSeaGreen);
                ellnext.SetValue(Canvas.LeftProperty, (150 - ellnext.Width / 2 + 10));
                ellnext.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - ellnext.Height / 2 + 5);

                tnext.Text = next[1].ToString();
                tnext.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size sizeTnext = tnext.DesiredSize;
                tnext.SetValue(Canvas.LeftProperty, (150 - sizeTnext.Width / 2 + 10));
                tnext.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - sizeTnext.Height / 2 + 5);//出现next数组位置-5
                tnext.HorizontalAlignment = HorizontalAlignment.Center;
                tnext.VerticalAlignment = VerticalAlignment.Center;
                tnext.TextAlignment = TextAlignment.Center;
                tnext.Name = "tn0";
                this.RegisterName("tn0", tnext);

                MainCanvas.Children.Add(ellnext);
                MainCanvas.Children.Add(tnext);

                for (int i = 0; i < pattenStr.Length; i++) {
                    Ellipse ellipse = new Ellipse();
                    TextBlock text = new TextBlock();
                    ellipse.Width = 30;
                    ellipse.Height = 30;
                    ellipse.Name = "vd" + i;
                    this.RegisterName("vd" + i, ellipse);
                    ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    ellipse.SetValue(Canvas.LeftProperty, 150 + 35 * (i + 1) - ellipse.Width / 2);
                    ellipse.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - ellipse.Height / 2 + 40);

                    text.Text = pattenStr[i].ToString();
                    text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size sizeText = text.DesiredSize;
                    text.SetValue(Canvas.LeftProperty, 150 + 35 * (i + 1) - sizeText.Width / 2);
                    text.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - sizeText.Height / 2 + 40);//出现next数组位置-5
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    text.TextAlignment = TextAlignment.Center;
                    text.Name = "td" + i;
                    this.RegisterName("td" + i, text);

                    MainCanvas.Children.Add(ellipse);
                    MainCanvas.Children.Add(text);
                }
            } else if (m_flagCtrl == 1) {
                TextBlock text = this.FindName("t" + m_count) as TextBlock;
                Size sizeText = text.DesiredSize;
                text.SetValue(Canvas.LeftProperty, 150 + 35 * m_count - sizeText.Width / 2 - 5);
                text.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - sizeText.Height / 2 - 5);

                Ellipse ellnext = new Ellipse();
                TextBlock tnext = new TextBlock();
                ellnext.Width = 20;
                ellnext.Height = 20;
                ellnext.Name = "vn" + m_count;
                this.RegisterName("vn" + m_count, ellnext);
                ellnext.Fill = new SolidColorBrush(Colors.DarkSeaGreen);
                ellnext.SetValue(Canvas.LeftProperty, 150 + 35 * m_count - ellnext.Width / 2 + 10);
                ellnext.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - ellnext.Height / 2 + 5);

                tnext.Text = next[m_count + 1].ToString();
                tnext.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size sizeTnext = tnext.DesiredSize;
                tnext.SetValue(Canvas.LeftProperty, 150 + 35 * m_count - sizeTnext.Width / 2 + 10);
                tnext.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - sizeTnext.Height / 2 + 5);
                tnext.HorizontalAlignment = HorizontalAlignment.Center;
                tnext.VerticalAlignment = VerticalAlignment.Center;
                tnext.TextAlignment = TextAlignment.Center;
                tnext.Name = "tn" + m_count;
                this.RegisterName("tn" + m_count, tnext);

                MainCanvas.Children.Add(ellnext);
                MainCanvas.Children.Add(tnext);

                for (int i = 0; i < pattenStr.Length; i++) {
                    Ellipse ellipse = this.FindName("vd" + i) as Ellipse;
                    TextBlock textd = this.FindName("td" + i) as TextBlock;
                    ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    if (radiobtnNext.IsChecked == true) {
                        ellipse.SetValue(Canvas.LeftProperty, 150 + 35 * (m_count + 1 - next[m_count + 1] + i) - ellipse.Width / 2);
                        ellipse.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - ellipse.Height / 2 + 40);
                    } else {
                        ellipse.SetValue(Canvas.LeftProperty, 150 + 35 * (m_count + 1 - nextbp[m_count + 1] + i) - ellipse.Width / 2);
                        ellipse.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - ellipse.Height / 2 + 40);
                    }
                    textd.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size sizeTextd = textd.DesiredSize;
                    if (radiobtnNext.IsChecked == true) {
                        textd.SetValue(Canvas.LeftProperty, 150 + 35 * (m_count + 1 - next[m_count + 1] + i) - sizeTextd.Width / 2);
                        textd.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - sizeTextd.Height / 2 + 40);
                    } else {
                        textd.SetValue(Canvas.LeftProperty, 150 + 35 * (m_count + 1 - nextbp[m_count + 1] + i) - sizeTextd.Width / 2);
                        textd.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - sizeTextd.Height / 2 + 40);
                    }
                }
                if (radiobtnNextval.IsChecked == true) {
                    for (int i = 0; i < pattenStr.Length; i++) {
                        Ellipse ellipse = this.FindName("v" + i) as Ellipse;
                        ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    }
                    if (next[m_count+1] != nextbp[m_count+1]) {
                        Ellipse ellipse = this.FindName("v" + (listSame[m_countSame].Item1 - 1)) as Ellipse;
                        ellipse.Fill = new SolidColorBrush(Colors.IndianRed);
                        ellipse = this.FindName("v" + (listSame[m_countSame].Item2 - 1)) as Ellipse;
                        ellipse.Fill = new SolidColorBrush(Colors.IndianRed);
                        m_countSame++;
                    }
                }
            } else if (m_flagCtrl == 2 && m_count == 0) {
                for (int i = 0; i < pattenStr.Length; i++) {
                    if (this.FindName("vd" + i) != null) {
                        MainCanvas.Children.Remove((UIElement)this.FindName("vd" + i));
                        this.UnregisterName("vd" + i);
                        MainCanvas.Children.Remove((UIElement)this.FindName("td" + i));
                        this.UnregisterName("td" + i);
                    }
                }
                for (int i = 0; i < mainStr.Length; i++) {
                    Ellipse ellipse = new Ellipse();
                    TextBlock text = new TextBlock();
                    ellipse.Width = 30;
                    ellipse.Height = 30;
                    ellipse.Name = "vm" + i;
                    this.RegisterName("vm" + i, ellipse);
                    if (i == 0) {
                        ellipse.Fill = new SolidColorBrush(Colors.IndianRed);
                    } else {
                        ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    }
                    ellipse.SetValue(Canvas.LeftProperty, 150 + 35 * i - ellipse.Width / 2);
                    ellipse.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - ellipse.Height / 2 - 40);

                    text.Text = mainStr[i].ToString();
                    text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size sizeText = text.DesiredSize;
                    text.SetValue(Canvas.LeftProperty, 150 + 35 * i - sizeText.Width / 2);
                    text.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - sizeText.Height / 2 - 40);
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    text.TextAlignment = TextAlignment.Center;
                    text.Name = "tm" + i;
                    this.RegisterName("tm" + i, text);

                    MainCanvas.Children.Add(ellipse);
                    MainCanvas.Children.Add(text);
                }
                Ellipse ellp = this.FindName("v0") as Ellipse;
                ellp.Fill = new SolidColorBrush(Colors.IndianRed);
            } else if (m_flagCtrl == 2 && m_count != 0) {
                for (int i = 0; i < pattenStr.Length; i++) {
                    Ellipse ellipse = this.FindName("v" + i) as Ellipse;
                    TextBlock text = this.FindName("t" + i) as TextBlock;
                    if (i < m_indexj) {
                        if (m_indexFlag == false) {
                            ellipse.Fill = new SolidColorBrush(Colors.IndianRed);
                        } else {
                            if (m_indexj > pattenStr.Length) {
                                ellipse.Fill = new SolidColorBrush(Colors.DarkSeaGreen);
                            } else if (m_indexi > mainStr.Length) {
                                ellipse.Fill = new SolidColorBrush(Colors.IndianRed);
                            }
                        }
                    } else {
                        ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    }
                    ellipse.SetValue(Canvas.LeftProperty, 150 + 35 * ((m_indexi - m_indexj) + i) - ellipse.Width / 2);
                    ellipse.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - ellipse.Height / 2);
                    text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size sizeText = text.DesiredSize;
                    text.SetValue(Canvas.LeftProperty, 150 + 35 * ((m_indexi - m_indexj) + i) - sizeText.Width / 2 - 5);
                    text.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - sizeText.Height / 2 - 5);

                    Ellipse ellnext = this.FindName("vn" + i) as Ellipse;
                    TextBlock tnext = this.FindName("tn" + i) as TextBlock;
                    ellnext.SetValue(Canvas.LeftProperty, 150 + 35 * ((m_indexi - m_indexj) + i) - ellnext.Width / 2 + 10);
                    ellnext.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - ellnext.Height / 2 + 5);
                    tnext.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size sizeTnext = tnext.DesiredSize;
                    tnext.SetValue(Canvas.LeftProperty, 150 + 35 * ((m_indexi - m_indexj) + i) - sizeTnext.Width / 2 + 10);
                    tnext.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - sizeTnext.Height / 2 + 5);
                }
                for(int i = 0; i < mainStr.Length; i++) {
                    Ellipse ellipse = this.FindName("vm" + i) as Ellipse;
                    if (i < m_indexi && i >= m_indexi - m_indexj) {
                        if (m_indexFlag == false) {
                            ellipse.Fill = new SolidColorBrush(Colors.IndianRed);
                        } else {
                            if (m_indexj > pattenStr.Length) {
                                ellipse.Fill = new SolidColorBrush(Colors.DarkSeaGreen);
                            } else if (m_indexi > mainStr.Length) {
                                ellipse.Fill = new SolidColorBrush(Colors.IndianRed);
                            }
                        }
                    } else {
                        ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    }   
                }
            } 
        }
        private void SetNext() {
            int i = 1, j = 0;
            next[1] = 0;
            while (i < pattenStr.Length) {
                if (j == 0 || pattenStr[i - 1] == pattenStr[j - 1]) {
                    ++i;
                    ++j;
                    next[i] = j;
                    nextbp[i] = j;
                } else {
                    j = next[j];
                }
            }
        }
        private void SetNextval() {
            int i = 1, j = 0;
            next[1] = 0;
            while (i < pattenStr.Length) {
                if (j == 0 || pattenStr[i - 1] == pattenStr[j - 1]) {
                    ++i;
                    ++j;
                    if (pattenStr[i - 1] != pattenStr[j - 1]) {
                        next[i] = j;
                    } else {
                        next[i] = next[j];
                        listSame.Add((i, j));
                    }
                } else {
                    j = next[j];
                }
            }
        }

        private void Index() {
            if (m_indexi <= mainStr.Length && m_indexj <= pattenStr.Length) {
                if (m_indexj == 0 || mainStr[m_indexi - 1] == pattenStr[m_indexj - 1]) {
                    m_indexi++;
                    m_indexj++;
                } else {
                    m_indexj = next[m_indexj];
                }
            }
            if (m_indexj > pattenStr.Length) {
                m_indexFlag = true;
            } else if (m_indexi > mainStr.Length) {
                MessageBox.Show("匹配失败，主串不包含此模式串"); 
                m_indexFlag = true;
            }
        }
        public void Filesave() {
            FileStream fs = new FileStream(MainWindowInfo.fileLocation, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            JObject jObject = new JObject();
            jObject["type"] = "KMP";
            jObject["mainStr"] = mainStr;
            jObject["pattenStr"] = pattenStr;
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
                Recevie(jObject["mainStr"].ToString(), jObject["pattenStr"].ToString());
                sr.Close();
                fs.Close();
            } catch {
                MessageBox.Show("请检查文件是否完整");
            }
        }
    }
}

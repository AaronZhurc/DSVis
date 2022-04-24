using DSVis.DataStruct;
using DSVis.Tools;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
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
    /// TreeTraWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TreeTraWindow : Page {
        private int m_flagCtrl;
        private bool m_flagLine = false, m_flagThread = false;
        private string titlename = "树的遍历";
        private List<TNode<char>> m_nodes = new List<TNode<char>>();
        private List<Dictionary<string, double>> VertexsPosi = new List<Dictionary<string, double>>();
        private List<Dictionary<string, double>> EdgesPosi = new List<Dictionary<string, double>>();
        private List<(Dictionary<string, double>, char)> ThreadsPosi = new List<(Dictionary<string, double>, char)>();
        private ArrayList m_VertexVisted = new ArrayList();
        private int m_countVertex, m_countEdge, m_countThread = 0;
        private int m_t_ellipse;
        private int m_count;
        public delegate void DataConfirm();
        public DataConfirm dataConfirm;
        public delegate void DataClean();
        public DataClean dataClean;
        public TreeTraWindow() {
            InitializeComponent();
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
        }

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e) {
            if (m_flagCtrl == 2) {
                if (m_flagLine == false)
                    return;
                line.X2 = e.GetPosition(MainCanvas).X;
                line.Y2 = e.GetPosition(MainCanvas).Y;
            }
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
            if (m_flagCtrl == 1) {
                Ellipse ellipse = new Ellipse();
                int x = Convert.ToInt32(e.GetPosition(MainCanvas).X);
                int y = Convert.ToInt32(e.GetPosition(MainCanvas).Y);
                double rx, ry;
                Dictionary<string, double> dict = new Dictionary<string, double>();
                rx = x / this.Width;
                ry = y / this.Height;
                dict.Add("X", rx);
                dict.Add("Y", ry);
                VertexsPosi.Add(dict);
                ellipse.Width = 30;
                ellipse.Height = 30;
                ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                ellipse.SetValue(Canvas.LeftProperty, x - ellipse.Width / 2);
                ellipse.SetValue(Canvas.TopProperty, y - ellipse.Height / 2);
                ellipse.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Ellipse_MouseLeftButtonDown);
                ellipse.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.Ellipse_MouseLeftButtonUp);
                ellipse.MouseEnter += new System.Windows.Input.MouseEventHandler(this.Ellipse_MouseEnter);
                ellipse.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Ellipse_MouseLeave);
                ellipse.Name = "v" + m_countVertex.ToString();
                this.RegisterName("v" + m_countVertex, ellipse);
                TextBlock text = new TextBlock();
                text.Text = ((char)('A' + m_countVertex)).ToString();
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size sizeText = text.DesiredSize;
                text.SetValue(Canvas.LeftProperty, x - sizeText.Width / 2);
                text.SetValue(Canvas.TopProperty, y - sizeText.Height / 2);
                text.HorizontalAlignment = HorizontalAlignment.Center;
                text.VerticalAlignment = VerticalAlignment.Center;
                text.TextAlignment = TextAlignment.Center;
                this.RegisterName("t" + m_countVertex, text);
                text.Name = "t" + m_countVertex.ToString();
                text.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Ellipse_MouseLeftButtonDown);
                text.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.Ellipse_MouseLeftButtonUp);
                text.MouseEnter += new System.Windows.Input.MouseEventHandler(this.Ellipse_MouseEnter);
                text.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Ellipse_MouseLeave);
                MainCanvas.Children.Add(ellipse);
                MainCanvas.Children.Add(text);
                TNode<char> node = new TNode<char>((char)('A' + m_countVertex), rx, ry);
                m_nodes.Add(node);
                m_countVertex++;
            } else if (m_flagCtrl == 2) {
                m_flagLine = true;
                line.X1 = e.GetPosition(MainCanvas).X;
                line.Y1 = e.GetPosition(MainCanvas).Y;
            }
        }

        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            if (m_flagCtrl == 2) {
                line.Visibility = Visibility.Hidden;
                m_t_ellipse = -1;
            }
        }

        private void SetVertex_Click(object sender, RoutedEventArgs e) {
            m_flagCtrl = 1;
            m_countVertex = 0;
            btnSetVertex.IsEnabled = false;
            btnClean.IsEnabled = true;
            btnSetEdge.IsEnabled = true;
            this.Title = titlename + " - 正在绘点";
        }

        private void SetEdge_Click(object sender, RoutedEventArgs e) {
            m_flagCtrl = 2;
            line.Visibility = Visibility.Visible;
            btnSetEdge.IsEnabled = false;
            btnSetVertex.IsEnabled = false;
            btnConfirm.IsEnabled = true;
            this.Title = titlename + " - 正在绘边";
        }
        private void btnConfirm_Click(object sender, RoutedEventArgs e) {
            dataConfirm();
            m_flagCtrl = 3;
            m_count = 0;
            line.Visibility = Visibility.Hidden;
            radiobtnPre.IsEnabled = true;
            radiobtnIn.IsEnabled = true;
            radiobtnPost.IsEnabled = true;
            btnNext.IsEnabled = true;
            btnSetVertex.IsEnabled = false;
            btnSetEdge.IsEnabled = false;
            btnClean.IsEnabled = true;
            btnConfirm.IsEnabled = false;
            this.Title = titlename + " - 正在演示";
        }
        private void btnClean_Click(object sender, RoutedEventArgs e) {
            dataClean();
            line.Visibility = Visibility.Hidden;
            for (int i = 0; i < m_countVertex; i++) {
                MainCanvas.Children.Remove((UIElement)this.FindName("v" + i));
                this.UnregisterName("v" + i);
                MainCanvas.Children.Remove((UIElement)this.FindName("t" + i));
                this.UnregisterName("t" + i);
            }
            m_countVertex = 0;
            for (int i = 0; i < m_countEdge; i++) {
                MainCanvas.Children.Remove((UIElement)this.FindName("l" + i));
                this.UnregisterName("l" + i);
            }
            for (int i = 0; i < m_countThread; i++) {
                if (this.FindName("th" + i) != null) {
                    MainCanvas.Children.Remove((UIElement)this.FindName("th" + i));
                    this.UnregisterName("th" + i);
                }
            }
            m_countEdge = 0;
            if (m_nodes != null) {
                m_nodes.Clear();
            }
            line.X1 = -1;
            line.X2 = -1;
            line.Y1 = -1;
            line.Y2 = -1;
            m_nodes.Clear();
            m_count = 0;
            m_VertexVisted.Clear();
            VertexsPosi.Clear();
            EdgesPosi.Clear();
            textResult.Text = "遍历结果:";
            m_flagThread = false;
            btnConfirm.IsEnabled = false;
            btnSetVertex.IsEnabled = true;
            btnSetEdge.IsEnabled = false;
            btnThread.IsEnabled = false;
            radiobtnPre.IsEnabled = false;
            radiobtnIn.IsEnabled = false;
            radiobtnPost.IsEnabled = false;
            btnNext.IsEnabled = false;
            this.Title = titlename;
            m_flagCtrl = -1;
        }
        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            m_t_ellipse = -1;
            if (sender is Ellipse) {
                Ellipse ellipse = sender as Ellipse;
                if (m_flagCtrl == 2) {
                    line.Visibility = Visibility.Visible;
                    m_flagLine = true;
                    line.X1 = ellipse.TranslatePoint(new Point(), MainCanvas).X;
                    line.Y1 = ellipse.TranslatePoint(new Point(), MainCanvas).Y;
                    String str = ellipse.Name;
                    String[] sstr = str.Split('v');
                    m_t_ellipse = int.Parse(sstr[1]);
                }
            } else if (sender is TextBlock) {
                TextBlock ellipse = sender as TextBlock;
                if (m_flagCtrl == 2) {
                    line.Visibility = Visibility.Visible;
                    m_flagLine = true;
                    line.X1 = ellipse.TranslatePoint(new Point(), MainCanvas).X;
                    line.Y1 = ellipse.TranslatePoint(new Point(), MainCanvas).Y;
                    String str = ellipse.Name;
                    String[] sstr = str.Split('t');
                    m_t_ellipse = int.Parse(sstr[1]);
                }
            }
        }

        private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
            double v1x, v1y, v2x, v2y;
            if (m_t_ellipse == -1) {
                return;
            }
            if (sender is Ellipse) {
                Ellipse ellipse = sender as Ellipse;
                if (m_flagCtrl == 2) {
                    m_flagLine = false;
                    TNode<char> v1 = new TNode<char>();
                    TNode<char> v2 = new TNode<char>();
                    String str = ellipse.Name;
                    String[] sstr = str.Split('v');
                    if (int.Parse(sstr[1]) == m_t_ellipse) {
                        return;
                    }
                    v1 = m_nodes[m_t_ellipse];
                    v2 = m_nodes[int.Parse(sstr[1])];
                    if (v1.TestLoopE(v2.Name) || (v1.Lchild != null && v1.Rchild != null)) {
                        return;
                    }
                    if (v1.SetEdge(v2)) {
                        Line line = new Line();
                        v1x = v1.X * this.Width;
                        v1y = v1.Y * this.Height;
                        v2x = v2.X * this.Width;
                        v2y = v2.Y * this.Height;
                        line.X1 = v1x;
                        line.Y1 = v1y;
                        line.X2 = v2x;
                        line.Y2 = v2y;
                        line.Stroke = new SolidColorBrush(Colors.Black);
                        line.StrokeThickness = 1;
                        line.Name = "l" + m_countEdge;
                        this.RegisterName("l" + m_countEdge, line);
                        m_countEdge++;
                        Dictionary<string, double> dict = new Dictionary<string, double>();
                        dict.Add("X1", line.X1 / this.Width);
                        dict.Add("Y1", line.Y1 / this.Height);
                        dict.Add("X2", line.X2 / this.Width);
                        dict.Add("Y2", line.Y2 / this.Height);
                        EdgesPosi.Add(dict);
                        MainCanvas.Children.Add(line);
                    }
                }
            } else if (sender is TextBlock) {
                TextBlock ellipse = sender as TextBlock;
                if (m_flagCtrl == 2) {
                    m_flagLine = false;
                    TNode<char> v1 = new TNode<char>();
                    TNode<char> v2 = new TNode<char>();
                    String str = ellipse.Name;
                    String[] sstr = str.Split('t');
                    if (int.Parse(sstr[1]) == m_t_ellipse) {
                        return;
                    }
                    v1 = m_nodes[m_t_ellipse];
                    v2 = m_nodes[int.Parse(sstr[1])];
                    if (v1.TestLoopE(v2.Name) || (v1.Lchild != null && v1.Rchild != null)) {
                        return;
                    }
                    if (v1.SetEdge(v2)) {
                        Line line = new Line();
                        v1x = v1.X * this.Width;
                        v1y = v1.Y * this.Height;
                        v2x = v2.X * this.Width;
                        v2y = v2.Y * this.Height;
                        line.X1 = v1x;
                        line.Y1 = v1y;
                        line.X2 = v2x;
                        line.Y2 = v2y;
                        line.Stroke = new SolidColorBrush(Colors.Black);
                        line.StrokeThickness = 1;
                        line.Name = "l" + m_countEdge;
                        this.RegisterName("l" + m_countEdge, line);
                        m_countEdge++;
                        Dictionary<string, double> dict = new Dictionary<string, double>();
                        dict.Add("X1", line.X1 / this.Width);
                        dict.Add("Y1", line.Y1 / this.Height);
                        dict.Add("X2", line.X2 / this.Width);
                        dict.Add("Y2", line.Y2 / this.Height);
                        EdgesPosi.Add(dict);
                        MainCanvas.Children.Add(line);
                    }
                }
            }
        }
        private void Ellipse_MouseEnter(object sender, MouseEventArgs e) {
            if (m_flagCtrl == 2) {
                Cursor = Cursors.Hand;
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e) {
            if (radiobtnPre.IsChecked == true && m_count < m_nodes.Count()) {
                if (m_count == 0) {
                    textResult.Text = " 遍历结果:";
                }
                btnNext.Content = "下一步";
                radiobtnIn.IsEnabled = false;
                radiobtnPost.IsEnabled = false;
                m_VertexVisted = m_nodes[0].GetVisited(0);
                Ellipse ellipse = FindName("v" + m_VertexVisted[m_count]) as Ellipse;
                ellipse.Fill = new SolidColorBrush(Colors.PaleVioletRed);
                textResult.Text += ((char)('A' + Convert.ToInt32(m_VertexVisted[m_count]))).ToString();
                m_count++;
            } else if (radiobtnIn.IsChecked == true && m_count < m_nodes.Count()) {
                if (m_count == 0) {
                    textResult.Text = " 遍历结果:";
                }
                btnNext.Content = "下一步";
                radiobtnPre.IsEnabled = false;
                radiobtnPost.IsEnabled = false;
                m_VertexVisted = m_nodes[0].GetVisited(1);
                Ellipse ellipse = FindName("v" + m_VertexVisted[m_count]) as Ellipse;
                ellipse.Fill = new SolidColorBrush(Colors.PaleVioletRed);
                textResult.Text += ((char)('A' + Convert.ToInt32(m_VertexVisted[m_count]))).ToString();
                m_count++;
            } else if (radiobtnPost.IsChecked == true && m_count < m_nodes.Count()) {
                if (m_count == 0) {
                    textResult.Text = " 遍历结果:";
                }
                btnNext.Content = "下一步";
                radiobtnPre.IsEnabled = false;
                radiobtnIn.IsEnabled = false;
                m_VertexVisted = m_nodes[0].GetVisited(2);
                Ellipse ellipse = FindName("v" + m_VertexVisted[m_count]) as Ellipse;
                ellipse.Fill = new SolidColorBrush(Colors.PaleVioletRed);
                textResult.Text += ((char)('A' + Convert.ToInt32(m_VertexVisted[m_count]))).ToString();
                m_count++;
            } else if (m_count == m_nodes.Count()) {
                m_count = 0;
                radiobtnPre.IsEnabled = true;
                radiobtnIn.IsEnabled = true; 
                radiobtnPost.IsEnabled = true;
                btnThread.IsEnabled = true;
                btnNext.Content = "重新遍历";
                for (int i = 0; i < m_countVertex; i++) {
                    Ellipse ellipse = FindName("v" + m_VertexVisted[i]) as Ellipse;
                    ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                }
                m_VertexVisted.Clear();
            }
        }

        public void windowchanged() {
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
            CtrlBg.Height = this.Height;
            MainCanvas.Height = this.Height;
            MainCanvas.Width = this.Width - 170;
            for (int i = 0; i < m_countVertex; i++) {
                Ellipse ellipse = this.FindName("v" + i) as Ellipse;
                Point point = ellipse.TranslatePoint(new Point(), MainCanvas);
                ellipse.SetValue(Canvas.LeftProperty, (VertexsPosi[i]["X"] * this.Width) - (ellipse.Width / 2));
                ellipse.SetValue(Canvas.TopProperty, (VertexsPosi[i]["Y"] * this.Height) - (ellipse.Height / 2));
                TextBlock tb = this.FindName("t" + i) as TextBlock;
                Size sizeText = tb.DesiredSize;
                tb.SetValue(Canvas.LeftProperty, (VertexsPosi[i]["X"] * this.Width) - (sizeText.Width / 2));
                tb.SetValue(Canvas.TopProperty, (VertexsPosi[i]["Y"] * this.Height) - (sizeText.Height / 2));
            }
            for (int i = 0; i < m_countEdge; i++) {
                Line l = this.FindName("l" + i) as Line;
                l.X1 = EdgesPosi[i]["X1"] * this.Width;
                l.Y1 = EdgesPosi[i]["Y1"] * this.Height;
                l.X2 = EdgesPosi[i]["X2"] * this.Width;
                l.Y2 = EdgesPosi[i]["Y2"] * this.Height;
            }
            for(int i = 0; i < m_countThread; i++) {
                Arrow arrow = this.FindName("th" + i) as Arrow;
                if (ThreadsPosi[i].Item2 == 'l') {
                    arrow.X1 = ThreadsPosi[i].Item1["X1"] * this.Width - 15;
                } else {
                    arrow.X1 = ThreadsPosi[i].Item1["X1"] * this.Width + 15;
                }
                arrow.Y1 = ThreadsPosi[i].Item1["Y1"] * this.Height;
                arrow.X2 = ThreadsPosi[i].Item1["X2"] * this.Width;
                arrow.Y2 = ThreadsPosi[i].Item1["Y2"] * this.Height;
            }
        }

        private void Ellipse_MouseLeave(object sender, MouseEventArgs e) {
            if (m_flagCtrl == 2) {
                Cursor = Cursors.Arrow;
            }
        }
        public void Filesave() {
            FileStream fs = new FileStream(MainWindowInfo.fileLocation, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            JObject jObject = new JObject();
            jObject["type"] = "TreeTra";
            JArray jArray = new JArray();
            foreach (TNode<char> node in m_nodes) {
                JObject jonode = new JObject();
                jonode["name"] = node.Name.ToString();
                jonode["mark"] = node.Mark;
                jonode["height"] = node.Height;
                jonode["x"] = node.X;
                jonode["y"] = node.Y;
                jonode["ltag"] = node.Ltag;
                jonode["rtag"] = node.Rtag;
                if (node.Parent != null) {
                    jonode["parent"] = node.Parent.Name.ToString();
                } else {
                    jonode["parent"] = "null";
                }
                if (node.Lchild != null) {
                    jonode["lchild"] = node.Lchild.Name.ToString();
                } else {
                    jonode["lchild"] = "null";
                }
                if (node.Rchild != null) {
                    jonode["rchild"] = node.Rchild.Name.ToString();
                } else {
                    jonode["rchild"] = "null";
                }
                jArray.Add(jonode);
            }
            jObject["nodes"] = jArray;
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
            foreach(JObject jonode in jObject["nodes"]) {
                TNode<char> node = new TNode<char>((char)jonode["name"], (double)jonode["x"], (double)jonode["y"]);
                node.Mark = (int)jonode["mark"];
                node.Height = (int)jonode["height"];
                node.Ltag = (int)jonode["ltag"];
                node.Rtag = (int)jonode["rtag"];
                m_nodes.Add(node);
            }
            foreach (JObject jonode in jObject["nodes"]) {
                if (jonode["parent"].ToString() != "null") {
                    FindNode((char)jonode["parent"]).SetEdge(FindNode((char)jonode["name"]));
                }
            }
            Receive();
            btnConfirm_Click(null, null);
            sr.Close();
            fs.Close();
        }
        public TNode<char> FindNode(char c) {
            foreach(TNode<char> node in m_nodes) {
                if (node.Name == c) {
                    return node;
                }
            }
            return null;
        }

        private void btnThread_Click(object sender, RoutedEventArgs e) {
            if (m_flagThread == false) {
                m_flagThread = true;
                m_countThread = 0;
                btnThread.Content = "关闭线索";
                TNode<char> t = m_nodes[0];
                if (radiobtnPre.IsChecked == true) {
                    m_nodes[0].CreateThread(ref t, 1);
                } else if (radiobtnIn.IsChecked == true) {
                    m_nodes[0].CreateThread(ref t, 2);
                } else if (radiobtnPost.IsChecked == true) {
                    m_nodes[0].CreateThread(ref t, 3);
                }
                DrawThread();
            } else {
                btnThread.Content = "线索化";
                m_flagThread = false;
                for (int i = 0; i < m_countThread; i++) {
                    MainCanvas.Children.Remove((UIElement)this.FindName("th" + i));
                    this.UnregisterName("th" + i);
                }
                m_nodes[0].ClearThread();
                m_countThread = 0;
            }
        }


        private void DrawThread() {
            for(int i = 0; i < m_nodes.Count; i++) {
                double v1x, v1y, v2x, v2y;
                if (m_nodes[i].Ltag == 1 && m_nodes[i].Lchild != null) {
                    Arrow arrow = new Arrow();
                    arrow.HeadHeight = 5;
                    arrow.HeadWidth = 10;
                    v1x = m_nodes[i].X * this.Width;
                    v1y = m_nodes[i].Y * this.Height;
                    v2x = m_nodes[i].Lchild.X * this.Width;
                    v2y = m_nodes[i].Lchild.Y * this.Height;
                    if (m_nodes[i].Lchild == m_nodes[i].Parent.Lchild) {
                        arrow.X1 = v1x;
                        arrow.Y1 = v1y + 15;
                        arrow.X2 = v2x;
                        arrow.Y2 = v2y;
                    } else {
                        arrow.X1 = v1x - 15;
                        arrow.Y1 = v1y;
                        arrow.X2 = v2x;
                        arrow.Y2 = v2y;
                    }
                    arrow.Stroke = new SolidColorBrush(Colors.OrangeRed);
                    arrow.StrokeThickness = 1;
                    arrow.StrokeDashArray = new DoubleCollection { 5, 3 };
                    arrow.Name = "th" + m_countThread;
                    this.RegisterName("th" + m_countThread, arrow);
                    Dictionary<string, double> dict = new Dictionary<string, double>();
                    dict.Add("X1", v1x / this.Width);
                    dict.Add("Y1", arrow.Y1 / this.Height);
                    dict.Add("X2", arrow.X2 / this.Width);
                    dict.Add("Y2", arrow.Y2 / this.Height);
                    ThreadsPosi.Add((dict, 'l'));
                    MainCanvas.Children.Add(arrow);
                    m_countThread++;
                }
                if(m_nodes[i].Rtag == 1 && m_nodes[i].Rchild != null) {
                    Arrow arrow = new Arrow();
                    arrow.HeadHeight = 5;
                    arrow.HeadWidth = 10;
                    v1x = m_nodes[i].X * this.Width;
                    v1y = m_nodes[i].Y * this.Height;
                    v2x = m_nodes[i].Rchild.X * this.Width;
                    v2y = m_nodes[i].Rchild.Y * this.Height;
                    arrow.X1 = v1x + 15;
                    arrow.Y1 = v1y;
                    arrow.X2 = v2x;
                    arrow.Y2 = v2y;
                    arrow.Stroke = new SolidColorBrush(Colors.OrangeRed);
                    arrow.StrokeThickness = 1;
                    arrow.StrokeDashArray = new DoubleCollection { 5, 3 };
                    arrow.Name = "th" + m_countThread;
                    this.RegisterName("th" + m_countThread, arrow);
                    Dictionary<string, double> dict = new Dictionary<string, double>();
                    dict.Add("X1", v1x / this.Width);
                    dict.Add("Y1", arrow.Y1 / this.Height);
                    dict.Add("X2", arrow.X2 / this.Width);
                    dict.Add("Y2", arrow.Y2 / this.Height);
                    ThreadsPosi.Add((dict, 'r'));
                    MainCanvas.Children.Add(arrow);
                    m_countThread++;
                }
            }
        }

        public void Receive() {
            foreach(TNode<char> node in m_nodes) {
                Ellipse ellipse = new Ellipse();
                Dictionary<string, double> dict = new Dictionary<string, double>();
                dict.Add("X", node.X);
                dict.Add("Y", node.Y);
                VertexsPosi.Add(dict);
                double x = node.X * this.Width;
                double y = node.Y * this.Height;
                ellipse.Width = 30;
                ellipse.Height = 30;
                ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                ellipse.SetValue(Canvas.LeftProperty, x - ellipse.Width / 2);
                ellipse.SetValue(Canvas.TopProperty, y - ellipse.Height / 2);
                ellipse.Name = "v" + m_countVertex.ToString();
                this.RegisterName("v" + m_countVertex, ellipse);
                TextBlock text = new TextBlock();
                text.Text = ((char)('A' + m_countVertex)).ToString();
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size sizeText = text.DesiredSize;
                text.SetValue(Canvas.LeftProperty, x - sizeText.Width / 2);
                text.SetValue(Canvas.TopProperty, y - sizeText.Height / 2);
                text.HorizontalAlignment = HorizontalAlignment.Center;
                text.VerticalAlignment = VerticalAlignment.Center;
                text.TextAlignment = TextAlignment.Center;
                this.RegisterName("t" + m_countVertex, text);
                text.Name = "t" + m_countVertex.ToString();
                MainCanvas.Children.Add(ellipse);
                MainCanvas.Children.Add(text);
                m_countVertex++;
                if (node.Parent != null) {
                    double v1x, v1y, v2x, v2y;
                    Line line = new Line();
                    v1x = node.X * this.Width;
                    v1y = node.Y * this.Height;
                    v2x = node.Parent.X * this.Width;
                    v2y = node.Parent.Y * this.Height;
                    line.X1 = v1x;
                    line.Y1 = v1y;
                    line.X2 = v2x;
                    line.Y2 = v2y;
                    line.Stroke = new SolidColorBrush(Colors.Black);
                    line.StrokeThickness = 1;
                    line.Name = "l" + m_countEdge;
                    this.RegisterName("l" + m_countEdge, line);
                    m_countEdge++;
                    Dictionary<string, double> dictl = new Dictionary<string, double>();
                    dictl.Add("X1", line.X1 / this.Width);
                    dictl.Add("Y1", line.Y1 / this.Height);
                    dictl.Add("X2", line.X2 / this.Width);
                    dictl.Add("Y2", line.Y2 / this.Height);
                    EdgesPosi.Add(dictl);
                    MainCanvas.Children.Add(line);
                }
            }
        }
    }
}

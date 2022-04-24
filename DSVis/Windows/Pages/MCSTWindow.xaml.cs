using DSVis.DataStruct;
using DSVis.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// MCSTWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MCSTWindow : Page {
        private int m_flagCtrl, m_flagEnd = 0;
        private bool m_flagLine = false;
        private List<Node<int>> m_nodes = new List<Node<int>>();
        private List<(int, int, int)> m_edges = new List<(int, int, int)>();
        private List<Dictionary<string, double>> VertexsPosi = new List<Dictionary<string, double>>();
        private List<Dictionary<string, double>> EdgesPosi = new List<Dictionary<string, double>>();
        private ELGraph<int> m_graph;
        private int m_countVertex, m_countEdge, m_countWeight;
        private int m_t_ellipse;
        private int m_count;
        private String strLine;
        private int[] m_recWeight;
        private ArrayList m_EdgeVisted = new ArrayList();
        private string titlename = "最小生成树";
        public delegate void DataConfirm();
        public DataConfirm dataConfirm;
        public delegate void DataClean();
        public DataClean dataClean;
        public MCSTWindow() {
            InitializeComponent();
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
            CtrlBg.Height = this.Height;
            MainCanvas.Height = this.Height;
            MainCanvas.Width = this.Width - 170;
        }
        private void btnSetVertex_Click(object sender, RoutedEventArgs e) {
            m_flagCtrl = 1;
            m_countVertex = 0;
            btnSetVertex.IsEnabled = false;
            btnClean.IsEnabled = true;
            btnSetEdge.IsEnabled = true;
            this.Title = titlename + " - 正在绘点";
        }
        private void btnSetEdge_Click(object sender, RoutedEventArgs e) {
            m_flagCtrl = 2;
            m_countEdge = 0;
            line.Visibility = Visibility.Visible;
            m_graph = new ELGraph<int>(m_nodes.ToArray());
            btnSetEdge.IsEnabled = false;
            btnSetVertex.IsEnabled = false;
            btnSetWeight.IsEnabled = true;
            this.Title = titlename + " - 正在绘边";
        }
        private void btnConfirm_Click(object sender, RoutedEventArgs e) {
            if (m_countWeight == m_countEdge) {
                dataConfirm();
                m_flagCtrl = 3;
                m_count = 0;
                line.Visibility = Visibility.Hidden;
                radiobtnPrim.IsEnabled = true;
                radiobtnKrus.IsEnabled = true;
                btnNext.IsEnabled = true;
                btnSetVertex.IsEnabled = false;
                btnSetEdge.IsEnabled = false;
                btnSetWeight.IsEnabled = false;
                btnConfirm.IsEnabled = false;
                btnClean.IsEnabled = true;
                this.Title = titlename + " - 正在演示";
            } else {
                m_flagCtrl = 4;
                string str = "请确认是否为 ";
                for (int i = 0; i < m_countEdge; i++) {
                    if (m_recWeight[i] != -1) {
                        str += "l" + i + " ";
                    }
                }
                str += "付了权值";
                MessageBox.Show(str);
            }
        }
        private void MainCanvas_MouseMove(object sender, MouseEventArgs e) {//Canvas鼠标移动
            if (m_flagCtrl == 2) {
                if (m_flagLine == false)//画线
                    return;
                line.X2 = e.GetPosition(MainCanvas).X;
                line.Y2 = e.GetPosition(MainCanvas).Y;
            }
        }

        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {//Canvas左鼠标按下
            this.Height = MainWindowInfo.mainPageHeight;//这段主要是方便全屏或者拖拽放大窗口时的ui自适应
            this.Width = MainWindowInfo.mainPageWidth;
            if (m_flagCtrl == 1) {//设置结点
                Ellipse ellipse = new Ellipse();
                int x = Convert.ToInt32(e.GetPosition(MainCanvas).X);//获取鼠标x坐标
                int y = Convert.ToInt32(e.GetPosition(MainCanvas).Y);//y坐标
                double rx, ry;
                Dictionary<string, double> dict = new Dictionary<string, double>();//方便自适应
                rx = x / this.Width;
                ry = y / this.Height;
                dict.Add("X", rx);
                dict.Add("Y", ry);
                VertexsPosi.Add(dict);
                ellipse.Width = 30;//设置宽度
                ellipse.Height = 30;//高度
                ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);//填充颜色
                ellipse.SetValue(Canvas.LeftProperty, x - ellipse.Width / 2);//x坐标位置设定
                ellipse.SetValue(Canvas.TopProperty, y - ellipse.Height / 2);//y坐标位置设定
                ellipse.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Ellipse_MouseLeftButtonDown);//ellipse左鼠标按下
                ellipse.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.Ellipse_MouseLeftButtonUp);//左鼠标松起
                ellipse.MouseEnter += new System.Windows.Input.MouseEventHandler(this.Ellipse_MouseEnter);//鼠标进入ellipse
                ellipse.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Ellipse_MouseLeave);//鼠标离开
                ellipse.Name = "v" + m_countVertex.ToString();//命名
                this.RegisterName("v" + m_countVertex, ellipse);//注册名称
                TextBlock text = new TextBlock();//ellipse上显示的字，后同，略
                text.Text = "v" + m_countVertex;
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));//这个是计算text的实际长宽
                Size sizeText = text.DesiredSize;//获取text长宽
                text.SetValue(Canvas.LeftProperty, x - sizeText.Width / 2);
                text.SetValue(Canvas.TopProperty, y - sizeText.Height / 2);
                text.HorizontalAlignment = HorizontalAlignment.Center;//几个居中显示的属性设置
                text.VerticalAlignment = VerticalAlignment.Center;
                text.TextAlignment = TextAlignment.Center;
                this.RegisterName("t" + m_countVertex, text);
                text.Name = "t" + m_countVertex.ToString();
                text.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Ellipse_MouseLeftButtonDown);
                text.MouseLeftButtonUp += new System.Windows.Input.MouseButtonEventHandler(this.Ellipse_MouseLeftButtonUp);
                text.MouseEnter += new System.Windows.Input.MouseEventHandler(this.Ellipse_MouseEnter);
                text.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Ellipse_MouseLeave);
                MainCanvas.Children.Add(ellipse);//成为canvas的子控件
                MainCanvas.Children.Add(text);
                Node<int> node = new Node<int>(m_countVertex, rx, ry);
                m_countVertex++;
                m_nodes.Add(node);
            } else if (m_flagCtrl == 2) {//设置边
                m_flagLine = true;//开始画边
                line.X1 = e.GetPosition(MainCanvas).X;
                line.Y1 = e.GetPosition(MainCanvas).Y;
            }
        }

        private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {//Canvas 鼠标松开
            if (m_flagCtrl == 2) {//设置边
                line.Visibility = Visibility.Hidden;//隐藏line
                m_t_ellipse = -1;//避免边连上同样的结点
            }
        }

        private void btnClean_Click(object sender, RoutedEventArgs e) {
            dataClean();
            line.Visibility = Visibility.Hidden;
            for (int i = 0; i < m_countVertex; i++) {
                MainCanvas.Children.Remove((UIElement)this.FindName("v" + i));//从canvas上移除ellipse，只要知道名字就行
                this.UnregisterName("v" + i);//注销名称
                MainCanvas.Children.Remove((UIElement)this.FindName("t" + i));
                this.UnregisterName("t" + i);
            }
            m_countVertex = 0;
            for (int i = 0; i < m_countEdge; i++) {
                MainCanvas.Children.Remove((UIElement)this.FindName("l" + i));
                this.UnregisterName("l" + i);
                MainCanvas.Children.Remove((UIElement)this.FindName("tl" + i));
                this.UnregisterName("tl" + i);
            }
            m_countEdge = 0;
            m_countWeight = 0;
            if (m_graph != null) {
                m_graph.DelGraph();
            }
            line.X1 = -1;
            line.X2 = -1;
            line.Y1 = -1;
            line.Y2 = -1;
            m_nodes.Clear();
            VertexsPosi.Clear();
            EdgesPosi.Clear();
            btnConfirm.IsEnabled = false;
            btnSetVertex.IsEnabled = true;
            btnSetEdge.IsEnabled = false;
            btnSetWeight.IsEnabled = false;
            radiobtnPrim.IsEnabled = false;
            radiobtnKrus.IsEnabled = false;
            btnNext.IsEnabled = false;
            this.Title = titlename;
            m_flagCtrl = -1;
            m_flagEnd = 0;
        }

        private void btnSetWeight_Click(object sender, RoutedEventArgs e) {
            m_flagCtrl = 4;
            m_countWeight = 0;
            btnSetWeight.IsEnabled = false;
            btnSetEdge.IsEnabled = false;
            btnConfirm.IsEnabled = true;
            line.Visibility = Visibility.Hidden;
            m_recWeight = new int[m_countEdge];
            for (int i = 0; i < m_countEdge; i++) {
                m_recWeight[i] = 1;
            }
            this.Title = titlename + " - 正在赋权值";
        }

        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {//结点选中
            m_t_ellipse = -1;
            if (sender is Ellipse) {//如果控件是Ellipse类型，sender获取选中的控件自己
                Ellipse ellipse = sender as Ellipse;
                if (m_flagCtrl == 2) {
                    line.Visibility = Visibility.Visible;//显示线
                    m_flagLine = true;//画线开始
                    line.X1 = ellipse.TranslatePoint(new Point(), MainCanvas).X;
                    line.Y1 = ellipse.TranslatePoint(new Point(), MainCanvas).Y;
                    String str = ellipse.Name;//通过名称获取我控制的是哪一个结点，主要是数据结构层的操作
                    String[] sstr = str.Split('v');
                    m_t_ellipse = int.Parse(sstr[1]);
                }
            } else if (sender is TextBlock) {//如果控件是TextBlock类型，如果点结点点上的是文字就走这里，后同，略
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

        private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {//结点左鼠标松起，注意我们拖动Line的时候是拖到另一个结点上松起
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
            double v1x, v1y, v2x, v2y;
            if (m_t_ellipse == -1) {//如果在同一个结点上松起直接返回，后面的不执行
                return;
            }
            if (sender is Ellipse) {
                Ellipse ellipse = sender as Ellipse;
                if (m_flagCtrl == 2) {
                    m_flagLine = false;//画线结束
                    Node<int> v1 = new Node<int>(-1, -1, -1);
                    Node<int> v2 = new Node<int>(-1, -1, -1);
                    String str = ellipse.Name;
                    String[] sstr = str.Split('v');
                    if (int.Parse(sstr[1]) == m_t_ellipse) {
                        return;
                    }
                    v1 = m_nodes[m_t_ellipse];
                    v2 = m_nodes[int.Parse(sstr[1])];
                    if (m_graph.SetEdge(v1, v2) == true) {//创建边
                        m_edges.Add((v1.Data, v2.Data, 0));
                        Line line = new Line();//建一条边
                        v1x = v1.X * this.Width;
                        v1y = v1.Y * this.Height;
                        v2x = v2.X * this.Width;
                        v2y = v2.Y * this.Height;
                        line.X1 = v1x;//边的起始点
                        line.Y1 = v1y;
                        line.X2 = v2x;//结束点
                        line.Y2 = v2y;
                        line.Stroke = new SolidColorBrush(Colors.Black);//边颜色，注意不是Fill
                        line.StrokeThickness = 1;//边宽度
                        line.MouseEnter += new System.Windows.Input.MouseEventHandler(this.Line_MouseEnter);
                        line.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Line_MouseLeave);
                        line.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Line_MouseLeftButtonDown);
                        line.Name = "l" + m_countEdge;
                        this.RegisterName("l" + m_countEdge, line);
                        TextBlock text = new TextBlock();
                        text.Text = "l" + m_countEdge;
                        text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        Size sizeText = text.DesiredSize;
                        text.SetValue(Canvas.LeftProperty, Convert.ToDouble(v1x + (v2x - v1x) / 2));
                        text.SetValue(Canvas.TopProperty, Convert.ToDouble(v1y + (v2y - v1y) / 2));
                        text.HorizontalAlignment = HorizontalAlignment.Center;
                        text.VerticalAlignment = VerticalAlignment.Center;
                        text.TextAlignment = TextAlignment.Center;
                        this.RegisterName("tl" + m_countEdge, text);
                        text.Name = "tl" + m_countEdge.ToString();
                        text.MouseEnter += new System.Windows.Input.MouseEventHandler(this.Line_MouseEnter);
                        text.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Line_MouseLeave);
                        text.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Line_MouseLeftButtonDown);
                        m_countEdge++;
                        Dictionary<string, double> dict = new Dictionary<string, double>();
                        dict.Add("X1", line.X1 / this.Width);
                        dict.Add("Y1", line.Y1 / this.Height);
                        dict.Add("X2", line.X2 / this.Width);
                        dict.Add("Y2", line.Y2 / this.Height);
                        EdgesPosi.Add(dict);
                        MainCanvas.Children.Add(line);
                        MainCanvas.Children.Add(text);
                    }
                }
            } else if (sender is TextBlock) {
                TextBlock ellipse = sender as TextBlock;
                if (m_flagCtrl == 2) {
                    m_flagLine = false;
                    Node<int> v1 = new Node<int>(-1, -1, -1);
                    Node<int> v2 = new Node<int>(-1, -1, -1);
                    String str = ellipse.Name;
                    String[] sstr = str.Split('t');
                    if (int.Parse(sstr[1]) == m_t_ellipse) {
                        return;
                    }
                    v1 = m_nodes[m_t_ellipse];
                    v2 = m_nodes[int.Parse(sstr[1])];
                    if (m_graph.SetEdge(v1, v2) == true) {
                        m_edges.Add((v1.Data, v2.Data, 0));
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
                        line.MouseEnter += new System.Windows.Input.MouseEventHandler(this.Line_MouseEnter);
                        line.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Line_MouseLeave);
                        line.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Line_MouseLeftButtonDown);
                        line.Name = "l" + m_countEdge;
                        this.RegisterName("l" + m_countEdge, line);
                        TextBlock text = new TextBlock();
                        text.Text = "l" + m_countEdge;
                        text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        Size sizeText = text.DesiredSize;
                        text.SetValue(Canvas.LeftProperty, Convert.ToDouble(v1x + (v2x - v1x) / 2));
                        text.SetValue(Canvas.TopProperty, Convert.ToDouble(v1y + (v2y - v1y) / 2));
                        text.HorizontalAlignment = HorizontalAlignment.Center;
                        text.VerticalAlignment = VerticalAlignment.Center;
                        text.TextAlignment = TextAlignment.Center;
                        this.RegisterName("tl" + m_countEdge, text);
                        text.Name = "tl" + m_countEdge.ToString();
                        text.MouseEnter += new System.Windows.Input.MouseEventHandler(this.Line_MouseEnter);
                        text.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Line_MouseLeave);
                        text.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Line_MouseLeftButtonDown);
                        m_countEdge++;
                        Dictionary<string, double> dict = new Dictionary<string, double>();
                        dict.Add("X1", line.X1 / this.Width);
                        dict.Add("Y1", line.Y1 / this.Height);
                        dict.Add("X2", line.X2 / this.Width);
                        dict.Add("Y2", line.Y2 / this.Height);
                        EdgesPosi.Add(dict);
                        MainCanvas.Children.Add(line);
                        MainCanvas.Children.Add(text);
                    }
                }
            }
        }
        private void Ellipse_MouseEnter(object sender, MouseEventArgs e) {//鼠标进入ellipse
            if (m_flagCtrl == 2) {
                Cursor = Cursors.Hand;//鼠标变手型
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e) {
            if (radiobtnPrim.IsChecked == true && m_count == 0) {
                btnNext.Content = "下一步";
                radiobtnKrus.IsEnabled = false;
                m_EdgeVisted = m_graph.GetVisited(0);
            } else if (radiobtnKrus.IsChecked == true && m_count == 0) {
                m_EdgeVisted = m_graph.GetVisited(1);
                btnNext.Content = "下一步";
                radiobtnPrim.IsEnabled = false;
            }
            if (radiobtnPrim.IsChecked == true && m_count < m_graph.GetVertexNum() - 1) {
                Line line = FindName("l" + m_EdgeVisted[m_count]) as Line;
                line.Stroke = new SolidColorBrush(Colors.PaleVioletRed);
                line.StrokeThickness = 2;
                m_count++;
            } else if (radiobtnKrus.IsChecked == true && m_count < m_graph.GetVertexNum() - 1) {
                Line line = FindName("l" + m_EdgeVisted[m_count]) as Line;
                line.Stroke = new SolidColorBrush(Colors.PaleVioletRed);
                line.StrokeThickness = 2;
                m_count++;
            } else if (m_count == m_graph.GetVertexNum() - 1) {
                if (m_flagEnd == 0) {
                    for (int i = 0; i < m_countEdge; i++) {
                        if (!m_EdgeVisted.Contains(i)) {
                            Line line = FindName("l" + i) as Line;
                            line.Visibility = Visibility.Hidden;
                            TextBlock text = FindName("tl" + i) as TextBlock;
                            text.Visibility = Visibility.Hidden;
                        }
                    }
                    m_flagEnd = 1;
                } else {
                    m_count = 0;
                    radiobtnPrim.IsEnabled = true;
                    radiobtnKrus.IsEnabled = true;
                    btnNext.Content = "重新遍历";
                    for (int i = 0; i < m_countEdge; i++) {
                        Line line = FindName("l" + i) as Line;
                        line.Stroke = new SolidColorBrush(Colors.Black);
                        line.StrokeThickness = 1;
                        if (!m_EdgeVisted.Contains(i)) {
                            line.Visibility = Visibility.Visible;
                            TextBlock text = FindName("tl" + i) as TextBlock;
                            text.Visibility = Visibility.Visible;
                        }
                    }
                    m_EdgeVisted.Clear();
                    m_graph.InitVisited();
                    m_flagEnd = 0;
                }
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
                TextBlock text = this.FindName("tl" + i) as TextBlock;
                Size sizeText = text.DesiredSize;
                text.SetValue(Canvas.LeftProperty, Convert.ToDouble(l.X1 + (l.X2 - l.X1) / 2));
                text.SetValue(Canvas.TopProperty, Convert.ToDouble(l.Y1 + (l.Y2 - l.Y1) / 2));
            }
        }

        private void Ellipse_MouseLeave(object sender, MouseEventArgs e) {//鼠标移出ellipse
            if (m_flagCtrl == 2) {
                Cursor = Cursors.Arrow;//鼠标变箭头
            }
        }

        private void Line_MouseEnter(object sender, MouseEventArgs e) {
            if (m_flagCtrl == 4)
                Cursor = Cursors.Hand;
        }
        private void Line_MouseLeave(object sender, MouseEventArgs e) {
            if (m_flagCtrl == 4)
                Cursor = Cursors.Arrow;
        }
        private void Line_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (m_flagCtrl == 4 && sender is Line) {
                Line line = sender as Line;
                SetWeightForm form = new SetWeightForm(line.Name);
                strLine = line.Name;
                form.Show();
                form.sendMessage = Recevie;
            } else if (m_flagCtrl == 4 && sender is TextBlock) {
                TextBlock text = sender as TextBlock;
                SetWeightForm form = new SetWeightForm(text.Text);
                strLine = text.Text;
                form.Show();
                form.sendMessage = Recevie;
            }
        }
        public void Recevie(int value) {
            TextBlock text = this.FindName("t" + strLine) as TextBlock;
            text.Text = value.ToString();
            string[] str = strLine.Split('l');//分割
            int i = Convert.ToInt32(str[1]);
            m_graph.SetWeight(i, value);
            m_edges[i] = (m_edges[i].Item1, m_edges[i].Item2, value);
            m_recWeight[i] = -1;
            m_countWeight++;
        }
        public void Filesave() {
            FileStream fs = new FileStream(MainWindowInfo.fileLocation, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            JObject jObject = new JObject();
            jObject["type"] = "MCST";
            JArray jArray = new JArray();
            foreach (Node<int> node in m_nodes) {
                JObject jonode = new JObject();
                jonode["data"] = node.Data;
                jonode["x"] = node.X;
                jonode["y"] = node.Y;
                jArray.Add(jonode);
            }
            jObject["nodes"] = jArray;
            jArray = new JArray();
            foreach ((int, int, int) edge in m_edges) {
                JObject jonode = new JObject();
                jonode["start"] = edge.Item1;
                jonode["end"] = edge.Item2;
                jonode["weight"] = edge.Item3;
                jArray.Add(jonode);
            }
            jObject["edges"] = jArray;
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
            foreach (JObject jonode in jObject["nodes"]) {
                Node<int> node = new Node<int>();
                node.Data = (int)jonode["data"];
                node.X = (double)jonode["x"];
                node.Y = (double)jonode["y"];
                m_nodes.Add(node);
            }
            foreach (JObject jonode in jObject["edges"]) {
                m_edges.Add(((int)jonode["start"], (int)jonode["end"], (int)jonode["weight"]));
            }
            Receive();
            btnConfirm_Click(null, null);
            sr.Close();
            fs.Close();
        }
        public Node<int> FindNode(int c) {
            foreach (Node<int> node in m_nodes) {
                if (node.Data == c) {
                    return node;
                }
            }
            return null;
        }
        public void Receive() {
            m_graph = new ELGraph<int>(m_nodes.ToArray());
            foreach (Node<int> node in m_nodes) {
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
            }
            m_recWeight = new int[m_edges.Count];
            for (int i = 0; i < m_edges.Count; i++) {
                m_recWeight[i] = 1;
            }
            double v1x, v1y, v2x, v2y;
            foreach ((int, int, int) edge in m_edges) {
                Node<int> v1 = FindNode(edge.Item1);
                Node<int> v2 = FindNode(edge.Item2);
                if (m_graph.SetEdge(v1, v2, edge.Item3) == true) {
                    m_recWeight[m_countEdge] = edge.Item3;
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
                    this.RegisterName("l" + m_countEdge, line);
                    TextBlock text = new TextBlock();
                    text.Text = edge.Item3.ToString();
                    text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size sizeText = text.DesiredSize;
                    text.SetValue(Canvas.LeftProperty, Convert.ToDouble(v1x + (v2x - v1x) / 2));
                    text.SetValue(Canvas.TopProperty, Convert.ToDouble(v1y + (v2y - v1y) / 2));
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    text.TextAlignment = TextAlignment.Center;
                    this.RegisterName("tl" + m_countEdge, text);
                    text.Name = "tl" + m_countEdge.ToString();
                    m_countEdge++;
                    m_countWeight++;
                    Dictionary<string, double> dict = new Dictionary<string, double>();
                    dict.Add("X1", line.X1 / this.Width);
                    dict.Add("Y1", line.Y1 / this.Height);
                    dict.Add("X2", line.X2 / this.Width);
                    dict.Add("Y2", line.Y2 / this.Height);
                    EdgesPosi.Add(dict);
                    MainCanvas.Children.Add(line);
                    MainCanvas.Children.Add(text);
                }
            }
        }
    }
}

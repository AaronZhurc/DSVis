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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DSVis.Windows.Pages {
    /// <summary>
    /// TopoSortWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TopoSortWindow : Page {
        private int m_flagCtrl;
        //1 设点 2 设边 3 演示
        private bool m_flagLine = false;
        private int m_countVertex, m_countEdge;
        private ALGraph<int> m_graph;
        private List<Dictionary<string, double>> VertexsPosi = new List<Dictionary<string, double>>();
        private List<Dictionary<string, double>> EdgesPosi = new List<Dictionary<string, double>>();
        private List<Node<int>> m_nodes = new List<Node<int>>();
        private List<ENode<int>> m_edges = new List<ENode<int>>();
        private ArrayList m_VertexVisted = new ArrayList();
        private int m_t_ellipse;//避免边连上同样的结点
        private int m_count;
        public delegate void DataConfirm();
        public DataConfirm dataConfirm;
        public delegate void DataClean();
        public DataClean dataClean;
        public TopoSortWindow() {
            InitializeComponent();
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
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
                Arrow l = this.FindName("l" + i) as Arrow;
                l.X1 = EdgesPosi[i]["X1"] * this.Width;
                l.Y1 = EdgesPosi[i]["Y1"] * this.Height;
                l.X2 = EdgesPosi[i]["X2"] * this.Width;
                l.Y2 = EdgesPosi[i]["Y2"] * this.Height;
            }
        }

        private void SetVertex_Click(object sender, RoutedEventArgs e) {
            m_flagCtrl = 1;
            m_countVertex = 0;
            btnSetVertex.IsEnabled = false;
            btnClean.IsEnabled = true;
            btnSetEdge.IsEnabled = true;
            line.X1 = -100;
            line.X2 = -100;
            line.Y1 = -100;
            line.Y2 = -100;
            line.HeadHeight = 5;
            line.HeadWidth = 10;
        }

        private void SetEdge_Click(object sender, RoutedEventArgs e) {
            m_flagCtrl = 2;
            m_countEdge = 0;
            line.Visibility = Visibility.Visible;
            m_graph = new ALGraph<int>(m_nodes.ToArray());
            btnSetEdge.IsEnabled = false;
            btnSetVertex.IsEnabled = false;
            btnConfirm.IsEnabled = true;
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e) {
            m_flagCtrl = 3;
            m_count = 0;
            line.Visibility = Visibility.Hidden;
            btnSetVertex.IsEnabled = false;
            btnSetEdge.IsEnabled = false;
            btnConfirm.IsEnabled = false;
            m_graph.getTopoSort();
            if (m_graph.TopoSort[0] == -1) {
                btnNext.IsEnabled = false;
                System.Windows.MessageBox.Show("出现回路，无法操作，请清空画面");
            } else {
                dataConfirm();
                btnNext.IsEnabled = true;
                m_graph.InitVisited();
            }
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
            m_countEdge = 0;
            if (m_graph != null) {
                m_graph.DelGraph();
            }
            line.X1 = -1;
            line.X2 = -1;
            line.Y1 = -1;
            line.Y2 = -1;
            m_nodes.Clear();
            m_VertexVisted.Clear();
            VertexsPosi.Clear();
            EdgesPosi.Clear();
            textResult.Text = "排序结果:";
            btnConfirm.IsEnabled = false;
            btnSetVertex.IsEnabled = true;
            btnSetEdge.IsEnabled = false;
            btnNext.IsEnabled = false;
            m_flagCtrl = -1;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e) {
            if (m_count < m_graph.GetVertexNum()) {
                btnNext.Content = "下一步";
                int m_visit = m_graph.getTopoStart();
                m_VertexVisted.Add(m_visit);
                Ellipse ellipse = FindName("v" + m_visit) as Ellipse;
                ellipse.Fill = new SolidColorBrush(Colors.PaleVioletRed);
                textResult.Text += "v" + m_visit + " ";
                m_count++;
                EdgeNode<int> edge = m_graph[m_visit].Firstedge;
                while (edge != null) {
                    Arrow line = FindName("l" + edge.Mark) as Arrow;
                    line.Visibility = Visibility.Hidden;
                    edge = edge.Next;
                }
            } else if (m_count == m_graph.GetVertexNum()) {
                m_count = 0;
                btnNext.Content = "重新排序";
                for (int i = 0; i < m_countVertex; i++) {
                    Ellipse ellipse = FindName("v" + i) as Ellipse;
                    ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                }
                for(int i = 0; i < m_countEdge; i++) {
                    Arrow line = FindName("l" + i) as Arrow;
                    line.Visibility = Visibility.Visible;
                }
                textResult.Text = "排序结果:";
                m_graph.InitVisited();
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
                text.Text = "v" + m_countVertex;
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
                Node<int> node = new Node<int>(m_countVertex, rx, ry);
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

        private void MainCanvas_MouseMove(object sender, MouseEventArgs e) {
            if (m_flagCtrl == 2) {
                if (m_flagLine == false)
                    return;
                line.X2 = e.GetPosition(MainCanvas).X;
                line.Y2 = e.GetPosition(MainCanvas).Y;
            }
        }
        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (sender is Ellipse) {
                Ellipse ellipse = sender as Ellipse;
                if (m_flagCtrl == 2) {
                    m_t_ellipse = -1;
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
                    m_t_ellipse = -1;
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
                    Node<int> v1 = new Node<int>(-1, -1, -1);
                    Node<int> v2 = new Node<int>(-1, -1, -1);
                    String str = ellipse.Name;
                    String[] sstr = str.Split('v');
                    if (int.Parse(sstr[1]) == m_t_ellipse) {
                        return;
                    }
                    v1 = m_nodes[m_t_ellipse];
                    v2 = m_nodes[int.Parse(sstr[1])];
                    if (m_graph.SetDEdge(v1, v2, m_countEdge) == true) {
                        Arrow line = new Arrow();
                        line.HeadHeight = 5;
                        line.HeadWidth = 10;
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
                        ENode<int> edge = new ENode<int>(v1, v2, m_countEdge);
                        m_edges.Add(edge);
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
                    Node<int> v1 = new Node<int>(-1, -1, -1);
                    Node<int> v2 = new Node<int>(-1, -1, -1);
                    String str = ellipse.Name;
                    String[] sstr = str.Split('t');
                    if (int.Parse(sstr[1]) == m_t_ellipse) {
                        return;
                    }
                    v1 = m_nodes[m_t_ellipse];
                    v2 = m_nodes[int.Parse(sstr[1])];
                    if (m_graph.SetDEdge(v1, v2, m_countEdge) == true) {
                        Arrow line = new Arrow();
                        line.HeadHeight = 5;
                        line.HeadWidth = 10;
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
                        ENode<int> edge = new ENode<int>(v1, v2, m_countEdge);
                        m_edges.Add(edge);
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
            if (m_flagCtrl == 2 || m_flagCtrl == 5) {
                Cursor = Cursors.Hand;
            }
        }
        private void Ellipse_MouseLeave(object sender, MouseEventArgs e) {
            Cursor = Cursors.Arrow;
        }
        public void Filesave() {
            FileStream fs = new FileStream(MainWindowInfo.fileLocation, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            JObject jObject = new JObject();
            jObject["type"] = "TopoSort";
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
            foreach (ENode<int> edge in m_edges) {
                JObject jonode = new JObject();
                if (this != null) {
                    jonode["start"] = edge.V1.Data;
                    jonode["end"] = edge.V2.Data;
                    jonode["weight"] = edge.Weight;
                    jonode["index"] = edge.Index;
                    jonode["mark"] = edge.Mark;
                    jArray.Add(jonode);
                }
            }
            jObject["edges"] = jArray;
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
                foreach (JObject jonode in jObject["nodes"]) {
                    Node<int> node = new Node<int>();
                    node.Data = (int)jonode["data"];
                    node.X = (double)jonode["x"];
                    node.Y = (double)jonode["y"];
                    m_nodes.Add(node);
                }
                foreach (JObject jonode in jObject["edges"]) {
                    ENode<int> edge = new ENode<int>();
                    edge.V1 = FindNode((int)jonode["start"]);
                    edge.V2 = FindNode((int)jonode["end"]);
                    edge.Weight = (int)jonode["weight"];
                    edge.Index = (int)jonode["index"];
                    edge.Mark = (int)jonode["mark"];
                    m_edges.Add(edge);
                }
                Receive();
                btnConfirm_Click(null, null);
                sr.Close();
                fs.Close();
            } catch {
                MessageBox.Show("请检查文件是否完整");
            }
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
            btnClean.IsEnabled = true;
            m_graph = new ALGraph<int>(m_nodes.ToArray());
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
                text.Text = "v" + m_countVertex;
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
            foreach (ENode<int> edge in m_edges) {
                Node<int> v1 = edge.V1;
                Node<int> v2 = edge.V2;
                double v1x, v1y, v2x, v2y;
                if (m_graph.SetDEdge(v1, v2, m_countEdge) == true) {
                    Arrow line = new Arrow();
                    line.HeadHeight = 5;
                    line.HeadWidth = 10;
                    v1x = v1.X * this.Width;
                    v1y = v1.Y * this.Height;
                    v2x = v2.X * this.Width;
                    v2y = v2.Y * this.Height;
                    line.X1 = v1x;
                    line.Y1 = v1y;
                    line.X2 = v2x;
                    line.Y2 = v2y;
                    line.Stroke = new SolidColorBrush(Colors.Black);
                    line.StrokeThickness = 1; line.Name = "l" + m_countEdge;
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
}

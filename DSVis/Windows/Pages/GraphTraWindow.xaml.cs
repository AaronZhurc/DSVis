using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Runtime.Remoting.Messaging;
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
using DSVis.DataStruct;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DSVis.Windows.Pages {
    /// <summary>
    /// GraphTraWindow.xaml 的交互逻辑
    /// </summary>
    public partial class GraphTraWindow : Page {

        private int m_flagCtrl;
        private bool m_flagLine = false;
        private List<Node<int>> m_nodes = new List<Node<int>>();
        private List<Tuple<int, int>> m_edges = new List<Tuple<int, int>>();
        private List<Dictionary<string, double>> VertexsPosi = new List<Dictionary<string, double>>();
        private List<Dictionary<string, double>> EdgesPosi = new List<Dictionary<string, double>>();
        private ALGraph<int> m_graph;
        private int m_countVertex, m_countEdge;
        private ArrayList m_VertexVisted = new ArrayList();
        private int m_count;
        private int m_t_ellipse;
        private string titlename = "图的遍历";
        public delegate void DataConfirm();
        public DataConfirm dataConfirm;
        public delegate void DataClean();
        public DataClean dataClean;
        public GraphTraWindow() {
            InitializeComponent();
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
            CtrlBg.Height = this.Height;
            MainCanvas.Height = this.Height;
            MainCanvas.Width = this.Width - 170;
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
            m_countEdge = 0;
            line.Visibility = Visibility.Visible;
            m_graph = new ALGraph<int>(m_nodes.ToArray());
            btnSetEdge.IsEnabled = false;
            btnConfirm.IsEnabled = true;
            btnSetVertex.IsEnabled = false;
            this.Title = titlename + " - 正在绘边";
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e) {
            dataConfirm();
            m_flagCtrl = 3;
            m_count = 0;
            line.Visibility = Visibility.Hidden;
            radiobtnBFS.IsEnabled = true;
            radiobtnDFS.IsEnabled = true;
            btnNext.IsEnabled = true;
            btnSetVertex.IsEnabled = false;
            btnSetEdge.IsEnabled = false;
            btnConfirm.IsEnabled = false;
            btnClean.IsEnabled = true;
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
            textResult.Text = "遍历结果:";
            btnConfirm.IsEnabled = false;
            btnSetVertex.IsEnabled = true;
            btnSetEdge.IsEnabled = false;
            radiobtnBFS.IsEnabled = false;
            radiobtnDFS.IsEnabled = false;
            btnNext.IsEnabled = false;
            this.Title = titlename;
            m_flagCtrl = -1;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e) {
            if (radiobtnBFS.IsChecked == true && m_count < m_graph.GetVertexNum()) {
                btnNext.Content = "下一步";
                radiobtnDFS.IsEnabled = false;
                m_VertexVisted = m_graph.GetVisited(1);
                Ellipse ellipse = FindName("v" + m_VertexVisted[m_count]) as Ellipse;
                ellipse.Fill = new SolidColorBrush(Colors.PaleVioletRed);
                textResult.Text += "v" + m_VertexVisted[m_count] + " ";
                m_count++;
            } else if (radiobtnDFS.IsChecked == true && m_count < m_graph.GetVertexNum()) {
                btnNext.Content = "下一步";
                radiobtnBFS.IsEnabled = false;
                m_VertexVisted = m_graph.GetVisited(0);
                Ellipse ellipse = FindName("v" + m_VertexVisted[m_count]) as Ellipse;
                ellipse.Fill = new SolidColorBrush(Colors.PaleVioletRed);
                textResult.Text += "v" + m_VertexVisted[m_count] + " ";
                m_count++;
            } else if (m_count == m_graph.GetVertexNum()) {
                m_count = 0;
                radiobtnDFS.IsEnabled = true;
                radiobtnBFS.IsEnabled = true;
                btnNext.Content = "重新遍历";
                for (int i = 0; i < m_countVertex; i++) {
                    Ellipse ellipse = FindName("v" + m_VertexVisted[i]) as Ellipse;
                    ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                }
                textResult.Text = "遍历结果:";
                m_VertexVisted.Clear();
                m_graph.InitVisited();
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
                m_countVertex++;
                m_nodes.Add(node);
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
            } else if(sender is TextBlock) {
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
                    if(int.Parse(sstr[1])== m_t_ellipse) {
                        return;
                    }
                    v1 = m_nodes[m_t_ellipse];
                    v2 = m_nodes[int.Parse(sstr[1])];
                    if (m_graph.SetUdEdge(v1, v2) == true) {
                        m_edges.Add(new Tuple<int, int>(v1.Data, v2.Data));
                        Line line = new Line();
                        line.X1 = v1.X * this.Width;
                        line.Y1 = v1.Y * this.Height;
                        line.X2 = v2.X * this.Width;
                        line.Y2 = v2.Y * this.Height;
                        line.Stroke = new SolidColorBrush(Colors.Black);
                        line.StrokeThickness = 1;
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
            } else if(sender is TextBlock) {
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
                    if (m_graph.SetUdEdge(v1, v2) == true) {
                        m_edges.Add(new Tuple<int, int>(v1.Data, v2.Data));
                        Line line = new Line();
                        line.X1 = v1.X * this.Width;
                        line.Y1 = v1.Y * this.Height;
                        line.X2 = v2.X * this.Width;
                        line.Y2 = v2.Y * this.Height;
                        line.Stroke = new SolidColorBrush(Colors.Black);
                        line.StrokeThickness = 1;
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
            Cursor = Cursors.Hand;
        }
        private void Ellipse_MouseLeave(object sender, MouseEventArgs e) {
            Cursor = Cursors.Arrow;
        }

        public void Filesave() {
            FileStream fs = new FileStream(MainWindowInfo.fileLocation, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            JObject jObject = new JObject();
            jObject["type"] = "GraphTra";
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
            foreach (Tuple<int, int> edge in m_edges) {
                JObject jonode = new JObject();
                if (this != null) {
                    jonode["start"] = edge.Item1;
                    jonode["end"] = edge.Item2;
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
                    Tuple<int, int> edge = new Tuple<int, int>((int)jonode["start"], (int)jonode["end"]);
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
            foreach(Tuple<int,int> edge in m_edges) {
                Node<int> v1 = FindNode(edge.Item1);
                Node<int> v2 = FindNode(edge.Item2);
                if (m_graph.SetUdEdge(v1, v2) == true) {
                    Line line = new Line();
                    line.X1 = v1.X * this.Width;
                    line.Y1 = v1.Y * this.Height;
                    line.X2 = v2.X * this.Width;
                    line.Y2 = v2.Y * this.Height;
                    line.Stroke = new SolidColorBrush(Colors.Black);
                    line.StrokeThickness = 1;
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

using DSVis.DataStruct;
using DSVis.Tools;
using DSVis.Windows.Forms;
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
    /// SPath.xaml 的交互逻辑
    /// </summary>
    public partial class SPathWindow : Page {
        private int m_flagCtrl;
        //1 设点 2 设边 3 演示 4 设权值 5 Dij设起始点
        private bool m_flagLine = false;
        private List<Node<int>> m_nodes = new List<Node<int>>();
        private List<Dictionary<string, double>> VertexsPosi = new List<Dictionary<string, double>>();
        private List<Dictionary<string, double>> EdgesPosi = new List<Dictionary<string, double>>();
        private DirAdjMatrix<int> m_graph = new DirAdjMatrix<int>();
        private List<ENode<int>> m_edges = new List<ENode<int>>();
        private int m_countVertex, m_countEdge, m_countWeight;
        private int m_t_ellipse;
        private int m_count;
        private String strLine;
        private int[] m_recWeight;
        private ArrayList m_EdgeVisted = new ArrayList();
        private ArrayList m_VertexVisted = new ArrayList();
        private ArrayList m_posiVisit = new ArrayList();
        private string titlename = "最短路径";
        public delegate void DataConfirm();
        public DataConfirm dataConfirm;
        public delegate void DataClean();
        public DataClean dataClean;
        public SPathWindow() {
            InitializeComponent();
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
        }
        private void btnSetVertex_Click(object sender, RoutedEventArgs e) {
            m_flagCtrl = 1;
            btnSetVertex.IsEnabled = false;
            btnClean.IsEnabled = true;
            btnSetEdge.IsEnabled = true;
            this.Title = titlename + " - 正在绘点";
            line.X1 = -100;
            line.X2 = -100;
            line.Y1 = -100;
            line.Y2 = -100;
            line.HeadHeight = 5;
            line.HeadWidth = 10;
        }
        private void btnSetEdge_Click(object sender, RoutedEventArgs e) {
            m_flagCtrl = 2;
            line.Visibility = Visibility.Visible;
            btnSetEdge.IsEnabled = false;
            btnSetVertex.IsEnabled = false;
            btnSetWeight.IsEnabled = true;
            this.Title = titlename + " - 正在绘边";
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

        private void btnConfirm_Click(object sender, RoutedEventArgs e) {
            if (m_countWeight == m_countEdge) {
                dataConfirm();
                m_flagCtrl = 3;
                m_count = 0;
                line.Visibility = Visibility.Hidden;
                radiobtn12N.IsEnabled = true;
                radiobtnDij.IsEnabled = true;
                btnNext.IsEnabled = true;
                btnSetVertex.IsEnabled = false;
                btnSetEdge.IsEnabled = false;
                btnSetWeight.IsEnabled = false;
                btnConfirm.IsEnabled = false;
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
                MainCanvas.Children.Remove((UIElement)this.FindName("tl" + i));
                this.UnregisterName("tl" + i);
            }
            m_countEdge = 0;
            m_countWeight = 0;
            if (m_graph != null) {
                m_graph.DelGraph();
            }
            line.X1 = -100;
            line.X2 = -100;
            line.Y1 = -100;
            line.Y2 = -100;
            m_nodes.Clear();
            VertexsPosi.Clear();
            EdgesPosi.Clear();
            m_EdgeVisted = new ArrayList();
            m_edges.Clear();
            btnConfirm.IsEnabled = false;
            btnSetVertex.IsEnabled = true;
            btnSetEdge.IsEnabled = false;
            btnSetWeight.IsEnabled = false;
            radiobtn12N.IsEnabled = false;
            radiobtnDij.IsEnabled = false;
            btnNext.IsEnabled = false;
            this.Title = titlename;
            m_flagCtrl = -1;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e) {
            int flag;
            if (radiobtn12N.IsChecked == true && m_count == 0) {
                if (m_t_ellipse == -1) {
                    MessageBox.Show("请点选起始点");
                    m_flagCtrl = 5;
                } else {
                    m_flagCtrl = 3;
                    btnNext.Content = "下一步";
                    radiobtnDij.IsEnabled = false;
                    m_VertexVisted = m_graph.Dijkstra12N(m_t_ellipse);
                    m_EdgeVisted.Add(-1);
                    for (int i = 0; i < m_VertexVisted.Count - 1; i++) {
                        if (Convert.ToInt32(m_VertexVisted[i]) == -1 || Convert.ToInt32(m_VertexVisted[i + 1]) == -1) { 
                            if (Convert.ToInt32(m_VertexVisted[i + 1]) == -1) {
                                m_EdgeVisted.Add(-1);
                            }
                            if (Convert.ToInt32(m_VertexVisted[i]) == -1) {
                                m_posiVisit.Add(m_VertexVisted[i + 1]);
                            }
                        } else {
                            foreach (ENode<int> eNode in m_edges) {
                                int index = eNode.findEdge(Convert.ToInt32(m_VertexVisted[i]), Convert.ToInt32(m_VertexVisted[i + 1]));
                                if (index != -1) {
                                    m_EdgeVisted.Add(index);
                                }
                            }
                        }
                    }
                }
            } else if (radiobtnDij.IsChecked == true && m_count == 0) {
                if (m_t_ellipse == -1) {
                    MessageBox.Show("请点选起始点");
                    m_flagCtrl = 5;
                } else {
                    m_flagCtrl = 3;
                    m_EdgeVisted = m_graph.Dijkstra(m_t_ellipse);
                    btnNext.Content = "下一步";
                    radiobtn12N.IsEnabled = false;
                }
            }
            if (radiobtnDij.IsChecked == true && m_count < m_EdgeVisted.Count && m_t_ellipse != -1) {
                int index = -1;
                Tuple<int,int> t = m_EdgeVisted[m_count] as Tuple<int, int>;
                foreach (ENode<int> eNode in m_edges) {
                    index = eNode.findEdge(t.Item1, t.Item2);
                    if (index != -1) {
                        break;
                    } 
                }
                if (index != -1) {
                    Arrow line = FindName("l" + index) as Arrow;
                    line.Stroke = new SolidColorBrush(Colors.PaleVioletRed);
                    line.StrokeThickness = 2;
                    m_count++;
                }
            } else if (radiobtn12N.IsChecked == true && m_count < m_posiVisit.Count && m_t_ellipse != -1) {
                flag = 0;
                for (int i = 0; i < m_countVertex; i++) {
                    if (i == m_t_ellipse) {
                        Ellipse ell = FindName("v" + i) as Ellipse;
                        ell.Fill = new SolidColorBrush(Colors.PaleVioletRed);
                    } else {
                        Ellipse ell = FindName("v" + i) as Ellipse;
                        ell.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    }
                }
                for (int i = 0; i < m_countEdge; i++) {
                    Arrow line = FindName("l" + i) as Arrow;
                    line.Stroke = new SolidColorBrush(Colors.Black);
                    line.StrokeThickness = 1;
                }
                Ellipse ellipse = FindName("v" + m_posiVisit[m_count]) as Ellipse;
                ellipse.Fill = new SolidColorBrush(Colors.IndianRed);
                this.Title = titlename + " - 正在演示到v" + m_posiVisit[m_count] + "最短路径";
                for (int i = 1; i < m_EdgeVisted.Count; i++) {
                    if (Convert.ToInt32(m_EdgeVisted[i]) == -1) {
                        flag++;
                    } else {
                        if (flag == m_count) {
                            Arrow line = FindName("l" + m_EdgeVisted[i]) as Arrow;
                            line.Stroke = new SolidColorBrush(Colors.IndianRed);
                            line.StrokeThickness = 2;
                        }
                    }
                }
                m_count++;
            } else if (radiobtn12N.IsChecked == true && m_count == m_posiVisit.Count && m_t_ellipse != -1) {
                this.Title = titlename + " - 正在演示";
                for (int i = 0; i < m_countVertex; i++) {
                    if (i == m_t_ellipse) {
                        Ellipse ell = FindName("v" + i) as Ellipse;
                        ell.Fill = new SolidColorBrush(Colors.PaleVioletRed);
                    } else {
                        Ellipse ell = FindName("v" + i) as Ellipse;
                        ell.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    }
                }
                for (int i = 0; i < m_countEdge; i++) {
                    Arrow line = FindName("l" + i) as Arrow;
                    line.Stroke = new SolidColorBrush(Colors.Black);
                    line.StrokeThickness = 1;
                }
                for (int i = 1; i < m_EdgeVisted.Count; i++) {
                    if (Convert.ToInt32(m_EdgeVisted[i]) != -1) {
                        Arrow line = FindName("l" + m_EdgeVisted[i]) as Arrow;
                        line.Stroke = new SolidColorBrush(Colors.IndianRed);
                        line.StrokeThickness = 2;
                    }
                }
                for(int i = 0; i < m_posiVisit.Count; i++) {
                    Ellipse ellipse = FindName("v" + m_posiVisit[i]) as Ellipse;
                    ellipse.Fill = new SolidColorBrush(Colors.IndianRed);
                }
                m_count++;
            } else if (radiobtn12N.IsChecked == true && m_count > m_posiVisit.Count && m_t_ellipse != -1) {
                m_count = 0;
                radiobtnDij.IsEnabled = true;
                btnNext.Content = "重新遍历";
                for (int i = 0; i < m_countEdge; i++) {
                    Arrow line = FindName("l" + i) as Arrow;
                    line.Stroke = new SolidColorBrush(Colors.Black);
                    line.StrokeThickness = 1;
                }
                for (int i = 0; i < m_countVertex; i++) {
                    if (i == m_t_ellipse) {
                        Ellipse ell = FindName("v" + i) as Ellipse;
                        ell.Fill = new SolidColorBrush(Colors.PaleVioletRed);
                    } else {
                        Ellipse ell = FindName("v" + i) as Ellipse;
                        ell.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    }
                }
                m_EdgeVisted.Clear();
                m_graph.InitVisited();
                m_posiVisit.Clear();
                if (radiobtn12N.IsChecked == true) {
                    m_flagCtrl = 5;
                } else {
                    m_flagCtrl = 3;
                }
            } else if (radiobtnDij.IsChecked == true && m_count == m_EdgeVisted.Count && m_t_ellipse != -1) {
                m_count = 0;
                radiobtn12N.IsEnabled = true;
                btnNext.Content = "重新遍历";
                for (int i = 0; i < m_countEdge; i++) {
                    Arrow line = FindName("l" + i) as Arrow;
                    line.Stroke = new SolidColorBrush(Colors.Black);
                    line.StrokeThickness = 1;
                }
                m_EdgeVisted=new ArrayList();
                m_graph.InitVisited();
                //m_edges.Clear();
                if (radiobtnDij.IsChecked == true) {
                    m_flagCtrl = 5;
                } else {
                    m_flagCtrl = 3;
                }
            }
        }
        private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
            if (m_flagCtrl == 1) {
                if (m_countVertex >= 30) {
                    MessageBox.Show("最大设置点数为30");
                } else {
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
                    m_graph.SetNode(m_countVertex, node);
                    m_nodes.Add(node);
                    m_countVertex++;
                }
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
                }else if (m_flagCtrl == 5) {
                    String str = ellipse.Name;
                    String[] sstr = str.Split('v');
                    m_t_ellipse = int.Parse(sstr[1]);
                    for (int i = 0; i < m_countVertex; i++) {
                        if (i == m_t_ellipse) {
                            Ellipse ell = FindName("v" + i) as Ellipse;
                            ell.Fill = new SolidColorBrush(Colors.PaleVioletRed);
                        } else {
                            Ellipse ell = FindName("v" + i) as Ellipse;
                            ell.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                        }
                    }
                    m_flagCtrl = 3;
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
                }else if (m_flagCtrl == 5) {
                    String str = ellipse.Name;
                    String[] sstr = str.Split('t');
                    m_t_ellipse = int.Parse(sstr[1]);
                    for (int i = 0; i < m_countVertex; i++) {
                        if (i == m_t_ellipse) {
                            Ellipse ell = FindName("v" + i) as Ellipse;
                            ell.Fill = new SolidColorBrush(Colors.PaleVioletRed);
                        } else {
                            Ellipse ell = FindName("v" + i) as Ellipse;
                            ell.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                        }
                    }
                    m_flagCtrl = 3;
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
                    if (m_graph.SetEdge(v1, v2) == true) {
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
                        MainCanvas.Children.Add(text);
                    }
                }
            }
        }
        private void Ellipse_MouseEnter(object sender, MouseEventArgs e) {
            if(m_flagCtrl== 2 || m_flagCtrl == 5) {
                Cursor = Cursors.Hand;
            }
        }
        private void Ellipse_MouseLeave(object sender, MouseEventArgs e) {
            Cursor = Cursors.Arrow;
        }
        private void Line_MouseEnter(object sender, MouseEventArgs e) {
            if (m_flagCtrl == 4)
                Cursor = Cursors.Hand;
        }
        private void Line_MouseLeave(object sender, MouseEventArgs e) {
            if (m_flagCtrl == 4)
                Cursor = Cursors.Arrow;
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
                TextBlock text = this.FindName("tl" + i) as TextBlock;
                Size sizeText = text.DesiredSize;
                text.SetValue(Canvas.LeftProperty, Convert.ToDouble(l.X1 + (l.X2 - l.X1) / 2));
                text.SetValue(Canvas.TopProperty, Convert.ToDouble(l.Y1 + (l.Y2 - l.Y1) / 2));
            }
        }

        private void radiobtn12N_Checked(object sender, RoutedEventArgs e) {
            m_flagCtrl = 5;
        }

        private void radiobtnDij_Checked(object sender, RoutedEventArgs e) {
            m_flagCtrl = 5;
        }

        private void Line_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            if (m_flagCtrl == 4 && sender is Arrow) {
                Arrow line = sender as Arrow;
                SetWeightForm form = new SetWeightForm(line.Name);
                strLine = line.Name;
                form.Show();
                form.sendMessage = Recevie;
            } else if (m_flagCtrl == 4 && sender is TextBlock) {
                TextBlock text = sender as TextBlock;
                string[] str = text.Name.Split('t');
                SetWeightForm form = new SetWeightForm(str[1]);
                strLine = str[1];
                form.Show();
                form.sendMessage = Recevie;
            }
        }
        public void Recevie(int value) {
            TextBlock text = this.FindName("t" + strLine) as TextBlock;
            text.Text = value.ToString();
            string[] str = strLine.Split('l');
            int index = Convert.ToInt32(str[1]);
            m_graph.SetWeight(m_edges[index].V1, m_edges[index].V2, value);
            m_edges[index].Weight = value;
            m_recWeight[index] = -1;
            m_countWeight++;
        }
        public void Filesave() {
            FileStream fs = new FileStream(MainWindowInfo.fileLocation, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            JObject jObject = new JObject();
            jObject["type"] = "SPath";
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
                m_graph.SetNode(m_countVertex, node);
                m_countVertex++;
            }
            foreach (ENode<int> edge in m_edges) {
                Node<int> v1 = edge.V1;
                Node<int> v2 = edge.V2;
                double v1x, v1y, v2x, v2y;
                if (m_graph.SetEdge(v1, v2) == true) {
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
                    m_graph.SetWeight(v1, v2, edge.Weight);
                    TextBlock text = new TextBlock();
                    text.Text = edge.Weight.ToString();
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

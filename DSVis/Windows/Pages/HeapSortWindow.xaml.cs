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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DSVis.Windows.Pages {
    /// <summary>
    /// HeapSortWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HeapSortWindow : Page {
        List<int> m_weights = new List<int>();
        int m_ArrayCount = 0, m_DrawCount = 0;
        TNode<int> m_tree = new TNode<int>();
        int m_node = -1, m_parent = -1;
        private List<Tuple<double, double>> VertexsPosi = new List<Tuple<double, double>>();
        private List<Tuple<double, double, double, double>> EdgesPosi = new List<Tuple<double, double, double, double>>();

        public delegate void DataConfirm();
        public DataConfirm dataConfirm;
        public delegate void DataClean();
        public DataClean dataClean;
        public HeapSortWindow() {
            InitializeComponent();
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
            if (MainWindowInfo.fileFlag == true) {
                this.IsEnabled = true;
            } else {
                SetArrayForm form = new SetArrayForm();
                form.Show();
                form.sendMessage = Recevie;
                form.windowClosed = FormClosed;
                this.IsEnabled = false;
            }
        }
        public void Recevie(List<int> value) {
            dataConfirm();
            textOrigin.Text = "原数组 ";
            textHeap.Text = "堆 ";
            cleanCanvas();
            //m_flagStart = 1;
            m_weights = value;
            foreach (int w in m_weights) {
                textOrigin.Text += w.ToString() + " ";
            }
            
            btnNext.IsEnabled = true;
            m_tree = new TNode<int>();
            m_tree.Name = m_weights[0];
            m_tree.X = 0.5;
            m_tree.Y = 0.1;
            for(int i = 1; i < m_weights.Count; i++) {
                TNode<int> node = new TNode<int>();
                node.Name = m_weights[i];
                BuildHeap(node);
            }
            m_tree.LevelOrder();
            DrawTree();
            radiobtnMax.IsEnabled = true;
            radiobtnMin.IsEnabled = true;
            btnAdd.IsEnabled = false;
        }
        public void FormClosed() {
            this.IsEnabled = true;
        }
        public void BuildHeap(TNode<int> node) {
            m_tree.LevelOrder();
            TNode<int> last = m_tree.VertexVisit[m_tree.VertexVisit.Count - 1] as TNode<int>;
            if (last.Parent != null) {
                if (last.Parent.Lchild == last) {
                    last.Parent.Rchild = node;
                    node.Parent = last.Parent;
                    node.X = node.Parent.X + 1.0 / Math.Pow(2, node.Depth());
                    node.Y = node.Parent.Y + 0.1;
                } else if (last.Parent.Rchild == last) {
                    TNode<int> parent;
                    for (int i = 0; i < m_tree.VertexVisit.Count; i++) {
                        if (m_tree.VertexVisit[i] == last.Parent) {
                            parent = m_tree.VertexVisit[i + 1] as TNode<int>;
                            parent.Lchild = node;
                            node.Parent = parent;
                            node.X = parent.X - 1.0 / Math.Pow(2, node.Depth());
                            node.Y = parent.Y + 0.1;
                            break;
                        }
                    }
                }
            } else {
                last.Lchild = node;
                node.Parent = last;
                node.X = last.X - 1.0 / Math.Pow(2, node.Depth());
                node.Y = last.Y + 0.1;
            }
        }
        private void cleanCanvas() {
            for (int i = 0; i < m_weights.Count; i++) {
                MainCanvas.Children.Remove((UIElement)this.FindName("t" + i));
                this.UnregisterName("t" + i);
                MainCanvas.Children.Remove((UIElement)this.FindName("v" + i));
                this.UnregisterName("v" + i);
                if (i != 0) {
                    MainCanvas.Children.Remove((UIElement)this.FindName("l" + i));
                    this.UnregisterName("l" + i);
                }
            }
            m_DrawCount = 0;
            VertexsPosi.Clear();
            EdgesPosi.Clear();
        }
        private void DrawTree() {
            textHeap.Text = "堆 ";
            m_tree.LevelOrder();
            for (int i = 0; i < m_tree.VertexVisit.Count; i++) {
                TNode<int> node = m_tree.VertexVisit[i] as TNode<int>;
                DrawNode(node);
                textHeap.Text += node.Name.ToString() + " ";
                if (i == m_node || i == m_parent) {
                    Ellipse ellipse = this.FindName("v" + i) as Ellipse;
                    ellipse.Fill = new SolidColorBrush(Colors.IndianRed);
                }
            }
        }
        private void DrawNode(TNode<int> node) {//修改成对树的重新绘制
            if (node != null) {
                double x, y;
                x = node.X * MainCanvas.ActualWidth;
                y = node.Y * MainCanvas.ActualHeight;
                Ellipse ellipse = new Ellipse();
                ellipse.Width = 30;
                ellipse.Height = 30;
                ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                ellipse.SetValue(Canvas.LeftProperty, x - ellipse.Width / 2);
                ellipse.SetValue(Canvas.TopProperty, y - ellipse.Height / 2);
                ellipse.Name = "v" + m_DrawCount.ToString();
                this.RegisterName("v" + m_DrawCount, ellipse);

                TextBlock text = new TextBlock();
                text.Text = node.Name.ToString();
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size sizeText = text.DesiredSize;
                text.SetValue(Canvas.LeftProperty, x - sizeText.Width / 2);
                text.SetValue(Canvas.TopProperty, y - sizeText.Height / 2);
                text.HorizontalAlignment = HorizontalAlignment.Center;
                text.VerticalAlignment = VerticalAlignment.Center;
                text.TextAlignment = TextAlignment.Center;
                this.RegisterName("t" + m_DrawCount, text);

                MainCanvas.Children.Add(ellipse);
                MainCanvas.Children.Add(text);

                VertexsPosi.Add(new Tuple<double, double>(node.X, node.Y));

                if (node.Parent != null) {
                    Line line = new Line();
                    line.X1 = x;
                    line.Y1 = y;
                    line.X2 = node.Parent.X * MainCanvas.ActualWidth;
                    line.Y2 = node.Parent.Y * MainCanvas.ActualHeight;
                    line.Stroke = new SolidColorBrush(Colors.Black);
                    line.StrokeThickness = 1;
                    this.RegisterName("l" + m_DrawCount, line);

                    MainCanvas.Children.Add(line);

                    EdgesPosi.Add(new Tuple<double, double, double, double>(node.X, node.Y, node.Parent.X, node.Parent.Y));
                }
                m_DrawCount++;
            }
        }
        private void btnSet_Click(object sender, RoutedEventArgs e) {
            SetArrayForm form = new SetArrayForm();
            form.Show();
            form.sendMessage = Recevie;
            form.windowClosed = FormClosed;
            this.IsEnabled = false;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e) {
            if (radiobtnMax.IsChecked == true) {
                if (!m_tree.IsMaxHeap()) {
                    radiobtnMin.IsChecked = false;
                    m_tree.LevelOrder();
                    for(int i = m_tree.VertexVisit.Count - 1; i > 0; i--) {
                        TNode<int> node = m_tree.VertexVisit[i] as TNode<int>;
                        if (node.Name > node.Parent.Name) {
                            int swap=node.Name;
                            node.Name = node.Parent.Name;
                            node.Parent.Name = swap;
                            m_node = i;
                            for(int j=0;j< m_tree.VertexVisit.Count; j++) {
                                if(node.Parent==m_tree.VertexVisit[j] as TNode<int>) {
                                    m_parent = j;
                                    break;
                                }
                            }
                            cleanCanvas();
                            DrawTree();
                            break;
                        }
                    }
                } else {
                    radiobtnMin.IsChecked = true;
                    m_node = -1;
                    m_parent = -1;
                    cleanCanvas();
                    DrawTree();
                    btnAdd.IsEnabled = true;
                }
            } else if (radiobtnMin.IsChecked == true) {
                if (!m_tree.IsMinHeap()) {
                    radiobtnMax.IsChecked = false;
                    m_tree.LevelOrder();
                    for (int i = m_tree.VertexVisit.Count - 1; i > 0; i--) {
                        TNode<int> node = m_tree.VertexVisit[i] as TNode<int>;
                        if (node.Name < node.Parent.Name) {
                            int swap = node.Name;
                            node.Name = node.Parent.Name;
                            node.Parent.Name = swap;
                            m_node = i;
                            for (int j = 0; j < m_tree.VertexVisit.Count; j++) {
                                if (node.Parent == m_tree.VertexVisit[j] as TNode<int>) {
                                    m_parent = j;
                                    break;
                                }
                            }
                            cleanCanvas();
                            DrawTree();
                            break;
                        }
                    }
                } else {
                    radiobtnMax.IsChecked = true;
                    m_node = -1;
                    m_parent = -1;
                    cleanCanvas();
                    DrawTree();
                    btnAdd.IsEnabled = true;
                }
            } else {
                MessageBox.Show("请先选择构建堆的方式");
            }
        }
        public void windowchanged() {
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
            CtrlBg.Height = this.Height;
            MainCanvas.Height = this.Height;
            MainCanvas.Width = this.Width - 170 - 30;
            for (int i = 0; i <= m_ArrayCount; i++) {
                Object obj = this.FindName("v" + i);
                Ellipse ellipse = obj as Ellipse;
                if (ellipse != null) {
                    ellipse.SetValue(Canvas.LeftProperty, (VertexsPosi[i].Item1 * MainCanvas.Width) - (ellipse.Width / 2));
                    ellipse.SetValue(Canvas.TopProperty, (VertexsPosi[i].Item2 * MainCanvas.Height) - (ellipse.Height / 2));
                    TextBlock tb = this.FindName("t" + i) as TextBlock;
                    Size sizeText = tb.DesiredSize;
                    tb.SetValue(Canvas.LeftProperty, (VertexsPosi[i].Item1 * MainCanvas.Width) - (sizeText.Width / 2));
                    tb.SetValue(Canvas.TopProperty, (VertexsPosi[i].Item2 * MainCanvas.Height) - (sizeText.Height / 2));
                    if (i != 0) {
                        Line l = this.FindName("l" + i) as Line;
                        l.X1 = EdgesPosi[i - 1].Item1 * MainCanvas.Width;
                        l.Y1 = EdgesPosi[i - 1].Item2 * MainCanvas.Height;
                        l.X2 = EdgesPosi[i - 1].Item3 * MainCanvas.Width;
                        l.Y2 = EdgesPosi[i - 1].Item4 * MainCanvas.Height;
                    }
                }
            }
        }
        public void Filesave() {
            FileStream fs = new FileStream(MainWindowInfo.fileLocation, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            JObject jObject = new JObject();
            jObject["type"] = "HeapSort";
            JArray jArray = new JArray();
            for (int i = 0; i < m_weights.Count; i++) {
                jArray.Add(m_weights[i]);
            }
            jObject["m_weights"] = jArray;
            sw.Write(jObject);
            sw.Close();
            fs.Close();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e) {
            SetWeightForm form = new SetWeightForm();
            form.Show();
            form.sendMessage = RecevieWeight;
        }
        public void RecevieWeight(int value) {
            TNode<int> node = new TNode<int>();
            node.Name = value;
            BuildHeap(node);
            cleanCanvas();
            m_weights.Add(value);
            DrawTree();
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
                Recevie(jObject["m_weights"].ToObject<List<int>>());
                sr.Close();
                fs.Close();
            } catch {
                MessageBox.Show("请检查文件是否完整");
            }
}
    }
}

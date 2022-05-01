using DSVis.DataStruct;
using System;
using System.Collections;
using System.Collections.Generic;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Windows.Threading;
using DSVis.Windows.Forms;

namespace DSVis.Windows.Pages {
    /// <summary>
    /// BSTWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BSTWindow : Page {
        List<int> m_weights = new List<int>();
        int m_ArrayCount = 0, m_DrawCount = 0;
        TNode<int> m_tree = new TNode<int>();
        int m_flagStart = 0;
        private List<Tuple<double, double>> VertexsPosi = new List<Tuple<double, double>>();
        private List<Tuple<double, double, double, double>> EdgesPosi = new List<Tuple<double, double, double, double>>();
        public delegate void DataConfirm();
        public DataConfirm dataConfirm;
        public delegate void DataClean();
        public DataClean dataClean;
        public BSTWindow() {
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

        private void btnSetWeight_Click(object sender, RoutedEventArgs e) {
            SetArrayForm form = new SetArrayForm();
            form.Show();
            form.sendMessage = Recevie;
            form.windowClosed = FormClosed;
            this.IsEnabled = false;
        }
        public void Recevie(List<int> value) {
            dataConfirm();
            textOrigin.Text = "原数组\n";
            textSorted.Text = "排序结果\n";
            cleanCanvas();
            m_flagStart = 1;
            m_weights = value;
            List<int> m_sorted = new List<int>(m_weights);
            m_sorted.Sort();
            foreach (int w in m_weights) {
                textOrigin.Text += w.ToString() + " ";
            }
            foreach (int s in m_sorted) {
                textSorted.Text += s.ToString() + " ";
            }
            //btnNext.IsEnabled = true;
            m_tree = new TNode<int>();
            m_tree.Name = m_weights[0];
            m_tree.X = 0.5;
            m_tree.Y = 0.1;
            DrawTree(m_tree);
            radiobtnAVL.IsEnabled = true;
            radiobtnBST.IsEnabled = true;
        }
        public void FormClosed() {
            this.IsEnabled = true;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e) {
            cleanCanvas();
            if (m_ArrayCount < m_weights.Count() - 1) {
                btnSetWeight.IsEnabled = false;
                btnNext.Content = "下一步";
                m_flagStart = 1;
                m_ArrayCount++;
                if (radiobtnBST.IsChecked == true) { 
                    BuildBSTree(m_tree);
                } else {
                    BuildAVLTree();
                }
                DrawTree(m_tree);
            } else {
                btnSetWeight.IsEnabled = true;
                m_tree = new TNode<int>();
                m_tree.Name = m_weights[0];
                m_tree.X = 0.5;
                m_tree.Y = 0.1;
                DrawTree(m_tree);
                m_ArrayCount = 0;
                btnNext.Content = "重新演示";
                m_flagStart = 0;
                m_DrawCount = 0;
            }
        }

        private void cleanCanvas() {
            if (m_flagStart == 1) {
                for (int i = 0; i <= m_ArrayCount; i++) {
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
            } else {
                if (this.FindName("v0") != null) {
                    MainCanvas.Children.Remove((UIElement)this.FindName("t0"));
                    this.UnregisterName("t0");
                    MainCanvas.Children.Remove((UIElement)this.FindName("v0"));
                    this.UnregisterName("v0");
                }
            }
        }

        private void DrawTree(TNode<int> node) {//修改成对树的重新绘制
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
                DrawTree(node.Lchild);
                DrawTree(node.Rchild);
            }        
        }

        private TNode<int> BuildBSTree(TNode<int> node) {
            if (m_weights[m_ArrayCount] < node.Name) {
                if (node.Lchild == null) {
                    TNode<int> lnode = new TNode<int>();
                    lnode.Name = m_weights[m_ArrayCount];
                    node.Lchild = lnode;
                    lnode.Parent = node;
                    lnode.X = node.X - 1.0 / Math.Pow(2, lnode.Depth());
                    lnode.Y = node.Y + 0.1;
                    return lnode;
                } else {
                    return BuildBSTree(node.Lchild);
                }
            } else if (m_weights[m_ArrayCount] > node.Name) {
                if (node.Rchild == null) {
                    TNode<int> rnode = new TNode<int>();
                    rnode.Name = m_weights[m_ArrayCount];
                    node.Rchild = rnode;
                    rnode.Parent = node;
                    rnode.X = node.X + 1.0 / Math.Pow(2, rnode.Depth());
                    rnode.Y = node.Y + 0.1;
                    return rnode;
                } else {
                    return BuildBSTree(node.Rchild);
                }
            } else {
                return null;
            }
        }
        private void BuildAVLTree() {
            TNode<int> insert = BuildBSTree(m_tree);
            m_tree.GetBfE();
            TNode<int> ubnode = m_tree.getUnbalancedE();
            if (ubnode != null) {
                if (ubnode.Mark==2) {
                    if (ubnode.Lchild.Mark == 1) {//LL
                        TNode<int> swap = ubnode;
                        ubnode = ubnode.Lchild;
                        ubnode.Parent = swap.Parent;
                        if (swap.Parent != null) {
                            if (swap == swap.Parent.Lchild) {
                                swap.Parent.Lchild = ubnode;
                            } else {
                                swap.Parent.Rchild = ubnode;
                            }
                        }
                        swap.Lchild = ubnode.Rchild;
                        ubnode.Rchild = swap;
                        if (ubnode.Rchild != null) {
                            ubnode.Rchild.Parent = ubnode;
                        }
                        if (ubnode.Lchild != null) { 
                            ubnode.Lchild.Parent = ubnode;
                        }
                        if (swap.Lchild != null) {
                            swap.Lchild.Parent = swap;
                        }
                    } else if (ubnode.Lchild.Mark == -1) {//LR
                        TNode<int> swap1 = ubnode;
                        TNode<int> swap2 = ubnode.Lchild;
                        ubnode = swap2.Rchild;
                        ubnode.Parent = swap1.Parent;
                        if (swap1.Parent != null) {
                            if (swap1 == swap1.Parent.Lchild) {
                                swap1.Parent.Lchild = ubnode;
                            } else {
                                swap1.Parent.Rchild = ubnode;
                            }
                        }
                        swap2.Rchild = ubnode.Lchild;
                        swap1.Lchild = ubnode.Rchild;
                        ubnode.Lchild = swap2;
                        ubnode.Rchild = swap1;
                        if (ubnode.Lchild != null) {
                            ubnode.Lchild.Parent = ubnode;
                        }
                        if (ubnode.Rchild != null) {
                            ubnode.Rchild.Parent = ubnode;
                        }
                        if (swap1.Lchild != null) {
                            swap1.Lchild.Parent = swap1;
                        }
                        if (swap1.Rchild != null) {
                            swap1.Rchild.Parent = swap1;
                        }
                        if (swap2.Lchild != null) {
                            swap2.Lchild.Parent = swap2;
                        }
                        if (swap2.Rchild != null) {
                            swap2.Rchild.Parent = swap2;
                        }
                    }
                } else {
                    if (ubnode.Rchild.Mark == -1) {//RR
                        TNode<int> swap = ubnode;
                        ubnode = ubnode.Rchild;
                        ubnode.Parent = swap.Parent;
                        if (swap.Parent != null) {
                            if (swap == swap.Parent.Lchild) {
                                swap.Parent.Lchild = ubnode;
                            } else {
                                swap.Parent.Rchild = ubnode;
                            }
                        }
                        swap.Rchild = ubnode.Lchild;
                        ubnode.Lchild = swap;
                        if (ubnode.Lchild != null) {
                            ubnode.Lchild.Parent = ubnode;
                        }
                        if (ubnode.Rchild != null) {
                            ubnode.Rchild.Parent = ubnode;
                        }
                        if (swap.Rchild!=null) {
                            swap.Rchild.Parent = swap;
                        }
                    } else if (ubnode.Rchild.Mark == 1) {//RL
                        TNode<int> swap1 = ubnode;
                        TNode<int> swap2 = ubnode.Rchild;
                        ubnode = swap2.Lchild;
                        ubnode.Parent = swap1.Parent;
                        if (swap1.Parent != null) {
                            if (swap1 == swap1.Parent.Lchild) {
                                swap1.Parent.Lchild = ubnode;
                            } else {
                                swap1.Parent.Rchild = ubnode;
                            }
                        }
                        swap1.Rchild = ubnode.Lchild;
                        swap2.Lchild = ubnode.Rchild;
                        ubnode.Lchild = swap1;
                        ubnode.Rchild = swap2;
                        if (ubnode.Rchild != null) {
                            ubnode.Rchild.Parent = ubnode;
                        }
                        if (ubnode.Lchild != null) {
                            ubnode.Lchild.Parent = ubnode;
                        }
                        if (swap1.Lchild != null) {
                            swap1.Lchild.Parent = swap1;
                        }
                        if (swap1.Rchild != null) {
                            swap1.Rchild.Parent = swap1;
                        }
                        if (swap2.Lchild != null) {
                            swap2.Lchild.Parent = swap2;
                        }
                        if (swap2.Rchild != null) {
                            swap2.Rchild.Parent = swap2;
                        }
                    }
                }
                changeXY(ubnode);
            }
            while (m_tree.Parent != null) {
                m_tree = m_tree.Parent;
            }
        }
        public void changeXY(TNode<int> node) {
            if (node != null) {
                if (node.Parent == null) {
                    node.X = 0.5;
                    node.Y = 0.1;
                } else if (node.Parent.Lchild == node) {
                    node.X = node.Parent.X - 1.0 / Math.Pow(2, node.Depth());
                    node.Y = node.Parent.Y + 0.1;
                } else if (node.Parent.Rchild == node) {
                    node.X = node.Parent.X + 1.0 / Math.Pow(2, node.Depth());
                    node.Y = node.Parent.Y + 0.1;
                }
                changeXY(node.Lchild);
                changeXY(node.Rchild);
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
        private void radiobtnAVL_Checked(object sender, RoutedEventArgs e) {
            btnNext.IsEnabled = true;
        }

        private void radiobtnBST_Checked(object sender, RoutedEventArgs e) {
            btnNext.IsEnabled = true;
        }

        public void Filesave() {
            FileStream fs = new FileStream(MainWindowInfo.fileLocation, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            JObject jObject = new JObject();
            jObject["type"] = "BST";
            JArray jArray = new JArray();
            for(int i = 0; i < m_weights.Count; i++) {
                jArray.Add(m_weights[i]);
            }
            jObject["m_weights"] = jArray;
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
                Recevie(jObject["m_weights"].ToObject<List<int>>());
                sr.Close();
                fs.Close();
            } catch {
                MessageBox.Show("请检查文件是否完整");
            }
        }
    }
}

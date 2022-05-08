using DSVis.DataStruct;
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
    /// HuffmanWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HuffmanWindow : Page {
        List<int> m_weights = new List<int>();
        HTree m_tree;
        int m_countVertex = 0;
        int m_countEdge = 0;
        private List<Tuple<double, double>> VertexsPosi = new List<Tuple<double, double>>();
        private List<Tuple<double, double>> CodePosi = new List<Tuple<double, double>>();
        private List<Tuple<double, double, double, double>> EdgesPosi = new List<Tuple<double, double, double, double>>();
        int m_tccount = 0;
        int m_rsflag = 0;
        public delegate void DataConfirm();
        public DataConfirm dataConfirm;
        public delegate void DataClean();
        public DataClean dataClean;
        public HuffmanWindow() {
            InitializeComponent();
            if (MainWindowInfo.fileFlag == true) {
                this.IsEnabled = true;
            } else {
                SetTreeWeightForm form = new SetTreeWeightForm();
                form.Show();
                form.sendMessage = Recevie;
                form.windowClosed = FormClosed;
                this.IsEnabled = false;
            }
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
        }

        private void btnSetWeight_Click(object sender, RoutedEventArgs e) {
            SetTreeWeightForm form = new SetTreeWeightForm();
            form.Show(); 
            form.sendMessage = Recevie;
            form.windowClosed = FormClosed;
            this.IsEnabled = false;
        }
        public void Recevie(List<int> value) {
            dataConfirm();
            textWeight.Text = "叶子节点权值\n";
            cleanCodes();
            m_weights = value;
            m_tree = new HTree();
            int length = 0;
            m_weights.Sort();
            foreach (int w in m_weights) {
                textWeight.Text += w.ToString() + " ";
                HNode node = new HNode((char)('A' + length), w, 0.16+0.08*length, 0.25);
                m_tree.SetLeaf(node);
                length++;
            }
            m_tree.Length = length;
            m_tree.Sort();
            btnNext.IsEnabled = true;
            cleanCanvas();
            for (int i = 0; i < m_tree.GetLeafNum(); i++) {
                DrawTree(m_tree[i]);
            }
        }
        public void FormClosed() {
            this.IsEnabled = true;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e) {
            double x, y;
            if (m_tree.GetLeafNum() != 1 || m_rsflag == 1) {
                m_tree.Sort();
                if (m_rsflag == 1) {
                    m_rsflag = 0;
                    m_tree.BackUp();
                    cleanCodes();
                    CodePosi.Clear();
                }
                VertexsPosi.Clear();
                EdgesPosi.Clear();
                btnSetWeight.IsEnabled = false;
                m_tree.Huffman();
                cleanCanvas();
                for (int i = 0; i < m_tree.GetLeafNum(); i++) {
                    DrawTree(m_tree[i]);
                }
                btnNext.Content = "下一步";
            } else {
                btnSetWeight.IsEnabled = true;
                m_tree.SetCode(m_tree[0]);
                m_tccount = m_tree.Code.Count;
                for (int i = 0; i < m_tccount; i++) {
                    TextBlock text = new TextBlock();
                    HNode node = m_tree.Code[i] as HNode;
                    /*if (WindowState == WindowState.Maximized) {
                        x = node.X * SystemParameters.PrimaryScreenWidth;
                        y = node.Y * SystemParameters.PrimaryScreenHeight;
                    } else {*/
                        x = node.X * this.Width;
                        y = node.Y * this.Height;
                    //}
                    text.Text = node.Code;
                    text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size sizeText = text.DesiredSize;
                    text.SetValue(Canvas.LeftProperty, x - sizeText.Width / 2);
                    text.SetValue(Canvas.TopProperty, y - sizeText.Height / 2 + 25);
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    text.TextAlignment = TextAlignment.Center;
                    this.RegisterName("tc" + i, text);
                    MainCanvas.Children.Add(text);
                    CodePosi.Add(new Tuple<double, double>(node.X, node.Y));
                }
                btnNext.Content = "重新演示";
                m_rsflag = 1;
            }
        }
        private void DrawTree(HNode node) {
            double x, y;
            /*if (WindowState == WindowState.Maximized) {
                x = node.X * SystemParameters.PrimaryScreenWidth;
                y = node.Y * SystemParameters.PrimaryScreenHeight;
            } else {*/
                x = node.X * this.Width;
                y = node.Y * this.Height;
            //}
            if (node.RChild == null && node.LChild == null) {
                Ellipse ellipse = new Ellipse();
                ellipse.Width = 30;
                ellipse.Height = 30;
                ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                ellipse.SetValue(Canvas.LeftProperty, x - ellipse.Width / 2);
                ellipse.SetValue(Canvas.TopProperty, y - ellipse.Height / 2);
                ellipse.Name = "v" + m_countVertex.ToString();
                this.RegisterName("v" + m_countVertex, ellipse);

                TextBlock text = new TextBlock();
                text.Text = node.Name + "(" + node.Weight + ")";
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size sizeText = text.DesiredSize;
                text.SetValue(Canvas.LeftProperty, x - sizeText.Width / 2);
                text.SetValue(Canvas.TopProperty, y - sizeText.Height / 2);
                text.HorizontalAlignment = HorizontalAlignment.Center;
                text.VerticalAlignment = VerticalAlignment.Center;
                text.TextAlignment = TextAlignment.Center;
                this.RegisterName("t" + m_countVertex, text);

                MainCanvas.Children.Add(ellipse);
                MainCanvas.Children.Add(text);
                VertexsPosi.Add(new Tuple<double, double>(node.X,node.Y));
                m_countVertex++;
            } else {
                Rectangle rect = new Rectangle();
                rect.Width = 30;
                rect.Height = 30;
                rect.Fill = new SolidColorBrush(Colors.IndianRed);
                rect.SetValue(Canvas.LeftProperty, x - rect.Width / 2);
                rect.SetValue(Canvas.TopProperty, y - rect.Height / 2);
                rect.Name = "v" + m_countVertex.ToString();
                this.RegisterName("v" + m_countVertex, rect);

                TextBlock text = new TextBlock();
                text.Text = "(" + node.Weight + ")";
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size sizeText = text.DesiredSize;
                text.SetValue(Canvas.LeftProperty, x - sizeText.Width / 2);
                text.SetValue(Canvas.TopProperty, y - sizeText.Height / 2);
                text.HorizontalAlignment = HorizontalAlignment.Center;
                text.VerticalAlignment = VerticalAlignment.Center;
                text.TextAlignment = TextAlignment.Center;
                this.RegisterName("t" + m_countVertex, text);

                Line linel = new Line();
                Line liner = new Line();
                linel.X1 = x;
                linel.Y1 = y;
                /*if (WindowState == WindowState.Maximized) {
                    linel.X2 = node.LChild.X * SystemParameters.PrimaryScreenWidth;
                    linel.Y2 = node.LChild.Y * SystemParameters.PrimaryScreenHeight;
                } else {*/
                    linel.X2 = node.LChild.X * this.Width;
                    linel.Y2 = node.LChild.Y * this.Height;
                //}
                linel.Stroke = new SolidColorBrush(Colors.Black);
                linel.StrokeThickness = 1;
                liner.X1 = x;
                liner.Y1 = y;
                /*if (WindowState == WindowState.Maximized) {
                    liner.X2 = node.RChild.X * SystemParameters.PrimaryScreenWidth;
                    liner.Y2 = node.RChild.Y * SystemParameters.PrimaryScreenHeight;
                } else {*/
                    liner.X2 = node.RChild.X * this.Width;
                    liner.Y2 = node.RChild.Y * this.Height;
                //}
                liner.Stroke = new SolidColorBrush(Colors.Black);
                liner.StrokeThickness = 1;
                this.RegisterName("ll" + m_countEdge, linel);
                this.RegisterName("lr" + m_countEdge, liner);

                MainCanvas.Children.Add(rect);
                MainCanvas.Children.Add(text);
                MainCanvas.Children.Add(linel);
                MainCanvas.Children.Add(liner);

                m_countVertex++;
                m_countEdge++;

                VertexsPosi.Add(new Tuple<double, double>(node.X, node.Y));
                EdgesPosi.Add(new Tuple<double, double, double, double>(node.X, node.Y, node.LChild.X, node.LChild.Y));
                EdgesPosi.Add(new Tuple<double, double, double, double>(node.X, node.Y, node.RChild.X, node.RChild.Y));
                DrawTree(node.LChild);
                DrawTree(node.RChild);
            }
        }
        private void cleanCanvas() {
            for (int i = 0; i < m_countVertex; i++) {
                MainCanvas.Children.Remove((UIElement)this.FindName("v" + i));
                this.UnregisterName("v" + i);
                MainCanvas.Children.Remove((UIElement)this.FindName("t" + i));
                this.UnregisterName("t" + i);
            }
            for(int i = 0; i < m_countEdge; i++){
                MainCanvas.Children.Remove((UIElement)this.FindName("ll" + i));
                this.UnregisterName("ll" + i);
                MainCanvas.Children.Remove((UIElement)this.FindName("lr" + i));
                this.UnregisterName("lr" + i);
            }
            m_countVertex = 0;
            m_countEdge = 0;
        }
        private void cleanCodes() {
            for (int i = 0; i < m_tccount; i++) {
                MainCanvas.Children.Remove((UIElement)this.FindName("tc" + i));
                this.UnregisterName("tc" + i);
            }
            m_tccount = 0;
        }

        public void windowchanged() {
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
            CtrlBg.Height = this.Height;
            MainCanvas.Height = this.Height;
            MainCanvas.Width = this.Width - 170;
            for (int i = 0; i < m_countVertex; i++) {
                Object obj = this.FindName("v" + i);
                if (obj is Ellipse) {
                    Ellipse ellipse = obj as Ellipse;
                    ellipse.SetValue(Canvas.LeftProperty, (VertexsPosi[i].Item1 * this.Width) - (ellipse.Width / 2));
                    ellipse.SetValue(Canvas.TopProperty, (VertexsPosi[i].Item2 * this.Height) - (ellipse.Height / 2));
                } else {
                    Rectangle rect = obj as Rectangle;
                    rect.SetValue(Canvas.LeftProperty, (VertexsPosi[i].Item1 * this.Width) - (rect.Width / 2));
                    rect.SetValue(Canvas.TopProperty, (VertexsPosi[i].Item2 * this.Height) - (rect.Height / 2));
                }
                //Point point = ellipse.TranslatePoint(new Point(), MainCanvas);
                TextBlock tb = this.FindName("t" + i) as TextBlock;
                Size sizeText = tb.DesiredSize;
                tb.SetValue(Canvas.LeftProperty, (VertexsPosi[i].Item1 * this.Width) - (sizeText.Width / 2));
                tb.SetValue(Canvas.TopProperty, (VertexsPosi[i].Item2 * this.Height) - (sizeText.Height / 2));
            }
            for (int i = 0; i < m_countEdge; i++) {
                Line ll = this.FindName("ll" + i) as Line;
                ll.X1 = EdgesPosi[2 * i].Item1 * this.Width;
                ll.Y1 = EdgesPosi[2 * i].Item2 * this.Height;
                ll.X2 = EdgesPosi[2 * i].Item3 * this.Width;
                ll.Y2 = EdgesPosi[2 * i].Item4 * this.Height;
                Line lr = this.FindName("lr" + i) as Line;
                lr.X1 = EdgesPosi[2 * i + 1].Item1 * this.Width;
                lr.Y1 = EdgesPosi[2 * i + 1].Item2 * this.Height;
                lr.X2 = EdgesPosi[2 * i + 1].Item3 * this.Width;
                lr.Y2 = EdgesPosi[2 * i + 1].Item4 * this.Height;
            }
            if (m_rsflag == 1) {
                for (int i = 0; i < m_tccount; i++) {
                    TextBlock tb = this.FindName("tc" + i) as TextBlock;
                    Size sizeText = tb.DesiredSize;
                    tb.SetValue(Canvas.LeftProperty, (CodePosi[i].Item1 * this.Width) - (sizeText.Width / 2));
                    tb.SetValue(Canvas.TopProperty, (CodePosi[i].Item2 * this.Height) - (sizeText.Height / 2) + 25);
                }
            }
        }
        public void Filesave() {
            FileStream fs = new FileStream(MainWindowInfo.fileLocation, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            JObject jObject = new JObject();
            jObject["type"] = "Huffman";
            JArray jArray = new JArray();
            for (int i = 0; i < m_weights.Count; i++) {
                jArray.Add(m_weights[i]);
            }
            jObject["weights"] = jArray;
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
                Recevie(jObject["weights"].ToObject<List<int>>());
                sr.Close();
                fs.Close();
            } catch {
                MessageBox.Show("请检查文件是否完整");
            }
        }
    }
}

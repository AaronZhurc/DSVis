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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace DSVis.Windows.Pages {
    /// <summary>
    /// SortWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SortWindow : Page {
        ListArray array = new ListArray();
        List<int> gap = new List<int>();
        List<int> quickSorted = new List<int>();
        List<PivotList> quickSubArray = new List<PivotList>();
        bool CanvasClear = true;
        int m_countSort = 0, m_countGap = 0, m_countPivot = 0, m_countMerge = 2;
        bool m_flagBtnPivot = false;
        List<int> quickHigh = new List<int>();
        List<int> quickLow = new List<int>();
        List<bool> quickDir = new List<bool>();
        bool quickFlag = false;
        public delegate void DataConfirm();
        public DataConfirm dataConfirm;
        public delegate void DataClean();
        public DataClean dataClean;
        int maxCount50;
        public SortWindow() {
            InitializeComponent();
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
            maxCount50 = (int)((this.Width - 30) / 50) + 1;
            if (MainWindowInfo.fileFlag == true) {
                this.IsEnabled = true;
            } else {
                SetArrayForm form = new SetArrayForm();
                form.Show();
                form.sendMessage = Recevie;
                form.windowClosed = FormClosed;
                form.getMaxCount((int)this.Width/30);
                this.IsEnabled = false;
            }
        }
        public void Recevie(List<int> value) {
            dataConfirm();
            if (CanvasClear != true) {
                Clear();
            }
            array.setArray(value);
            textOut_origin.Text = "原数组";
            textOut_result.Text = "排序结果";
            for (int i = 0; i < array.Array.Count; i++) {
                textOut_origin.Text += " " + array[i] ;
            }
            this.IsEnabled = true;
            btnNext.IsEnabled = true;
            radiobtnBub.IsEnabled = true;
            radiobtnIns.IsEnabled = true;
            radiobtnSel.IsEnabled = true;
            radiobtnShell.IsEnabled = true;
            radiobtnQuick.IsEnabled = true;
            radiobtnMerge.IsEnabled = true;
            btnSet.IsEnabled = true;
            quickHigh.Add(array.Array.Count - 1);
            quickLow.Add(0);
            quickDir.Add(false);
            array.GetSorted();
            for (int i = 0; i < array.Array.Count; i++) {
                textOut_result.Text += " " + array.Sorted[i];
            }
            Draw();
            InitQuickSubArray();
        }
        public void FormClosed() {
            this.IsEnabled = true;
        }

        private void btnSet_Click(object sender, RoutedEventArgs e) {
            SetArrayForm form = new SetArrayForm();
            form.Show();
            form.sendMessage = Recevie;
            form.windowClosed = FormClosed;
            form.getMaxCount((int)this.Width / 30);
            this.IsEnabled = false;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e) {
            if (array.Over == false) {
                if (radiobtnBub.IsChecked == true) {
                    btnNext.Content = "下一步";
                    radiobtnIns.IsEnabled = false;
                    radiobtnSel.IsEnabled = false;
                    radiobtnShell.IsEnabled = false;
                    radiobtnQuick.IsEnabled = false;
                    radiobtnMerge.IsEnabled = false;
                    btnSet.IsEnabled = false;
                    array.BubbleSort();
                    Clear();
                    Draw();
                } else if (radiobtnIns.IsChecked == true) {
                    btnNext.Content = "下一步";
                    radiobtnBub.IsEnabled = false;
                    radiobtnSel.IsEnabled = false;
                    radiobtnShell.IsEnabled = false;
                    radiobtnQuick.IsEnabled = false;
                    radiobtnMerge.IsEnabled = false;
                    btnSet.IsEnabled = false;
                    array.InsertionSort();
                    Clear();
                    Draw();
                } else if (radiobtnSel.IsChecked == true) {
                    btnNext.Content = "下一步";
                    radiobtnBub.IsEnabled = false;
                    radiobtnIns.IsEnabled = false;
                    radiobtnShell.IsEnabled = false;
                    radiobtnQuick.IsEnabled = false;
                    radiobtnMerge.IsEnabled = false;
                    btnSet.IsEnabled = false;
                    array.SelectionSort();
                    Clear();
                    Draw();
                } else if (radiobtnShell.IsChecked == true) {
                    btnNext.Content = "下一步";
                    radiobtnBub.IsEnabled = false;
                    radiobtnIns.IsEnabled = false;
                    radiobtnSel.IsEnabled = false;
                    radiobtnQuick.IsEnabled = false;
                    radiobtnMerge.IsEnabled = false;
                    btnSet.IsEnabled = false;
                    btnGap.IsEnabled = false;
                    if (gap.Count == 0) {
                        MessageBox.Show("请先设置步长");
                    } else {
                        if (m_countGap < gap[m_countSort]) {
                            array.ShellSort(gap[m_countSort], m_countGap);
                            Clear();
                            Draw();
                            m_countGap++;
                            if (m_countGap == gap[m_countSort]) {
                                m_countGap = 0;
                                m_countSort++;
                            }
                        }
                    }
                } else if (radiobtnQuick.IsChecked == true) {
                    if (m_countPivot == 0) {//枢轴数量还得和子数组相同
                        MessageBox.Show("请选择一个枢轴");
                    } else {
                        if (quickFlag == false) {
                            btnNext.Content = "下一步";
                            radiobtnBub.IsEnabled = false;
                            radiobtnIns.IsEnabled = false;
                            radiobtnSel.IsEnabled = false;
                            radiobtnShell.IsEnabled = false;
                            radiobtnMerge.IsEnabled = false;
                            btnSet.IsEnabled = false;
                            btnPivot.IsEnabled = false;
                            quickFlag = true;
                            for (int i = 0; i < quickSubArray.Count; i++) {
                                quickDir[i] = !quickDir[i];
                                bool dir = quickDir[i];
                                int high = quickHigh[i];
                                int low = quickLow[i];
                                quickFlag &= array.QuickSort(quickSubArray[i].Pivot, ref high, ref low, ref dir);
                                quickLow[i] = low;
                                quickHigh[i] = high;
                            }
                            Clear();
                            Draw();
                        } else {
                            btnPivot.IsEnabled = true;
                            btnNext.IsEnabled = false;
                            InitQuickSubArray();
                            m_countPivot = 0;
                            quickHigh.Clear();
                            quickLow.Clear();
                            quickDir.Clear();
                            for (int i = 0; i < quickSubArray.Count; i++) {
                                quickLow.Add(quickSubArray[i].SubList[0]);
                                quickHigh.Add(quickSubArray[i].SubList[quickSubArray[i].SubList.Count - 1]);
                                quickDir.Add(false);
                            }
                            quickFlag = false;
                        }
                        array.SortOver();
                    }
                } else if (radiobtnMerge.IsChecked == true) {
                    btnNext.Content = "下一步";
                    radiobtnIns.IsEnabled = false;
                    radiobtnSel.IsEnabled = false;
                    radiobtnBub.IsEnabled = false;
                    radiobtnShell.IsEnabled = false;
                    radiobtnQuick.IsEnabled = false;
                    btnSet.IsEnabled = false;
                    array.MergeSort(m_countMerge);
                    Clear();
                    Draw();
                    m_countMerge = m_countMerge * 2;
                } else {
                    MessageBox.Show("请先选择排序方式");
                }
            } else if (array.Over == true) {
                btnNext.Content = "重新排序";
                radiobtnBub.IsEnabled = true;
                radiobtnIns.IsEnabled = true;
                radiobtnSel.IsEnabled = true;
                radiobtnShell.IsEnabled = true;
                radiobtnQuick.IsEnabled = true;
                btnSet.IsEnabled = true;
                m_countSort = 0;
                m_countMerge = 2;
                if (radiobtnShell.IsChecked == true) {
                    btnGap.IsEnabled = true;
                } else if (radiobtnQuick.IsChecked == true) {
                    btnPivot.IsEnabled = true;
                }
                quickSubArray.Clear();
                quickSorted.Clear();
                m_countPivot = 0;
                array.Clear();
                Clear();
                Draw();
            } 
        }

        public void Draw() {
            for(int i = 0; i < array.Array.Count; i++) {
                Ellipse ellipse = new Ellipse();
                TextBlock text = new TextBlock();
                ellipse.Width = 30;
                ellipse.Height = 30;
                if (radiobtnIns.IsChecked == true) {
                    if (i <= array.Bloop)
                        ellipse.Fill = new SolidColorBrush(Colors.DarkSeaGreen);
                    else
                        ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    if (i == array.Swapa)
                        ellipse.Fill = new SolidColorBrush(Colors.IndianRed);
                } else if (radiobtnBub.IsChecked == true) {
                    if (i >= array.Array.Count - array.Bloop)
                        ellipse.Fill = new SolidColorBrush(Colors.DarkSeaGreen);
                    else
                        ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    if (i == array.Swapa || i == array.Swapb)
                        ellipse.Fill = new SolidColorBrush(Colors.IndianRed);
                } else if (radiobtnSel.IsChecked == true) {
                    if (array.Sorting[i] == 1) {
                        ellipse.Fill = new SolidColorBrush(Colors.DarkSeaGreen);
                    }
                    if (i == array.Swapa || i == array.Swapb) {
                        ellipse.Fill = new SolidColorBrush(Colors.IndianRed);
                    } else if (array.Sorting[i] != 1) {
                        ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    }
                } else if (radiobtnShell.IsChecked == true) {
                    if (gap[m_countSort] != 1) {
                        if ((i - m_countGap) % gap[m_countSort] == 0) {
                            ellipse.Fill = new SolidColorBrush(Colors.IndianRed);
                        } else {
                            ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                        }
                    } else {
                        ellipse.Fill = new SolidColorBrush(Colors.DarkSeaGreen);
                    }
                } else if (radiobtnQuick.IsChecked == true) {
                    ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    if (array.Over == true) {
                        ellipse.Fill = new SolidColorBrush(Colors.DarkSeaGreen);
                    }
                    for (int j = 0; j < quickHigh.Count; j++) {
                        if (quickSubArray.Count != 0) {
                            if (i >= quickSubArray[j].SubList[0] && i <= quickSubArray[j].SubList[quickSubArray[j].SubList.Count - 1]) {
                                if (quickHigh[j] == quickLow[j]) {
                                    if (quickLow[j] == i) {
                                        ellipse.Fill = new SolidColorBrush(Colors.DarkSeaGreen);
                                        quickSorted.Add(i);
                                    } else {
                                        ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                                    }
                                } else {//可能要大改动
                                    if ((quickDir[j] == true && quickHigh[j] == i) || (quickDir[j] == false && quickLow[j] == i)) {
                                        ellipse.Visibility = Visibility.Hidden;
                                        text.Visibility = Visibility.Hidden;
                                    } else if ((quickDir[j] == true && quickLow[j] == i) || (quickDir[j] == false && quickHigh[j] == i)) {
                                        ellipse.Fill = new SolidColorBrush(Colors.IndianRed);
                                    } else {
                                        ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                                    }
                                }
                            } else if (quickSorted.Contains(i)) {
                                ellipse.Fill = new SolidColorBrush(Colors.DarkSeaGreen);
                            } else {
                                ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                            }
                        }
                    }
                } else if (radiobtnMerge.IsChecked == true) {
                    if (i / m_countMerge % 2 == 0) {
                        ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    } else {
                        ellipse.Fill = new SolidColorBrush(Colors.DeepSkyBlue);
                    }
                    if (array.Over == true) {
                        ellipse.Fill = new SolidColorBrush(Colors.DarkSeaGreen);
                    }
                } else {
                    ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                }
                ellipse.Name = "v" + i;
                this.RegisterName("v" + i, ellipse);
                if (array.Array.Count <= maxCount50) {
                    ellipse.SetValue(Canvas.LeftProperty, (MainCanvas.ActualWidth - (array.Array.Count * ellipse.Width + (array.Array.Count - 1) * 20)) / 2 + 50 * i);
                } else {
                    ellipse.SetValue(Canvas.LeftProperty, (MainCanvas.ActualWidth - (array.Array.Count * ellipse.Width)) / 2 + 30 * i);
                }
                ellipse.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - ellipse.Height / 2);

                text.Text = array[i].ToString();
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size sizeText = text.DesiredSize;
                if (array.Array.Count <= maxCount50) {
                    text.SetValue(Canvas.LeftProperty, (MainCanvas.ActualWidth - (array.Array.Count * ellipse.Width + (array.Array.Count - 1) * 20)) / 2 + 50 * i + 15 - sizeText.Width / 2);
                } else {
                    text.SetValue(Canvas.LeftProperty, (MainCanvas.ActualWidth - (array.Array.Count * ellipse.Width)) / 2 + 30 * i + 15 - sizeText.Width / 2);
                }
                text.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - sizeText.Height / 2);
                text.HorizontalAlignment = HorizontalAlignment.Center;
                text.VerticalAlignment = VerticalAlignment.Center;
                text.TextAlignment = TextAlignment.Center;
                text.Name = "t" + i;
                this.RegisterName("t" + i, text);

                ellipse.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Ellipse_MouseLeftButtonDown);
                ellipse.MouseEnter += new System.Windows.Input.MouseEventHandler(this.Ellipse_MouseEnter);
                ellipse.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Ellipse_MouseLeave);
                text.MouseLeftButtonDown += new System.Windows.Input.MouseButtonEventHandler(this.Ellipse_MouseLeftButtonDown);
                text.MouseEnter += new System.Windows.Input.MouseEventHandler(this.Ellipse_MouseEnter);
                text.MouseLeave += new System.Windows.Input.MouseEventHandler(this.Ellipse_MouseLeave);

                MainCanvas.Children.Add(ellipse);
                MainCanvas.Children.Add(text);
            }
            CanvasClear = false;
        }

        public void Clear() {
            for (int i = 0; i < array.Array.Count; i++) {
                MainCanvas.Children.Remove((UIElement)this.FindName("v" + i));
                this.UnregisterName("v" + i);
                MainCanvas.Children.Remove((UIElement)this.FindName("t" + i));
                this.UnregisterName("t" + i);
            }
            CanvasClear = true;
        }

        public void windowchanged() {
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
            maxCount50 = (int)((this.Width - 30) / 50) + 1;
            CtrlBg.Height = this.Height;
            MainCanvas.Height = this.Height;
            MainCanvas.Width = this.Width - 170;
            for (int i = 0; i < array.Array.Count; i++) {
                Ellipse ellipse = (Ellipse)this.FindName("v" + i);
                TextBlock text = (TextBlock)this.FindName("t" + i);
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size sizeText = text.DesiredSize;
                if (array.Array.Count <= maxCount50) {
                    ellipse.SetValue(Canvas.LeftProperty, (MainCanvas.Width - (array.Array.Count * ellipse.Width + (array.Array.Count - 1) * 20)) / 2 + 50 * i);
                    text.SetValue(Canvas.LeftProperty, (MainCanvas.Width - (array.Array.Count * ellipse.Width + (array.Array.Count - 1) * 20)) / 2 + 50 * i + 15 - sizeText.Width / 2);
                } else {
                    ellipse.SetValue(Canvas.LeftProperty, (MainCanvas.Width - (array.Array.Count * ellipse.Width)) / 2 + 30 * i);
                    text.SetValue(Canvas.LeftProperty, (MainCanvas.Width - (array.Array.Count * ellipse.Width)) / 2 + 30 * i + 15 - sizeText.Width / 2);
                }
                ellipse.SetValue(Canvas.TopProperty, MainCanvas.Height / 2 - ellipse.Height / 2);
                text.SetValue(Canvas.TopProperty, MainCanvas.Height / 2 - sizeText.Height / 2);
            }
        }

        private void radiobtnBub_Checked(object sender, RoutedEventArgs e) {
            btnGap.IsEnabled = false;
            btnPivot.IsEnabled = false;
            textGap.Visibility = Visibility.Hidden;
        }

        private void radiobtnSel_Checked(object sender, RoutedEventArgs e) {
            btnGap.IsEnabled = false;
            btnPivot.IsEnabled = false;
            textGap.Visibility = Visibility.Hidden;
        }

        private void radiobtnIns_Checked(object sender, RoutedEventArgs e) {
            btnGap.IsEnabled = false;
            btnPivot.IsEnabled = false;
            textGap.Visibility = Visibility.Hidden;
        }

        private void radiobtnShell_Checked(object sender, RoutedEventArgs e) {
            btnGap.IsEnabled = true;
            btnPivot.IsEnabled = false;
            textGap.Visibility = Visibility.Visible;
        }

        private void radiobtnQuick_Checked(object sender, RoutedEventArgs e) {
            InitQuickSubArray();
            btnGap.IsEnabled = false;
            btnPivot.IsEnabled = true;
            textGap.Visibility = Visibility.Hidden;
        }

        private void btnGap_Click(object sender, RoutedEventArgs e) {
            SetGapForm form = new SetGapForm();
            form.sendLength(array.Array.Count);
            form.Show();
            form.sendMessage = GapRecevie;
            form.windowClosed = FormClosed;
        }

        public void GapRecevie(List<int> value) {
            gap=value;
            if (!gap.Contains(1)) {
                gap.Add(1);
            }
            textGap.Text = "步长 ";
            for(int i = 0; i < gap.Count; i++) {
                textGap.Text += gap[i];
                textGap.Text += " ";
            }
            this.IsEnabled = true;
        }

        private void btnPivot_MouseEnter(object sender, MouseEventArgs e) {
            if (radiobtnQuick.IsChecked == true) {
                ToolTip tip = new ToolTip {
                    Content = "通过点按带数字的圆确定枢轴"
                };
                btnPivot.ToolTip = tip;
            }
        }

        private void btnPivot_Click(object sender, RoutedEventArgs e) {
            m_flagBtnPivot = true;
            btnPivot.IsEnabled = false;
            for(int i = 0; i < array.Array.Count; i++) {
                Ellipse ellipse = this.FindName("v" + i) as Ellipse;
                ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                if (quickSorted.Contains(i)) {
                    ellipse.Fill = new SolidColorBrush(Colors.DarkSeaGreen);
                }
            }
            for(int i = 0; i < quickSubArray.Count; i++) {
                if (quickSubArray[i].Piloc != -1) {
                    Ellipse ellipse = this.FindName("v" + quickSubArray[i].Piloc) as Ellipse;
                    ellipse.Fill = new SolidColorBrush(Colors.DarkRed);
                }
            }
        }
        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            bool flag = false;
            int p = -1;
            if (sender is Ellipse) {
                Ellipse ellipse = sender as Ellipse;
                String str = ellipse.Name;
                String[] sstr = str.Split('v');
                if (quickSorted.Contains(int.Parse(sstr[1]))) {
                    flag = true;
                }
                p = int.Parse(sstr[1]);
            } else if (sender is TextBlock) {
                TextBlock ellipse = sender as TextBlock;
                String str = ellipse.Name;
                String[] sstr = str.Split('t');
                if (quickSorted.Contains(int.Parse(sstr[1]))) {
                    flag = true;
                }
                p = int.Parse(sstr[1]);
            }
            PivotList plist = new PivotList();
            for (int i = 0; i < quickSubArray.Count; i++) {
                if (quickSubArray[i].SubList.Contains(p)) {
                    plist = quickSubArray[i];
                }
            }
            // 1.点选的个数应该等于被分割的字符串个数 2.被分割的字符串中应该有一个(可以用被分割的字符串控制快排执行次数)
            // 注意有时被分割的字符串里只有一个元素
            // 需要能够重新选择
            if (radiobtnQuick.IsChecked == true && m_flagBtnPivot == true && flag == false && m_countPivot < quickSubArray.Count && !quickSorted.Contains(p)) {
                //m_countPivot = 0;
                if (sender is Ellipse) {
                    Ellipse ellipse = sender as Ellipse;
                    ellipse.Fill = new SolidColorBrush(Colors.DarkRed);
                    String str = ellipse.Name;
                    String[] sstr = str.Split('v');
                    for(int i = 0; i < quickSubArray.Count; i++) {
                        if (quickSubArray[i].SubList.Contains(int.Parse(sstr[1]))) {
                            quickSubArray[i].Pivot = array[int.Parse(sstr[1])];
                            quickSubArray[i].Piloc = int.Parse(sstr[1]);
                            int temp = array.Array[quickSubArray[i].SubList[0]];
                            array.Array[quickSubArray[i].SubList[0]] = quickSubArray[i].Pivot;
                            array.Array[int.Parse(sstr[1])] = temp;
                        }
                    }
                } else if (sender is TextBlock) {
                    TextBlock text = sender as TextBlock;
                    String str = text.Name;
                    String[] sstr = str.Split('t');
                    Ellipse ellipse = this.FindName("v" + int.Parse(sstr[1])) as Ellipse;
                    ellipse.Fill = new SolidColorBrush(Colors.DarkRed);
                    for (int i = 0; i < quickSubArray.Count; i++) {
                        if (quickSubArray[i].SubList.Contains(int.Parse(sstr[1]))) {
                            quickSubArray[i].Pivot = array[int.Parse(sstr[1])];
                            quickSubArray[i].Piloc = int.Parse(sstr[1]);
                            int temp = array.Array[quickSubArray[i].SubList[0]];
                            array.Array[quickSubArray[i].SubList[0]] = quickSubArray[i].Pivot;
                            array.Array[int.Parse(sstr[1])] = temp;
                        }
                    }
                }
                m_countPivot++;
            }
            if(m_countPivot == quickSubArray.Count) {
                m_flagBtnPivot = false;
                btnNext.IsEnabled = true;
                btnPivot.IsEnabled = false;
            }
        }

        private void Ellipse_MouseEnter(object sender, MouseEventArgs e) {
            bool flag = false;
            if (sender is Ellipse) {
                Ellipse ellipse = sender as Ellipse;
                String str = ellipse.Name;
                String[] sstr = str.Split('v');
                if (quickSorted.Contains(int.Parse(sstr[1]))) {
                    flag = true;
                }
            } else if (sender is TextBlock) {
                TextBlock ellipse = sender as TextBlock;
                String str = ellipse.Name;
                String[] sstr = str.Split('t');
                if (quickSorted.Contains(int.Parse(sstr[1]))) {
                    flag = true;
                }
            }

            if (radiobtnQuick.IsChecked == true && m_flagBtnPivot == true && flag == false) {
                Cursor = Cursors.Hand;
            }
        }

        private void Ellipse_MouseLeave(object sender, MouseEventArgs e) {
            Cursor = Cursors.Arrow;
        }

        private void InitQuickSubArray() {
            quickSubArray.Clear();
            PivotList list = new PivotList();
            for(int i = 0; i < array.Array.Count; i++) {
                if (quickSorted.Contains(i) && list.SubList.Count != 0) {
                    quickSubArray.Add(list);
                    list = new PivotList();
                } else if (quickSorted.Contains(i - 1) && quickSorted.Contains(i + 1) ||
                    quickSorted.Contains(i - 1) && array.Array.Count == (i + 1) ||
                    quickSorted.Contains(i + 1) && (i - 1) == -1) {//子数组只有一个元素
                    quickSorted.Add(i);
                    list.SubList.Clear();
                } else if (i == array.Array.Count - 1) {
                    list.SubList.Add(i);
                    quickSubArray.Add(list);
                    list = new PivotList();
                } else if (quickSorted.Contains(i)) {
                    continue;
                } else {
                    list.SubList.Add(i);
                }
            }
        }
        private bool DetectPivot(PivotList list) {
            if (list.SubList.Count == 0) {
                return false;
            } else {
                if (list.Piloc == -1) {
                    return true;
                } else {
                    return false;
                }
            }
        }

        private void radiobtnMerge_Checked(object sender, RoutedEventArgs e) {
            btnGap.IsEnabled = false;
            btnPivot.IsEnabled = false;
            textGap.Visibility = Visibility.Hidden;
        }

        public void Filesave() {
            FileStream fs = new FileStream(MainWindowInfo.fileLocation, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            JObject jObject = new JObject();
            jObject["type"] = "Sort";
            JArray jArray = new JArray();
            for (int i = 0; i < array.Array.Count; i++) {
                jArray.Add(array[i]);
            }
            jObject["array"] = jArray;
            if (radiobtnBub.IsChecked == true) {
                jObject["way"] = "Bub";
            } else if (radiobtnIns.IsChecked == true) {
                jObject["way"] = "Ins";
            }else if (radiobtnSel.IsChecked == true) {
                jObject["way"] = "Sel";
            }else if (radiobtnQuick.IsChecked == true) {
                jObject["way"] = "Quick";
            } else if (radiobtnMerge.IsChecked == true) {
                jObject["way"] = "Merge";
            } else if (radiobtnShell.IsChecked == true) {
                jObject["way"] = "Shell";
                jArray = new JArray();
                for (int i = 0; i < gap.Count; i++) {
                    jArray.Add(gap[i]);
                }
                jObject["gap"] = jArray;
            } else {
                jObject["way"] = "Null";
            }
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
                Recevie(jObject["array"].ToObject<List<int>>());
                if (jObject["way"].ToString().Equals("Null")) {

                } else if (jObject["way"].ToString().Equals("Bub")) {
                    radiobtnBub.IsChecked = true;
                } else if (jObject["way"].ToString().Equals("Ins")) {
                    radiobtnIns.IsChecked = true;
                } else if (jObject["way"].ToString().Equals("Sel")) {
                    radiobtnSel.IsChecked = true;
                } else if (jObject["way"].ToString().Equals("Quick")) {
                    radiobtnQuick.IsChecked = true;
                } else if (jObject["way"].ToString().Equals("Merge")) {
                    radiobtnMerge.IsChecked = true;
                } else if (jObject["way"].ToString().Equals("Shell")) {
                    radiobtnShell.IsChecked = true;
                    if (jObject["gap"] != null) {
                        GapRecevie(jObject["gap"].ToObject<List<int>>());
                    }
                }
                sr.Close();
                fs.Close();
            } catch {
                MessageBox.Show("请检查文件是否完整");
            }
        }
    }
}

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
    /// SearchWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SearchWindow : Page {
        List<int> array = new List<int>();
        List<int> sort = new List<int>();
        List<int> block = new List<int>();
        List<int> blockmax = new List<int>();
        List<int> blockstart = new List<int>();
        int search;
        int ssub, bsub;
        int flagNext;
        Boolean CanvasClear,flagBlock,flagSearch;
        public delegate void DataConfirm();
        public DataConfirm dataConfirm;
        public delegate void DataClean();
        public DataClean dataClean;
        public SearchWindow() {
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
            CanvasClear = true;
            flagBlock = false;
            flagSearch = false;
            flagNext = 0;
        }

        private void btnSet_Click(object sender, RoutedEventArgs e) {
            SetArrayForm form = new SetArrayForm();
            form.Show();
            form.sendMessage = Recevie;
            form.windowClosed = FormClosed;
        }
        public void Recevie(List<int> value) {
            dataConfirm();
            if (CanvasClear == false) {
                Clear();
            }
            array.Clear();
            array = value;
            this.IsEnabled = true;
            btnNext.IsEnabled = true;
            btnSet.IsEnabled = true;
            radiobtnBinary.IsChecked = false;
            radiobtnBlock.IsChecked = false;
            radiobtnBinary.IsEnabled = true;
            radiobtnBlock.IsEnabled = true;
            for (int i = 0; i < array.Count; i++) {
                sort.Add(array[i]);
            }
            sort.Sort();
            flagBlock = false;
            flagSearch = false;
            textSearch.Text = "搜索值";
            textBlock.Text = "";
            Draw();
        }
        public void SearchRecevie(int value) {
            flagSearch = true;
            search = value;
            textSearch.Text = "搜索值 " + value;
            bsub = array.Count - 1;
            ssub = 0;
            flagNext = 0;
            Clear();
            Draw();
        }
        public void BlockRecevie(List<int> value) {
            blockstart.Clear();
            blockmax.Clear();
            block.Clear();
            blockmax = value;
            blockmax.Sort();
            int count = 0;
            for (int j = 0; j < array.Count; j++) {
                if (array[j] <= blockmax[0]) {
                    block.Add(array[j]);
                    count++;
                }
            }
            blockstart.Add(count);
            textBlock.Text = "最大关键字 " + blockmax[0];
            for (int i = 1; i < blockmax.Count; i++) {
                for (int j = 0; j < array.Count; j++) {
                    if (array[j] > blockmax[i - 1] && array[j] <= blockmax[i]) {
                        block.Add(array[j]);
                        count++;
                    }
                }
                blockstart.Add(count);
                textBlock.Text += " " + blockmax[i];    
            }
            for (int j = 0; j < array.Count; j++) {
                if (array[j] > blockmax[blockmax.Count - 1]) {
                    block.Add(array[j]);
                    count++;
                }
            }
            flagBlock = true;
            flagNext = 0;
            Clear();
            Draw();
        }
        public void FormClosed() {
            this.IsEnabled = true;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e) {
            SetPushForm form = new SetPushForm();
            form.Show();
            form.sendMessage = SearchRecevie;
            form.windowClosed = FormClosed;
        }

        private void btnBlock_Click(object sender, RoutedEventArgs e) {
            SetBlockForm form = new SetBlockForm();
            form.Show();
            form.sendMessage = BlockRecevie;
            form.windowClosed = FormClosed;
        }

        private void radiobtnBlock_Checked(object sender, RoutedEventArgs e) {
            textBlock.Text = "最大关键字";
            btnBlock.IsEnabled = true;
            flagBlock = false;
            Clear();
            Draw();
        }

        private void radiobtnBinary_Checked(object sender, RoutedEventArgs e) {
            textBlock.Text = "";
            btnBlock.IsEnabled = false;
            flagBlock = false;
            Clear();
            Draw();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e) {
            if (radiobtnBinary.IsChecked == true) {
                if (flagSearch != true) {
                    MessageBox.Show("请检查是否设置搜索值");
                } else {
                    if (ssub <= bsub) {
                        int mid = (bsub + ssub) / 2;
                        if (search == sort[mid]) {
                            Ellipse ellipse = this.FindName("v" + mid) as Ellipse;
                            ellipse.Fill = new SolidColorBrush(Colors.IndianRed);
                        } else if (sort[mid] > search) {
                            bsub = mid - 1;
                        } else {
                            ssub = mid + 1;
                        }
                        for(int i = 0; i < sort.Count; i++) {
                            if (i < ssub || i > bsub) {
                                Ellipse ellipse = this.FindName("v" + i) as Ellipse;
                                ellipse.Fill = new SolidColorBrush(Colors.DarkGray);
                            }
                        }
                    } else {
                        MessageBox.Show("没有找到该元素");
                    }
                }
            } else if (radiobtnBlock.IsChecked == true) {
                if (flagBlock != true) {
                    MessageBox.Show("请检查是否设置分块");
                } else if (flagSearch != true) {
                    MessageBox.Show("请检查是否设置搜索值");
                } else {
                    if (flagNext == 0) {
                        flagNext++;
                        for (int i = 0; i < blockmax.Count - 1; i++) {
                            if (blockmax[i] < search && search <= blockmax[i + 1]) {
                                ssub = blockstart[i];
                                bsub = blockstart[i + 1];
                                break;
                            } else if(search > blockmax[i + 1]) {
                                ssub = blockstart[i + 1];
                            } else {
                                bsub = blockstart[i];
                            }
                        }
                        for (int i = 0; i < block.Count; i++) {
                            if (i < ssub || i > bsub) {
                                Ellipse ellipse = this.FindName("v" + i) as Ellipse;
                                ellipse.Fill = new SolidColorBrush(Colors.DarkGray);
                            }
                        }
                    } else {
                        int i;
                        for(i = ssub; i <= bsub; i++) {
                            if (block[i] == search) {
                                Ellipse ellipse = this.FindName("v" + i) as Ellipse;
                                ellipse.Fill = new SolidColorBrush(Colors.IndianRed);
                                break;
                            }
                        }
                        if (i > bsub) {
                            MessageBox.Show("没有找到该元素");
                        }
                    }
                }
            } else {
                MessageBox.Show("请检查是否选择搜索模式");
            }
        }

        public void Draw() {
            if (radiobtnBinary.IsChecked == true) {
                for (int i = 0; i < array.Count; i++) {
                    Ellipse ellipse = new Ellipse();
                    ellipse.Width = 30;
                    ellipse.Height = 30;
                    ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    ellipse.Name = "v" + i;
                    this.RegisterName("v" + i, ellipse);
                    ellipse.SetValue(Canvas.LeftProperty, (MainCanvas.ActualWidth - (array.Count * ellipse.Width + (array.Count - 1) * 50)) / 2 + 150 + 50 * i - ellipse.Width / 2);
                    ellipse.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - ellipse.Height / 2);

                    TextBlock text = new TextBlock();
                    text.Text = sort[i].ToString();
                    text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size sizeText = text.DesiredSize;
                    text.SetValue(Canvas.LeftProperty, (MainCanvas.ActualWidth - (array.Count * ellipse.Width + (array.Count - 1) * 50)) / 2 + 150 + 50 * i - sizeText.Width / 2);
                    text.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - sizeText.Height / 2);
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    text.TextAlignment = TextAlignment.Center;
                    text.Name = "t" + i;
                    this.RegisterName("t" + i, text);

                    MainCanvas.Children.Add(ellipse);
                    MainCanvas.Children.Add(text);
                }
            } else if (radiobtnBlock.IsChecked == true && flagBlock == true) {
                for (int i = 0; i < array.Count; i++) {
                    Ellipse ellipse = new Ellipse();
                    ellipse.Width = 30;
                    ellipse.Height = 30;
                    if (blockstart.Exists(x => x == i) || i == 0) {
                        ellipse.Fill = new SolidColorBrush(Colors.DarkSeaGreen);
                    } else {
                        ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    }
                    ellipse.Name = "v" + i;
                    this.RegisterName("v" + i, ellipse);
                    ellipse.SetValue(Canvas.LeftProperty, (MainCanvas.ActualWidth - (array.Count * ellipse.Width + (array.Count - 1) * 50)) / 2 + 150 + 50 * i - ellipse.Width / 2);
                    ellipse.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - ellipse.Height / 2);

                    TextBlock text = new TextBlock();
                    text.Text = block[i].ToString();
                    text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size sizeText = text.DesiredSize;
                    text.SetValue(Canvas.LeftProperty, (MainCanvas.ActualWidth - (array.Count * ellipse.Width + (array.Count - 1) * 50)) / 2 + 150 + 50 * i - sizeText.Width / 2);
                    text.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - sizeText.Height / 2);
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    text.TextAlignment = TextAlignment.Center;
                    text.Name = "t" + i;
                    this.RegisterName("t" + i, text);

                    MainCanvas.Children.Add(ellipse);
                    MainCanvas.Children.Add(text);
                }
            } else {
                for (int i = 0; i < array.Count; i++) {
                    Ellipse ellipse = new Ellipse();
                    ellipse.Width = 30;
                    ellipse.Height = 30;
                    ellipse.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    ellipse.Name = "v" + i;
                    this.RegisterName("v" + i, ellipse);
                    ellipse.SetValue(Canvas.LeftProperty, (MainCanvas.ActualWidth - (array.Count * ellipse.Width + (array.Count - 1) * 50)) / 2 + 150 + 50 * i - ellipse.Width / 2);
                    ellipse.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - ellipse.Height / 2);

                    TextBlock text = new TextBlock();
                    text.Text = array[i].ToString();
                    text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size sizeText = text.DesiredSize;
                    text.SetValue(Canvas.LeftProperty, (MainCanvas.ActualWidth - (array.Count * ellipse.Width + (array.Count - 1) * 50)) / 2 + 150 + 50 * i - sizeText.Width / 2);
                    text.SetValue(Canvas.TopProperty, MainCanvas.ActualHeight / 2 - sizeText.Height / 2);
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    text.TextAlignment = TextAlignment.Center;
                    text.Name = "t" + i;
                    this.RegisterName("t" + i, text);

                    MainCanvas.Children.Add(ellipse);
                    MainCanvas.Children.Add(text);
                }
            }
            CanvasClear = false;
        }
        public void Clear() {
            for (int i = 0; i < array.Count; i++) {
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
            CtrlBg.Height = this.Height;
            MainCanvas.Height = this.Height;
            MainCanvas.Width = this.Width - 170;
            for (int i = 0; i < array.Count; i++) {
                Ellipse ellipse = (Ellipse)this.FindName("v" + i);
                TextBlock text = (TextBlock)this.FindName("t" + i);
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size sizeText = text.DesiredSize;
                ellipse.SetValue(Canvas.LeftProperty, (MainCanvas.Width - (array.Count * ellipse.Width + (array.Count - 1) * 50)) / 2 + 150 + 50 * i - ellipse.Width / 2);
                ellipse.SetValue(Canvas.TopProperty, MainCanvas.Height / 2 - ellipse.Height / 2);
                text.SetValue(Canvas.LeftProperty, (MainCanvas.Width - (array.Count * ellipse.Width + (array.Count - 1) * 50)) / 2 + 150 + 50 * i - sizeText.Width / 2);
                text.SetValue(Canvas.TopProperty, MainCanvas.Height / 2 - sizeText.Height / 2);
            }
        }
        public void Filesave() {
            FileStream fs = new FileStream(MainWindowInfo.fileLocation, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            JObject jObject = new JObject();
            jObject["type"] = "Search";
            JArray jArray = new JArray();
            for (int i = 0; i < array.Count; i++) {
                jArray.Add(array[i]);
            }
            jObject["array"] = jArray;
            if (radiobtnBinary.IsChecked == true) {
                jObject["way"] = "Binary";
            } else if (radiobtnBlock.IsChecked == true) {
                jObject["way"] = "Block";
                jArray = new JArray();
                for (int i = 0; i < blockmax.Count; i++) {
                    jArray.Add(blockmax[i]);
                }
                jObject["block"] = jArray;
            }  else {
                jObject["way"] = "Null";
            }
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
            Recevie(jObject["array"].ToObject<List<int>>());
            if (jObject["way"].ToString().Equals("Null")) {

            } else if (jObject["way"].ToString().Equals("Binary")) {
                radiobtnBinary.IsChecked = true;
            } else if (jObject["way"].ToString().Equals("Block")) {
                radiobtnBlock.IsChecked = true;
                if (jObject["block"] != null) {
                    BlockRecevie(jObject["block"].ToObject<List<int>>());
                }
            }
            sr.Close();
            fs.Close();
        }
    }
}

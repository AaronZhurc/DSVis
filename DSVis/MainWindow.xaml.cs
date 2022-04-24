using DSVis.Windows;
using DSVis.Windows.Pages;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DSVis {
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window {
        private int m_click = 0, m_openPage = 0, m_flagEgg = 0;
        public delegate void WindowChanged();
        WindowChanged windowchanged;
        public delegate void FileSave();
        FileSave fileSave;
        public delegate void FileOpen();
        FileOpen fileOpen;
        String pageName, mainTitle = "数据结构可视化虚拟实验系统";
        public MainWindow() {
            InitializeComponent();
            SaveFile.IsEnabled = false;
        }

        private void GraphTra_Click(object sender, RoutedEventArgs e) {
            MainWindowInfo.fileFlag = false;
            OpenGraphTraPage();
            
        }
        private void OpenGraphTraPage() {
            pageName = "图的遍历";
            cleanEgg();
            SaveFile.IsEnabled = false;
            GraphTraWindow window = new GraphTraWindow();
            MainPage.Content = new Frame() {
                Content = window,
            };
            m_openPage = 1;
            windowchanged = new WindowChanged(window.windowchanged);
            fileSave = new FileSave(window.Filesave);
            fileOpen = new FileOpen(window.Fileopen);
            window.dataConfirm = DataConfirm;
            window.dataClean = DataClean;
            this.Title = mainTitle + " - " + pageName;
        }
        private void MCST_Click(object sender, RoutedEventArgs e) {
            MainWindowInfo.fileFlag = false;
            OpenMCSTPage();
        }

        private void OpenMCSTPage() {
            pageName = "最小生成树";
            cleanEgg();
            SaveFile.IsEnabled = false;
            MCSTWindow window = new MCSTWindow();
            MainPage.Content = new Frame() {
                Content = window,
            };
            m_openPage = 1;
            windowchanged = new WindowChanged(window.windowchanged);
            fileSave = new FileSave(window.Filesave);
            fileOpen = new FileOpen(window.Fileopen);
            window.dataConfirm = DataConfirm;
            window.dataClean = DataClean;
            this.Title = mainTitle + " - " + pageName;
        }
        private void CanvasMouseDown(object sender, MouseButtonEventArgs e) {
            if (m_openPage == 0) {
                m_click++;
                int[] m_get;
                ImageBrush ib1 = new ImageBrush();
                ib1.ImageSource = new BitmapImage(new Uri("./images/cards.jpg", UriKind.Relative));
                ImageBrush ib2 = new ImageBrush();
                ib2.ImageSource = new BitmapImage(new Uri("./images/cardg.jpg", UriKind.Relative));
                ImageBrush ib3 = new ImageBrush();
                ib3.ImageSource = new BitmapImage(new Uri("./images/cardc.jpg", UriKind.Relative));
                if (m_click > 19) {
                    Random random = new Random();
                    int r, flag = 0;
                    m_get = new int[10];
                    for (int i = 0; i < 9; i++) {
                        r = random.Next(1, 1000);
                        if (r > 0 && r <= 25) {
                            m_get[i] = 3;
                            flag = 1;
                        } else if (r > 25 && r <= 180) {
                            m_get[i] = 2;
                            flag = 1;
                        } else {
                            m_get[i] = 1;
                        }
                    }
                    r = random.Next(1, 1000);
                    if (flag == 0) {
                        if (r > 0 && r <= 25) {
                            m_get[9] = 3;
                        } else {
                            m_get[9] = 2;
                        }
                    } else {
                        if (r > 0 && r <= 25) {
                            m_get[9] = 3;
                            flag = 1;
                        } else if (r > 25 && r <= 180) {
                            m_get[9] = 2;
                        } else {
                            m_get[9] = 1;
                        }
                    }
                    if (m_flagEgg == 1) {
                        cleanEgg();
                    }
                    m_flagEgg = 1;
                    for (int i = 0; i < 10; i++) {
                        Rectangle rect = new Rectangle();
                        rect.Width = 60;
                        rect.Height = 80;
                        if (m_get[i] == 1) {
                            rect.Fill = ib1;
                        } else if (m_get[i] == 2) {
                            rect.Fill = ib2;
                        } else {
                            rect.Fill = ib3;
                        }
                        rect.SetValue(Canvas.LeftProperty, (double)210 + 80 * (i % 5));
                        if (i < 5)
                            rect.SetValue(Canvas.TopProperty, (double)100);
                        else
                            rect.SetValue(Canvas.TopProperty, (double)200);
                        this.RegisterName("r" + i.ToString(), rect);
                        MainCanvas.Children.Add(rect);
                    }
                    TextBlock text = new TextBlock();
                    text.Text = "点击一次即可抽取一次十连";
                    text.SetValue(Canvas.TopProperty, (double)300);
                    text.SetValue(Canvas.LeftProperty, (double)210);
                    this.RegisterName("eggtext", text);
                    MainCanvas.Children.Add(text);
                }
            }
        }

        private void MainWindow_Resize(object sender, SizeChangedEventArgs e) {
            if (WindowState == WindowState.Maximized) {
                MainMenu.Width = SystemParameters.PrimaryScreenWidth;
                MainWindowInfo.mainPageHeight = MainCanvas.ActualHeight;
                MainWindowInfo.mainPageWidth = MainCanvas.ActualWidth;
            } else {
                MainMenu.Width = Width;
                MainWindowInfo.mainPageHeight = MainCanvas.ActualHeight;
                MainWindowInfo.mainPageWidth = MainCanvas.ActualWidth;
            }
            if (m_openPage == 1) {
                windowchanged();
            }
        }

        private void SPath_Click(object sender, RoutedEventArgs e) {
            MainWindowInfo.fileFlag = false;
            OpenSPathPage();
            
        }

        private void OpenSPathPage() {
            pageName = "最短路径";
            cleanEgg();
            SaveFile.IsEnabled = false;
            SPathWindow window = new SPathWindow();
            MainPage.Content = new Frame() {
                Content = window,
            };
            m_openPage = 1;
            windowchanged = new WindowChanged(window.windowchanged);
            fileSave = new FileSave(window.Filesave);
            fileOpen = new FileOpen(window.Fileopen);
            window.dataConfirm = DataConfirm;
            window.dataClean = DataClean;
            this.Title = mainTitle + " - " + pageName;
        }

        private void Huffman_Click(object sender, RoutedEventArgs e) {
            MainWindowInfo.fileFlag = false;
            OpenHuffmanPage();
        }

        private void OpenHuffmanPage() {
            pageName = "哈夫曼树";
            cleanEgg();
            SaveFile.IsEnabled = false;
            HuffmanWindow window = new HuffmanWindow();
            MainPage.Content = new Frame() {
                Content = window,
            };
            m_openPage = 1;
            windowchanged = new WindowChanged(window.windowchanged);
            fileSave = new FileSave(window.Filesave);
            fileOpen = new FileOpen(window.Fileopen);
            window.dataConfirm = DataConfirm;
            window.dataClean = DataClean;
            this.Title = mainTitle + " - " + pageName;
        }

        private void TreeTra_Click(object sender, RoutedEventArgs e) {
            MainWindowInfo.fileFlag = false;
            OpenTreeTraPage();
        }

        private void OpenTreeTraPage() {
            pageName = "二叉树遍历";
            cleanEgg();
            SaveFile.IsEnabled = false;
            TreeTraWindow window = new TreeTraWindow();
            MainPage.Content = new Frame() {
                Content = window,
            };
            m_openPage = 1;
            windowchanged = new WindowChanged(window.windowchanged);
            fileSave = new FileSave(window.Filesave);
            fileOpen = new FileOpen(window.Fileopen);
            window.dataConfirm = DataConfirm;
            window.dataClean = DataClean;
            this.Title = mainTitle + " - " + pageName;
        }

        private void Josephus_Click(object sender, RoutedEventArgs e) {
            MainWindowInfo.fileFlag = false;
            OpenJosephusPage();
        }

        private void OpenJosephusPage() {
            pageName = "约瑟夫问题";
            cleanEgg();
            SaveFile.IsEnabled = false;
            JosephusWindow window = new JosephusWindow();
            MainPage.Content = new Frame() {
                Content = window,
            };
            m_openPage = 1;
            windowchanged = new WindowChanged(window.windowchanged);
            fileSave = new FileSave(window.Filesave);
            fileOpen = new FileOpen(window.Fileopen);
            window.dataConfirm = DataConfirm;
            window.dataClean = DataClean;
            this.Title = mainTitle + " - " + pageName;
        }

        private void Sort_Click(object sender, RoutedEventArgs e) {
            MainWindowInfo.fileFlag = false;
            OpenSortPage();
        }

        private void OpenSortPage() {
            pageName = "排序";
            cleanEgg();
            SaveFile.IsEnabled = false;
            SortWindow window = new SortWindow();
            MainPage.Content = new Frame() {
                Content = window,
            };
            m_openPage = 1;
            windowchanged = new WindowChanged(window.windowchanged);
            fileSave = new FileSave(window.Filesave);
            fileOpen = new FileOpen(window.Fileopen);
            window.dataConfirm = DataConfirm;
            window.dataClean = DataClean;
            this.Title = mainTitle + " - " + pageName;
        }

        private void LinearBasic_Click(object sender, RoutedEventArgs e) {
            MainWindowInfo.fileFlag = false;
            OpenLinearBasicPage();
        }

        private void OpenLinearBasicPage() {
            pageName = "线性结构基本操作";
            cleanEgg();
            SaveFile.IsEnabled = false;
            LinearBasicWindow window = new LinearBasicWindow();
            MainPage.Content = new Frame() {
                Content = window,
            };
            m_openPage = 1;
            windowchanged = new WindowChanged(window.windowchanged);
            fileSave = new FileSave(window.Filesave);
            fileOpen = new FileOpen(window.Fileopen);
            window.dataConfirm = DataConfirm;
            window.dataClean = DataClean;
            this.Title = mainTitle + " - " + pageName;
        }

        private void Search_Click(object sender, RoutedEventArgs e) {
            MainWindowInfo.fileFlag = false;
            OpenSearchPage();
        }

        private void OpenSearchPage() {
            pageName = "查找";
            cleanEgg();
            SaveFile.IsEnabled = false;
            SearchWindow window = new SearchWindow();
            MainPage.Content = new Frame() {
                Content = window,
            };
            m_openPage = 1;
            windowchanged = new WindowChanged(window.windowchanged);
            fileSave = new FileSave(window.Filesave);
            fileOpen = new FileOpen(window.Fileopen);
            window.dataConfirm = DataConfirm;
            window.dataClean = DataClean;
            this.Title = mainTitle + " - " + pageName;
        }

        private void TopoSort_Click(object sender, RoutedEventArgs e) {
            MainWindowInfo.fileFlag = false;
            OpenTopoSortPage();
        }
        private void OpenTopoSortPage() {
            pageName = "拓扑排序";
            cleanEgg();
            SaveFile.IsEnabled = false;
            TopoSortWindow window = new TopoSortWindow();
            MainPage.Content = new Frame() {
                Content = window,
            };
            m_openPage = 1;
            windowchanged = new WindowChanged(window.windowchanged);
            fileSave = new FileSave(window.Filesave);
            fileOpen = new FileOpen(window.Fileopen);
            window.dataConfirm = DataConfirm;
            window.dataClean = DataClean;
            this.Title = mainTitle + " - " + pageName;
        }

        private void CriticalPath_Click(object sender, RoutedEventArgs e) {
            MainWindowInfo.fileFlag = false;
            OpenCriticalPathPage();
            
        }
        private void OpenCriticalPathPage() {
            pageName = "关键路径";
            cleanEgg();
            SaveFile.IsEnabled = false;
            CriticalPathWindows window = new CriticalPathWindows();
            MainPage.Content = new Frame() {
                Content = window,
            };
            m_openPage = 1;
            windowchanged = new WindowChanged(window.windowchanged);
            fileSave = new FileSave(window.Filesave);
            fileOpen = new FileOpen(window.Fileopen);
            window.dataConfirm = DataConfirm;
            window.dataClean = DataClean;
            this.Title = mainTitle + " - " + pageName;
        }

        private void OpenFile_Click(object sender, RoutedEventArgs e) {
            OpenFileDialog fd = new OpenFileDialog();
            fd.Filter = "(*.json)|*.json";
            fd.ValidateNames = true;
            fd.CheckFileExists = true;
            fd.CheckPathExists = true;
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                //根据json记录的信息选择子窗体并载入数据(交由子窗体进行)
                MainWindowInfo.fileLocation = fd.FileName;
                FileStream fs = new FileStream(fd.FileName, FileMode.Open);
                StreamReader sr = new StreamReader(fs);
                JObject jObject = (JObject)JsonConvert.DeserializeObject(sr.ReadToEnd());
                sr.Close();
                fs.Close();
                if (jObject == null) {
                    System.Windows.MessageBox.Show("该文件不适用于本系统");
                } else if (!jObject.ContainsKey("type")) {
                    System.Windows.MessageBox.Show("该文件不适用于本系统");
                } else {
                    MainWindowInfo.fileFlag = true;
                    SaveFile.IsEnabled = false;
                    if (jObject["type"].ToString().Equals("BST")) {
                        OpenBstPage();
                        fileOpen();
                    } else if (jObject["type"].ToString().Equals("Sort")) {
                        OpenSortPage();
                        fileOpen();
                    } else if (jObject["type"].ToString().Equals("Search")) {
                        OpenSearchPage();
                        fileOpen();
                    } else if (jObject["type"].ToString().Equals("LinearBasic")) {
                        OpenLinearBasicPage();
                        fileOpen();
                    } else if (jObject["type"].ToString().Equals("Josephus")) {
                        OpenJosephusPage();
                        fileOpen();
                    } else if (jObject["type"].ToString().Equals("TopoSort")) {
                        OpenTopoSortPage();
                        fileOpen();
                    } else if (jObject["type"].ToString().Equals("CriticalPath")) {
                        OpenCriticalPathPage();
                        fileOpen();
                    } else if (jObject["type"].ToString().Equals("TreeTra")) {
                        OpenTreeTraPage();
                        fileOpen();
                    } else if (jObject["type"].ToString().Equals("Huffman")) {
                        OpenHuffmanPage();
                        fileOpen();
                    } else if (jObject["type"].ToString().Equals("SPath")) {
                        OpenSPathPage();
                        fileOpen();
                    } else if (jObject["type"].ToString().Equals("MCST")) {
                        OpenMCSTPage();
                        fileOpen();
                    } else if (jObject["type"].ToString().Equals("GraphTra")) {
                        OpenGraphTraPage();
                        fileOpen();
                    } else if (jObject["type"].ToString().Equals("KMP")) {
                        OpenKMPPage();
                        fileOpen();
                    } else if (jObject["type"].ToString().Equals("HeapSort")) {
                        OpenHeapSortPage();
                        fileOpen();
                    } else if (jObject["type"].ToString().Equals("ExpConverse")) {
                        OpenExpConversePage();
                        fileOpen();
                    } else if (jObject["type"].ToString().Equals("NumConver")) {
                        OpenNumConverPage();
                        fileOpen();
                    } else if (jObject["type"].ToString().Equals("Labyrinth")) {
                        OpenLabyrinthPage();
                        fileOpen();
                    } else {
                        System.Windows.MessageBox.Show("该文件不适用于本系统");
                    }
                }
            }
        }

        private void SaveFile_Click(object sender, RoutedEventArgs e) {
            //点击保存按钮-向窗体类发送信息和保存位置-窗体类保存文件
            if (m_openPage == 1) {
                SaveFileDialog fd = new SaveFileDialog();
                fd.Filter = "(*.json)|*.json";
                fd.ValidateNames = true;
                fd.CheckPathExists = true;
                //默认文件名 算法类名+创建时间
                //增加 默认保存位置 软件安装文件夹？
                //对于画图类的页面来说，如果图没有确认画完不应该弹出保存窗口
                fd.FileName = pageName + "_" + DateTime.Now.ToString("_yyyy_MM_dd_HH_mm_ss") + ".json";
                if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    MainWindowInfo.fileLocation = fd.FileName;
                    fileSave();
                }
            }
        }

        private void BST_Click(object sender, RoutedEventArgs e) {
            MainWindowInfo.fileFlag = false;
            OpenBstPage();
        }

        private void KMP_Click(object sender, RoutedEventArgs e) {
            MainWindowInfo.fileFlag = false;
            OpenKMPPage();
        }
        private void OpenKMPPage() {
            pageName = "KMP算法";
            cleanEgg();
            SaveFile.IsEnabled = false;
            KMPWindow window = new KMPWindow();
            MainPage.Content = new Frame() {
                Content = window,
            };
            m_openPage = 1;
            windowchanged = new WindowChanged(window.windowchanged);
            fileSave = new FileSave(window.Filesave);
            fileOpen = new FileOpen(window.Fileopen);
            window.dataConfirm = DataConfirm;
            window.dataClean = DataClean;
            this.Title = mainTitle + " - " + pageName;
        }

        private void OpenBstPage() {
            pageName = "二叉排序平衡树";
            cleanEgg();
            SaveFile.IsEnabled = false;
            BSTWindow window = new BSTWindow();
            MainPage.Content = new Frame() {
                Content = window,
            };
            m_openPage = 1;
            windowchanged = new WindowChanged(window.windowchanged);
            fileSave = new FileSave(window.Filesave);
            fileOpen = new FileOpen(window.Fileopen);
            window.dataConfirm = DataConfirm;
            window.dataClean = DataClean;
            this.Title = mainTitle + " - " + pageName;
        }

        private void HeapSort_Click(object sender, RoutedEventArgs e) {
            MainWindowInfo.fileFlag = false;
            OpenHeapSortPage();
        }
        private void OpenHeapSortPage() {
            pageName = "堆排序";
            cleanEgg();
            SaveFile.IsEnabled = false;
            HeapSortWindow window = new HeapSortWindow();
            MainPage.Content = new Frame() {
                Content = window,
            };
            m_openPage = 1;
            windowchanged = new WindowChanged(window.windowchanged);
            fileSave = new FileSave(window.Filesave);
            fileOpen = new FileOpen(window.Fileopen);
            window.dataConfirm = DataConfirm;
            window.dataClean = DataClean;
            this.Title = mainTitle + " - " + pageName;
        }

        private void ExpConverse_Click(object sender, RoutedEventArgs e) {
            MainWindowInfo.fileFlag = false;
            OpenExpConversePage();
        }
        private void OpenExpConversePage() {
            pageName = "后缀表达式转换和计算";
            cleanEgg();
            SaveFile.IsEnabled = false;
            ExpConverseWindow window = new ExpConverseWindow();
            MainPage.Content = new Frame() {
                Content = window,
            };
            m_openPage = 1;
            windowchanged = new WindowChanged(window.windowchanged);
            fileSave = new FileSave(window.Filesave);
            fileOpen = new FileOpen(window.Fileopen);
            window.dataConfirm = DataConfirm;
            window.dataClean = DataClean;
            this.Title = mainTitle + " - " + pageName;
        }

        private void Labyrinth_Click(object sender, RoutedEventArgs e) {
            MainWindowInfo.fileFlag = false;
            OpenLabyrinthPage();
        }
        private void OpenLabyrinthPage() {
            pageName = "迷宫问题";
            cleanEgg();
            SaveFile.IsEnabled = false;
            LabyrinthWindow window = new LabyrinthWindow();
            MainPage.Content = new Frame() {
                Content = window,
            };
            m_openPage = 1;
            windowchanged = new WindowChanged(window.windowchanged);
            fileSave = new FileSave(window.Filesave);
            fileOpen = new FileOpen(window.Fileopen);
            window.dataConfirm = DataConfirm;
            window.dataClean = DataClean;
            this.Title = mainTitle + " - " + pageName;
        }
        private void NumConver_Click(object sender, RoutedEventArgs e) {
            MainWindowInfo.fileFlag = false;
            OpenNumConverPage();
        }
        private void OpenNumConverPage() {
            pageName = "进制转换";
            cleanEgg();
            SaveFile.IsEnabled = false;
            NumConverWindow window = new NumConverWindow();
            MainPage.Content = new Frame() {
                Content = window,
            };
            m_openPage = 1;
            windowchanged = new WindowChanged(window.windowchanged);
            fileSave = new FileSave(window.Filesave);
            fileOpen = new FileOpen(window.Fileopen);
            window.dataConfirm = DataConfirm;
            window.dataClean = DataClean;
            this.Title = mainTitle + " - " + pageName;
        }
        private void cleanEgg() {
            if (m_flagEgg == 1) {
                for(int i = 0; i < 10; i++) {
                    MainCanvas.Children.Remove((UIElement)this.FindName("r" + i));
                    this.UnregisterName("r" + i);
                }
                MainCanvas.Children.Remove((UIElement)this.FindName("eggtext"));
                this.UnregisterName("eggtext");
            }
        }

        public void DataConfirm() {
            SaveFile.IsEnabled = true;
        }
        public void DataClean() {
            SaveFile.IsEnabled = false;
        }
    }
}

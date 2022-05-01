using DSVis.DataStruct;
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
    /// LabyrinthWindows.xaml 的交互逻辑
    /// </summary>
    public partial class LabyrinthWindow : Page {
        int m_flagCtrl;
        Maze maze;
        int mazeX = 0, mazeY = 0, m_countDraw = 0;
        Stack<(int, int, int)> stack = new Stack<(int, int, int)>();
        (int, int)[] move = new (int, int)[4] { (0, -1), (1, 0), (0, 1), (-1, 0) };
        (int, int, int) temp;
        int x, y, d, i, j;
        public delegate void DataConfirm();
        public DataConfirm dataConfirm;
        public delegate void DataClean();
        public DataClean dataClean;
        public LabyrinthWindow() {
            InitializeComponent();
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e) {
            if (m_flagCtrl == 1) {
                btnNext.Content = "下一步";
                if (d < 4) {
                    i = x + move[d].Item1;
                    j = y + move[d].Item2;
                    if (maze[i, j] == 0) {
                        temp.Item1 = x;
                        temp.Item2 = y;
                        temp.Item3 = d;
                        stack.Push(temp);
                        x = i;
                        y = j;
                        maze[x, y] = -1;
                        if (x == maze.X + 1 && y == maze.Y) {
                            m_flagCtrl = 2;
                        } else {
                            d = 0;
                        }
                    } else {
                        d++;
                    }
                }
                if (d >= 4 && stack.Count != 0) {
                    temp = stack.Pop();
                    x = temp.Item1;
                    y = temp.Item2;
                    d = temp.Item3 + 1;
                }
                ClearStack();
                DrawStack();
                ClearMaze();
                DrawMaze();
            } else if (m_flagCtrl == 2) {
                btnNext.Content = "重新演示"; 
                ClearStack();
                ClearMaze();
                stack.Clear();
                m_countDraw = 0;
                temp.Item1 = 1;
                temp.Item2 = 1;
                temp.Item3 = -1;
                stack.Push(temp);
                temp = stack.Pop();
                x = temp.Item1;
                y = temp.Item2;
                d = temp.Item3 + 1;
                for (int i = 0; i < mazeX; i++) {
                    for (int j = 0; j < mazeY; j++) {
                        if (maze[i,j] == -1) {
                            maze[i, j] = 0;
                        }
                    }
                }
                maze[0, 1] = -1;
                maze[1, 1] = -1;
                DrawStack();
                DrawMaze();
                m_flagCtrl = 1;
            }
        }

        private void btnSet_Click(object sender, RoutedEventArgs e) {
            ClearMaze();
            ClearStack();
            stack.Clear();
            maze = new Maze();
            dataConfirm();
            mazeX = maze.X + 2;
            mazeY = maze.Y + 2;
            m_countDraw = 0;
            temp.Item1 = 1;
            temp.Item2 = 1;
            temp.Item3 = -1;
            stack.Push(temp);
            temp = stack.Pop();
            x = temp.Item1;
            y = temp.Item2;
            d = temp.Item3 + 1;
            btnNext.IsEnabled = true;
            m_flagCtrl = 1;
            DrawMaze();
        }
        public void ClearMaze() {
            for (int i = 0; i < mazeX; i++) {
                for (int j = 0; j < mazeY; j++) {
                    MainMaze.Children.Remove((UIElement)this.FindName("v" + i + "_" + j));
                    this.UnregisterName("v" + i + "_" + j);
                }
            }
        }
        public void DrawMaze() {
            for (int i = 0; i < maze.X + 2; i++) {
                for (int j = 0; j < maze.Y + 2; j++) {
                    double startX = MainMaze.ActualWidth / 2 - (maze.X + 2) * 10;
                    double startY = MainMaze.ActualHeight / 2 - (maze.Y + 2) * 10;
                    Rectangle rect = new Rectangle();
                    rect.Width = 20;
                    rect.Height = 20;
                    if (maze[i, j] == 1) {
                        rect.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    } else if (maze[i, j] == 0) {
                        rect.Fill = new SolidColorBrush(Colors.White);
                    } else {
                        rect.Fill = new SolidColorBrush(Colors.DarkSeaGreen);
                    }
                    if (i == 0 && j == 1 || i == 1 && j == 1) {
                        rect.Fill = new SolidColorBrush(Colors.DarkSeaGreen);
                    }
                    if (i == x && j == y) {
                        rect.Fill = new SolidColorBrush(Colors.IndianRed);
                    }
                    if (d < 4 && i == x + move[d].Item1 && j == y + move[d].Item2) {
                        if (maze[i, j] == 1) {
                            rect.Fill = new SolidColorBrush(Colors.LightBlue);
                        } else if (maze[i, j] == 0) {
                            rect.Fill = new SolidColorBrush(Colors.LightGray);
                        } else {
                            rect.Fill = new SolidColorBrush(Colors.LightGreen);
                        }
                    }
                    rect.SetValue(Canvas.LeftProperty, 20 * i + startX);
                    rect.SetValue(Canvas.TopProperty, 20 * j + startY);
                    rect.Name = "v" + i + "_" + j;
                    this.RegisterName("v" + i + "_" + j, rect);
                    MainMaze.Children.Add(rect);
                }
            }
        }
        public void DrawStack() {
            int maxStack = (int)MainStack.ActualHeight / 20;
            for (int i = stack.Count - 1; i >= 0; i--) {
                Point point = MainStack.TranslatePoint(new Point(), MainStack);
                Rectangle rect = new Rectangle();
                rect.Width = 50;
                rect.Height = 20;
                rect.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                rect.Name = "r" + i;
                this.RegisterName("r" + i, rect);

                TextBlock text = new TextBlock();
                (int, int, int) element = stack.ElementAt(stack.Count - i - 1);
                text.Text = "(" + element.Item1 + "," + element.Item2 + "),";
                if(element.Item3 == 0) {
                    text.Text += "上";
                }else if (element.Item3 == 1) {
                    text.Text += "右";
                } else if (element.Item3 == 2) {
                    text.Text += "下";
                } else {
                    text.Text += "左";
                }
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size sizeText = text.DesiredSize;

                text.Name = "t" + i;
                this.RegisterName("t" + i, text);

                if (stack.Count >= 2 * maxStack) {
                    if (i >= 2 * maxStack) {
                        rect.SetValue(Canvas.LeftProperty, point.X + 100);
                        rect.SetValue(Canvas.TopProperty, point.Y + MainStack.ActualHeight - rect.Height * (i + 1 - 2 * maxStack));
                        text.SetValue(Canvas.LeftProperty, point.X + 125 - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, point.Y + MainStack.ActualHeight - rect.Height * (i + 1 - 2 * maxStack) + rect.Height / 2 - sizeText.Height / 2);
                    } else if (i >= maxStack) {
                        rect.SetValue(Canvas.LeftProperty, point.X + 50);
                        rect.SetValue(Canvas.TopProperty, point.Y + MainStack.ActualHeight - rect.Height * (i + 1 - maxStack));
                        text.SetValue(Canvas.LeftProperty, point.X + 75 - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, point.Y + MainStack.ActualHeight - rect.Height * (i + 1 - maxStack) + rect.Height / 2 - sizeText.Height / 2);
                    } else {
                        rect.SetValue(Canvas.LeftProperty, point.X);
                        rect.SetValue(Canvas.TopProperty, point.Y + MainStack.ActualHeight - rect.Height * (i + 1));
                        text.SetValue(Canvas.LeftProperty, point.X + 25 - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, point.Y + MainStack.ActualHeight - rect.Height * (i + 1) + rect.Height / 2 - sizeText.Height / 2);
                    }
                } else if (stack.Count >= maxStack) {
                    if (i >= maxStack) {
                        rect.SetValue(Canvas.LeftProperty, point.X + 50);
                        rect.SetValue(Canvas.TopProperty, point.Y + MainStack.ActualHeight - rect.Height * (i + 1 - maxStack));
                        text.SetValue(Canvas.LeftProperty, point.X + 75 - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, point.Y + MainStack.ActualHeight - rect.Height * (i + 1 - maxStack) + rect.Height / 2 - sizeText.Height / 2);
                    } else {
                        rect.SetValue(Canvas.LeftProperty, point.X);
                        rect.SetValue(Canvas.TopProperty, point.Y + MainStack.ActualHeight - rect.Height * (i + 1));
                        text.SetValue(Canvas.LeftProperty, point.X + 25 - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, point.Y + MainStack.ActualHeight - rect.Height * (i + 1) + rect.Height / 2 - sizeText.Height / 2);
                    }
                } else {
                    rect.SetValue(Canvas.LeftProperty, point.X);
                    rect.SetValue(Canvas.TopProperty, point.Y + MainStack.ActualHeight - rect.Height * (i + 1));
                    text.SetValue(Canvas.LeftProperty, point.X + 25 - sizeText.Width / 2);
                    text.SetValue(Canvas.TopProperty, point.Y + MainStack.ActualHeight - rect.Height * (i + 1) + rect.Height / 2 - sizeText.Height / 2);
                }

                MainStack.Children.Add(rect);
                MainStack.Children.Add(text);
            }
            m_countDraw = stack.Count;
        }
        public void ClearStack() {
            for (int i = 0; i < m_countDraw; i++) {
                MainStack.Children.Remove((UIElement)this.FindName("r" + i));
                this.UnregisterName("r" + i);
                MainStack.Children.Remove((UIElement)this.FindName("t" + i));
                this.UnregisterName("t" + i);
            }
        }
        public void windowchanged() {
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
            CtrlBg.Height = this.Height;
            MainStack.Height = this.Height - 40;
            MainStack.Width = 150;
            MainMaze.Height = this.Height;
            MainMaze.Width = this.Height;
            Point point = MainStack.TranslatePoint(new Point(), MainStack);
            int maxStack = (int)MainStack.Height / 20;
            for (int i = 0; i < m_countDraw; i++) {
                Rectangle rect = this.FindName("r" + i) as Rectangle;
                TextBlock text = this.FindName("t" + i) as TextBlock;
                Size sizeText = text.DesiredSize;
                if (stack.Count >= 2 * maxStack) {
                    if (i >= 2 * maxStack) {
                        rect.SetValue(Canvas.LeftProperty, point.X + 100);
                        rect.SetValue(Canvas.TopProperty, point.Y + MainStack.Height - rect.Height * (i + 1 - 2 * maxStack));
                        text.SetValue(Canvas.LeftProperty, point.X + 125 - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, point.Y + MainStack.Height - rect.Height * (i + 1 - 2 * maxStack) + rect.Height / 2 - sizeText.Height / 2);
                    } else if (i >= maxStack) {
                        rect.SetValue(Canvas.LeftProperty, point.X + 50);
                        rect.SetValue(Canvas.TopProperty, point.Y + MainStack.Height - rect.Height * (i + 1 - maxStack));
                        text.SetValue(Canvas.LeftProperty, point.X + 75 - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, point.Y + MainStack.Height - rect.Height * (i + 1 - maxStack) + rect.Height / 2 - sizeText.Height / 2);
                    } else {
                        rect.SetValue(Canvas.LeftProperty, point.X);
                        rect.SetValue(Canvas.TopProperty, point.Y + MainStack.Height - rect.Height * (i + 1));
                        text.SetValue(Canvas.LeftProperty, point.X + 25 - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, point.Y + MainStack.Height - rect.Height * (i + 1) + rect.Height / 2 - sizeText.Height / 2);
                    }
                } else if (stack.Count >= maxStack) {
                    if (i >= maxStack) {
                        rect.SetValue(Canvas.LeftProperty, point.X + 50);
                        rect.SetValue(Canvas.TopProperty, point.Y + MainStack.Height - rect.Height * (i + 1 - maxStack));
                        text.SetValue(Canvas.LeftProperty, point.X + 75 - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, point.Y + MainStack.Height - rect.Height * (i + 1 - maxStack) + rect.Height / 2 - sizeText.Height / 2);
                    } else {
                        rect.SetValue(Canvas.LeftProperty, point.X);
                        rect.SetValue(Canvas.TopProperty, point.Y + MainStack.Height - rect.Height * (i + 1));
                        text.SetValue(Canvas.LeftProperty, point.X + 25 - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, point.Y + MainStack.Height - rect.Height * (i + 1) + rect.Height / 2 - sizeText.Height / 2);
                    }
                } else {
                    rect.SetValue(Canvas.LeftProperty, point.X);
                    rect.SetValue(Canvas.TopProperty, point.Y + MainStack.Height - rect.Height * (i + 1));
                    text.SetValue(Canvas.LeftProperty, point.X + 25 - sizeText.Width / 2);
                    text.SetValue(Canvas.TopProperty, point.Y + MainStack.Height - rect.Height * (i + 1) + rect.Height / 2 - sizeText.Height / 2);
                }
            }
            if (maze != null) {
                for (int i = 0; i < maze.X + 2; i++) {
                    for (int j = 0; j < maze.Y + 2; j++) {
                        double startX = MainMaze.Width / 2 - (maze.X + 2) * 10;
                        double startY = MainMaze.Height / 2 - (maze.Y + 2) * 10;
                        Rectangle rect = this.FindName("v" + i + "_" + j) as Rectangle;
                        rect.SetValue(Canvas.LeftProperty, 20 * i + startX);
                        rect.SetValue(Canvas.TopProperty, 20 * j + startY);
                    }
                }
            }
        }

        public void Filesave() {
            FileStream fs = new FileStream(MainWindowInfo.fileLocation, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            JObject jObject = new JObject();
            jObject["type"] = "Labyrinth";
            JArray jArray = new JArray();
            for (int i = 0; i < maze.X + 2; i++) {
                JArray array = new JArray();
                for (int j = 0; j < maze.Y + 2; j++) {
                    if (i == 0 && j == 1 || i == 1 && j == 1) {
                        array.Add(-1);
                    } else {
                        if (maze[i, j] == -1) {
                            array.Add(0);
                        } else {
                            array.Add(maze[i, j]);
                        }
                    }
                }
                jArray.Add(array);
            }
            jObject["maze"] = jArray;
            jObject["x"] = maze.X;
            jObject["y"] = maze.Y;
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
            maze = new Maze(jObject["maze"].ToObject<int[,]>(), (int)jObject["x"], (int)jObject["y"]);
            dataConfirm();
            mazeX = maze.X + 2;
            mazeY = maze.Y + 2;
            m_countDraw = 0;
            temp.Item1 = 1;
            temp.Item2 = 1;
            temp.Item3 = -1;
            stack.Push(temp);
            temp = stack.Pop();
            x = temp.Item1;
            y = temp.Item2;
            btnNext.IsEnabled = true;
            m_flagCtrl = 1;
            DrawMaze();
            sr.Close();
            fs.Close();
        }
    }
}

using DSVis.DataStruct;
using DSVis.Tools;
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
    /// LinearBasicWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LinearBasicWindow : Page {
        List<int> array = new List<int>();
        int flagLastCh, flagLastOp;
        bool CanvasClear = true;
        int OperValue, OperPosi;
        int flagNext;
        double startx, starty;
        public delegate void DataConfirm();
        public DataConfirm dataConfirm;
        public delegate void DataClean();
        public DataClean dataClean;
        public LinearBasicWindow() {
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
            flagLastCh = 0;
            flagLastOp = 0;
            flagNext = 0;
        }

        private void btnSet_Click(object sender, RoutedEventArgs e) {
            SetArrayForm form = new SetArrayForm();
            form.Show();
            form.sendMessage = Recevie;
            form.windowClosed = FormClosed;
            flagNext = 0;
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
            btnPush.IsEnabled = true;
            btnPop.IsEnabled = true;
            btnSet.IsEnabled = true;
            radiobtnSList.IsEnabled = true;
            radiobtnDList.IsEnabled = true;
            radiobtnStack.IsEnabled = true;
            radiobtnQueue.IsEnabled = true;
            Draw();
        }
        public void FormClosed() {
            this.IsEnabled = true;
        }

        public void PushRecevie(int value) {
            Clear();
            OperValue = value;
            array.Insert(0, OperValue);
            Draw();
        }

        public void InsertRecevie(int value, int posi,int mode) {
            int space = 35;

            Clear();
            Draw();

            btnPush.IsEnabled = false;
            btnPop.IsEnabled = false;

            flagNext = 0;
            OperValue = value;
            OperPosi = posi;

            if (mode == 1 && OperPosi <= array.Count) {
                btnNext.IsEnabled = true;

                Rectangle rect = new Rectangle();
                rect.Width = 30;
                rect.Height = 30;
                rect.Fill = new SolidColorBrush(Colors.IndianRed);
                TextBlock text = new TextBlock();
                text.Text = OperValue.ToString();
                text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Size sizeText = text.DesiredSize;

                if (radiobtnSList.IsChecked == true) {
                    Arrow arrow = this.FindName("a" + OperPosi) as Arrow;
                    if ((OperPosi - 1) % 5 != 0 || OperPosi == 1) {
                        rect.SetValue(Canvas.LeftProperty, (arrow.X1 + arrow.X2) / 2 - rect.Width / 2);
                        rect.SetValue(Canvas.TopProperty, (arrow.Y1 + arrow.Y1) / 2 - rect.Height / 2 - space);
                        text.SetValue(Canvas.LeftProperty, (arrow.X1 + arrow.X2) / 2 - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, (arrow.Y1 + arrow.Y1) / 2 - sizeText.Height / 2 - space);
                    } else if ((OperPosi - 1) % 10 != 0) {
                        rect.SetValue(Canvas.LeftProperty, (arrow.X1 + arrow.X1) / 2 - rect.Width / 2 + space);
                        rect.SetValue(Canvas.TopProperty, (arrow.Y1 + arrow.Y2) / 2 - rect.Height / 2);
                        text.SetValue(Canvas.LeftProperty, (arrow.X1 + arrow.X1) / 2 - sizeText.Width / 2 + space);
                        text.SetValue(Canvas.TopProperty, (arrow.Y1 + arrow.Y2) / 2 - sizeText.Height / 2);
                    } else {
                        rect.SetValue(Canvas.LeftProperty, (arrow.X1 + arrow.X1) / 2 - rect.Width / 2 - space);
                        rect.SetValue(Canvas.TopProperty, (arrow.Y1 + arrow.Y2) / 2 - rect.Height / 2);
                        text.SetValue(Canvas.LeftProperty, (arrow.X1 + arrow.X1) / 2 - sizeText.Width / 2 - space);
                        text.SetValue(Canvas.TopProperty, (arrow.Y1 + arrow.Y2) / 2 - sizeText.Height / 2);
                    }
                } else if (radiobtnDList.IsChecked == true) {
                    Arrow arrowa = this.FindName("a" + OperPosi) as Arrow;
                    Arrow arrowb = this.FindName("b" + OperPosi) as Arrow;
                    if ((OperPosi - 1) % 5 != 0 || OperPosi == 1) {
                        rect.SetValue(Canvas.LeftProperty, (arrowa.X1 + arrowa.X2) / 2 - rect.Width / 2);
                        rect.SetValue(Canvas.TopProperty, (arrowa.Y1 + arrowb.Y1) / 2 - rect.Height / 2 - space);
                        text.SetValue(Canvas.LeftProperty, (arrowa.X1 + arrowa.X2) / 2 - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, (arrowa.Y1 + arrowb.Y1) / 2 - sizeText.Height / 2 - space);
                    } else if ((OperPosi - 1) % 10 != 0) {
                        rect.SetValue(Canvas.LeftProperty, (arrowa.X1 + arrowb.X1) / 2 - rect.Width / 2 + space);
                        rect.SetValue(Canvas.TopProperty, (arrowa.Y1 + arrowa.Y2) / 2 - rect.Height / 2);
                        text.SetValue(Canvas.LeftProperty, (arrowa.X1 + arrowb.X1) / 2 - sizeText.Width / 2 + space);
                        text.SetValue(Canvas.TopProperty, (arrowa.Y1 + arrowa.Y2) / 2 - sizeText.Height / 2);
                    } else {
                        rect.SetValue(Canvas.LeftProperty, (arrowa.X1 + arrowb.X1) / 2 - rect.Width / 2 - space);
                        rect.SetValue(Canvas.TopProperty, (arrowa.Y1 + arrowa.Y2) / 2 - rect.Height / 2);
                        text.SetValue(Canvas.LeftProperty, (arrowa.X1 + arrowb.X1) / 2 - sizeText.Width / 2 - space);
                        text.SetValue(Canvas.TopProperty, (arrowa.Y1 + arrowa.Y2) / 2 - sizeText.Height / 2);
                    }
                }

                rect.Name = "vins";
                this.RegisterName("vins", rect);
                text.Name = "tins";
                this.RegisterName("tins", text);

                MainCanvas.Children.Add(rect);
                MainCanvas.Children.Add(text);
            } else {
                btnNext.IsEnabled = false;
                Clear();
                if (OperPosi > array.Count) {
                    array.Insert(array.Count, OperValue);
                } else {
                    array.Insert(OperPosi - 1, OperValue);
                }
                Draw();
                if (OperPosi > array.Count) {
                    Rectangle rect = this.FindName("v" + array.Count) as Rectangle;
                    rect.Fill = new SolidColorBrush(Colors.IndianRed);
                } else {
                    Rectangle rect = this.FindName("v" + OperPosi) as Rectangle;
                    rect.Fill = new SolidColorBrush(Colors.IndianRed);
                }
                btnPop.IsEnabled = true;
                btnPush.IsEnabled = true;
            }
        }

        private void btnPush_Click(object sender, RoutedEventArgs e) {
            flagLastOp = 1;
            if (radiobtnSList.IsChecked == true) {
                this.IsEnabled = false;
                SetInsertForm form = new SetInsertForm();
                form.Show();
                form.sendMessage = InsertRecevie;
                form.windowClosed = FormClosed;
                radiobtnDList.IsEnabled = false;
                radiobtnStack.IsEnabled = false;
                radiobtnQueue.IsEnabled = false;
            } else if (radiobtnDList.IsChecked == true) {
                this.IsEnabled = false;
                SetInsertForm form = new SetInsertForm();
                form.Show();
                form.sendMessage = InsertRecevie;
                form.windowClosed = FormClosed;
                radiobtnSList.IsEnabled = false;
                radiobtnStack.IsEnabled = false;
                radiobtnQueue.IsEnabled = false;
            } else if (radiobtnStack.IsChecked == true) {
                this.IsEnabled = false;
                SetPushForm form = new SetPushForm();
                form.Show();
                form.sendMessage = PushRecevie;
                form.windowClosed = FormClosed;
                radiobtnSList.IsEnabled = false;
                radiobtnDList.IsEnabled = false;
                radiobtnQueue.IsEnabled = false;
            } else if (radiobtnQueue.IsChecked == true) {
                this.IsEnabled = false;
                SetPushForm form = new SetPushForm();
                form.Show();
                form.sendMessage = PushRecevie;
                form.windowClosed = FormClosed;
                radiobtnSList.IsEnabled = false;
                radiobtnDList.IsEnabled = false;
                radiobtnStack.IsEnabled = false;
            } else {
                MessageBox.Show("请选择类型");
            }
        }

        public void DeleteRecevie(int value,int mode) {
            Clear();
            Draw();

            flagNext = 0;
            OperPosi = value;

            btnPush.IsEnabled = false;
            btnPop.IsEnabled = false;

            if (mode == 0 || OperPosi > array.Count) {
                Clear();
                if (OperPosi > array.Count) {
                    array.RemoveAt(array.Count - 1);
                } else {
                    array.RemoveAt(OperPosi - 1);
                }
                Draw();
                btnPop.IsEnabled = true;
                btnPush.IsEnabled = true;
            } else {
                btnNext.IsEnabled = true;
                if (OperPosi > array.Count) {
                    Rectangle rect = this.FindName("v" + array.Count) as Rectangle;
                    rect.Fill = new SolidColorBrush(Colors.IndianRed);
                } else {
                    Rectangle rect = this.FindName("v" + OperPosi) as Rectangle;
                    rect.Fill = new SolidColorBrush(Colors.IndianRed);
                }
            }
        }

        private void btnPop_Click(object sender, RoutedEventArgs e) {
            flagLastOp = 2;
            if (radiobtnSList.IsChecked == true) {
                this.IsEnabled = false;
                SetDeleteForm form = new SetDeleteForm();
                form.Show();
                form.sendMessage = DeleteRecevie;
                form.windowClosed = FormClosed;
                radiobtnDList.IsEnabled = false;
                radiobtnStack.IsEnabled = false;
                radiobtnQueue.IsEnabled = false;
            } else if (radiobtnDList.IsChecked == true) {
                this.IsEnabled = false;
                SetDeleteForm form = new SetDeleteForm();
                form.Show();
                form.sendMessage = DeleteRecevie;
                form.windowClosed = FormClosed;
                radiobtnSList.IsEnabled = false;
                radiobtnStack.IsEnabled = false;
                radiobtnQueue.IsEnabled = false;
            } else if (radiobtnStack.IsChecked == true) {
                if (array.Count != 0) {
                    radiobtnSList.IsEnabled = false;
                    radiobtnDList.IsEnabled = false;
                    radiobtnQueue.IsEnabled = false;
                    Clear(); 
                    array.RemoveAt(0);
                    Draw();
                } else {
                    MessageBox.Show("栈已空");
                }
            } else if (radiobtnQueue.IsChecked == true) {
                if (array.Count != 0) {
                    radiobtnSList.IsEnabled = false;
                    radiobtnDList.IsEnabled = false;
                    radiobtnStack.IsEnabled = false;
                    Clear();
                    array.RemoveAt(array.Count - 1);
                    Draw();
                } else {
                    MessageBox.Show("队列已空");
                }
            } else {
                MessageBox.Show("请选择类型");
            }
        }

        private void btnNext_Click(object sender, RoutedEventArgs e) {
            if (flagLastOp == 1) {
                if (radiobtnSList.IsChecked == true) {
                    if (flagNext == 0) {
                        flagNext++;
                        Arrow arrowp = this.FindName("a" + OperPosi) as Arrow;
                        Arrow arrow = new Arrow();
                        arrow.HeadHeight = 5;
                        arrow.HeadWidth = 10;
                        arrow.Stroke = new SolidColorBrush(Colors.Black);
                        arrow.StrokeThickness = 1;

                        if ((OperPosi - 1) % 10 < 5 && (OperPosi - 1) % 10 > 0 || OperPosi == 1) {
                            arrow.X1 = (arrowp.X1 + arrowp.X2) / 2 + 15;
                            arrow.X2 = arrowp.X2;
                            arrow.Y1 = arrowp.Y1 - 35;
                            arrow.Y2 = arrowp.Y2;
                        } else if ((OperPosi - 1) % 10 > 5) {
                            arrow.X1 = (arrowp.X1 + arrowp.X2) / 2 - 15;
                            arrow.X2 = arrowp.X2;
                            arrow.Y1 = arrowp.Y1 - 35;
                            arrow.Y2 = arrowp.Y2;
                        } else if ((OperPosi - 1) % 10 != 0) {
                            arrow.X1 = arrowp.X1 + 35;
                            arrow.X2 = arrowp.X2;
                            arrow.Y1 = (arrowp.Y1 + arrowp.Y2) / 2 + 15;
                            arrow.Y2 = arrowp.Y2;
                        } else {
                            arrow.X1 = arrowp.X1 - 35;
                            arrow.X2 = arrowp.X2;
                            arrow.Y1 = (arrowp.Y1 + arrowp.Y2) / 2 + 15;
                            arrow.Y2 = arrowp.Y2;
                        }
                        arrow.Name = "aa";
                        this.RegisterName("aa", arrow);
                        MainCanvas.Children.Add(arrow);
                    } else if (flagNext == 1) {
                        flagNext++;
                        Arrow arrowp = this.FindName("a" + OperPosi) as Arrow;

                        if ((OperPosi - 1) % 10 < 5 && (OperPosi - 1) % 10 > 0 || OperPosi == 1) {
                            arrowp.X2 = (arrowp.X1 + arrowp.X2) / 2 - 15;
                            arrowp.Y2 = arrowp.Y1 - 35;
                        } else if ((OperPosi - 1) % 10 > 5) {
                            arrowp.X2 = (arrowp.X1 + arrowp.X2) / 2 + 15;
                            arrowp.Y2 = arrowp.Y1 - 35;
                        } else if ((OperPosi - 1) % 10 != 0) {
                            arrowp.X2 = arrowp.X1 + 35;
                            arrowp.Y2 = (arrowp.Y1 + arrowp.Y2) / 2 - 15;
                        } else {
                            arrowp.X2 = arrowp.X1 - 35;
                            arrowp.Y2 = (arrowp.Y1 + arrowp.Y2) / 2 - 15;
                        }
                    } else if (flagNext == 2) {
                        flagNext++;
                        MainCanvas.Children.Remove((UIElement)this.FindName("vins"));
                        this.UnregisterName("vins");
                        MainCanvas.Children.Remove((UIElement)this.FindName("tins"));
                        this.UnregisterName("tins");
                        MainCanvas.Children.Remove((UIElement)this.FindName("aa"));
                        this.UnregisterName("aa");
                        Clear();
                        array.Insert(OperPosi - 1, OperValue);
                        Draw();
                        Rectangle rect = this.FindName("v" + OperPosi) as Rectangle;
                        rect.Fill = new SolidColorBrush(Colors.IndianRed);
                        btnPop.IsEnabled = true;
                        btnPush.IsEnabled = true;
                        btnNext.IsEnabled = false;
                    }
                } else if (radiobtnDList.IsChecked == true) {
                    if (flagNext == 0) {
                        flagNext++;
                        Arrow arrowp = this.FindName("a" + OperPosi) as Arrow;
                        Arrow arrow = new Arrow();
                        arrow.HeadHeight = 5;
                        arrow.HeadWidth = 10;
                        arrow.Stroke = new SolidColorBrush(Colors.Black);
                        arrow.StrokeThickness = 1;

                        if ((OperPosi - 1) % 10 < 5 && (OperPosi - 1) % 10 > 0 || OperPosi == 1) {
                            arrow.X1 = (arrowp.X1 + arrowp.X2) / 2 + 15;
                            arrow.X2 = arrowp.X2 + 25;
                            arrow.Y1 = arrowp.Y1 - 35;
                            arrow.Y2 = arrowp.Y2 - 5;
                        } else if ((OperPosi - 1) % 10 > 5) {
                            arrow.X1 = (arrowp.X1 + arrowp.X2) / 2 - 15;
                            arrow.X2 = arrowp.X2 - 5;
                            arrow.Y1 = arrowp.Y1 - 35;
                            arrow.Y2 = arrowp.Y2 - 25;
                        } else if ((OperPosi - 1) % 10 != 0) {
                            arrow.X1 = arrowp.X1 + 35;
                            arrow.X2 = arrowp.X2 + 5;
                            arrow.Y1 = (arrowp.Y1 + arrowp.Y2) / 2 + 15;
                            arrow.Y2 = arrowp.Y2 + 25;
                        } else {
                            arrow.X1 = arrowp.X1 - 35;
                            arrow.X2 = arrowp.X2 - 25;
                            arrow.Y1 = (arrowp.Y1 + arrowp.Y2) / 2 + 15;
                            arrow.Y2 = arrowp.Y2 + 5;
                        }

                        arrow.Name = "aa";
                        this.RegisterName("aa", arrow);
                        MainCanvas.Children.Add(arrow);
                    } else if (flagNext == 1) {
                        flagNext++;
                        Arrow arrowp = this.FindName("b" + OperPosi) as Arrow;

                        double x1 = arrowp.X1;
                        double x2 = arrowp.X2;
                        double y1 = arrowp.Y1;
                        double y2 = arrowp.Y2;

                        if ((OperPosi - 1) % 10 < 5 && (OperPosi - 1) % 10 > 0 || OperPosi == 1) {
                            arrowp.X1 = x1 + 5;
                            arrowp.Y1 = y1 - 25;
                            arrowp.X2 = (x1 + x2) / 2 + 15;
                            arrowp.Y2 = y1 - 35;
                        } else if ((OperPosi - 1) % 10 > 5) {
                            arrowp.X1 = x1 - 25;
                            arrowp.Y1 = y1 - 5;
                            arrowp.X2 = (x1 + x2) / 2 - 15;
                            arrowp.Y2 = y1 - 35;
                        } else if ((OperPosi - 1) % 10 != 0) {
                            arrowp.X1 = x1 + 25;
                            arrowp.Y1 = y1 + 5;
                            arrowp.X2 = x1 + 35;
                            arrowp.Y2 = (y1 + y2) / 2 + 15;
                        } else {
                            arrowp.X1 = x1 - 5;
                            arrowp.Y1 = y1 + 25;
                            arrowp.X2 = x1 - 35;
                            arrowp.Y2 = (y1 + y2) / 2 + 15;
                        }
                    } else if (flagNext == 2) {
                        flagNext++;
                        Arrow arrowp = this.FindName("a" + OperPosi) as Arrow;

                        double x1 = arrowp.X1;
                        double x2 = arrowp.X2;
                        double y1 = arrowp.Y1;
                        double y2 = arrowp.Y2;

                        if ((OperPosi - 1) % 10 < 5 && (OperPosi - 1) % 10 > 0 || OperPosi == 1) {
                            arrowp.X1 = x1 - 25;
                            arrowp.Y1 = y1 - 5;
                            arrowp.X2 = (x1 + x2) / 2 - 15;
                            arrowp.Y2 = y1 - 35;
                        } else if ((OperPosi - 1) % 10 > 5) {
                            arrowp.X1 = x1 + 5;
                            arrowp.Y1 = y1 - 25;
                            arrowp.X2 = (x1 + x2) / 2 + 15;
                            arrowp.Y2 = y1 - 35;
                        } else if ((OperPosi - 1) % 10 != 0) {
                            arrowp.X1 = x1 + 5;
                            arrowp.Y1 = y1 - 25;
                            arrowp.X2 = x1 + 35;
                            arrowp.Y2 = (y1 + y2) / 2 - 15;
                        } else {
                            arrowp.X1 = x1 - 25;
                            arrowp.Y1 = y1 - 5;
                            arrowp.X2 = x1 - 35;
                            arrowp.Y2 = (y1 + y2) / 2 - 15;
                        }
                    } else if (flagNext == 3) {
                        flagNext++;
                        Arrow arrowp = this.FindName("a" + OperPosi) as Arrow;
                        Arrow arrow = new Arrow();
                        arrow.HeadHeight = 5;
                        arrow.HeadWidth = 10;
                        arrow.Stroke = new SolidColorBrush(Colors.Black);
                        arrow.StrokeThickness = 1;

                        if ((OperPosi - 1) % 10 < 5 && (OperPosi - 1) % 10 > 0 || OperPosi == 1) {
                            arrow.X1 = arrowp.X2;
                            arrow.X2 = arrowp.X1 + 20;
                            arrow.Y1 = arrowp.Y2 + 20;
                            arrow.Y2 = arrowp.Y1;
                        } else if ((OperPosi - 1) % 10 > 5) {
                            arrow.X1 = arrowp.X2;
                            arrow.X2 = arrowp.X1 + 20;
                            arrow.Y1 = arrowp.Y2 - 20;
                            arrow.Y2 = arrowp.Y1;
                        } else if ((OperPosi - 1) % 10 != 0) {
                            arrow.X1 = arrowp.X2 - 20;
                            arrow.X2 = arrowp.X1;
                            arrow.Y1 = arrowp.Y2;
                            arrow.Y2 = arrowp.Y1 + 20;
                        } else {
                            arrow.X1 = arrowp.X2 - 20;
                            arrow.X2 = arrowp.X1;
                            arrow.Y1 = arrowp.Y2;
                            arrow.Y2 = arrowp.Y1 - 20;
                        }

                        arrow.Name = "ab";
                        this.RegisterName("ab", arrow);
                        MainCanvas.Children.Add(arrow);
                    } else if (flagNext == 4) {
                        flagNext++;
                        MainCanvas.Children.Remove((UIElement)this.FindName("vins"));
                        this.UnregisterName("vins");
                        MainCanvas.Children.Remove((UIElement)this.FindName("tins"));
                        this.UnregisterName("tins");
                        MainCanvas.Children.Remove((UIElement)this.FindName("aa"));
                        this.UnregisterName("aa");
                        MainCanvas.Children.Remove((UIElement)this.FindName("ab"));
                        this.UnregisterName("ab");
                        Clear();
                        array.Insert(OperPosi - 1, OperValue);
                        Draw();
                        btnPop.IsEnabled = true;
                        btnPush.IsEnabled = true;
                        btnNext.IsEnabled = false;
                    }
                }
            }else if (flagLastOp == 2) {
                if (radiobtnSList.IsChecked == true) {
                    if (flagNext == 0) {
                        flagNext++;
                        Arrow arrow = this.FindName("a" + OperPosi) as Arrow;
                        Arrow arrown = this.FindName("a" + (OperPosi + 1)) as Arrow;
                        double x1 = arrow.X1;
                        double x2 = arrow.X2;
                        double y1 = arrow.Y1;
                        double y2 = arrow.Y2;
                        if ((OperPosi % 5 != 0 && OperPosi % 5 != 1) || OperPosi == 1) {
                            if ((OperPosi - 1) % 10 < 5) {
                                arrow.X1 = arrown.X2 + 15;
                                arrow.X2 = arrown.X2 + 15;
                                arrow.Y1 = arrown.Y1 - 35;
                                arrow.Y2 = arrown.Y1 - 15;

                                Line line1 = new Line();
                                Line line2 = new Line();
                                line1.Stroke = new SolidColorBrush(Colors.Black);
                                line1.StrokeThickness = 1;
                                line2.Stroke = new SolidColorBrush(Colors.Black);
                                line2.StrokeThickness = 1;

                                line1.X1 = x1 - 15;
                                line1.X2 = x1 - 15;
                                line1.Y1 = y1 - 15;
                                line1.Y2 = y1 - 35;
                                line2.X1 = line1.X2;
                                line2.X2 = arrow.X1;
                                line2.Y1 = line1.Y2;
                                line2.Y2 = arrow.Y1;

                                line1.Name = "la";
                                this.RegisterName("la", line1);
                                line2.Name = "lb";
                                this.RegisterName("lb", line2);

                                MainCanvas.Children.Add(line1);
                                MainCanvas.Children.Add(line2);
                            } else {
                                arrow.X1 = arrown.X2 - 15;
                                arrow.X2 = arrown.X2 - 15;
                                arrow.Y1 = arrown.Y1 - 35;
                                arrow.Y2 = arrown.Y1 - 15;

                                Line line1 = new Line();
                                Line line2 = new Line();
                                line1.Stroke = new SolidColorBrush(Colors.Black);
                                line1.StrokeThickness = 1;
                                line2.Stroke = new SolidColorBrush(Colors.Black);
                                line2.StrokeThickness = 1;

                                line1.X1 = x1 + 15;
                                line1.X2 = x1 + 15;
                                line1.Y1 = y1 - 15;
                                line1.Y2 = y1 - 35;
                                line2.X1 = line1.X2;
                                line2.X2 = arrow.X1;
                                line2.Y1 = line1.Y2;
                                line2.Y2 = arrow.Y1;

                                line1.Name = "la";
                                this.RegisterName("la", line1);
                                line2.Name = "lb";
                                this.RegisterName("lb", line2);

                                MainCanvas.Children.Add(line1);
                                MainCanvas.Children.Add(line2);
                            }
                        } else {
                            if (OperPosi % 10 == 5) {
                                arrow.Y1 = arrow.Y1 + 15;
                                arrow.Y2 = arrown.Y2;
                            }else if (OperPosi % 10 == 6) {
                                arrow.X1 = arrow.X1 - 15;
                                arrow.X2 = arrown.X2;
                            }else if (OperPosi % 10 == 0) {
                                arrow.Y1 = arrow.Y1 + 15;
                                arrow.Y2 = arrown.Y2;
                            }else if (OperPosi % 10 == 1) {
                                arrow.X1 = arrow.X1 + 15;
                                arrow.X2 = arrown.X2;
                            }
                        }
                    }else if (flagNext == 1) {
                        flagNext++;
                        Arrow arrown = this.FindName("a" + (OperPosi + 1)) as Arrow;
                        arrown.Stroke = new SolidColorBrush(Colors.White);
                    }else if (flagNext == 2) {
                        flagNext++;
                        Clear();
                        if ((OperPosi % 5 != 0 && OperPosi % 5 != 1) || OperPosi == 1) {
                            MainCanvas.Children.Remove((UIElement)this.FindName("la"));
                            this.UnregisterName("la");
                            MainCanvas.Children.Remove((UIElement)this.FindName("lb"));
                            this.UnregisterName("lb");
                        }
                        array.RemoveAt(OperPosi - 1);
                        Draw();
                        btnPop.IsEnabled = true;
                        btnPush.IsEnabled = true;
                    }
                }else if(radiobtnDList.IsChecked == true) {
                    if (flagNext == 0) {
                        flagNext++;
                        Arrow arrow = this.FindName("a" + OperPosi) as Arrow;
                        Arrow arrown = this.FindName("a" + (OperPosi + 1)) as Arrow;
                        double x1 = arrow.X1;
                        double x2 = arrow.X2;
                        double y1 = arrow.Y1;
                        double y2 = arrow.Y2;
                        if ((OperPosi % 5 != 0 && OperPosi % 5 != 1) || OperPosi == 1) {
                            if ((OperPosi - 1) % 10 < 5) {
                                arrow.X1 = arrown.X2 + 15;
                                arrow.X2 = arrown.X2 + 15;
                                arrow.Y1 = arrown.Y1 - 25;
                                arrow.Y2 = arrown.Y1 - 5;

                                Line line1 = new Line();
                                Line line2 = new Line();
                                line1.Stroke = new SolidColorBrush(Colors.Black);
                                line1.StrokeThickness = 1;
                                line2.Stroke = new SolidColorBrush(Colors.Black);
                                line2.StrokeThickness = 1;

                                line1.X1 = x1 - 15;
                                line1.X2 = x1 - 15;
                                line1.Y1 = y1 - 5;
                                line1.Y2 = y1 - 25;
                                line2.X1 = line1.X2;
                                line2.X2 = arrow.X1;
                                line2.Y1 = line1.Y2;
                                line2.Y2 = arrow.Y1;

                                line1.Name = "la";
                                this.RegisterName("la", line1);
                                line2.Name = "lb";
                                this.RegisterName("lb", line2);

                                MainCanvas.Children.Add(line1);
                                MainCanvas.Children.Add(line2);
                            } else {
                                arrow.X1 = arrown.X2 - 15;
                                arrow.X2 = arrown.X2 - 15;
                                arrow.Y1 = arrown.Y1 + 25;
                                arrow.Y2 = arrown.Y1 + 5;

                                Line line1 = new Line();
                                Line line2 = new Line();
                                line1.Stroke = new SolidColorBrush(Colors.Black);
                                line1.StrokeThickness = 1;
                                line2.Stroke = new SolidColorBrush(Colors.Black);
                                line2.StrokeThickness = 1;

                                line1.X1 = x1 + 15;
                                line1.X2 = x1 + 15;
                                line1.Y1 = y1 + 5;
                                line1.Y2 = y1 + 25;
                                line2.X1 = line1.X2;
                                line2.X2 = arrow.X1;
                                line2.Y1 = line1.Y2;
                                line2.Y2 = arrow.Y1;

                                line1.Name = "la";
                                this.RegisterName("la", line1);
                                line2.Name = "lb";
                                this.RegisterName("lb", line2);

                                MainCanvas.Children.Add(line1);
                                MainCanvas.Children.Add(line2);
                            }
                        } else {
                            if (OperPosi % 10 == 5) {
                                arrow.Y1 = arrow.Y1 + 20;
                                arrow.Y2 = arrown.Y2;
                                arrow.X2 = arrown.X2 - 20;
                            }else if (OperPosi % 10 == 6) {
                                arrow.X1 = arrow.X1 - 20;
                                arrow.X2 = arrown.X2;
                                arrow.Y2 = arrown.Y2 - 20;
                            } else if (OperPosi % 10 == 0) {
                                arrow.X1 = arrow.X1 + 5;
                                arrow.X2 = arrown.X2 + 5;
                                arrow.Y1 = arrow.Y1 + 5;
                                arrow.Y2 = arrown.Y2 + 5;
                            } else if (OperPosi % 10 == 1) {
                                arrow.X1 = arrow.X1 + 5;
                                arrow.X2 = arrown.X2 + 5;
                                arrow.Y1 = arrow.Y1 - 5;
                                arrow.Y2 = arrown.Y2 - 5;
                            }
                        }
                    } else if (flagNext == 1) {
                        flagNext++;
                        Arrow arrown = this.FindName("b" + OperPosi) as Arrow;
                        Arrow arrow = this.FindName("b" + (OperPosi + 1)) as Arrow;
                        double x1 = arrow.X1;
                        double x2 = arrow.X2;
                        double y1 = arrow.Y1;
                        double y2 = arrow.Y2;

                        if ((OperPosi % 5 != 0 && OperPosi % 5 != 1) || OperPosi == 1) {
                            if ((OperPosi - 1) % 10 < 5) {
                                arrow.X1 = arrown.X2 - 15;
                                arrow.X2 = arrown.X2 - 15;
                                arrow.Y1 = arrown.Y1 + 25;
                                arrow.Y2 = arrown.Y1 + 5;

                                Line line1 = new Line();
                                Line line2 = new Line();
                                line1.Stroke = new SolidColorBrush(Colors.Black);
                                line1.StrokeThickness = 1;
                                line2.Stroke = new SolidColorBrush(Colors.Black);
                                line2.StrokeThickness = 1;

                                line1.X1 = x1 + 15;
                                line1.X2 = x1 + 15;
                                line1.Y1 = y1 + 5;
                                line1.Y2 = y1 + 25;
                                line2.X1 = line1.X2;
                                line2.X2 = arrow.X1;
                                line2.Y1 = line1.Y2;
                                line2.Y2 = arrow.Y1;

                                line1.Name = "lc";
                                this.RegisterName("lc", line1);
                                line2.Name = "ld";
                                this.RegisterName("ld", line2);

                                MainCanvas.Children.Add(line1);
                                MainCanvas.Children.Add(line2);
                            } else {
                                arrow.X1 = arrown.X2 + 15;
                                arrow.X2 = arrown.X2 + 15;
                                arrow.Y1 = arrown.Y1 - 25;
                                arrow.Y2 = arrown.Y1 - 5;

                                Line line1 = new Line();
                                Line line2 = new Line();
                                line1.Stroke = new SolidColorBrush(Colors.Black);
                                line1.StrokeThickness = 1;
                                line2.Stroke = new SolidColorBrush(Colors.Black);
                                line2.StrokeThickness = 1;

                                line1.X1 = x1 - 15;
                                line1.X2 = x1 - 15;
                                line1.Y1 = y1 - 5;
                                line1.Y2 = y1 - 25;
                                line2.X1 = line1.X2;
                                line2.X2 = arrow.X1;
                                line2.Y1 = line1.Y2;
                                line2.Y2 = arrow.Y1;

                                line1.Name = "lc";
                                this.RegisterName("lc", line1);
                                line2.Name = "ld";
                                this.RegisterName("ld", line2);

                                MainCanvas.Children.Add(line1);
                                MainCanvas.Children.Add(line2);
                            }
                        } else {
                            if (OperPosi % 10 == 5) {
                                arrow.X1 = arrow.X1 - 5;
                                arrow.X2 = arrown.X2 - 5;
                                arrow.Y1 = arrow.Y1 + 5;
                                arrow.Y2 = arrown.Y2 + 5;
                            } else if (OperPosi % 10 == 6) {
                                arrow.X1 = arrow.X1 - 5;
                                arrow.X2 = arrown.X2 - 5;
                                arrow.Y1 = arrow.Y1 - 5;
                                arrow.Y2 = arrown.Y2 - 5;
                            } else if (OperPosi % 10 == 0) {
                                arrow.X1 = arrow.X1 + 20;
                                arrow.X2 = arrown.X2;
                                arrow.Y2 = arrown.Y2 + 20;
                            } else if (OperPosi % 10 == 1) {
                                arrow.X2 = arrown.X2 + 20;
                                arrow.Y1 = arrow.Y1 - 20;
                                arrow.Y2 = arrown.Y2; 
                            }
                        }
                    } else if (flagNext == 2) {
                        flagNext++;
                        Arrow arrowa = this.FindName("a" + (OperPosi + 1)) as Arrow;
                        Arrow arrowb = this.FindName("b" + OperPosi) as Arrow;
                        arrowa.Stroke = new SolidColorBrush(Colors.White);
                        arrowb.Stroke = new SolidColorBrush(Colors.White);
                    } else if (flagNext == 3) {
                        flagNext++;
                        Clear();
                        if ((OperPosi % 5 != 0 && OperPosi % 5 != 1) || OperPosi == 1) {
                            MainCanvas.Children.Remove((UIElement)this.FindName("la"));
                            this.UnregisterName("la");
                            MainCanvas.Children.Remove((UIElement)this.FindName("lb"));
                            this.UnregisterName("lb");
                            MainCanvas.Children.Remove((UIElement)this.FindName("lc"));
                            this.UnregisterName("lc");
                            MainCanvas.Children.Remove((UIElement)this.FindName("ld"));
                            this.UnregisterName("ld");
                        }
                        array.RemoveAt(OperPosi - 1);
                        Draw();
                        btnPop.IsEnabled = true;
                        btnPush.IsEnabled = true;
                    }
                }
            }
        }

        private void radiobtnSList_Checked(object sender, RoutedEventArgs e) {
            Clear();
            Draw();
            flagLastCh = 1;
            btnNext.IsEnabled = false;
        }

        private void radiobtnDList_Checked(object sender, RoutedEventArgs e) {
            Clear();
            Draw();
            flagLastCh = 2;
            btnNext.IsEnabled = false;
        }

        private void radiobtnStack_Checked(object sender, RoutedEventArgs e) {
            Clear();
            Draw();
            flagLastCh = 3;
            btnNext.IsEnabled = false;
        }

        private void radiobtnQueue_Checked(object sender, RoutedEventArgs e) {
            Clear();
            Draw();
            flagLastCh = 4;
            btnNext.IsEnabled = false;
        } 

        public void Draw() {
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;

            CanvasClear = false;
            int space = 70;

            CtrlBg.Height = this.Height;
            MainCanvas.Height = this.Height;
            MainCanvas.Width = this.Width - 170;
            startx = (MainCanvas.Width - 530 + 170) / 2;
            starty = (MainCanvas.Height - (array.Count - 1) / 5 * 70 - ((array.Count - 1) / 5 + 1) * 30) / 2;
            if (radiobtnSList.IsChecked == true) {
                for(int i = 0; i <= array.Count; i++) {
                    Rectangle rect = new Rectangle();
                    rect.Width = 30;
                    rect.Height = 30;
                    rect.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    if (i == 0) {
                        rect.SetValue(Canvas.LeftProperty, startx + space * i - rect.Width / 2);
                        rect.SetValue(Canvas.TopProperty, starty - rect.Height / 2);
                    }else if ((i - 1) % 10 < 5) {
                        rect.SetValue(Canvas.LeftProperty, startx + space * (i % 10) - rect.Width / 2);
                        rect.SetValue(Canvas.TopProperty, starty + space * ((i - 1) / 5) - rect.Height / 2);
                    } else {    
                        rect.SetValue(Canvas.LeftProperty, startx + space * (10 - (i - 1) % 10) - rect.Width / 2);
                        rect.SetValue(Canvas.TopProperty, starty + space * ((i - 1) / 5) - rect.Height / 2);
                    }
                    
                    rect.Name = "v" + i;
                    this.RegisterName("v" + i, rect);

                    TextBlock text = new TextBlock();
                    if (i == 0) {
                        text.Text = "头";
                    } else {
                        text.Text = array[i-1].ToString();
                    }                   
                    text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size sizeText = text.DesiredSize;
                    if (i == 0) {
                        text.SetValue(Canvas.LeftProperty, startx + space * i - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, starty - sizeText.Height / 2);
                    }else if ((i - 1) % 10 < 5) {
                        text.SetValue(Canvas.LeftProperty, startx + space * (i % 10) - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, starty + space * ((i - 1) / 5) - sizeText.Height / 2);
                    } else {
                        text.SetValue(Canvas.LeftProperty, startx + space * (10 - (i - 1) % 10) - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, starty + space * ((i - 1) / 5) - sizeText.Height / 2);
                    }
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    text.TextAlignment = TextAlignment.Center;
                    text.Name = "t" + i;
                    this.RegisterName("t" + i, text);

                    MainCanvas.Children.Add(rect);
                    MainCanvas.Children.Add(text);

                    if (i != 0) {
                        Arrow arrow = new Arrow();
                        TextBlock texta = new TextBlock();
                        texta.Text = i.ToString();
                        texta.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        Size sizeTexta = text.DesiredSize;
                        if (i == 1) {
                            arrow.X1 = startx + 15;
                            arrow.X2 = startx + space * (i % 10) - 15;
                            arrow.Y1 = starty;
                            arrow.Y2 = starty;
                        } else if ((i - 1) % 5 == 0) {
                            if ((i - 1) % 10 != 0) {
                                arrow.X1 = startx + space * ((i - 1) % 10);
                                arrow.X2 = startx + space * ((i - 1) % 10);
                            } else {
                                arrow.X1 = startx + space * (i % 10);
                                arrow.X2 = startx + space * (i % 10);
                            }
                            arrow.Y1 = starty + space * ((i - 2) / 5) + 15;
                            arrow.Y2 = starty + space * (i / 5) - 15;
                        } else if ((i - 1) % 10 < 5) {
                            arrow.X1 = startx + space * ((i - 1) % 10) + 15;
                            arrow.X2 = startx + space * (i % 10) - 15;
                            arrow.Y1 = starty + space * ((i - 1) / 5);
                            arrow.Y2 = starty + space * ((i - 1) / 5);
                        } else {
                            arrow.X1 = startx + space * (10 - (i - 1) % 10 + 1) - 15;
                            if (i % 10 != 0) {
                                arrow.X2 = startx + space * (10 - (i % 10) + 1) + 15;
                            } else {
                                arrow.X2 = startx + space + 15;
                            }
                            arrow.Y1 = starty + space * ((i - 1) / 5);
                            arrow.Y2 = starty + space * ((i - 1) / 5);
                        }
                        if ((i - 1) % 5 != 0 || i == 1) {
                            texta.SetValue(Canvas.LeftProperty, (arrow.X1 + arrow.X2) / 2 - sizeTexta.Width / 2);
                            texta.SetValue(Canvas.TopProperty, arrow.Y1);
                        } else {
                            texta.SetValue(Canvas.LeftProperty, arrow.X1);
                            texta.SetValue(Canvas.TopProperty, (arrow.Y1 + arrow.Y2) / 2 - sizeTexta.Height / 2);
                        }
                        arrow.HeadHeight = 5;
                        arrow.HeadWidth = 10;
                        arrow.Stroke = new SolidColorBrush(Colors.Black);
                        arrow.StrokeThickness = 1;
                        arrow.Name = "a" + i;
                        this.RegisterName("a" + i, arrow);
                        texta.Name = "u" + i;
                        this.RegisterName("u" + i, texta);
                        MainCanvas.Children.Add(arrow);
                        MainCanvas.Children.Add(texta);
                    }
                }
            }else if(radiobtnDList.IsChecked == true) {
                int spacea = 10;
                for (int i = 0; i <= array.Count; i++) {
                    Rectangle rect = new Rectangle();
                    rect.Width = 30;
                    rect.Height = 30;
                    rect.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    if (i == 0) {
                        rect.SetValue(Canvas.LeftProperty, startx + space * i - rect.Width / 2);
                        rect.SetValue(Canvas.TopProperty, starty - rect.Height / 2);
                    } else if ((i - 1) % 10 < 5) {
                        rect.SetValue(Canvas.LeftProperty, startx + space * (i % 10) - rect.Width / 2);
                        rect.SetValue(Canvas.TopProperty, starty + space * ((i - 1) / 5) - rect.Height / 2);
                    } else {
                        rect.SetValue(Canvas.LeftProperty, startx + space * (10 - (i - 1) % 10) - rect.Width / 2);
                        rect.SetValue(Canvas.TopProperty, starty + space * ((i - 1) / 5) - rect.Height / 2);
                    }
                    rect.Name = "v" + i;
                    this.RegisterName("v" + i, rect);

                    TextBlock text = new TextBlock();
                    if (i == 0) {
                        text.Text = "头";
                    } else {
                        text.Text = array[i - 1].ToString();
                    }
                    text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size sizeText = text.DesiredSize;
                    if (i == 0) {
                        text.SetValue(Canvas.LeftProperty, startx + space * i - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, starty - sizeText.Height / 2);
                    } else if ((i - 1) % 10 < 5) {
                        text.SetValue(Canvas.LeftProperty, startx + space * (i % 10) - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, starty + space * ((i - 1) / 5) - sizeText.Height / 2);
                    } else {
                        text.SetValue(Canvas.LeftProperty, startx + space * (10 - (i - 1) % 10) - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, starty + space * ((i - 1) / 5) - sizeText.Height / 2);
                    }
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    text.TextAlignment = TextAlignment.Center;
                    text.Name = "t" + i;
                    this.RegisterName("t" + i, text);

                    MainCanvas.Children.Add(rect);
                    MainCanvas.Children.Add(text);

                    if (i != 0) {
                        Arrow arrowa = new Arrow();
                        Arrow arrowb = new Arrow();
                        TextBlock texta = new TextBlock();
                        texta.Text = i.ToString();
                        texta.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        Size sizeTexta = text.DesiredSize;
                        if (i == 1) {
                            arrowa.X1 = startx + 15;
                            arrowa.X2 = startx + space * (i % 10) - 15;
                            arrowa.Y1 = starty - spacea;
                            arrowa.Y2 = starty - spacea;

                            arrowb.X2 = startx + 15;
                            arrowb.X1 = startx + space * (i % 10) - 15;
                            arrowb.Y1 = starty + spacea;
                            arrowb.Y2 = starty + spacea;
                        } else if ((i - 1) % 5 == 0) {
                            if ((i - 1) % 10 != 0) {
                                arrowa.X1 = startx + space * ((i - 1) % 10) + spacea;
                                arrowa.X2 = startx + space * ((i - 1) % 10) + spacea;
                                arrowb.X1 = startx + space * ((i - 1) % 10) - spacea;
                                arrowb.X2 = startx + space * ((i - 1) % 10) - spacea;
                            } else {
                                arrowa.X1 = startx + space * (i % 10) + spacea;
                                arrowa.X2 = startx + space * (i % 10) + spacea;
                                arrowb.X1 = startx + space * (i % 10) - spacea;
                                arrowb.X2 = startx + space * (i % 10) - spacea;
                            }
                            arrowa.Y1 = starty + space * ((i - 2) / 5) + 15;
                            arrowa.Y2 = starty + space * (i / 5) - 15;

                            arrowb.Y2 = starty + space * ((i - 2) / 5) + 15;
                            arrowb.Y1 = starty + space * (i / 5) - 15;
                        } else if ((i - 1) % 10 < 5) {
                            arrowa.X1 = startx + space * ((i - 1) % 10) + 15;
                            arrowa.X2 = startx + space * (i % 10) - 15;
                            arrowa.Y1 = starty + space * ((i - 1) / 5) - spacea;
                            arrowa.Y2 = starty + space * ((i - 1) / 5) - spacea;

                            arrowb.X2 = startx + space * ((i - 1) % 10) + 15;
                            arrowb.X1 = startx + space * (i % 10) - 15;
                            arrowb.Y1 = starty + space * ((i - 1) / 5) + spacea;
                            arrowb.Y2 = starty + space * ((i - 1) / 5) + spacea;
                        } else {
                            arrowa.X1 = startx + space * (10 - (i - 1) % 10 + 1) - 15;
                            if (i % 10 != 0) {
                                arrowa.X2 = startx + space * (10 - (i % 10) + 1) + 15;
                            } else {
                                arrowa.X2 = startx + space + 15;
                            }
                            arrowa.Y1 = starty + space * ((i - 1) / 5) + spacea;
                            arrowa.Y2 = starty + space * ((i - 1) / 5) + spacea;

                            arrowb.X2 = startx + space * (10 - (i - 1) % 10 + 1) - 15;
                            if (i % 10 != 0) {
                                arrowb.X1 = startx + space * (10 - (i % 10) + 1) + 15;
                            } else {
                                arrowb.X1 = startx + space + 15;
                            }
                            arrowb.Y1 = starty + space * ((i - 1) / 5) - spacea;
                            arrowb.Y2 = starty + space * ((i - 1) / 5) - spacea;
                        }
                        if ((i - 1) % 5 != 0 || i == 1) {
                            texta.SetValue(Canvas.LeftProperty, (arrowa.X1 + arrowa.X2) / 2 - sizeTexta.Width / 2);
                            texta.SetValue(Canvas.TopProperty, (arrowa.Y1 + arrowb.Y1) / 2 - sizeTexta.Height / 2);
                        } else {
                            texta.SetValue(Canvas.LeftProperty, (arrowa.X1 + arrowb.X1) / 2 - sizeTexta.Width / 2);
                            texta.SetValue(Canvas.TopProperty, (arrowa.Y1 + arrowa.Y2) / 2 - sizeTexta.Height / 2);
                        }
                        arrowa.HeadHeight = 5;
                        arrowa.HeadWidth = 10;
                        arrowa.Stroke = new SolidColorBrush(Colors.Black);
                        arrowa.StrokeThickness = 1;
                        arrowa.Name = "a" + i;
                        this.RegisterName("a" + i, arrowa);
                        arrowb.HeadHeight = 5;
                        arrowb.HeadWidth = 10;
                        arrowb.Stroke = new SolidColorBrush(Colors.Black);
                        arrowb.StrokeThickness = 1;
                        arrowb.Name = "b" + i;
                        this.RegisterName("b" + i, arrowb);
                        texta.Name = "u" + i;
                        this.RegisterName("u" + i, texta);
                        MainCanvas.Children.Add(arrowa);
                        MainCanvas.Children.Add(arrowb);
                        MainCanvas.Children.Add(texta);
                    }
                }
            } else if (radiobtnStack.IsChecked == true) {
                for (int i = 0; i <= array.Count; i++) {
                    Rectangle rect = new Rectangle();
                    rect.Width = 30;
                    rect.Height = 30;
                    if (i == 0) {
                        rect.Fill = new SolidColorBrush(Colors.DarkSeaGreen);
                    } else if (flagLastOp == 1 && i == 1) {
                        rect.Fill = new SolidColorBrush(Colors.IndianRed);
                    } else {
                        rect.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    }
                    if (i == 0) {
                        rect.SetValue(Canvas.LeftProperty, startx + space * i - rect.Width / 2);
                        rect.SetValue(Canvas.TopProperty, starty - rect.Height / 2);
                    } else if ((i - 1) % 10 < 5) {
                        rect.SetValue(Canvas.LeftProperty, startx + space * (i % 10) - rect.Width / 2);
                        rect.SetValue(Canvas.TopProperty, starty + space * ((i - 1) / 5) - rect.Height / 2);
                    } else {
                        rect.SetValue(Canvas.LeftProperty, startx + space * (10 - (i - 1) % 10) - rect.Width / 2);
                        rect.SetValue(Canvas.TopProperty, starty + space * ((i - 1) / 5) - rect.Height / 2);
                    }

                    rect.Name = "v" + i;
                    this.RegisterName("v" + i, rect);

                    TextBlock text = new TextBlock();
                    if (i == 0) {
                        text.Text = "首";
                    } else {
                        text.Text = array[i - 1].ToString();
                    }
                    text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size sizeText = text.DesiredSize;
                    if (i == 0) {
                        text.SetValue(Canvas.LeftProperty, startx + space * i - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, starty - sizeText.Height / 2);
                    } else if ((i - 1) % 10 < 5) {
                        text.SetValue(Canvas.LeftProperty, startx + space * (i % 10) - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, starty + space * ((i - 1) / 5) - sizeText.Height / 2);
                    } else {
                        text.SetValue(Canvas.LeftProperty, startx + space * (10 - (i - 1) % 10) - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, starty + space * ((i - 1) / 5) - sizeText.Height / 2);
                    }
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    text.TextAlignment = TextAlignment.Center;
                    text.Name = "t" + i;
                    this.RegisterName("t" + i, text);

                    MainCanvas.Children.Add(rect);
                    MainCanvas.Children.Add(text);

                    if (i != 0) {
                        Line arrow = new Line();
                        if (i == 1) {
                            arrow.X1 = startx + 15;
                            arrow.X2 = startx + space * (i % 10) - 15;
                            arrow.Y1 = starty;
                            arrow.Y2 = starty;
                        } else if ((i - 1) % 5 == 0) {
                            if ((i - 1) % 10 != 0) {
                                arrow.X1 = startx + space * ((i - 1) % 10);
                                arrow.X2 = startx + space * ((i - 1) % 10);
                            } else {
                                arrow.X1 = startx + space * (i % 10);
                                arrow.X2 = startx + space * (i % 10);
                            }
                            arrow.Y1 = starty + space * ((i - 2) / 5) + 15;
                            arrow.Y2 = starty + space * (i / 5) - 15;
                        } else if ((i - 1) % 10 < 5) {
                            arrow.X1 = startx + space * ((i - 1) % 10) + 15;
                            arrow.X2 = startx + space * (i % 10) - 15;
                            arrow.Y1 = starty + space * ((i - 1) / 5);
                            arrow.Y2 = starty + space * ((i - 1) / 5);
                        } else {
                            arrow.X1 = startx + space * (10 - (i - 1) % 10 + 1) - 15;
                            if (i % 10 != 0) {
                                arrow.X2 = startx + space * (10 - (i % 10) + 1) + 15;
                            } else {
                                arrow.X2 = startx + space + 15;
                            }
                            arrow.Y1 = starty + space * ((i - 1) / 5);
                            arrow.Y2 = starty + space * ((i - 1) / 5);
                        }
                        arrow.Stroke = new SolidColorBrush(Colors.Black);
                        arrow.StrokeThickness = 1;
                        arrow.Name = "a" + i;
                        this.RegisterName("a" + i, arrow);
                        MainCanvas.Children.Add(arrow);
                    }
                }
            } else if (radiobtnQueue.IsChecked == true) {
                for (int i = 0; i <= array.Count + 1; i++) {
                    Rectangle rect = new Rectangle();
                    rect.Width = 30;
                    rect.Height = 30;
                    if (i == 0 || i == array.Count + 1) {
                        rect.Fill = new SolidColorBrush(Colors.DarkSeaGreen);
                    } else if (flagLastOp == 1 && i == 1) {
                        rect.Fill = new SolidColorBrush(Colors.IndianRed);
                    } else {
                        rect.Fill = new SolidColorBrush(Colors.CornflowerBlue);
                    }
                    if (i == 0) {
                        rect.SetValue(Canvas.LeftProperty, startx + space * i - rect.Width / 2);
                        rect.SetValue(Canvas.TopProperty, starty - rect.Height / 2);
                    } else if ((i - 1) % 10 < 5) {
                        rect.SetValue(Canvas.LeftProperty, startx + space * (i % 10) - rect.Width / 2);
                        rect.SetValue(Canvas.TopProperty, starty + space * ((i - 1) / 5) - rect.Height / 2);
                    } else {
                        rect.SetValue(Canvas.LeftProperty, startx + space * (10 - (i - 1) % 10) - rect.Width / 2);
                        rect.SetValue(Canvas.TopProperty, starty + space * ((i - 1) / 5) - rect.Height / 2);
                    }

                    rect.Name = "v" + i;
                    this.RegisterName("v" + i, rect);

                    TextBlock text = new TextBlock();
                    if (i == 0) {
                        text.Text = "顶";
                    } else if (i == array.Count + 1) {
                        text.Text = "尾";
                    } else {
                        text.Text = array[i - 1].ToString();
                    }
                    text.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    Size sizeText = text.DesiredSize;
                    if (i == 0) {
                        text.SetValue(Canvas.LeftProperty, startx + space * i - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, starty - sizeText.Height / 2);
                    } else if ((i - 1) % 10 < 5) {
                        text.SetValue(Canvas.LeftProperty, startx + space * (i % 10) - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, starty + space * ((i - 1) / 5) - sizeText.Height / 2);
                    } else {
                        text.SetValue(Canvas.LeftProperty, startx + space * (10 - (i - 1) % 10) - sizeText.Width / 2);
                        text.SetValue(Canvas.TopProperty, starty + space * ((i - 1) / 5) - sizeText.Height / 2);
                    }
                    text.HorizontalAlignment = HorizontalAlignment.Center;
                    text.VerticalAlignment = VerticalAlignment.Center;
                    text.TextAlignment = TextAlignment.Center;
                    text.Name = "t" + i;
                    this.RegisterName("t" + i, text);

                    MainCanvas.Children.Add(rect);
                    MainCanvas.Children.Add(text);

                    if (i != 0) {
                        Line arrow = new Line();
                        if (i == 1) {
                            arrow.X1 = startx + 15;
                            arrow.X2 = startx + space * (i % 10) - 15;
                            arrow.Y1 = starty;
                            arrow.Y2 = starty;
                        } else if ((i - 1) % 5 == 0) {
                            if ((i - 1) % 10 != 0) {
                                arrow.X1 = startx + space * ((i - 1) % 10);
                                arrow.X2 = startx + space * ((i - 1) % 10);
                            } else {
                                arrow.X1 = startx + space * (i % 10);
                                arrow.X2 = startx + space * (i % 10);
                            }
                            arrow.Y1 = starty + space * ((i - 2) / 5) + 15;
                            arrow.Y2 = starty + space * (i / 5) - 15;
                        } else if ((i - 1) % 10 < 5) {
                            arrow.X1 = startx + space * ((i - 1) % 10) + 15;
                            arrow.X2 = startx + space * (i % 10) - 15;
                            arrow.Y1 = starty + space * ((i - 1) / 5);
                            arrow.Y2 = starty + space * ((i - 1) / 5);
                        } else {
                            arrow.X1 = startx + space * (10 - (i - 1) % 10 + 1) - 15;
                            if (i % 10 != 0) {
                                arrow.X2 = startx + space * (10 - (i % 10) + 1) + 15;
                            } else {
                                arrow.X2 = startx + space + 15;
                            }
                            arrow.Y1 = starty + space * ((i - 1) / 5);
                            arrow.Y2 = starty + space * ((i - 1) / 5);
                        }
                        arrow.Stroke = new SolidColorBrush(Colors.Black);
                        arrow.StrokeThickness = 1;
                        arrow.Name = "a" + i;
                        this.RegisterName("a" + i, arrow);
                        MainCanvas.Children.Add(arrow);
                    }
                }
            }
        }

        public void Clear() {
            if (flagLastCh == 1) {
                for(int i = 0; i <= array.Count; i++) {
                    MainCanvas.Children.Remove((UIElement)this.FindName("v" + i));
                    this.UnregisterName("v" + i);
                    MainCanvas.Children.Remove((UIElement)this.FindName("t" + i));
                    this.UnregisterName("t" + i);
                    if (i != 0) {
                        MainCanvas.Children.Remove((UIElement)this.FindName("a" + i));
                        this.UnregisterName("a" + i);
                        MainCanvas.Children.Remove((UIElement)this.FindName("u" + i));
                        this.UnregisterName("u" + i);
                    }
                }
            } else if (flagLastCh == 2) {
                for (int i = 0; i <= array.Count; i++) {
                    MainCanvas.Children.Remove((UIElement)this.FindName("v" + i));
                    this.UnregisterName("v" + i);
                    MainCanvas.Children.Remove((UIElement)this.FindName("t" + i));
                    this.UnregisterName("t" + i);
                    if (i != 0) {
                        MainCanvas.Children.Remove((UIElement)this.FindName("a" + i));
                        this.UnregisterName("a" + i);
                        MainCanvas.Children.Remove((UIElement)this.FindName("b" + i));
                        this.UnregisterName("b" + i);
                        MainCanvas.Children.Remove((UIElement)this.FindName("u" + i));
                        this.UnregisterName("u" + i);
                    }
                }
            } else if (flagLastCh == 3) {
                for (int i = 0; i <= array.Count; i++) {
                    MainCanvas.Children.Remove((UIElement)this.FindName("v" + i));
                    this.UnregisterName("v" + i);
                    MainCanvas.Children.Remove((UIElement)this.FindName("t" + i));
                    this.UnregisterName("t" + i);
                    if (i != 0) {
                        MainCanvas.Children.Remove((UIElement)this.FindName("a" + i));
                        this.UnregisterName("a" + i);
                    }
                }
            } else if (flagLastCh == 4) {
                for (int i = 0; i <= array.Count + 1; i++) {
                    MainCanvas.Children.Remove((UIElement)this.FindName("v" + i));
                    this.UnregisterName("v" + i);
                    MainCanvas.Children.Remove((UIElement)this.FindName("t" + i));
                    this.UnregisterName("t" + i);
                    if (i != 0) {
                        MainCanvas.Children.Remove((UIElement)this.FindName("a" + i));
                        this.UnregisterName("a" + i);
                    }
                }
            }
        }
        public void windowchanged() {
            this.Height = MainWindowInfo.mainPageHeight;
            this.Width = MainWindowInfo.mainPageWidth;
            CtrlBg.Height = this.Height;
            MainCanvas.Height = this.Height;
            MainCanvas.Width = this.Width - 170;
            double dstartx, dstarty;
            Point point;
            dstartx = (MainCanvas.Width - 530 + 170) / 2 - startx;
            dstarty = (MainCanvas.Height - (array.Count - 1) / 5 * 70 - ((array.Count - 1) / 5 + 1) * 30) / 2 - starty;

            if (flagLastCh == 1) {
                for (int i = 0; i <= array.Count; i++) {
                    Rectangle rect = (Rectangle)this.FindName("v" + i);
                    point = rect.TransformToAncestor(MainCanvas).Transform(new Point());
                    rect.SetValue(Canvas.LeftProperty, dstartx + point.X);
                    rect.SetValue(Canvas.TopProperty, dstarty + point.Y);

                    TextBlock text = (TextBlock)this.FindName("t" + i);
                    point = text.TransformToAncestor(MainCanvas).Transform(new Point());
                    text.SetValue(Canvas.LeftProperty, dstartx + point.X);
                    text.SetValue(Canvas.TopProperty, dstarty + point.Y);

                    if (i != 0) {
                        Arrow arrow = (Arrow)this.FindName("a" + i);
                        arrow.X1 = arrow.X1 + dstartx;
                        arrow.X2 = arrow.X2 + dstartx;
                        arrow.Y1 = arrow.Y1 + dstarty;
                        arrow.Y2 = arrow.Y2 + dstarty;
                        TextBlock texta = (TextBlock)this.FindName("u" + i);
                        point = texta.TransformToAncestor(MainCanvas).Transform(new Point());
                        texta.SetValue(Canvas.LeftProperty, dstartx + point.X);
                        texta.SetValue(Canvas.TopProperty, dstarty + point.Y);
                    }
                }
            } else if (flagLastCh == 2) {
                for (int i = 0; i <= array.Count; i++) {
                    Rectangle rect = (Rectangle)this.FindName("v" + i);
                    point = rect.TransformToAncestor(MainCanvas).Transform(new Point());
                    rect.SetValue(Canvas.LeftProperty, dstartx + point.X);
                    rect.SetValue(Canvas.TopProperty, dstarty + point.Y);

                    TextBlock text = (TextBlock)this.FindName("t" + i);
                    point = text.TransformToAncestor(MainCanvas).Transform(new Point());
                    text.SetValue(Canvas.LeftProperty, dstartx + point.X);
                    text.SetValue(Canvas.TopProperty, dstarty + point.Y);
                    if (i != 0) {
                        Arrow arrowa = (Arrow)this.FindName("a" + i);
                        arrowa.X1 = arrowa.X1 + dstartx;
                        arrowa.X2 = arrowa.X2 + dstartx;
                        arrowa.Y1 = arrowa.Y1 + dstarty;
                        arrowa.Y2 = arrowa.Y2 + dstarty;
                        Arrow arrowb = (Arrow)this.FindName("b" + i);
                        arrowb.X1 = arrowb.X1 + dstartx;
                        arrowb.X2 = arrowb.X2 + dstartx;
                        arrowb.Y1 = arrowb.Y1 + dstarty;
                        arrowb.Y2 = arrowb.Y2 + dstarty;
                        TextBlock texta = (TextBlock)this.FindName("u" + i);
                        point = texta.TransformToAncestor(MainCanvas).Transform(new Point());
                        texta.SetValue(Canvas.LeftProperty, dstartx + point.X);
                        texta.SetValue(Canvas.TopProperty, dstarty + point.Y);
                    }
                }
            } else if (flagLastCh == 3) {
                for (int i = 0; i <= array.Count; i++) {
                    Rectangle rect = (Rectangle)this.FindName("v" + i);
                    point = rect.TransformToAncestor(MainCanvas).Transform(new Point());
                    rect.SetValue(Canvas.LeftProperty, dstartx + point.X);
                    rect.SetValue(Canvas.TopProperty, dstarty + point.Y);

                    TextBlock text = (TextBlock)this.FindName("t" + i);
                    point = text.TransformToAncestor(MainCanvas).Transform(new Point());
                    text.SetValue(Canvas.LeftProperty, dstartx + point.X);
                    text.SetValue(Canvas.TopProperty, dstarty + point.Y);

                    if (i != 0) {
                        Line line = (Line)this.FindName("a" + i);
                        line.X1 = line.X1 + dstartx;
                        line.X2 = line.X2 + dstartx;
                        line.Y1 = line.Y1 + dstarty;
                        line.Y2 = line.Y2 + dstarty;
                    }
                }
            } else if (flagLastCh == 4) {
                for (int i = 0; i <= array.Count + 1; i++) {
                    Rectangle rect = (Rectangle)this.FindName("v" + i);
                    point = rect.TransformToAncestor(MainCanvas).Transform(new Point());
                    rect.SetValue(Canvas.LeftProperty, dstartx + point.X);
                    rect.SetValue(Canvas.TopProperty, dstarty + point.Y);

                    TextBlock text = (TextBlock)this.FindName("t" + i);
                    point = text.TransformToAncestor(MainCanvas).Transform(new Point());
                    text.SetValue(Canvas.LeftProperty, dstartx + point.X);
                    text.SetValue(Canvas.TopProperty, dstarty + point.Y);

                    if (i != 0) {
                        Line line = (Line)this.FindName("a" + i);
                        line.X1 = line.X1 + dstartx;
                        line.X2 = line.X2 + dstartx;
                        line.Y1 = line.Y1 + dstarty;
                        line.Y2 = line.Y2 + dstarty;
                    }
                }
            }
            //l1lblcldaaab
            Rectangle vins = (Rectangle)this.FindName("vins");
            if (vins != null) {
                point = vins.TransformToAncestor(MainCanvas).Transform(new Point());
                vins.SetValue(Canvas.LeftProperty, dstartx + point.X);
                vins.SetValue(Canvas.TopProperty, dstarty + point.Y);
            }

            TextBlock tins = (TextBlock)this.FindName("tins");
            if (tins != null) {
                point = tins.TransformToAncestor(MainCanvas).Transform(new Point());
                tins.SetValue(Canvas.LeftProperty, dstartx + point.X);
                tins.SetValue(Canvas.TopProperty, dstarty + point.Y);
            }

            Arrow aa = (Arrow)this.FindName("aa");
            if (aa != null) {
                aa.X1 = aa.X1 + dstartx;
                aa.X2 = aa.X2 + dstartx;
                aa.Y1 = aa.Y1 + dstarty;
                aa.Y2 = aa.Y2 + dstarty;
            }

            Arrow ab = (Arrow)this.FindName("ab");
            if (ab != null) {
                ab.X1 = ab.X1 + dstartx;
                ab.X2 = ab.X2 + dstartx;
                ab.Y1 = ab.Y1 + dstarty;
                ab.Y2 = ab.Y2 + dstarty;
            }

            Line la = (Line)this.FindName("la");
            Line lb = (Line)this.FindName("lb");
            if (la != null && lb != null) {
                la.X1 = la.X1 + dstartx;
                la.X2 = la.X2 + dstartx;
                la.Y1 = la.Y1 + dstarty;
                la.Y2 = la.Y2 + dstarty;
                lb.X1 = la.X1;
                lb.X2 = lb.X2 + dstartx;
                lb.Y1 = la.Y2;
                lb.Y2 = lb.Y2 + dstarty;
            }

            Line lc = (Line)this.FindName("lc");
            Line ld = (Line)this.FindName("ld");
            if (lc != null && ld != null) {
                lc.X1 = lc.X1 + dstartx;
                lc.X2 = lc.X2 + dstartx;
                lc.Y1 = lc.Y1 + dstarty;
                lc.Y2 = lc.Y2 + dstarty;
                ld.X1 = lc.X1;
                ld.X2 = ld.X2 + dstartx;
                ld.Y1 = lc.Y2;
                ld.Y2 = ld.Y2 + dstarty;
            }

            startx = (MainCanvas.Width - 530 + 170) / 2;
            starty = (MainCanvas.Height - (array.Count - 1) / 5 * 70 - ((array.Count - 1) / 5 + 1) * 30) / 2;
        }

        public void Filesave() {
            FileStream fs = new FileStream(MainWindowInfo.fileLocation, FileMode.Create);
            StreamWriter sw = new StreamWriter(fs);
            JObject jObject = new JObject();
            jObject["type"] = "LinearBasic";
            JArray jArray = new JArray();
            for (int i = 0; i < array.Count; i++) {
                jArray.Add(array[i]);
            }
            jObject["array"] = jArray;
            if (radiobtnSList.IsChecked == true) {
                jObject["way"] = "SList";
            } else if (radiobtnDList.IsChecked == true) {
                jObject["way"] = "DList";
            } else if (radiobtnStack.IsChecked == true) {
                jObject["way"] = "Stack";
            } else if (radiobtnQueue.IsChecked == true) {
                jObject["way"] = "Queue";
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

                } else if (jObject["way"].ToString().Equals("SList")) {
                    radiobtnSList.IsChecked = true;
                } else if (jObject["way"].ToString().Equals("DList")) {
                    radiobtnDList.IsChecked = true;
                } else if (jObject["way"].ToString().Equals("Stack")) {
                    radiobtnStack.IsChecked = true;
                } else if (jObject["way"].ToString().Equals("Queue")) {
                    radiobtnQueue.IsChecked = true;
                }
                sr.Close();
                fs.Close();
            } catch {
                MessageBox.Show("请检查文件是否完整");
            }
        }
    }
}

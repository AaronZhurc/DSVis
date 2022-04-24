using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DSVis.Windows.Forms {
    /// <summary>
    /// SetJosephusForm.xaml 的交互逻辑
    /// </summary>
    public partial class SetJosephusForm : Window {
        public delegate void SendMessage(int num,int start,int gap);
        public SendMessage sendMessage;
        public delegate void WindowClosed();
        public WindowClosed windowClosed;
        private int maxCircle;
        public SetJosephusForm() {
            InitializeComponent();
            textBoxNum.Focus();
        }

        public void getMaxCircle(int max) {
            maxCircle = max;
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            try {
                if (Convert.ToInt32(textBoxStart.Text) > Convert.ToInt32(textBoxNum.Text) || Convert.ToInt32(textBoxGap.Text) > Convert.ToInt32(textBoxNum.Text) || Convert.ToInt32(textBoxNum.Text) <= 0 || Convert.ToInt32(textBoxStart.Text) < 0 || Convert.ToInt32(textBoxGap.Text) <= 0) {
                    throw new Exception();
                } else {
                    if (Convert.ToInt32(textBoxNum.Text) > maxCircle) {
                        MessageBoxResult result = System.Windows.MessageBox.Show("当前设置的数目过大，是否继续", "", MessageBoxButton.OKCancel);
                        switch (result) {
                            case MessageBoxResult.OK:
                                sendMessage(Convert.ToInt32(textBoxNum.Text), Convert.ToInt32(textBoxStart.Text), Convert.ToInt32(textBoxGap.Text));
                                this.Close();
                                break;
                            case MessageBoxResult.Cancel:
                                break;
                        }
                    } else {
                        sendMessage(Convert.ToInt32(textBoxNum.Text), Convert.ToInt32(textBoxStart.Text), Convert.ToInt32(textBoxGap.Text));
                        this.Close();
                    }
                }
            } catch {
                System.Windows.Forms.MessageBox.Show("请检测输入是否正确");
            }
        }
        private void MainWindow_Closed(object sender, EventArgs e) {
            try {
                windowClosed();
            } catch {

            }
        }

        private void textBoxNum_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            Regex re = new Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void textBoxStart_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            Regex re = new Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void textBoxGap_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            Regex re = new Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }
    }
}

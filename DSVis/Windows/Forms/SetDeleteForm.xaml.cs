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
    /// SetDeleteForm.xaml 的交互逻辑
    /// </summary>
    public partial class SetDeleteForm : Window {
        public delegate void SendMessage(int value, int mode);
        public SendMessage sendMessage;
        public delegate void WindowClosed();
        public WindowClosed windowClosed;
        int mode;
        public SetDeleteForm() {
            InitializeComponent();
            textBox.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            try {
                if (Convert.ToInt32(textBox.Text) <= 0) {
                    throw new Exception("请检测输入是否正确");
                } else {
                    if (radioBtnDir.IsChecked == true) {
                        mode = 0;
                    } else {
                        mode = 1;
                    }
                    sendMessage(Convert.ToInt32(textBox.Text), mode);
                    this.Close();
                }
            } catch {
                System.Windows.MessageBox.Show("请检测输入的数字是否正确");
            }
        }

        private void textBox_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            Regex re = new Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void MainWindow_Closed(object sender, EventArgs e) {
            try {
                windowClosed();
            } catch {

            }
        }
    }
}

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
    /// SetInsertForm.xaml 的交互逻辑
    /// </summary>
    public partial class SetInsertForm : Window {
        public delegate void SendMessage(int value,int posi,int mode);
        public SendMessage sendMessage;
        public delegate void WindowClosed();
        public WindowClosed windowClosed;
        int mode;
        public SetInsertForm() {
            InitializeComponent();
            textBox_value.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            try {
                if (Convert.ToInt32(textBox_posi.Text) <= 0) {
                    throw new Exception();
                } else {
                    if (radioBtnDir.IsChecked == true) {
                        mode = 0;
                    } else {
                        mode = 1;
                    }
                    sendMessage(Convert.ToInt32(textBox_value.Text), Convert.ToInt32(textBox_posi.Text), mode);
                    this.Close();
                }
            } catch {
                MessageBox.Show("请检测输入的数字是否正确");
            }
        }

        private void textBox_value_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            Regex re = new Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void textBox_posi_PreviewTextInput(object sender, TextCompositionEventArgs e) {
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

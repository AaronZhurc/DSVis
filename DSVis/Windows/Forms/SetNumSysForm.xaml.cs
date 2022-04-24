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
    /// SetNumSysForm.xaml 的交互逻辑
    /// </summary>
    public partial class SetNumSysForm : Window {
        public delegate void SendMessage(int num, int sys);
        public SendMessage sendMessage;
        public delegate void WindowClosed();
        public WindowClosed windowClosed;
        public SetNumSysForm() {
            InitializeComponent();
            textBoxNum.Focus();
        }

        private void textBoxSys_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            Regex re = new Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void textBoxNum_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            Regex re = new Regex("[^0-9]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            if (textBoxNum.Text == "" && textBoxSys.Text == "") {
                MessageBox.Show("请输入转换数和转换进制");
            } else if (textBoxNum.Text == "") {
                MessageBox.Show("请输入转换数");
            } else if (textBoxSys.Text == "") {
                MessageBox.Show("请输入转换进制");
            } else if (int.Parse(textBoxSys.Text) > 36 && int.Parse(textBoxSys.Text) != 120) {
                MessageBox.Show("进制过大，无法表示");
            } else if (int.Parse(textBoxSys.Text)==0) {
                MessageBox.Show("不存在0进制");
            } else {
                sendMessage(int.Parse(textBoxNum.Text), int.Parse(textBoxSys.Text));
                this.Close();
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e) {
            try {
                windowClosed();
            } catch {

            }
        }

    }
}

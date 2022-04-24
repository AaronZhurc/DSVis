using System;
using System.Collections.Generic;
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

namespace DSVis.Windows.Forms {
    /// <summary>
    /// SetExpForm.xaml 的交互逻辑
    /// </summary>
    public partial class SetExpForm : Window {
        public delegate void SendMessage(String str);
        public SendMessage sendMessage;
        public delegate void WindowClosed();
        public WindowClosed windowClosed;
        public SetExpForm() {
            InitializeComponent();
            textBox.Focus();
        }
        private void Button_Click(object sender, RoutedEventArgs e) {
            if (textBox.Text != null && textBox.Text != "") {
                sendMessage(textBox.Text);
                this.Close();
            } else {
                MessageBox.Show("请检测输入的表达式是否正确");
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

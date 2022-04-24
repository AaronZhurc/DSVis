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
    /// SetStringForm.xaml 的交互逻辑
    /// </summary>
    public partial class SetStringForm : Window {
        public delegate void SendMessage(string mainStr,string pattenStr);
        public SendMessage sendMessage;
        public delegate void WindowClosed();
        public WindowClosed windowClosed;
        public SetStringForm() {
            InitializeComponent();
            textBox_mainStr.Focus();
        }
        private void Button_Click(object sender, RoutedEventArgs e) {
            if (textBox_mainStr.Text.Length != 0 && textBox_pattenStr.Text.Length != 0) {
                sendMessage(textBox_mainStr.Text, textBox_pattenStr.Text);
                this.Close();
            } else {
                if (textBox_mainStr.Text.Length == 0 && textBox_pattenStr.Text.Length == 0) {
                    MessageBox.Show("请检查是否填写主串和模式串");
                } else if (textBox_mainStr.Text.Length == 0) {
                    MessageBox.Show("请检查是否填写主串");
                } else {
                    MessageBox.Show("请检查是否填写模式串");
                }
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

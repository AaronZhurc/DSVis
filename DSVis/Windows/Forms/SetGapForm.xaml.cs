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
    /// SetGapForm.xaml 的交互逻辑
    /// </summary>
    public partial class SetGapForm : Window {
        public delegate void SendMessage(List<int> value);
        public SendMessage sendMessage;
        public delegate void WindowClosed();
        public WindowClosed windowClosed;
        List<int> array = new List<int>();
        int length;
        public SetGapForm() {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            try {
                if (!Char.IsDigit(textBox.Text[textBox.Text.Length - 1])) {
                    textBox.Text = textBox.Text.Substring(0, textBox.Text.Length - 1);
                }
                string[] text = Regex.Split(textBox.Text, "[^0-9]+");
                foreach (String t in text) {
                    if (Convert.ToInt32(t) > length) {
                        throw new Exception();
                    }
                    if (!array.Contains(Convert.ToInt32(t))) {
                        array.Add(Convert.ToInt32(t));
                    }
                }
                if (!array.Contains(1)) {
                    array.Add(1);
                }
                array.Sort((x, y) => -x.CompareTo(y));
                sendMessage(array);
                this.Close();
            } catch {
                System.Windows.MessageBox.Show("请检测输入的数列是否正确");
            }
        }

        public void sendLength(int length) {
            this.length = length;
        }

        private void MainWindow_Closed(object sender, EventArgs e) {
            try {
                windowClosed();
            } catch {

            }
        }
    }
}

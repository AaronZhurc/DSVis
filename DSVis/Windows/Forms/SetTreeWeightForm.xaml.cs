using System;
using System.Collections;
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
    /// SetTreeWeightForm.xaml 的交互逻辑
    /// </summary>
    public partial class SetTreeWeightForm : Window {
        public delegate void SendMessage(List<int> value);
        public SendMessage sendMessage;
        public delegate void WindowClosed();
        public WindowClosed windowClosed;
        List<int> weights = new List<int>();
        public SetTreeWeightForm() {
            InitializeComponent();
            this.Title = "设置哈夫曼树结点权值";
            textBox.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            try {
                if (!Char.IsDigit(textBox.Text[textBox.Text.Length - 1])) {
                    textBox.Text = textBox.Text.Substring(0, textBox.Text.Length - 1);
                }
                string[] text = Regex.Split(textBox.Text, "[^0-9]+");
                foreach (String t in text) {
                    weights.Add(Convert.ToInt32(t));
                }
                sendMessage(weights);
                this.Close();
            } catch {
                System.Windows.MessageBox.Show("请检测输入的权值是否正确");
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e) {
            try {
                windowClosed();
            } catch {
                
            }
        }

        private void Button_Random(object sender, RoutedEventArgs e) {
            Random rand = new Random();
            textBox.Clear();
            for (int i = 0; i < 4; i++) {
                textBox.Text += rand.Next(1, 100).ToString() + " ";
            }
            textBox.Text += rand.Next(1, 100).ToString();
        }
    }
}

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
            string[] text = Regex.Split(textBox.Text, "[^0-9]+");
            foreach (String t in text) {
                int n;
                if (!Int32.TryParse(t, out n)) {
                    if (t == "") {
                        continue;
                    }
                } else {
                    weights.Add(n);
                }
            }
            if (weights.Count == 0) {
                MessageBox.Show("请检查输入的数组");
            } else {
                sendMessage(weights);
                this.Close();
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

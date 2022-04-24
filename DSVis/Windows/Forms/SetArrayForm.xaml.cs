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
    /// SetArrayForm.xaml 的交互逻辑
    /// </summary>
    public partial class SetArrayForm : Window {
        public delegate void SendMessage(List<int> value);
        public SendMessage sendMessage;
        public delegate void WindowClosed();
        public WindowClosed windowClosed;
        List<int> array = new List<int>();
        public SetArrayForm() {
            InitializeComponent();
            textBox.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            //if (!Char.IsDigit(textBox.Text[textBox.Text.Length - 1])) {
                //textBox.Text = textBox.Text.Substring(0, textBox.Text.Length - 1);
            //}
            string[] text = Regex.Split(textBox.Text, "[^0-9]+");
            foreach (String t in text) {
                int n;
                if (!Int32.TryParse(t, out n)) {
                    if (t == "") {
                        continue;
                    }
                } else {
                    array.Add(n);
                }
            }
            if (array.Count == 0) {
                MessageBox.Show("请检查输入的数组");
            } else {
                sendMessage(array);
                this.Close();
            }
        }

        private void Button_Random(object sender, RoutedEventArgs e) {
            Random rand = new Random();
            textBox.Clear();
            int n;
            ArrayList list = new ArrayList();
            while (list.Count<10) {
                if (!list.Contains(n = rand.Next(1, 100))) {
                    list.Add(n);
                    if (list.Count == 10) {
                        textBox.Text += n.ToString();
                    } else {
                        textBox.Text += n.ToString() + " ";
                    }
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

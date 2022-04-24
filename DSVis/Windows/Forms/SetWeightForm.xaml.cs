using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DSVis.Windows.Forms {
    /// <summary>
    /// SetWeightForm.xaml 的交互逻辑
    /// </summary>
    public partial class SetWeightForm : Window {
        public delegate void SendMessage(int value);
        public SendMessage sendMessage;
        public SetWeightForm(string nameline) {
            InitializeComponent();
            this.Title = "编辑" + nameline + "的权值";
            textBox.Focus();
        }
        public SetWeightForm() {
            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e) {
            try { 
                sendMessage(Convert.ToInt32(textBox.Text));
                this.Close();
            } catch {
                System.Windows.MessageBox.Show("请检测输入的权值是否正确");
            }
        }

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DSVis.SubWindow {
    public partial class MCSTSetWeightForm : Form {
        private string nameline;
        private int weight;
        public MCSTSetWeightForm() {
            InitializeComponent();
            this.Name = "设置边"+nameline+"的权值";
        }

        public string NameLine { get => nameline; set => nameline = value; }
        public int Weight { get => weight; set => weight = value; }

        private void Confirm_Click(object sender, EventArgs e) {
            weight = int.Parse(textBoxWeight.Text);
        }
    }
}

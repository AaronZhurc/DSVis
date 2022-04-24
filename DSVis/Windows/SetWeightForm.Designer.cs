namespace DSVis.SubWindow {
    partial class MCSTSetWeightForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.Confirm = new System.Windows.Forms.Button();
            this.textBoxWeight = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // Confirm
            // 
            this.Confirm.Location = new System.Drawing.Point(299, 47);
            this.Confirm.Name = "Confirm";
            this.Confirm.Size = new System.Drawing.Size(97, 28);
            this.Confirm.TabIndex = 0;
            this.Confirm.Text = "确认";
            this.Confirm.UseVisualStyleBackColor = true;
            this.Confirm.Click += new System.EventHandler(this.Confirm_Click);
            // 
            // textBoxWeight
            // 
            this.textBoxWeight.Location = new System.Drawing.Point(54, 47);
            this.textBoxWeight.Name = "textBoxWeight";
            this.textBoxWeight.Size = new System.Drawing.Size(216, 28);
            this.textBoxWeight.TabIndex = 1;
            // 
            // MCSTSetWeightForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(451, 113);
            this.Controls.Add(this.textBoxWeight);
            this.Controls.Add(this.Confirm);
            this.Name = "MCSTSetWeightForm";
            this.Text = "MCSTSetWeightForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Confirm;
        private System.Windows.Forms.TextBox textBoxWeight;
    }
}
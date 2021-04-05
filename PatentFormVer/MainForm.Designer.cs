namespace PatentFormVer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.numStartPage = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radDg = new System.Windows.Forms.RadioButton();
            this.radUg = new System.Windows.Forms.RadioButton();
            this.radIg = new System.Windows.Forms.RadioButton();
            this.radIp = new System.Windows.Forms.RadioButton();
            this.txtHolder = new System.Windows.Forms.TextBox();
            this.txtKeywords = new System.Windows.Forms.TextBox();
            this.btnProcess = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtStatus = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStartPage)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(800, 450);
            this.splitContainer1.SplitterDistance = 186;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.numStartPage);
            this.splitContainer2.Panel1.Controls.Add(this.label3);
            this.splitContainer2.Panel1.Controls.Add(this.label2);
            this.splitContainer2.Panel1.Controls.Add(this.label1);
            this.splitContainer2.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer2.Panel1.Controls.Add(this.txtHolder);
            this.splitContainer2.Panel1.Controls.Add(this.txtKeywords);
            this.splitContainer2.Panel1.Controls.Add(this.btnProcess);
            this.splitContainer2.Panel1.Controls.Add(this.btnSearch);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.txtStatus);
            this.splitContainer2.Size = new System.Drawing.Size(800, 260);
            this.splitContainer2.SplitterDistance = 266;
            this.splitContainer2.TabIndex = 1;
            // 
            // numStartPage
            // 
            this.numStartPage.Location = new System.Drawing.Point(154, 129);
            this.numStartPage.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numStartPage.Name = "numStartPage";
            this.numStartPage.Size = new System.Drawing.Size(100, 20);
            this.numStartPage.TabIndex = 4;
            this.numStartPage.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(93, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "起始页码";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "权利人";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "关键字";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radDg);
            this.groupBox1.Controls.Add(this.radUg);
            this.groupBox1.Controls.Add(this.radIg);
            this.groupBox1.Controls.Add(this.radIp);
            this.groupBox1.Location = new System.Drawing.Point(12, 69);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(242, 51);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "专利类型";
            // 
            // radDg
            // 
            this.radDg.AutoSize = true;
            this.radDg.Location = new System.Drawing.Point(94, 32);
            this.radDg.Name = "radDg";
            this.radDg.Size = new System.Drawing.Size(73, 17);
            this.radDg.TabIndex = 3;
            this.radDg.Text = "外观设计";
            this.radDg.UseVisualStyleBackColor = true;
            // 
            // radUg
            // 
            this.radUg.AutoSize = true;
            this.radUg.Location = new System.Drawing.Point(3, 32);
            this.radUg.Name = "radUg";
            this.radUg.Size = new System.Drawing.Size(73, 17);
            this.radUg.TabIndex = 2;
            this.radUg.Text = "实用新型";
            this.radUg.UseVisualStyleBackColor = true;
            // 
            // radIg
            // 
            this.radIg.AutoSize = true;
            this.radIg.Checked = true;
            this.radIg.Location = new System.Drawing.Point(94, 16);
            this.radIg.Name = "radIg";
            this.radIg.Size = new System.Drawing.Size(73, 17);
            this.radIg.TabIndex = 1;
            this.radIg.TabStop = true;
            this.radIg.Text = "发明授权";
            this.radIg.UseVisualStyleBackColor = true;
            // 
            // radIp
            // 
            this.radIp.AutoSize = true;
            this.radIp.Location = new System.Drawing.Point(3, 16);
            this.radIp.Name = "radIp";
            this.radIp.Size = new System.Drawing.Size(73, 17);
            this.radIp.TabIndex = 0;
            this.radIp.Text = "发明公布";
            this.radIp.UseVisualStyleBackColor = true;
            // 
            // txtHolder
            // 
            this.txtHolder.Location = new System.Drawing.Point(51, 40);
            this.txtHolder.Name = "txtHolder";
            this.txtHolder.Size = new System.Drawing.Size(203, 20);
            this.txtHolder.TabIndex = 1;
            // 
            // txtKeywords
            // 
            this.txtKeywords.Location = new System.Drawing.Point(51, 14);
            this.txtKeywords.Name = "txtKeywords";
            this.txtKeywords.Size = new System.Drawing.Size(203, 20);
            this.txtKeywords.TabIndex = 1;
            this.txtKeywords.Text = "区块链";
            // 
            // btnProcess
            // 
            this.btnProcess.Location = new System.Drawing.Point(12, 225);
            this.btnProcess.Name = "btnProcess";
            this.btnProcess.Size = new System.Drawing.Size(75, 23);
            this.btnProcess.TabIndex = 0;
            this.btnProcess.Text = "Process";
            this.btnProcess.UseVisualStyleBackColor = true;
            this.btnProcess.Click += new System.EventHandler(this.btnProcess_ClickAsync);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(12, 126);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(75, 23);
            this.btnSearch.TabIndex = 0;
            this.btnSearch.Text = "Search";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtStatus
            // 
            this.txtStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtStatus.Location = new System.Drawing.Point(0, 0);
            this.txtStatus.Multiline = true;
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtStatus.Size = new System.Drawing.Size(530, 260);
            this.txtStatus.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numStartPage)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TextBox txtKeywords;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnProcess;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radDg;
        private System.Windows.Forms.RadioButton radUg;
        private System.Windows.Forms.RadioButton radIg;
        private System.Windows.Forms.RadioButton radIp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtHolder;
        private System.Windows.Forms.NumericUpDown numStartPage;
        private System.Windows.Forms.Label label3;
    }
}


namespace TestTwseMining
{
    partial class Form1
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器
        /// 修改這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.BtnSave = new System.Windows.Forms.Button();
            this.cboTestList = new System.Windows.Forms.ComboBox();
            this.btnTest = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // BtnSave
            // 
            this.BtnSave.Location = new System.Drawing.Point(250, 22);
            this.BtnSave.Name = "BtnSave";
            this.BtnSave.Size = new System.Drawing.Size(75, 25);
            this.BtnSave.TabIndex = 2;
            this.BtnSave.Text = "存Log";
            this.BtnSave.UseVisualStyleBackColor = true;
            this.BtnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // cboTestList
            // 
            this.cboTestList.FormattingEnabled = true;
            this.cboTestList.Items.AddRange(new object[] {
            "mis.twse.com.tw",
            "210.69.240.179",
            "210.71.239.179"});
            this.cboTestList.Location = new System.Drawing.Point(7, 24);
            this.cboTestList.Name = "cboTestList";
            this.cboTestList.Size = new System.Drawing.Size(156, 21);
            this.cboTestList.TabIndex = 3;
            // 
            // btnTest
            // 
            this.btnTest.Location = new System.Drawing.Point(169, 22);
            this.btnTest.Name = "btnTest";
            this.btnTest.Size = new System.Drawing.Size(75, 25);
            this.btnTest.TabIndex = 4;
            this.btnTest.Text = "Start Test";
            this.btnTest.UseVisualStyleBackColor = true;
            this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(337, 67);
            this.Controls.Add(this.btnTest);
            this.Controls.Add(this.cboTestList);
            this.Controls.Add(this.BtnSave);
            this.Name = "Form1";
            this.Text = "測試網路品質";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button BtnSave;
        private System.Windows.Forms.ComboBox cboTestList;
        private System.Windows.Forms.Button btnTest;
    }
}


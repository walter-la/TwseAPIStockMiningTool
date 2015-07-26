namespace StockMiningTool
{
    partial class MainWindow
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
            this.components = new System.ComponentModel.Container();
            this.btnStartAutoMining = new System.Windows.Forms.Button();
            this.btnStopAutoMining = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.btnSaveExceptionList = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnStartAutoMining
            // 
            this.btnStartAutoMining.Location = new System.Drawing.Point(208, 39);
            this.btnStartAutoMining.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStartAutoMining.Name = "btnStartAutoMining";
            this.btnStartAutoMining.Size = new System.Drawing.Size(297, 54);
            this.btnStartAutoMining.TabIndex = 0;
            this.btnStartAutoMining.Text = "開始偵測交易時間就開始抓資料";
            this.btnStartAutoMining.UseVisualStyleBackColor = true;
            this.btnStartAutoMining.Click += new System.EventHandler(this.btnStartAutoMining_Click);
            // 
            // btnStopAutoMining
            // 
            this.btnStopAutoMining.Location = new System.Drawing.Point(208, 120);
            this.btnStopAutoMining.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnStopAutoMining.Name = "btnStopAutoMining";
            this.btnStopAutoMining.Size = new System.Drawing.Size(297, 48);
            this.btnStopAutoMining.TabIndex = 1;
            this.btnStopAutoMining.Text = "停止計數計偵測交易時間";
            this.btnStopAutoMining.UseVisualStyleBackColor = true;
            this.btnStopAutoMining.Click += new System.EventHandler(this.btnStopAutoMining_Click);
            // 
            // timer1
            // 
            this.timer1.Interval = 15000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // btnSaveExceptionList
            // 
            this.btnSaveExceptionList.Location = new System.Drawing.Point(208, 190);
            this.btnSaveExceptionList.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSaveExceptionList.Name = "btnSaveExceptionList";
            this.btnSaveExceptionList.Size = new System.Drawing.Size(297, 54);
            this.btnSaveExceptionList.TabIndex = 2;
            this.btnSaveExceptionList.Text = "儲存錯誤訊息至文字檔";
            this.btnSaveExceptionList.UseVisualStyleBackColor = true;
            this.btnSaveExceptionList.Click += new System.EventHandler(this.btnSaveExceptionList_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(544, 124);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(155, 44);
            this.button1.TabIndex = 3;
            this.button1.Text = "測試資料庫連接";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 260);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnSaveExceptionList);
            this.Controls.Add(this.btnStopAutoMining);
            this.Controls.Add(this.btnStartAutoMining);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "MainWindow";
            this.Text = "Mining...";
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStartAutoMining;
        private System.Windows.Forms.Button btnStopAutoMining;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnSaveExceptionList;
        private System.Windows.Forms.Button button1;
    }
}


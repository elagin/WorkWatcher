namespace WorkWatcher
{
	partial class Form1
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
            this.textBoxUserName = new System.Windows.Forms.TextBox();
            this.textBoxEventCount = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxStartWeek = new System.Windows.Forms.TextBox();
            this.listView = new System.Windows.Forms.ListView();
            this.textBoxTotalWork = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxTotalWait = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.Location = new System.Drawing.Point(12, 12);
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Size = new System.Drawing.Size(191, 20);
            this.textBoxUserName.TabIndex = 1;
            // 
            // textBoxEventCount
            // 
            this.textBoxEventCount.Location = new System.Drawing.Point(512, 12);
            this.textBoxEventCount.Name = "textBoxEventCount";
            this.textBoxEventCount.Size = new System.Drawing.Size(148, 20);
            this.textBoxEventCount.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Начало недели";
            // 
            // textBoxStartWeek
            // 
            this.textBoxStartWeek.Location = new System.Drawing.Point(103, 40);
            this.textBoxStartWeek.Name = "textBoxStartWeek";
            this.textBoxStartWeek.Size = new System.Drawing.Size(100, 20);
            this.textBoxStartWeek.TabIndex = 4;
            // 
            // listView
            // 
            this.listView.Location = new System.Drawing.Point(12, 66);
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(648, 466);
            this.listView.TabIndex = 5;
            this.listView.UseCompatibleStateImageBehavior = false;
            // 
            // textBoxTotalWork
            // 
            this.textBoxTotalWork.Location = new System.Drawing.Point(324, 41);
            this.textBoxTotalWork.Name = "textBoxTotalWork";
            this.textBoxTotalWork.Size = new System.Drawing.Size(100, 20);
            this.textBoxTotalWork.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(248, 46);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Отработано:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(447, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(59, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Осталось:";
            // 
            // textBoxTotalWait
            // 
            this.textBoxTotalWait.Location = new System.Drawing.Point(512, 41);
            this.textBoxTotalWait.Name = "textBoxTotalWait";
            this.textBoxTotalWait.Size = new System.Drawing.Size(100, 20);
            this.textBoxTotalWait.TabIndex = 9;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(672, 544);
            this.Controls.Add(this.textBoxTotalWait);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxTotalWork);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.textBoxStartWeek);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxEventCount);
            this.Controls.Add(this.textBoxUserName);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion
		private System.Windows.Forms.TextBox textBoxUserName;
		private System.Windows.Forms.TextBox textBoxEventCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxStartWeek;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.TextBox textBoxTotalWork;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxTotalWait;
    }
}


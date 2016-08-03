namespace ProtoDrawingCollector.csproj {
  partial class Message {
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
      this.rtbMessage = new System.Windows.Forms.RichTextBox();
      this.checkBox1 = new System.Windows.Forms.CheckBox();
      this.checkBox2 = new System.Windows.Forms.CheckBox();
      this.button1 = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // rtbMessage
      // 
      this.rtbMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                  | System.Windows.Forms.AnchorStyles.Left)
                  | System.Windows.Forms.AnchorStyles.Right)));
      this.rtbMessage.Location = new System.Drawing.Point(12, 35);
      this.rtbMessage.Name = "rtbMessage";
      this.rtbMessage.Size = new System.Drawing.Size(607, 184);
      this.rtbMessage.TabIndex = 0;
      this.rtbMessage.Text = "";
      // 
      // checkBox1
      // 
      this.checkBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.checkBox1.AutoSize = true;
      this.checkBox1.Location = new System.Drawing.Point(440, 12);
      this.checkBox1.Name = "checkBox1";
      this.checkBox1.Size = new System.Drawing.Size(179, 17);
      this.checkBox1.TabIndex = 1;
      this.checkBox1.Text = "Auto-Delete Pre-Merged PDFs";
      this.checkBox1.UseVisualStyleBackColor = true;
      // 
      // checkBox2
      // 
      this.checkBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.checkBox2.AutoSize = true;
      this.checkBox2.Location = new System.Drawing.Point(368, 12);
      this.checkBox2.Name = "checkBox2";
      this.checkBox2.Size = new System.Drawing.Size(66, 17);
      this.checkBox2.TabIndex = 2;
      this.checkBox2.Text = "Recurse";
      this.checkBox2.UseVisualStyleBackColor = true;
      // 
      // button1
      // 
      this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.button1.Location = new System.Drawing.Point(528, 225);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(91, 23);
      this.button1.TabIndex = 3;
      this.button1.Text = "Close Window";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // Message
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(631, 260);
      this.Controls.Add(this.button1);
      this.Controls.Add(this.checkBox2);
      this.Controls.Add(this.checkBox1);
      this.Controls.Add(this.rtbMessage);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Name = "Message";
      this.Text = "Searching and merging PDFs...";
      this.Load += new System.EventHandler(this.Message_Load);
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Message_FormClosed);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Message_FormClosing);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.RichTextBox rtbMessage;
    private System.Windows.Forms.CheckBox checkBox1;
    private System.Windows.Forms.CheckBox checkBox2;
    private System.Windows.Forms.Button button1;
  }
}
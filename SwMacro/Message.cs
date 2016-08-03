using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ProtoDrawingCollector.csproj {
  public partial class Message : Form {
    public Message() {
      InitializeComponent();
    }

    //public delegate void AppendEvent(object o, EventArgs e);

    public void Append(string str) {
      rtbMessage.AppendText(str);
    }

    public void AppendLine(string str) {
      Append(str + "\n");
    }

    public void AppendLineEvent(object o, EventArgs e) {
      AppendLine(e.ToString());
    }

    public virtual void OnAppendEvent(EventArgs e) {
      BeginInvoke(new EventHandler(AppendLineEvent), this, e);
    }

    private void Message_Load(object sender, EventArgs e) {
      Location = Properties.Settings.Default.MessageLocation;
      Size = Properties.Settings.Default.MessageSize;
      checkBox1.Checked = Properties.Settings.Default.AutoDeletePreMergedPDFs;
      checkBox2.Checked = Properties.Settings.Default.Recurse;
    }

    private void Message_FormClosing(object sender, FormClosingEventArgs e) {
      Properties.Settings.Default.MessageLocation = Location;
      Properties.Settings.Default.MessageSize = Size;
      Properties.Settings.Default.Save();
    }

    public EventHandler AppendEvent;

    private void Message_FormClosed(object sender, FormClosedEventArgs e) {
      System.GC.Collect(0, GCCollectionMode.Forced);
    }
    private void checkBox1_CheckedChanged(object sender, EventArgs e) {
      Properties.Settings.Default.AutoDeletePreMergedPDFs = checkBox1.Checked;
    }

    private void checkBox2_CheckedChanged(object sender, EventArgs e) {
      Properties.Settings.Default.Recurse = checkBox2.Checked;
    }

    private void button1_Click(object sender, EventArgs e) {
      Close();
    }
  }
}
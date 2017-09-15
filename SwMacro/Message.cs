using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace ProtoDrawingCollector.csproj {
  public partial class Message : Form {
    public Message() {
      InitializeComponent();
    }

    //public delegate void AppendEvent(object o, EventArgs e);
    public static event click_EventHandler click_go;

    public delegate void click_EventHandler(object sender, GoEventArgs e);

    public static void OnGo(GoEventArgs e) {
      click_EventHandler handler = click_go;
      if (handler != null) {
        handler(new object(), e);
      }
    }

    public delegate void select_BOM(object sender, BOMEventArgs e);

    public void Append(string str) {
      if (str.Contains(@"[EE]")) {
        Font cf = rtbMessage.SelectionFont;
        Color cc = rtbMessage.SelectionColor;
        rtbMessage.SelectionFont = new Font(cf.FontFamily, cf.Size, FontStyle.Bold);
        rtbMessage.SelectionColor = Color.Red;
        rtbMessage.AppendText(str);
        rtbMessage.SelectionFont = cf;
        rtbMessage.SelectionColor = cc;
      } else if (str.Contains("Using") || str.Contains("Found")) {
        Font cf = rtbMessage.SelectionFont;
        rtbMessage.SelectionFont = new Font(cf.FontFamily, cf.Size, FontStyle.Italic | FontStyle.Underline);
        rtbMessage.AppendText(str);
        rtbMessage.SelectionFont = cf;
      } else if (str.Contains("Added")) {
        Font cf = rtbMessage.SelectionFont;
        Color cc = rtbMessage.SelectionColor;
        rtbMessage.SelectionFont = new Font(cf.FontFamily, cf.Size, FontStyle.Bold);
        rtbMessage.SelectionColor = Color.Green;
        rtbMessage.AppendText(" +  " + str);
        rtbMessage.SelectionFont = cf;
        rtbMessage.SelectionColor = cc;
      } else {
        rtbMessage.AppendText(str);
      }
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

    public void DisableGo() {
      button2.Enabled = false;
    }

    private void Message_Load(object sender, EventArgs e) {
      Location = Properties.Settings.Default.MessageLocation;
      Size = Properties.Settings.Default.MessageSize;
      textBox1.Text = Properties.Settings.Default.TargetPath;
      checkBox1.Checked = Properties.Settings.Default.AutoDeletePreMergedPDFs;
      checkBox2.Checked = Properties.Settings.Default.Recurse;
      numericUpDown1.Value = Properties.Settings.Default.CompressionLevel;
    }

    private void Message_FormClosing(object sender, FormClosingEventArgs e) {
      Properties.Settings.Default.MessageLocation = Location;
      Properties.Settings.Default.MessageSize = Size;
      Properties.Settings.Default.AutoDeletePreMergedPDFs = checkBox1.Checked;
      Properties.Settings.Default.Recurse = checkBox2.Checked;
      Properties.Settings.Default.CompressionLevel = (int)numericUpDown1.Value;
      Properties.Settings.Default.TargetPath = textBox1.Text;
      Properties.Settings.Default.Save();
    }

    public EventHandler AppendEvent;

    private void Message_FormClosed(object sender, FormClosedEventArgs e) {
      System.GC.Collect(0, GCCollectionMode.Forced);
    }
    private void checkBox1_CheckedChanged(object sender, EventArgs e) {
      Properties.Settings.Default.AutoDeletePreMergedPDFs = checkBox1.Checked;
      Properties.Settings.Default.Save();
    }

    private void checkBox2_CheckedChanged(object sender, EventArgs e) {
      Properties.Settings.Default.Recurse = checkBox2.Checked;
      Properties.Settings.Default.Save();
    }

    private void button1_Click(object sender, EventArgs e) {
      Properties.Settings.Default.Save();
      Close();
    }

    private void button2_Click(object sender, EventArgs e) {
      OnGo(new GoEventArgs(checkBox2.Checked,
        checkBox1.Checked,
        radioButton2.Checked ? PDFCollector.CreateFileType.DXFS : PDFCollector.CreateFileType.PDFS,
        (int)numericUpDown1.Value,
        textBox1.Text
        ));
    }

    private bool Recurse;

    public bool _recurse {
      get { return Recurse; }
      set { Recurse = value; }
    }

    private bool DeletePDFs;

    public bool _deletePdfs {
      get { return DeletePDFs; }
      set { DeletePDFs = value; }
    }

    private void radioButton1_CheckedChanged(object sender, EventArgs e) {
      if ((sender as RadioButton).Checked) {
        Text = @"Creating and Merging PDFs...";
        checkBox1.Checked = Properties.Settings.Default.AutoDeletePreMergedPDFs;
        checkBox1.Enabled = true;
        label1.Visible = false;
        numericUpDown1.Visible = false;
      }
    }

    private void radioButton2_CheckedChanged(object sender, EventArgs e) {
      if ((sender as RadioButton).Checked) {
        Text = @"Creating DXFs...";
        checkBox1.Checked = false;
        checkBox1.Enabled = false;
        label1.Visible = true;
        numericUpDown1.Visible = true;
      }
    }

    private void textBox1_MouseDoubleClick(object sender, MouseEventArgs e) {
      FolderBrowserDialog fbd = new FolderBrowserDialog();
      fbd.SelectedPath = Properties.Settings.Default.TargetPath;
      if (fbd.ShowDialog() == DialogResult.OK) {
        textBox1.Text = fbd.SelectedPath;
        Properties.Settings.Default.TargetPath = fbd.SelectedPath;
      }
    }
  }

  public class BOMEventArgs : EventArgs {
    public BOMEventArgs(BomFeature bomf) {
      BOM = bomf;
      //if (e.BOM is BomFeature) {
      //  m.AppendLine(string.Format(@"Selected {0}.", e.BOM.Name));
      //}
    }

    private BomFeature _bom;

    public BomFeature BOM {
      get { return _bom; }
      set { _bom = value; }
    }

  }

  public class GoEventArgs : EventArgs {
    public GoEventArgs(bool recurse, bool delete, PDFCollector.CreateFileType type, int compression, string target) {
      Recurse = recurse;
      DeletePDFs = delete;
      TypeToCreate = type;
      CompressionLevel = compression;
      TargetDir = target;
    }

    private PDFCollector.CreateFileType _typeToCreate;

    public PDFCollector.CreateFileType TypeToCreate {
      get { return _typeToCreate; }
      set { _typeToCreate = value; }
    }

    private bool _delete;

    public bool DeletePDFs {
      get { return _delete; }
      set { _delete = value; }
    }


    private bool _recurse;

    public bool Recurse {
      get { return _recurse; }
      set { _recurse = value; }
    }
    
    private int _compression;

    public int CompressionLevel {
      get { return _compression; }
      set { _compression = value; }
    }

    private string _target;

    public string TargetDir {
      get { return _target; }
      set { _target = value; }
    }
  }
}
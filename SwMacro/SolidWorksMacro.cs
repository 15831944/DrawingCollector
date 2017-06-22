using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Collections.Generic;

namespace ProtoDrawingCollector.csproj {
  public partial class SolidWorksMacro {
    public void Main() {
      if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable()) {
        Redbrick_Addin.CutlistData cd = new Redbrick_Addin.CutlistData(Properties.Settings.Default.ConnectionString);
        try {
          cd.IncrementOdometer(Redbrick_Addin.CutlistData.Functions.DrawingCollector);
        } catch (Exception) {
          //ignore
        }
      }
      di = Path.GetDirectoryName((swApp.ActiveDoc as ModelDoc2).GetPathName());
      DrawingData d = new DrawingData(di);
      pc = new PDFCollector(swApp, Properties.Settings.Default.TableHashes, d);
      m = new Message();
      m.Show();

      Message.click_go += new Message.click_EventHandler(Message_click_go);
    }

    public void Message_click_go(object sender, GoEventArgs e) {
      m.AppendLine("Collecting PDF paths...");
      _autodelete = e.DeletePDFs;
      pc.Recurse = e.Recurse;
      try {
        PDFCollector.file_added += m.AppendLineEvent;
        PDFCollector.done += MergeEvent;
        pc.Collect();
      } catch (Exception ex) {
        string msg = string.Format("[EE] Message: {0}\n[EE] In: {1}\n[EE] Stack trace: {2}\n[EE] Source: {3}", 
          ex.Message, 
          ex.TargetSite.ToString(), 
          ex.StackTrace, 
          ex.Source);
        m.AppendLine(msg);
        //m.AppendLine("{\rtf1\ansi \b " + msg + "\b0}");
      }
    }

    private void MergeEvent(object o, EventArgs e) {
      Merge();
    }

    private void Merge() {
      int l = pc.PDFCollection.Count;
      bool last = false;
      m.Append("\nMerging ");
      foreach (FileInfo s in pc.PDFCollection) {
        if (s != null) {
          last = l-- < 2;
          m.Append((last ? "and " : string.Empty) + s.Name + (last ? ".\n" : ", "));
          m.Refresh();
        }
      }

      m.Append("\n");

      foreach (KeyValuePair<string, string> n in pc.NotFound) {
        m.AppendLine(string.Format("No drawing for '{0}' in '{1}'.",
          n.Key,
          n.Value.Split(new string[] { " - " }, StringSplitOptions.None)[0].Trim()));
      }

      try {
        tmpPath = Path.GetTempFileName().Replace(".tmp", ".PDF");
        path = di + @"\" + pc.PDFCollection[0].Name.Replace(".PDF", Properties.Settings.Default.Suffix + ".PDF");
        PDFMerger pm = new PDFMerger(pc.PDFCollection, new FileInfo(tmpPath));
        PDFMerger.deleting_file += m.AppendLineEvent;
        pm.Merge();
      } catch (Exception e) {
        m.AppendLine(e.Message);
      }

      try {
        File.Copy(tmpPath, path, true);
      } catch (Exception e) {
        m.AppendLine(e.Message);
      }

      m.DisableGo();
      m.AppendLine("Created '" + path + "'.");
      m.AppendLine("Opening...");
      GCandOpen();

      if (_autodelete) {
        PDFMerger.delete_pdfs(pc.PDFCollection);
      }
    }

    private void GCandOpen() {
      System.Diagnostics.Process.Start(path);
      System.GC.Collect(0, GCCollectionMode.Forced);
    }

    private Message m;
    private PDFCollector pc;
    private string di;
    private string tmpPath;
    private string path;

    private bool _autodelete;

    /// <summary>
    ///  The SldWorks swApp variable is pre-assigned for you.
    /// </summary>
    public SldWorks swApp;
  }
}

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Runtime.InteropServices;
using System;
using System.IO;
using System.Collections.Generic;

namespace ProtoDrawingCollector.csproj {
  public partial class SolidWorksMacro {
    public void Main() {
      m = new Message();
      di = Path.GetDirectoryName((swApp.ActiveDoc as ModelDoc2).GetPathName());
      DrawingData d = new DrawingData(di);
      pc = new PDFCollector(swApp, Properties.Settings.Default.TableHashes, d);
      m.Show();
      m.AppendLine("Collecting PDF paths...");

      try {
        PDFCollector.file_added += m.AppendLineEvent;
        PDFCollector.done += MergeEvent;
        pc.Collect();
      } catch (Exception e) {
        m.AppendLine(e.Message);
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
        pm.Merge();
      } catch (Exception e) {
        m.AppendLine(e.Message);
      }

      try {
        File.Copy(tmpPath, path, true);
      } catch (Exception e) {
        m.AppendLine(e.Message);
      }

      m.AppendLine("Created '" + path + "'.");
      m.AppendLine("Opening...");
      GCandOpen();
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

    /// <summary>
    ///  The SldWorks swApp variable is pre-assigned for you.
    /// </summary>
    public SldWorks swApp;
  }
}

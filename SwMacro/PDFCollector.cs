using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Runtime.InteropServices;

class PDFCollector {
  private List<FileInfo> lfi = new List<FileInfo>();
  private List<KeyValuePair<string, string>> nf = new List<KeyValuePair<string, string>>();

  public PDFCollector(SldWorks swApp, System.Collections.Specialized.StringCollection hashes, DrawingData dd) {
    _swApp = swApp;
    _hashes = hashes;
    _d = dd;
  }

  public static event EventHandler file_added;
  public static event EventHandler done;
  public delegate void AppendEvent(object o, EventArgs e);

  public static void OnAppend(EventArgs e) {
    EventHandler handler = file_added;
    if (handler != null) {
      handler(new object(), e);
    }
  }

  public static void OnDone(EventArgs e) {
    EventHandler handler = done;
    if (handler != null) {
      handler(new object(), e);
    }
  }

  public void Collect() {
    string fullpath = (SwApp.ActiveDoc as ModelDoc2).GetPathName();
    FileInfo top_level = d.GetPath(Path.GetFileNameWithoutExtension(fullpath));
    lfi.Add(top_level);
    
    collect_drwgs((ModelDoc2)SwApp.ActiveDoc);
    OnDone(EventArgs.Empty);
  }

  private void collect_drwgs(ModelDoc2 md) {
    SWTableType swt = null;
    string title = md.GetTitle();
    try {
      swt = new SWTableType(md, Hashes);
    } catch (Exception e) {
      System.Diagnostics.Debug.WriteLine(e.Message);
    }

    List<FileInfo> ss = new List<FileInfo>();
    if (swt != null) {
      string part = string.Empty;
      bool in_lfi;
      bool in_nf;
      for (int i = 1; i < swt.RowCount; i++) {
        System.Diagnostics.Debug.WriteLine("table: " + swt.GetProperty(i, "PART"));
        part = swt.GetProperty(i, "PART");
        //if (!part.StartsWith("0")) {
        FileInfo fi = swt.get_path(part);
        in_lfi = is_in(part, lfi);
        in_nf = is_in(part, nf);
        if (fi != null) {
          if (!in_lfi) {
            ss.Add(fi);
            OnAppend(new AppendEventArgs(fi.ToString()));
          } else {
            break;
          }
        } else {
          if (!in_nf) {
            nf.Add(new KeyValuePair<string, string>(part, title));
            OnAppend(new AppendEventArgs(part + " NOT found"));
          } else {
            break;
          }
        }
        //} else {
        //  System.Diagnostics.Debug.WriteLine("Skipping " + part);
        //}
      }

      lfi.AddRange(ss);
    }

    //if (ss.Count > 0) {
    //  foreach (FileInfo f in ss) {
    //    if (f != null) {
    //      string doc = f.FullName.ToUpper().Replace(".PDF", ".SLDDRW");
    //      OnAppend(new AppendEventArgs(string.Format("Opening '{0}'...", doc)));
    //      SwApp.OpenDoc(doc, (int)swDocumentTypes_e.swDocDRAWING);
    //      SwApp.ActivateDoc(doc);
    //      ModelDoc2 m = (ModelDoc2)SwApp.ActiveDoc;
    //      System.Diagnostics.Debug.WriteLine("ss   : " + f.Name);
    //      System.Diagnostics.Debug.WriteLine(doc);
    //      collect_drwgs(m);
    //      OnAppend(new AppendEventArgs(string.Format("Closing '{0}'...", doc)));
    //      SwApp.CloseDoc(doc);
    //    }
    //  }
    //}
  }

  

  public static bool is_in(FileInfo f, List<FileInfo> l) {
    foreach (FileInfo fi in l) {
      if (f != null && Path.GetFileNameWithoutExtension(f.Name).ToUpper() == 
        Path.GetFileNameWithoutExtension(fi.Name).ToUpper()) {
        return true;
      }
    }
    return false;
  }

  public static bool is_in(FileInfo f, List<KeyValuePair<string, string>> l) {
    foreach (KeyValuePair<string, string> fi in l) {
      if (f != null && Path.GetFileNameWithoutExtension(f.Name).ToUpper() == 
  Path.GetFileNameWithoutExtension(fi.Key).ToUpper()) {
        return true;
      }
    }
    return false;
  }

  public static bool is_in(string f, List<FileInfo> l) {
    foreach (FileInfo fi in l) {
      try {
        if (f != null && f.ToUpper() == Path.GetFileNameWithoutExtension(fi.Name).ToUpper()) {
          return true;
        }
      } catch (NullReferenceException n) {
        OnAppend(new AppendEventArgs(string.Format("{0}: {1} != null", n.Message, f)));
        return false;
      }
    }
    return false;
  }

  public static bool is_in(string f, List<KeyValuePair<string, string>> l) {
    foreach (KeyValuePair<string, string> fi in l) {
      if (f != null && f.ToUpper() == Path.GetFileNameWithoutExtension(fi.Key).ToUpper()) {
        return true;
      }
    }
    return false;
  }

  public List<FileInfo> PDFCollection {
    get { return lfi; }
    set { lfi = value; }
  }

  public List<KeyValuePair<string, string>> NotFound {
    get { return nf; }
    set { nf = value; }
  }

  private SldWorks _swApp;

  public SldWorks SwApp {
    get { return _swApp; }
    private set { _swApp = value; }
  }

  private System.Collections.Specialized.StringCollection _hashes;

  public System.Collections.Specialized.StringCollection Hashes {
    get { return _hashes; }
    private set { _hashes = value; }
  }

  private DrawingData _d;

  public DrawingData d {
    get { return _d; }
    private set { _d = value; }
  }

  class AppendEventArgs : EventArgs {
    public AppendEventArgs() {
      _msg = string.Empty;
    }


    public AppendEventArgs(string msg) {
      _msg = msg;
    }

    public override string ToString() {
      return _msg;
    }

    private string _msg;

    public string Message {
      get { return _msg; }
      set { _msg = value; }
    }

  }

}

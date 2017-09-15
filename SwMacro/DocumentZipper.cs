using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;

namespace ProtoDrawingCollector.csproj {
  public class DocumentZipper {
    public DocumentZipper(List<FileInfo> lfi, FileInfo target, int lvl) {
      _target = target;
      _file_paths.AddRange(lfi);
      _lvl = lvl;
    }

    public void Merge() {
      ZipFile zf = null;
      try {
        using (ZipOutputStream s = new ZipOutputStream(File.Create(_target.FullName))) {
          s.SetLevel(_lvl);
          byte[] buffer = new byte[4096];
          foreach (FileInfo file in _file_paths) {
            ZipEntry entry = new ZipEntry(file.Name);
            entry.DateTime = DateTime.Now;
            s.PutNextEntry(entry);
            using (FileStream fs = File.OpenRead(file.FullName)) {
              int sourceByts;
              do {
                sourceByts = fs.Read(buffer, 0, buffer.Length);
                s.Write(buffer, 0, sourceByts);
              } while (sourceByts > 0);
            }
          }
          s.Finish();
          s.Close();
        }
      } finally {
        if (zf != null) {
          zf.IsStreamOwner = true;
          zf.Close();
        }
      }
      //byte[] ba = merge_files(_pdf_paths);
      //using (FileStream fs = File.Create(_target.FullName)) {
      //  for (int i = 0; i < ba.Length; i++) {
      //    fs.WriteByte(ba[i]);
      //  }
      //  fs.Close();
      //}
    }
    
    public static event EventHandler deleting_file;
    public delegate void AppendEvent(object o, EventArgs e);

    public static void OnAppend(EventArgs e) {
      EventHandler handler = deleting_file;
      if (handler != null) {
        handler(new object(), e);
      }
    }

    private FileInfo _target;

    public FileInfo Target {
      get { return _target; }
      set { _target = value; }
    }

    private int _lvl;

    public int CompressionLevel {
      get { return _lvl; }
      set { _lvl = value; }
    }


    private List<FileInfo> _file_paths = new List<FileInfo>();

    public List<FileInfo> PDFCollection {
      get { return _file_paths; }
      set { _file_paths = value; }
    }
  }
}

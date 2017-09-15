using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Data.SqlClient;

public class DrawingData {
  public DrawingData(string current_dir) {
    CurrentWorkingDirectory = new DirectoryInfo(current_dir);
  }

  public FileInfo GetPath(string filename) {
    try {
      return CurrentWorkingDirectory.GetFiles(filename + ".PDF", SearchOption.TopDirectoryOnly)[0];
    } catch (IndexOutOfRangeException) {
      return null;
    }
  }

  private DirectoryInfo _cwd;

  public DirectoryInfo CurrentWorkingDirectory {
    get { return _cwd; }
    private set { _cwd = value; }
  }

}
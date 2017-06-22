using System;
using System.Data;
using System.Data.Odbc;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Redbrick_Addin {
  public class CutlistData : IDisposable {
    private bool ENABLE_DB_WRITE = true;
    private object threadLock = new object();
    private OdbcConnection conn;

    public OdbcConnection Connection {
      get { return conn; }
      private set { conn = value; }
    }

    public CutlistData(string connection_string) {
      conn = new OdbcConnection(connection_string);
      OpType = 1;
      conn.Open();
    }

    public void Dispose() {
      conn.Close();
    }

    public int GetCurrentAuthor() {
      string SQL = @"SELECT UID FROM GEN_USERS WHERE USERNAME = ?";
      using (OdbcCommand comm = new OdbcCommand(SQL, conn)) {
        comm.Parameters.AddWithValue("@uname", Environment.UserName);
        using (OdbcDataReader dr = comm.ExecuteReader()) {
          if (dr.Read() && !dr.IsDBNull(0))
            return dr.GetInt32(0);
          else
            return 0;
        }
      }
    }

    public enum Functions {
      GreenCheck,
      ArchivePDF,
      InsertECR,
      ExamineBOM,
      MaterialList,
      UpdateCutlistPart,
      UpdateCutlist,
      DrawingCollector,
      ExportPrograms,
      ConvertPrograms
    }

    public void IncrementOdometer(Functions funcCode) {
      if (ENABLE_DB_WRITE) {
        int current_value = GetOdometerValue(funcCode);
        int rowsAffected = 0;
        string SQL = @"UPDATE GEN_ODOMETER SET ODO = ? WHERE (FUNCID = ? AND USERID = ?);";

        using (OdbcCommand comm = new OdbcCommand(SQL, conn)) {
          comm.Parameters.AddWithValue("@odo", ++current_value);
          comm.Parameters.AddWithValue("@app", funcCode);
          comm.Parameters.AddWithValue("@user", GetCurrentAuthor());
          try {
            rowsAffected = comm.ExecuteNonQuery();
          } catch (InvalidOperationException ioe) {
            throw ioe;
          }
        }

        if (rowsAffected == 0) {
          SQL = @"INSERT INTO GEN_ODOMETER (ODO, FUNCID, USERID) VALUES (?, ?, ?);";

          using (OdbcCommand comm = new OdbcCommand(SQL, conn)) {
            comm.Parameters.AddWithValue("@odo", 1);
            comm.Parameters.AddWithValue("@app", (int)funcCode);
            comm.Parameters.AddWithValue("@user", GetCurrentAuthor());
            try {
              rowsAffected = comm.ExecuteNonQuery();
            } catch (InvalidOperationException ioe) {
              throw ioe;
            }
          }
        }

      }
    }

    private int GetOdometerValue(Functions funcCode) {
      using (OdbcCommand comm = new OdbcCommand(@"SELECT ODO FROM GEN_ODOMETER WHERE (FUNCID = ? AND USERID = ?)", conn)) {
        comm.Parameters.AddWithValue("@app", (int)funcCode);
        comm.Parameters.AddWithValue("@user", GetCurrentAuthor());
        using (OdbcDataReader dr = comm.ExecuteReader()) {
          if (dr.Read() && !dr.IsDBNull(0))
            return dr.GetInt32(0);
          else
            return 0;
        }
      }
    }

    public void InsertError(int errNo, string errmsg, string targetSite) {
      string SQL = "INSERT INTO GEN_ERRORS (ERRDATE, ERRUSER, ERRNUM, ERRMSG, ERROBJ, ERRCHK, ERRAPP) VALUES " +
        "(GETDATE(), ?, ?, ?, ?, ?, ?)";
      using (OdbcCommand comm = new OdbcCommand(SQL, conn)) {
        comm.Parameters.AddWithValue("@ErrUser", GetCurrentAuthor());
        comm.Parameters.AddWithValue("@ErrNum", errNo);
        comm.Parameters.AddWithValue("@ErrMsg", errmsg);
        comm.Parameters.AddWithValue("@ErrObj", targetSite);
        comm.Parameters.AddWithValue("@ErrChk", -1);
        comm.Parameters.AddWithValue("@ErrApp", @"Property Editor");
        comm.ExecuteNonQuery();
      }
    }

    private void parse(string s, out double d) {
      double dtp = 0.0f;
      if (double.TryParse(s, out dtp))
        d = dtp;
      else
        d = 0.0f;
    }

    private void parse(string s, out int i) {
      int itp = 0;
      if (int.TryParse(s, out itp))
        i = itp;
      else
        i = 0;
    }

    static public void FilterTextForControl(System.Windows.Forms.Control c) {
      //if (c is System.Windows.Forms.TextBox) {
      //  System.Windows.Forms.TextBox d = (c as System.Windows.Forms.TextBox);
      //  int pos = d.SelectionStart;
      //  d.Text = FilterString(d.Text);
      //  d.SelectionStart = pos;
      //} else if (c is System.Windows.Forms.ComboBox) {
      //  System.Windows.Forms.ComboBox d = (c as System.Windows.Forms.ComboBox);
      //  int pos = d.SelectionStart;
      //  d.Text = FilterString(d.Text);
      //  d.SelectionStart = pos;
      //}
    }

    public static string FilterString(string raw) {
      string filtered = raw.ToUpper();
      char[,] chars = new char[,] {
                     {'\u0027', '\u2032'},
                     {'\u0022', '\u2033'},
                     {';', '\u037E'},
                     {'%', '\u066A'},
                     {'*', '\u2217'}};

      for (int j = 0; j < chars.GetLength(0); j++) {
        filtered = filtered.Replace(chars[j, 0], chars[j, 1]);
      }

      return filtered.Trim();
    }

    public static string FilterString(string raw, bool flame) {
      string filtered = string.Empty;

      if (flame) {
        filtered = raw.ToUpper();
      } else {
        filtered = raw;
      }
      char[,] chars = new char[,] {
                     {'\u0027', '\u2032'},
                     {'\u0022', '\u2033'},
                     {';', '\u037E'},
                     {'%', '\u066A'},
                     {'*', '\u2217'}};

      for (int j = 0; j < chars.GetLength(0); j++) {
        filtered = filtered.Replace(chars[j, 0], chars[j, 1]);
      }

      return filtered.Trim();
    }

    public int UpdateMachinePrograms() {
      return -1;
    }

    private string ReturnString(OdbcDataReader dr, int i) {
      if (dr.Read()) {
        if (dr.IsDBNull(i)) {
          return string.Empty;
        } else {
          string returnString = dr.GetValue(i).ToString();
          return returnString;
        }
      }
      return string.Empty;
    }

    private int _opType;

    public int OpType {
      get { return _opType; }
      set {
        if (value > 2)
          _opType = 1;
        else
          _opType = value;
      }
    }

  }
}

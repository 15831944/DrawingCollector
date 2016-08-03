#define PAGE_NUMBERS
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

using iTextSharp.text;
using iTextSharp.text.pdf;

class PDFMerger {
  public PDFMerger(List<FileInfo> lfi, FileInfo target) {
    _target = target;
    _pdf_paths.AddRange(lfi);
  }

  public void Merge() {
    byte[] ba = merge_files(_pdf_paths);
    using (FileStream fs = File.Create(_target.FullName)) {
      for (int i = 0; i < ba.Length; i++) {
        fs.WriteByte(ba[i]);
      }
    }
  }

  public static int count_pages(List<FileInfo> docs) {
    int total = 0;
    foreach (FileInfo fi in docs) {
      PdfReader reader = new PdfReader(fi.FullName);
      total += reader.NumberOfPages;
    }
    return total;
  }

  public static byte[] merge_files(List<FileInfo> docs) {
    Document document = new Document();
    using (MemoryStream ms = new MemoryStream()) {
      PdfCopy copy = new PdfCopy(document, ms);
      document.Open();
      int document_page_counter = 0;
      int total_pages = count_pages(docs);
      System.Drawing.Point location = ProtoDrawingCollector.csproj.Properties.Settings.Default.PageStampLoc;
      int font_size = ProtoDrawingCollector.csproj.Properties.Settings.Default.PageStampSize;
      foreach (FileInfo fi in docs) {
        PdfReader reader = new PdfReader(fi.FullName);

        for (int i = 1; i <= reader.NumberOfPages; i++) {
          document_page_counter++;
          PdfImportedPage ip = copy.GetImportedPage(reader, i);
#if PAGE_NUMBERS
          PdfCopy.PageStamp ps = copy.CreatePageStamp(ip);
          PdfContentByte cb = ps.GetOverContent();
          //System.Drawing.Rectangle sdr = new System.Drawing.Rectangle(1154, 20, 47, 16);
          System.Drawing.Rectangle sdr = ProtoDrawingCollector.csproj.Properties.Settings.Default.PageStampWhiteoutRectangle;
          Rectangle r = new Rectangle(sdr.Left, sdr.Bottom, sdr.Right, sdr.Top);
          r.BackgroundColor = BaseColor.WHITE;
          cb.Rectangle(r);
          Font f = FontFactory.GetFont("Century Gothic", font_size);
          Chunk c = new Chunk(string.Format("{0} OF {1}", document_page_counter, total_pages), f);
          c.SetBackground(BaseColor.WHITE);
          ColumnText.ShowTextAligned(ps.GetOverContent(),
            Element.ALIGN_CENTER,
            new Phrase(c),
            location.X, location.Y, 
            ip.Width < ip.Height ? 0 : 1);
          ps.AlterContents();
#endif
          copy.AddPage(ip);
        }

        copy.FreeReader(reader);
        reader.Close();
        if (ProtoDrawingCollector.csproj.Properties.Settings.Default.AutoDeletePreMergedPDFs) {
          fi.Delete();
        }
      }
      document.Close();
      return ms.GetBuffer();
    }
  }

  private FileInfo _target;

  public FileInfo Target {
    get { return _target; }
    set { _target = value; }
  }


  private List<FileInfo> _pdf_paths = new List<FileInfo>();

  public List<FileInfo> PDFCollection {
    get { return _pdf_paths; }
    set { _pdf_paths = value; }
  }

}

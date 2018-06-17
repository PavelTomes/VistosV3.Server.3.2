using Core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Core.ActionResults
{
    public class DataExportResult : ActionResult
    {
        private ExportType _exportType;
        private string _fileName;
        private List<Column> _columns;
        private Column[] _columnsVisible;
        private DataTable _table;
        private string _tableName;
        private string _userCulture;
        private System.Globalization.CultureInfo _ci = null;
        private System.Globalization.DateTimeFormatInfo _dtfi = null;

        public DataExportResult(
            DataTable itemsToExport,
            ExportType exportType,
            string fileName,
            List<Column> columns,
            Column[] columnsVisible,
            string tableName,
            String userCulture)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ApplicationException("FileName could not be empty");
            if (string.IsNullOrEmpty(tableName)) throw new ApplicationException("TableName could not be empty");
            _table = itemsToExport;
            _exportType = exportType;
            _fileName = fileName;
            _columns = columns;
            _columnsVisible = columnsVisible;
            _tableName = tableName;
            _userCulture = userCulture;

            _ci = System.Globalization.CultureInfo.CreateSpecificCulture(this._userCulture);
            _dtfi = _ci.DateTimeFormat;
        }

        //public override void ExecuteResult(ControllerContext context)
        //{
        //    context.HttpContext.Response.ClearContent();
        //    context.HttpContext.Response.Buffer = true;
        //    context.HttpContext.Response.AddHeader("Content-Disposition", String.Format("attachment; filename={0}.{1}", _fileName, _exportType == ExportType.Excel ? "xlsx" : Structures.GetExtension(_exportType)));
        //    context.HttpContext.Response.ContentType = Structures.GetMimeType(_exportType);

        //    DataView view = new DataView(_table);
        //    _table = view.ToTable(false, _columnsVisible.Select(c => c.ColumnName).ToArray());


        //    if (_exportType == ExportType.Excel)
        //    {
        //        using (ExcelPackage pck = new ExcelPackage())
        //        {
        //            ExcelWorksheet ws = pck.Workbook.Worksheets.Add(_tableName);

        //            ws.Cells["A1"].LoadFromDataTable(_table, true);

        //            // Localize headers
        //            List<string> usedColumnNames = new List<string>();
        //            foreach (ExcelRangeBase headerCell in ws.Cells[1, 1, 1, _table.Columns.Count])
        //            {
        //                Column col = null;

        //                if (_columns != null)
        //                {
        //                    col = _columns.Where<Column>(c => c.ColumnName.Equals(headerCell.Text)).FirstOrDefault();
        //                }

        //                headerCell.Value = GenerateUniqueColumnName(col != null ? col.LocalizationString : headerCell.Value.ToString(), usedColumnNames);
        //            }

        //            // Fix data type
        //            int tmpInt;
        //            Int64 tmpInt64;
        //            decimal tmpDecimal;
        //            DateTime tmpDate;
        //            foreach (ExcelRangeBase cell in ws.Cells)
        //            {
        //                Column col = null;
        //                if (_columnsVisible != null && _columnsVisible.Count() > 0 && cell.Start != null && _columnsVisible.Count() >= cell.Start.Column)
        //                {
        //                    Column colVisible = _columnsVisible[cell.Start.Column - 1];
        //                    if (_columns != null && _columns.Count > 0 && colVisible != null && !string.IsNullOrEmpty(colVisible.ColumnName))
        //                    {
        //                        col = _columns.Where(c => c.ColumnName == colVisible.ColumnName).FirstOrDefault();
        //                    }
        //                }

        //                if (col != null)
        //                {
        //                    switch (col.Type_FK)
        //                    {
        //                        case DbColumnTypeEnum.Int:
        //                            if (int.TryParse(cell.Text, out tmpInt))
        //                            {
        //                                cell.Value = tmpInt;
        //                            }
        //                            break;
        //                        case DbColumnTypeEnum.Float:
        //                            if (decimal.TryParse(cell.Text, out tmpDecimal))
        //                            {
        //                                cell.Value = tmpDecimal;
        //                            }
        //                            break;
        //                        case DbColumnTypeEnum.Money:
        //                            if (decimal.TryParse(cell.Text, out tmpDecimal))
        //                            {
        //                                cell.Value = tmpDecimal;
        //                            }
        //                            break;
        //                        case DbColumnTypeEnum.BigInt:
        //                            if (Int64.TryParse(cell.Text, out tmpInt64))
        //                            {
        //                                cell.Value = tmpInt64;
        //                            }
        //                            break;

        //                        case DbColumnTypeEnum.Bit:
        //                        case DbColumnTypeEnum.String:
        //                        case DbColumnTypeEnum.Text:
        //                        case DbColumnTypeEnum.Enumeration:
        //                        case DbColumnTypeEnum.MultiEnumeration:
        //                        case DbColumnTypeEnum.Entity:
        //                        case DbColumnTypeEnum.Guid:
        //                        case DbColumnTypeEnum.Geography:
        //                        case DbColumnTypeEnum.EntityEnumeration:
        //                        case DbColumnTypeEnum.EmailAddress:
        //                        case DbColumnTypeEnum.WebUrl:
        //                        case DbColumnTypeEnum.PhoneNumber:
        //                            break;

        //                        case DbColumnTypeEnum.Date:
        //                            if (DateTime.TryParse(cell.Text, out tmpDate))
        //                            {
        //                                cell.Value = tmpDate;
        //                                cell.Style.Numberformat.Format = _dtfi.ShortDatePattern;
        //                            }
        //                            break;
        //                        case DbColumnTypeEnum.Time:
        //                            if (DateTime.TryParse(cell.Text, out tmpDate))
        //                            {
        //                                cell.Value = tmpDate;
        //                                cell.Style.Numberformat.Format = _dtfi.ShortTimePattern;
        //                            }
        //                            break;
        //                        case DbColumnTypeEnum.DateTime:
        //                            if (DateTime.TryParse(cell.Text, out tmpDate))
        //                            {
        //                                cell.Value = tmpDate;
        //                                cell.Style.Numberformat.Format = _dtfi.FullDateTimePattern;
        //                            }
        //                            break;

        //                        case DbColumnTypeEnum.DateCheckBox:
        //                        case DbColumnTypeEnum.Signature:
        //                        case DbColumnTypeEnum.Varbinary:
        //                        case DbColumnTypeEnum.Image:
        //                        case DbColumnTypeEnum.XML:
        //                        case DbColumnTypeEnum.TextSimple:
        //                        case DbColumnTypeEnum.WidgetHtml:
        //                        case DbColumnTypeEnum.SystemChart:
        //                        case DbColumnTypeEnum.UserChart:
        //                        case DbColumnTypeEnum.IconSelect:
        //                        case DbColumnTypeEnum.MultiStateBox:
        //                        case DbColumnTypeEnum.JSON:
        //                        case DbColumnTypeEnum.DashboardTable:
        //                        case DbColumnTypeEnum.BitIcon:
        //                            break;
        //                    }
        //                }
        //                else
        //                {
        //                    if (cell.Text.StartsWith("+") || (cell.Text.StartsWith("0") && !cell.Text.StartsWith("0,") && !cell.Text.StartsWith("0.")))
        //                        continue; // Phone number - keep it as a text

        //                    if (int.TryParse(cell.Text, out tmpInt))
        //                    {
        //                        cell.Value = tmpInt;
        //                    }
        //                    else if (decimal.TryParse(cell.Text, out tmpDecimal))
        //                    {
        //                        cell.Value = tmpDecimal;
        //                    }
        //                    else if (DateTime.TryParse(cell.Text, out tmpDate))
        //                    {
        //                        cell.Value = tmpDate;
        //                        cell.Style.Numberformat.Format = _dtfi.ShortDatePattern;
        //                    }
        //                }
        //            }

        //            // Format as Table
        //            // select the range that will be included in the table
        //            ExcelRange range = ws.Cells[1, 1, _table.Rows.Count + 1, _table.Columns.Count];
        //            // add the excel table entity
        //            ExcelTable table = ws.Tables.Add(range, new String(_tableName.Where(c => char.IsLetterOrDigit(c)).ToArray()));
        //            table.ShowFilter = false;
        //            table.ShowTotal = false;
        //            table.TableStyle = TableStyles.Light2;

        //            ws.Cells.AutoFitColumns();

        //            if (context.HttpContext.Response.IsClientConnected)
        //            {
        //                context.HttpContext.Response.Clear();
        //                context.HttpContext.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        //                context.HttpContext.Response.BinaryWrite(pck.GetAsByteArray());
        //                context.HttpContext.Response.Flush();
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // _table value format

        //        foreach (var item in _columns)
        //        {
        //            switch (item.Type_FK)
        //            {
        //                case DbColumnTypeEnum.DateTime:
        //                    ConvertDataTable.ConvertToDate(_table.Columns[item.ColumnName], _userCulture);
        //                    break;
        //                case DbColumnTypeEnum.Bit:
        //                    ConvertDataTable.Convert(_table.Columns[item.ColumnName], _userCulture);
        //                    break;
        //            }
        //        }

        //        GridView grid = new GridView();
        //        grid.DataSource = _table;
        //        grid.DataBind();

        //        // Localize headers
        //        if (_columns != null && grid.HeaderRow != null)
        //        {
        //            foreach (TableCell headerCell in grid.HeaderRow.Cells)
        //            {
        //                Column col = _columns.Where<Column>(c => c.ColumnName.Equals(headerCell.Text)).FirstOrDefault();
        //                if (col != null)
        //                {
        //                    headerCell.Text = col.LocalizationString;

        //                }
        //            }
        //        }

        //        StringWriter sw = new StringWriter();
        //        HtmlTextWriter htw = new HtmlTextWriter(sw);

        //        grid.RenderControl(htw);

        //        if (_exportType == ExportType.PDF)
        //        {
        //            MemoryStream ms = new MemoryStream();

        //            StringReader sr = new StringReader(sw.ToString());
        //            context.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);

        //            using (Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f))
        //            {

        //                pdfDoc.AddCreationDate();
        //                pdfDoc.AddCreator("VistosCRM - Euro Softworks s.r.o.");
        //                pdfDoc.AddHeader("Charset", "utf-8");

        //                HTMLWorker htmlparser = new HTMLWorker(pdfDoc);

        //                PdfWriter writer = PdfWriter.GetInstance(pdfDoc, ms);
        //                pdfDoc.Open();
        //                StyleSheet st = new StyleSheet();
        //                // Registrace ceskeho kodovani
        //                FontFactory.Register(@"c:\windows\fonts\arial.ttf");
        //                st.LoadTagStyle("body", "face", "arial");
        //                st.LoadTagStyle("body", "encoding", "Identity-H");
        //                htmlparser.SetStyleSheet(st);

        //                htmlparser.Parse(sr);
        //                pdfDoc.Close();

        //                if (context.HttpContext.Response.IsClientConnected)
        //                {
        //                    context.HttpContext.Response.OutputStream.Write(ms.GetBuffer(), 0, ms.GetBuffer().Length);
        //                    context.HttpContext.Response.OutputStream.Flush();
        //                    context.HttpContext.Response.End();
        //                }
        //            }
        //        }
        //        else // Word + HTML
        //        {
        //            string charset = "utf-8";
        //            string htmlEnvelope = "<html><head><meta http-equiv=\"content-type\" content=\"text/html; charset={1}\"></head><body>{0}</body></html>";

        //            if (context.HttpContext.Response.IsClientConnected)
        //            {
        //                context.HttpContext.Response.Charset = charset;
        //                context.HttpContext.Response.ContentEncoding = Encoding.UTF8;
        //                context.HttpContext.Response.Write(String.Format(htmlEnvelope, sw.ToString(), charset));
        //                context.HttpContext.Response.Flush();
        //            }
        //        }
        //    }
        //}


        private string GenerateUniqueColumnName(string columnName, List<string> usedColumnNames)
        {
            if (columnName == null)
                columnName = string.Empty;

            columnName = columnName.Trim();

            if (usedColumnNames.Count(i => i.Equals(columnName, StringComparison.InvariantCultureIgnoreCase)) == 0 && columnName != string.Empty)
            {
                usedColumnNames.Add(columnName);
                return columnName;
            }

            int v = 1;
            while (true)
            {
                string tmpName = string.Format("{0} ({1})", columnName, v++);

                if (usedColumnNames.Count(i => i.Equals(tmpName, StringComparison.InvariantCultureIgnoreCase)) == 0)
                {
                    usedColumnNames.Add(tmpName);
                    return tmpName.Trim();
                }
            }
        }
    }
}

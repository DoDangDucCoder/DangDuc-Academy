using ClosedXML.Excel;

namespace HDNXUdemyAPI.Helpers
{
    /// <summary>
    /// ExcelExportHelper - Generates Excel files from data collections
    /// </summary>
    public static class ExcelExportHelper
    {
        /// <summary>
        /// Export a list of objects to Excel byte array
        /// </summary>
        public static byte[] ExportToExcel<T>(IEnumerable<T> data, string sheetName = "Sheet1")
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(sheetName);

            var properties = typeof(T).GetProperties()
                .Where(p => p.PropertyType.IsPrimitive ||
                            p.PropertyType == typeof(string) ||
                            p.PropertyType == typeof(decimal) ||
                            p.PropertyType == typeof(DateTime) ||
                            p.PropertyType == typeof(DateTime?) ||
                            p.PropertyType == typeof(decimal?) ||
                            p.PropertyType == typeof(int?) ||
                            p.PropertyType == typeof(long?) ||
                            p.PropertyType == typeof(bool?) ||
                            p.PropertyType == typeof(double?) ||
                            Nullable.GetUnderlyingType(p.PropertyType)?.IsPrimitive == true)
                .ToList();

            // Headers
            for (int i = 0; i < properties.Count; i++)
            {
                var headerCell = worksheet.Cell(1, i + 1);
                headerCell.Value = properties[i].Name;
                headerCell.Style.Font.Bold = true;
                headerCell.Style.Fill.BackgroundColor = XLColor.FromHtml("#6366f1");
                headerCell.Style.Font.FontColor = XLColor.White;
            }

            // Data rows
            var dataList = data.ToList();
            for (int row = 0; row < dataList.Count; row++)
            {
                for (int col = 0; col < properties.Count; col++)
                {
                    var value = properties[col].GetValue(dataList[row]);
                    var cell = worksheet.Cell(row + 2, col + 1);
                    if (value != null)
                    {
                        cell.SetValue(value.ToString());
                    }
                }
            }

            // Auto-fit columns
            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}

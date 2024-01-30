using ClosedXML.Excel;

namespace Core.Helpers.Helpers
{
    public static class ExcelHelper
    {
        public static List<T?> ImportExcel<T>(string excelFilePath, string sheetName)
        {
            List<T?> list = new List<T?>();
            Type typeOfobject = typeof(T);
            using (IXLWorkbook workbook = new XLWorkbook(excelFilePath))
            {
                var worksheet = workbook.Worksheets.FirstOrDefault();
                if (worksheet == null)
                {
                    throw new InvalidOperationException("The worksheet could not be found.");
                }

                var properties = typeOfobject.GetProperties();
                //header column texts
                var columns = worksheet.FirstRow().Cells().Select((v, i) => new { Value = v.Value, Index = i + 1 });

                foreach (IXLRow row in worksheet.RowsUsed().Skip(1))
                {
                    T? obj = (T?)Activator.CreateInstance(typeOfobject) ?? throw new InvalidOperationException("The object could not be created.");
                    foreach (var prop in properties)
                    {
                        var column = columns.SingleOrDefault(c => c.Value.ToString() == prop.Name.ToString());
                        if (column == null)
                        {
                            throw new InvalidOperationException($"The column for property '{prop.Name}' could not be found.");
                        }

                        int colIndex = column.Index;
                        var val = row.Cell(colIndex).Value;
                        var type = prop.PropertyType;
                        prop.SetValue(obj, Convert.ChangeType(val, type));
                    }
                    list.Add(obj);
                }
            }
            return list;
        }

        public static bool ExportExcel<T>(List<T> list, string file, string sheetName)
        {
            bool exported = false;
            using (IXLWorkbook workbook = new XLWorkbook())
            {
                workbook.AddWorksheet(sheetName).FirstCell().InsertTable<T>(list, false);

                workbook.SaveAs(file);
                exported = true;
            }
            return exported;
        }
    }
}
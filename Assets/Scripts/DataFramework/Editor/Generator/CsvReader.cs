#if UNITY_EDITOR
using System;
using System.Data;
using System.IO;
using System.Text;

namespace Tools
{
    public class CsvReader
    {
        public DataTable ReadCsvToDataTable(string csvFilePath)
        {
            try
            {
                if (!File.Exists(csvFilePath))
                {
                    Logger.Error($"CSV file not found: {csvFilePath}");
                    return null;
                }

                DataTable dataTable = new DataTable();
                string[] lines = File.ReadAllLines(csvFilePath, Encoding.UTF8);

                if (lines.Length == 0)
                {
                    Logger.Warning($"CSV file is empty: {csvFilePath}");
                    return null;
                }

                if (lines.Length < 4)
                {
                    Logger.Error($"CSV file must have at least 4 rows (description, data types, column names, data): {csvFilePath}");
                    return null;
                }

                string[] dataTypes = ParseCsvLine(lines[1]);
                string[] columnNames = ParseCsvLine(lines[2]);

                int maxColumns = Math.Max(dataTypes.Length, columnNames.Length);
                for (int i = 0; i < maxColumns; i++)
                {
                    dataTable.Columns.Add($"Column{i}", typeof(string));
                }

                DataRow placeholderRow = dataTable.NewRow();
                for (int j = 0; j < maxColumns; j++)
                {
                    placeholderRow[j] = j < dataTypes.Length ? "placeholder" : "";
                }
                dataTable.Rows.Add(placeholderRow);

                DataRow dataTypeRow = dataTable.NewRow();
                for (int j = 0; j < maxColumns; j++)
                {
                    dataTypeRow[j] = j < dataTypes.Length ? dataTypes[j] : "";
                }
                dataTable.Rows.Add(dataTypeRow);

                DataRow columnNameRow = dataTable.NewRow();
                for (int j = 0; j < maxColumns; j++)
                {
                    columnNameRow[j] = j < columnNames.Length ? columnNames[j] : "";
                }
                dataTable.Rows.Add(columnNameRow);

                for (int i = 3; i < lines.Length; i++)
                {
                    string line = lines[i];

                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] values = ParseCsvLine(line);

                    DataRow row = dataTable.NewRow();
                    for (int j = 0; j < maxColumns; j++)
                    {
                        row[j] = j < values.Length ? values[j] : "";
                    }

                    dataTable.Rows.Add(row);
                }

                return dataTable;
            }
            catch (Exception e)
            {
                Logger.Exception($"Error reading CSV file: {csvFilePath}", e);
                return null;
            }
        }

        private string[] ParseCsvLine(string line)
        {
            if (string.IsNullOrEmpty(line))
                return new string[0];

            var result = new System.Collections.Generic.List<string>();
            bool inQuotes = false;
            StringBuilder currentField = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        currentField.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = !inQuotes;
                    }
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(currentField.ToString());
                    currentField.Clear();
                }
                else
                {
                    currentField.Append(c);
                }
            }

            result.Add(currentField.ToString());

            return result.ToArray();
        }
    }
}
#endif
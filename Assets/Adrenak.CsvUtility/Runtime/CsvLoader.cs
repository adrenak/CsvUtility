using System;
using System.Collections.Generic;
using System.Text;

namespace Adrenak.CsvUtility {
    /// <summary>
    /// Loads a CSV file contents into a string matrix
    /// and provides APIs to retrieve data. Essentially
    /// a wrapper over a string[][] object with helpers.
    /// </summary>
    public class CsvLoader : IDisposable {
        /// <summary>
        /// The internal data represented as
        /// string matrix that stores the CSV cell values
        /// </summary>
        string[][] cells;

        /// <summary>
        /// Whether the instance has any CSV data loaded
        /// </summary>
        public bool HasData => cells != null;

        /// <summary>
        /// The number of rows in a currently loaded CSV file
        /// </summary>
        public int RowCount {
            get {
                if (cells != null)
                    return cells.Length;
                return 0;
            }
        }

        /// <summary>
        /// The number of columns in the currently loaded CSV file
        /// </summary>
        public int ColumnCount {
            get {
                if (cells != null && cells.Length > 0)
                    return cells[0].Length;
                return 0;
            }
        }

        /// <summary>
        /// Takes a CSV in string form and 
        /// initializes the internal data
        /// </summary>
        /// <param name="csv"></param>
        public CsvLoader(string csv) {
            List<string[]> rows = new List<string[]>();
            List<string> currentRow = new List<string>();
            bool insideQuotes = false;
            StringBuilder fieldBuilder = new StringBuilder();

            for (int i = 0; i < csv.Length; i++) {
                char currentChar = csv[i];

                if (currentChar == '\"') {
                    insideQuotes = !insideQuotes;
                    // If we encounter two consecutive double quotes, treat it as an escaped quote
                    if (i < csv.Length - 1 && csv[i + 1] == '\"') {
                        fieldBuilder.Append('\"');
                        i++;
                    }
                }
                else if (currentChar == ',' && !insideQuotes) {
                    currentRow.Add(fieldBuilder.ToString());
                    fieldBuilder.Clear();
                }
                else if (currentChar == '\n' && !insideQuotes) {
                    currentRow.Add(fieldBuilder.ToString());
                    fieldBuilder.Clear();
                    rows.Add(currentRow.ToArray());
                    currentRow.Clear();
                }
                else if (currentChar == '\r') {
                    continue;
                }
                else {
                    fieldBuilder.Append(currentChar);
                }
            }

            if (insideQuotes) {
                throw new ArgumentException("Mismatched quotes in CSV string");
            }

            if (fieldBuilder.Length > 0) {
                currentRow.Add(fieldBuilder.ToString());
            }

            if (currentRow.Count > 0) {
                rows.Add(currentRow.ToArray());
            }
            cells = new string[rows.Count][];
            for (int i = 0; i < rows.Count; i++)
                cells[i] = rows[i];
        }

        /// <summary>
        /// Returns the data stored as a given row and column
        /// index as a string.
        /// </summary>
        /// <param name="row">The row index</param>
        /// <param name="col">The column index</param>
        /// <returns></returns>
        public string GetCell(int row, int col) {
            if (row >= RowCount || row < 0) throw new Exception($"Requested row {row} out of bounds");
            if (col >= ColumnCount || col < 0) throw new Exception($"Request column {col} out of bounds");

            return cells[row][col];
        }

        /// <summary>
        /// Returns all the cells of a given column
        /// </summary>
        /// <param name="columnIndex">The column index</param>
        /// <returns></returns>
        public string[] GetColumn(int columnIndex) {
            return GetColumnCells(columnIndex, 0, RowCount);
        }

        /// <summary>
        /// Returns all the cells of a given row
        /// </summary>
        /// <param name="rowIndex">The row index</param>
        /// <returns></returns>
        public string[] GetRow(int rowIndex) {
            return GetRowCells(rowIndex, 0, ColumnCount);
        }

        /// <summary>
        /// Returns a range of cells in a given column
        /// </summary>
        /// <param name="col">The column to return cells from</param>
        /// <param name="startingRow">The row index to start returning cells from</param>
        /// <param name="count">The number of cells to return</param>
        /// <returns></returns>
        public string[] GetColumnCells(int col, int startingRow, int count) {
            string[] values = new string[count];
            for (int i = 0; i < count; i++)
                values[i] = GetCell(startingRow + i, col);
            return values;
        }

        /// <summary>
        /// Returns a range of cells in a given row
        /// </summary>
        /// <param name="row">The row to return cells from</param>
        /// <param name="startingCol">The column index to start returning cells from</param>
        /// <param name="count">The number of cells to return</param>
        /// <returns></returns>
        public string[] GetRowCells(int row, int startingCol, int count) {
            string[] values = new string[count];
            for (int i = 0; i < count; i++)
                values[i] = GetCell(row, startingCol + i);
            return values;
        }

        /// <summary>
        /// Disposes the instance and clears the 
        /// internal data
        /// </summary>
        public void Dispose() {
            cells = null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;

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
        /// Initializes the internal data object
        /// with a given object
        /// </summary>
        /// <param name="cells"></param>
        public void SetCells(string[][] cells) {
            this.cells = cells;
        }

        /// <summary>
        /// Takes a CSV in string form and 
        /// initializes the internal data
        /// </summary>
        /// <param name="contents"></param>
        public void LoadFromFileText(string contents) {
            var rows = contents.Split('\n');
            LoadFromRows(rows);
        }

        /// <summary>
        /// Reads a given CSV file and initializes
        /// the internal data 
        /// </summary>
        /// <param name="filePath"></param>
        public void LoadFromFile(string filePath) {
            var rows = File.ReadAllLines(filePath);
            LoadFromRows(rows);
        }

        /// <summary>
        /// Initializes the internal data using 
        /// CSV rows
        /// </summary>
        /// <param name="rows"></param>
        public void LoadFromRows(string[] rows) {
            List<string> nonEmptyRows = new List<string>();
            for (int i = 0; i < rows.Length; i++)
                if (!string.IsNullOrEmpty(rows[i]))
                    nonEmptyRows.Add(rows[i]);

            cells = new string[nonEmptyRows.Count][];
            string[] splits;
            for (int i = 0; i < nonEmptyRows.Count; i++) {
                splits = rows[i].Split(',');
                for (int j = 0; j < splits.Length; j++)
                    splits[j] = splits[j].Trim();
                cells[i] = splits;

            }
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
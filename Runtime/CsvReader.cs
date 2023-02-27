using System;
using System.Reflection;
using System.Collections.Generic;

namespace Adrenak.CsvUtility {
    /// <summary>
    /// Opens a CSV file and provides an API to read it
    /// in string as well as deserialized type.
    /// </summary>
    /// <typeparam name="TRecord"></typeparam>
    public class CsvReader<TRecord> : IDisposable where TRecord : new() {
        /// <summary>
        /// The schema of the currently loaded CSV file
        /// </summary>
        public string[] Schema { get; private set; }

        /// <summary>
        /// The direction in which cells of records are aligned
        /// </summary>
        public DataOrder RecordDataOrder { get; private set; }

        /// <summary>
        /// The number of records currently available
        /// </summary>
        public int RecordCount {
            get {
                if (loader == null) return 0;
                var horizontal = RecordDataOrder == DataOrder.AlongRow;
                return (horizontal ? loader.RowCount : loader.ColumnCount) - 1;
            }
        }

        /// <summary>
        /// Whether the instance should cache record fetches.
        /// </summary>
        public bool UseCache { get; set; }

        /// <summary>
        /// Clears the records in the cache
        /// </summary>
        public void ClearCache() {
            cache.Clear();
        }

        /// <summary>
        /// Adds a record to the cache at a given index
        /// </summary>
        /// <param name="index">The index to cache at</param>
        /// <param name="record">The record to cache</param>
        public void CacheRecord(int index, TRecord record) {
            if (!cache.ContainsKey(index))
                cache.Add(index, record);
            else
                cache[index] = record;
        }

        CsvLoader loader;
        readonly Dictionary<int, TRecord> cache = new Dictionary<int, TRecord>();

        /// <summary>
        /// Constructs a reader with a given loader, record data order and caching flag
        /// </summary>
        /// <param name="loader">The loader to read CSV cells from</param>
        /// <param name="recordDataOrder">The data order of the CSV file</param>
        /// <param name="useCache">Whether the reader should cache read records</param>
        public CsvReader(CsvLoader loader, DataOrder recordDataOrder, bool useCache = true) {
            RecordDataOrder = recordDataOrder;
            UseCache = useCache;
            this.loader = loader;

            ReadSchema();
            for (int i = 0; i < Schema.Length; i++) {
                if (string.IsNullOrEmpty(Schema[i]))
                    throw new Exception($"The first {(recordDataOrder == DataOrder.AlongColumn ? " row " : " column ")} " +
                    $"which describes the schema should not contain any empty cells");
            }
        }

        /// <summary>
        /// Gets the cells of a record using the index
        /// </summary>
        /// <param name="recordIndex">The index of the record</param>
        /// <returns></returns>
        public string[] GetRecordCells(int recordIndex) {
            var horizontal = RecordDataOrder == DataOrder.AlongRow;
            return horizontal ? loader.GetRow(recordIndex + 1) : loader.GetColumn(recordIndex + 1);
        }

        /// <summary>
        /// Gets a record deserialized into <see cref="TRecord"/>
        /// </summary>
        /// <param name="recordIndex">The index of the record to be fetched</param>
        /// <returns></returns>
        public TRecord GetRecord(int recordIndex) {
            // Try to return from cache first
            if (UseCache && cache.ContainsKey(recordIndex))
                return cache[recordIndex];

            var cells = GetRecordCells(recordIndex);
            var record = DeserializeRecord(cells);

            if (UseCache)
                CacheRecord(recordIndex, record);
            return record;
        }

        /// <summary>
        /// Gets a range of records deserialized into <see cref="TRecord"/>
        /// in a list.
        /// </summary>
        /// <param name="startIndex">Index of record in the CSV to start reading from</param>
        /// <param name="count">Number of records to read</param>
        /// <returns></returns>
        public TRecord[] GetRecords(int startIndex, int count) {
            TRecord[] records = new TRecord[count];
            for (int i = 0; i < count; i++)
                records[i] = GetRecord(startIndex + i);
            return records;
        }

        /// <summary>
        /// Returns all the records in the CSV file in a list of 
        /// deserialized <see cref="TRecord"/> objects
        /// </summary>
        /// <returns></returns>
        public TRecord[] GetRecords() {
            return GetRecords(0, RecordCount);
        }

        /// <summary>
        /// Reads the schema from the CSV file. When the record data is 
        /// horizontally stored, it reads the first row for schema data,
        /// when vertically stored, it reads the first column.
        /// </summary>
        void ReadSchema() {
            var horizontal = RecordDataOrder == DataOrder.AlongRow;
            Schema = new string[horizontal ? loader.ColumnCount : loader.RowCount];

            for (int i = 0; i < Schema.Length; i++)
                Schema[i] = horizontal ? loader.GetCell(0, i) : loader.GetCell(i, 0);
        }

        /// <summary>
        /// Deserializes the string cells of a single CSV record into 
        /// individual fields inside type <see cref="TRecord"/>. Uses the
        /// schema data to map cells to fields.
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        TRecord DeserializeRecord(string[] cells) {
            var result = new TRecord();
            var type = typeof(TRecord);

            FieldInfo[] fields = type.GetFields();

            for(int i = 0; i < fields.Length; i++) {
                var field = fields[i];

                var customAtt = field.GetCustomAttribute(typeof(CsvFieldAttribute), true);
                if (customAtt == null) continue;

                var csvAtt = (CsvFieldAttribute)customAtt;
                var schemaName = csvAtt.name;

                var value = cells[Array.IndexOf(Schema, schemaName)];

                // We parse the string value to the right types
                // if the string is empty or null, we set to default value
                if (csvAtt is CsvInt) {
                    if (!string.IsNullOrEmpty(value))
                        field.SetValue(result, int.Parse(value));
                    else
                        field.SetValue(result, 0);
                }
                else if (csvAtt is CsvFloat) {
                    if (!string.IsNullOrEmpty(value))
                        field.SetValue(result, float.Parse(value.Replace("f", "0").Replace("F", "0")));
                    else
                        field.SetValue(result, 0);
                }
                else if (csvAtt is CsvString) 
                    field.SetValue(result, value);
                    
                else if (csvAtt is CsvLong) {
                    if (!string.IsNullOrEmpty(value))
                        field.SetValue(result, long.Parse(value));
                    else
                        field.SetValue(result, 0);
                }
                else if (csvAtt is CsvDouble) {
                    if (!string.IsNullOrEmpty(value))
                        field.SetValue(result, double.Parse(value));
                    else
                        field.SetValue(result, 0);
                }
            }
            return result;
        }

        /// <summary>
        /// Disposes the instance by clearing the 
        /// internal CSV data matrix and the CSV schema
        /// </summary>
        public void Dispose() {
            loader.Dispose();
            Schema = null;
        }
    }
}

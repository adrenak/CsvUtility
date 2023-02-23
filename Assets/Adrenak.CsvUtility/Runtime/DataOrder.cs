namespace Adrenak.CsvUtility {
    /// <summary>
    /// The direction in which the data of a record is stored. E.g.
    /// If a record contains the fields "name", "age", "phone_number"
    /// and all the fields are in the same row, the record direction
    /// it is along the row. If the fields are in the same column
    /// it is along the column.
    /// </summary>
    public enum DataOrder {
        AlongRow,
        AlongColumn
    }
}
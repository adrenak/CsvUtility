using System;

namespace Adrenak.CsvUtility {
    public class CsvFieldAttribute : Attribute {
        public string name;
        public CsvFieldAttribute(string _name) {
            name = _name;
        }
    }

    public class CsvInt : CsvFieldAttribute {
        public CsvInt(string _name) : base(_name) { }
    }

    public class CsvFloat : CsvFieldAttribute {
        public CsvFloat (string _name) : base(_name) { }
    }

    public class CsvString : CsvFieldAttribute {
        public CsvString(string _name) : base(_name) { }
    }

    public class CsvLong : CsvFieldAttribute {
        public CsvLong (string _name) : base(_name) { }
    }

    public class CsvDouble : CsvFieldAttribute {
        public CsvDouble(string _name) : base(_name) { }
    }
}

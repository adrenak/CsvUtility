using System;

public class CsvHelperAttribute : Attribute {
    public string name;
    public CsvHelperAttribute(string _name) {
        name = _name;
    }
}

public class CsvAuto : CsvHelperAttribute {
    public CsvAuto() : base(String.Empty) { }
}

public class CsvInt : CsvHelperAttribute {
    public CsvInt(string _name) : base(_name) { }
}

public class CsvFloat : CsvHelperAttribute {
    public CsvFloat (string _name) : base(_name) { }
}

public class CsvString : CsvHelperAttribute {
    public CsvString(string _name) : base(_name) { }
}

public class CsvLong : CsvHelperAttribute {
    public CsvLong (string _name) : base(_name) { }
}

public class CsvDouble : CsvHelperAttribute {
    public CsvDouble(string _name) : base(_name) { }
}


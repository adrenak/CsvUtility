using System.Reflection;
using System;
using Adrenak;
using UnityEngine;

public class Example : MonoBehaviour {
	void Start () {
        var csv = CsvTool.Read(Application.dataPath + "/Adrenak/CsvTool/test.csv");
        var csvAsObjects = CsvTool.Read<Person>(Application.dataPath + "/Adrenak/CsvTool/test.csv");

        foreach (var row in csvAsObjects.rows)
            Debug.Log(row.ToString());

        Debug.Log("****");

        foreach (var row in csv) {
            string rowStr = "";
            foreach (var r in row)
                rowStr += r + " | ";
            Debug.Log(rowStr);
        }             
    }

    [Serializable]
    public class Person {
        [CsvInt("ID")]
        public int id;

        [CsvString("Name")]
        public string name;
        
        [CsvDouble("Age")]
        public double age;

        public override string ToString() {
            return id + ", " + name + ", " + age;
        }
    }
}

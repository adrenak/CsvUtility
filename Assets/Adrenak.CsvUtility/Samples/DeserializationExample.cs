using UnityEngine;
using System;

namespace Adrenak.CsvUtility.Samples {
    public class DeserializationExample : MonoBehaviour {
        void Start() {
            // TIP: Also try loading player_data_vertical with DataOrder.AlongColumn 
            var csvAsset = Resources.Load<TextAsset>("player_data_horizontal");
        
            // Create a loader
            var loader = new CsvLoader(csvAsset.text);
            
            // Create a reader
            var reader = new CsvReader<Player>(loader, DataOrder.AlongRow);

            // Print some stuff
            Debug.Log("CSV Schema : " + string.Join(", ", reader.Schema));
            Debug.Log(reader.RecordCount + " record(s) found");
            var records = reader.GetRecords();
            foreach (var record in records)
                Debug.Log(record);
        }
    }


    // The class to which the csv reader deserializes
    // Ensure that the strings passed in the attributes
    // are same as the schema/header of your CSV file.
    // Also ensure that the type of the field and the 
    // attribute type are the same (a string should have
    // a CsvString attribute)
    [Serializable]
    public class Player {
        [CsvString("Name")]
        public string name;

        [CsvInt("ID")]
        public int id;

        [CsvString("Country")]
        public string country;

        [CsvInt("XP")]
        public int xp;

        [CsvInt("hours_played")]
        public int hoursPlayed;

        [CsvString("Work")]
        public string fieldOfWork;

        public override string ToString() {
            return $"{name} with ID {id} played for {hoursPlayed} hours and earned {xp} XP from {country}. He works in {fieldOfWork}";
        }
    }
}

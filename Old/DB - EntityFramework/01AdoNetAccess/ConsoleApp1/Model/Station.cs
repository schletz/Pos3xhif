namespace FirstDbApp.Model
{
    //    CREATE TABLE Station (
    //    S_ID              INTEGER      PRIMARY KEY,
    //    S_Location        VARCHAR(100) NOT NULL,
    //    S_Height          INTEGER
    //  );
    /// <summary>
    /// Modelklasse zu Station
    /// </summary>
    public class Station
    {
        public int? S_Height { get; set; }      // int?, da im CREATE TABLE NULL Werte erlaubt sind.
        public int S_ID { get; set; }
        public string S_Location { get; set; }   
    }
}

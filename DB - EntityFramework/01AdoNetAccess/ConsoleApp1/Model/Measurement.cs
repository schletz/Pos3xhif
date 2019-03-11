using System;

namespace FirstDbApp.Model
{
    //CREATE TABLE Measurement(
    //    M_Date DATETIME,
    //    M_Station INTEGER,
    //    M_Temperature DECIMAL(4,1),  -- xxx.x
    //    PRIMARY KEY(M_Date, M_Station),
    //    FOREIGN KEY(M_Station) REFERENCES Station(S_ID)
    //);
    /// <summary>
    /// Modelklasse zu Measurement
    /// </summary>
    public class Measurement
    {
        public DateTime M_Date { get; set; }
        public int M_Station { get; set; }
        public decimal? M_Temperature { get; set; }

        public override string ToString()
        {
            return $"{M_Station}, {M_Date}, {M_Temperature}";
        }
    }
}

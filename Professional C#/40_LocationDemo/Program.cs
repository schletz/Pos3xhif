// *************************************************************************************************
// DEMOPROGRAMM FÜR DIE ORTSBESTIMMUNG ÜBER EINE FELDSTÄRKENMESSUNG
// *************************************************************************************************
// Unser virtuelles Stockwerk hat folgende Räume und Positionen der Accesspoints
//
//   AP1 (0|30)
//    +-------------------+
//    |                   |
//    | C3.01             |
//    | (0|20) - (10|30)  |
//    |                   |
//    +-------------------+ AP2 (10|20)
//    |                   |
//    | C3.02             |
//    | (0|10) - (10|20)  |
//    |                   |
//    +-------------------+
//    |                   |
//    | C3.03             |
//    | (0|0) - (10|10)   |
//    |                   |
//    +-------------------+
//   AP3 (0|0)
//
// Dieses Programm generiert verteilte Punkte in diesem Stockwerk und simuliert eine gemessene
// Feldstärke. Diese Feldstärke wird mit der Formel 100 * 1 / DIST berechnet, wobei DIST die
// Entfernung zum Accesspoint ist.


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using Bogus;
using Bogus.Distributions.Gaussian;
using Microsoft.ML;                         // Install-Package Microsoft.ML
using Microsoft.ML.Data;
//using Microsoft.ML.Trainers.FastTree;      // Install-Package Microsoft.ML.FastTree
using Microsoft.ML.Trainers.LightGbm;          // Install-Package Install-Package Microsoft.ML.LightGBM

namespace LocationDemo
{
    public class RoomMeasurement
    {
        public string Label { get; set; }
        [VectorType(3)]
        public float[] Values;
    }

    public class RoomPrediction
    {
        // Original label.
        public uint Label { get; set; }
        // Predicted label from the trainer.
        public uint PredictedLabel { get; set; }
    }


    public class Program
    {
        private static void Main(string[] args)
        {
            // Simuliert die Messungenauigkeit. Ist der relative Standardfehler. 0.1 bedeutet also,
            // dass bei einem berechneten Wert von 70 der Messwert in 68% der Fälle
            // (Normalverteilung) zwischen 70 +/- 7 ist.
            float bias = 0.1f;

            Randomizer.Seed = new Random(82541);
            Faker fkr = new Faker();

            Accesspoint[] accesspoints = new Accesspoint[]
            {
                new Accesspoint("a1", new Point(0,30)),
                new Accesspoint("a2", new Point(10,20)),
                new Accesspoint("a3", new Point(0,0))
            };

            // WIr positionieren 30 virtuelle Smartphones im Stockwerk.
            var devices = (from i in Enumerable.Range(0, 30)
                           select new
                           {
                               Id = Guid.NewGuid().ToString("N"),
                               Point = new Point(fkr.Random.Float(0, 10), fkr.Random.Float(0, 30))
                           }).ToArray();

            List<Measurement> measurements = new List<Measurement>();

            // Nun führen wir jede Minute eine Messung durch.
            for (DateTime date = new DateTime(2020, 1, 1, 12, 0, 0); date < new DateTime(2020, 1, 1, 13, 0, 0); date = date.AddMinutes(1))
            {
                // Jedes Gerät liefert einen Messwert.
                foreach (var device in devices)
                {
                    // Wir bekommen zwischen 1 und (Anz AP) pro Messung gesendet. Da nicht immer alle
                    // APs empfangen werden, ist das ein guter Test, ob unser Programm damit umgehen
                    // kann.
                    var measuredAccesspoints = fkr.Random.ListItems(accesspoints, fkr.Random.Int(1, accesspoints.Length));
                    var signals = from ap in measuredAccesspoints
                                  let signalStrength = device.Point.SignalStrength(ap, val => fkr.Random.GaussianFloat(val, val * bias))
                                  select new Signal(ap, signalStrength);
                    Measurement m = new Measurement(device.Id, date, device.Point, signals);
                    measurements.Add(m);
                }
            }

            var avgAp = measurements.Select(m => m.Signals.Count()).Average();
            Console.WriteLine($"{measurements.Count} Messungen, {avgAp:0.00} APs pro Messung im Mittel.");

            var table = from m in measurements
                        select new
                        {
                            m.Device,
                            m.Date,
                            m.Location.Room,
                            // Floatarray mit den gemessenen Feldstärken der Accesspoint. Ist genau in
                            // der Reihenfolge und größe wie unser accesspoints Array.
                            // Details siehe in der Funktion Spread in der Datei ArrayExtensions.cs.
                            Signals = m.Signals.Spread(accesspoints, s => s.Accesspoint).Select(s => s?.Value ?? 0).ToArray()
                        };

            using (var outStream = new StreamWriter("measurements.txt", false, Encoding.UTF8))
            {
                outStream.WriteLine($"DEVICE\tROOM\tDATE\t{string.Join('\t', accesspoints.Select(s => s.Mac))}");
                foreach (var row in table)
                    outStream.WriteLine($"{row.Device}\t{row.Room}\t{row.Date}\t{string.Join('\t', row.Signals)}");
            }

            // https://docs.microsoft.com/en-us/dotnet/api/microsoft.ml.lightgbmextensions.lightgbm?view=ml-dotnet

            // Define trainer options.
            var options = new LightGbmMulticlassTrainer.Options
            {
                LabelColumnName = nameof(RoomMeasurement.Label),
                FeatureColumnName = nameof(RoomMeasurement.Values),
                Booster = new DartBooster.Options()
                {
                    TreeDropFraction = 0.15,
                    XgboostDartMode = false
                }
            };

            var mlContext = new MLContext(seed: 0);
            var trainData = mlContext.Data.LoadFromEnumerable(from m in measurements
                                                              select new RoomMeasurement
                                                              {
                                                                  Label = m.Location.Room,
                                                                  Values = m.Signals.Spread(accesspoints, s => s.Accesspoint).Select(s => s?.Value ?? 0).ToArray()
                                                              });
            // Define the trainer.
            var pipeline = mlContext.Transforms.Conversion.MapValueToKey(nameof(RoomMeasurement.Label))
                .Append(mlContext.MulticlassClassification.Trainers.LightGbm(options));
            var model = pipeline.Fit(trainData);

            // Create testing data. Use different random seed to make it different
            // from training data.
            var testData = trainData;

            // Run the model on test data set.
            var transformedTestData = model.Transform(testData);

            // Convert IDataView object to a list.
            var predictions = mlContext.Data
                .CreateEnumerable<RoomPrediction>(transformedTestData, reuseRowObject: false).ToList();

            // Look at 5 predictions
            foreach (var p in predictions.Take(5))
                Console.WriteLine($"Label: {p.Label}, Prediction: {p.PredictedLabel}");

            var metrics = mlContext.MulticlassClassification.Evaluate(transformedTestData);

            PrintMetrics(metrics);

        }

        private static void PrintMetrics(MulticlassClassificationMetrics metrics)
        {
            Console.WriteLine(JsonSerializer.Serialize(metrics));
        }
    }
}

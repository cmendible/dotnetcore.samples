using System;
using System.IO;
using Microsoft.ML.Runtime.Learners;
using Microsoft.ML.Runtime.Data;
using Microsoft.ML.Core.Data;
using Microsoft.ML;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms.Text;
using System.Data;
using System.Threading.Tasks;
using System.Linq;

namespace Titanic.ML
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //Create MLContext to be shared across the model creation workflow objects 
            //Set a random seed for repeatable/deterministic results across multiple trainings.
            var mlContext = new MLContext();

            // STEP 1: Common data loading configuration
            // PassengerId,Survived,Pclass,Name,Sex,Age,SibSp,Parch,Ticket,Fare,Cabin,Embarked
            var textLoader = mlContext.Data.TextReader(new TextLoader.Arguments()
            {
                Separator = ",",
                HasHeader = true,
                Column = new[]
                {
                    new TextLoader.Column("PassengerId", DataKind.Num, 0),
                    new TextLoader.Column("Label", DataKind.Bool, 1),
                    new TextLoader.Column("Pclass", DataKind.Num, 2),
                    new TextLoader.Column("Name", DataKind.Text, 3),
                    new TextLoader.Column("Sex", DataKind.Text, 4),
                    new TextLoader.Column("Age", DataKind.Num, 5),
                    new TextLoader.Column("SibSp", DataKind.Num, 6),
                    new TextLoader.Column("Parch", DataKind.Num, 7),
                    new TextLoader.Column("Ticket", DataKind.Text, 8),
                    new TextLoader.Column("Fare", DataKind.Text, 9),
                    new TextLoader.Column("Cabin", DataKind.Text, 10),
                    new TextLoader.Column("Embarked", DataKind.Text, 11)
                }
            });

            // Load training data and add it to the pipeline
            string trainingDataPath = @".\data\titanic.training.csv";
            string dataPath = @".\data\titanic.csv";
            var trainingData = textLoader.Read(trainingDataPath);
            var testData = textLoader.Read(dataPath);

            var features = trainingData.Schema.GetColumns()
                .Where(c => c.column.Name != "Label")
                .Select(c => c.column.Name)
                .ToArray();

            // Build several alternative featurization pipelines.
            var pipeline =
                mlContext.Transforms.Categorical.OneHotEncoding("Sex")
                .Append(mlContext.Transforms.Categorical.OneHotEncoding("Name"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding("Ticket"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding("Fare"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding("Cabin"))
                .Append(mlContext.Transforms.Categorical.OneHotEncoding("Embarked"))
                .Append(mlContext.Transforms.Concatenate("Features", features))
                .Append(mlContext.BinaryClassification.Trainers.FastTree(labelColumn: "Label", featureColumn: "Features"));

            // Train the model.
            var model = pipeline.Fit(trainingData);

            // Create a PredictionFunction from our model 
            var predictor = model.MakePredictionFunction<TitanicData, TitanicPrediction>(mlContext);
            var prediction = predictor.Predict(new TitanicData()
            {
                Pclass = 3f,
                Name = "Braund, Mr. Owen Harris",
                Sex = "male",
                Age = 31,
                SibSp = 0,
                Parch = 0,
                Ticket = "335097",
                Fare = "7.75",
                Cabin = "",
                Embarked = "Q"
            });

            Console.WriteLine($"Did this passenger survive? {(prediction.Survived ? "Yes" : "No")}");

            var context = new BinaryClassificationContext(mlContext);
            var metrics = context.Evaluate(model.Transform(testData), "Label");
            Console.WriteLine("Acuracy: " + metrics.Accuracy);
            Console.WriteLine($"Auc: {metrics.Auc:P2}");
            Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
        }
    }
}

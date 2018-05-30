using Microsoft.ML;
using Microsoft.ML.Runtime.Api;
using Microsoft.ML.Trainers;
using Microsoft.ML.Transforms;
using System;
using System.Threading.Tasks;

namespace Titanic.ML
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Create a pipeline and load your data
            var pipeline = new LearningPipeline();

            // Load training data and added to the pipeline
            string dataPath = @".\data\titanic.training.csv";
            var data = new TextLoader<TitanicData>(dataPath, useHeader: true, separator: ",");
            pipeline.Add(data);

            // Transform any text feature to numeric values
            pipeline.Add(new CategoricalOneHotVectorizer(
                "Sex",
                "Ticket",
                "Fare",
                "Cabin",
                "Embarked"));

            // Puts all features into a vector
            pipeline.Add(new ColumnConcatenator("Features", "Pclass", "Sex", "Age", "SibSp", "Parch", "Ticket", "Fare", "Cabin", "Embarked"));

            // Add a learning algorithm to the pipeline. 
            // This is a classification scenario (Did this passenger survive?)
            pipeline.Add(new FastTreeBinaryClassifier() { NumLeaves = 5, NumTrees = 5, MinDocumentsInLeafs = 2 });

            Console.WriteLine($"Training Titanic.ML model...");
            // Train your model based on the data set
            var model = pipeline.Train<TitanicData, TitanicPrediction>();

            // Save the model to a file
            var modelPath = @".\data\titanic.model";
            await model.WriteAsync(modelPath);

            // Use your model to make a prediction
            var prediction = model.Predict(new TitanicData()
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

            // Evaluate the model using the test data
            Console.WriteLine($"Evaluating Titanic.ML model...");
            dataPath = @".\data\titanic.csv";
            data = new TextLoader<TitanicData>(dataPath, useHeader: true, separator: ",");
            var evaluator = new Microsoft.ML.Models.BinaryClassificationEvaluator();
            var metrics = evaluator.Evaluate(model, data);
            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"Auc: {metrics.Auc:P2}");
            Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
        }
    }
}

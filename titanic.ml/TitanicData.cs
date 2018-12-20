using Microsoft.ML.Runtime.Api;

namespace Titanic.ML
{
    public class TitanicData
    {
        public float PassengerId;

        public float Pclass;

        public string Name;

        public string Sex;

        public float Age;

        public float SibSp;

        public float Parch;

        public string Ticket;

        public string Fare;

        public string Cabin;

        public string Embarked;
    }

    public class TitanicPrediction
    {
        [ColumnName("PredictedLabel")]
        public bool Survived;
    }
}
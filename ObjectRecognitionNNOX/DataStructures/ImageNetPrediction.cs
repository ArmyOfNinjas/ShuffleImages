using Microsoft.ML.Data;

namespace ObjectRecognitionONNX.DataStructures
{
    public class ImageNetPrediction
    {
        [ColumnName("grid")]
        public float[] PredictedLabels;
    }
}

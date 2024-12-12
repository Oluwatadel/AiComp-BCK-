using AiComp.Application.DTOs.ValueObjects;
using AiComp.Application.Interfaces.Service;
using AiComp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.ML;

namespace AiComp.Infrastructure.Services
{
    public class SentimentAnalysis : ISentimentAnalysis
    {
        private readonly MLContext _mlContext;
        private readonly ITransformer _model;
        private readonly string _dataPath = "Sentiment_data.csv";
        private readonly string _modelPath = "sentiment_model.zip";

        public SentimentAnalysis(MLContext mlContext)
        {
            _mlContext = mlContext;
            _model = LoadModel();
        }

        private ITransformer LoadModel()
        {
            var modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sentiment_model.zip");
            return _mlContext.Model.Load(modelPath, out _);
        }

        private async Task SaveModelAsync(ITransformer model, DataViewSchema schema)
        {
            await Task.Run(() => _mlContext.Model.Save(model, schema, _modelPath));
        }

        public async Task<SentimentPrediction> PredictSentiment(string text)
        {
            return await Task.Run(() =>
            {
                var sentimentData = new SentimentData { SentimentText = text };
                var predEngine = _mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(_model);
                var prediction = predEngine.Predict(sentimentData);

                var sentimentPrediction = new SentimentPrediction()
                {
                    Intensity = prediction.Intensity,
                    Emotion = prediction.Emotion
                };

                sentimentPrediction.SetText(text);
                return sentimentPrediction;
            });
        }

        public async Task TrainModelAsync()
        {
            var data = _mlContext.Data.LoadFromTextFile<SentimentData>(_dataPath, separatorChar: ',', hasHeader: true);

            // Split the data into training and testing sets
            var trainTestSplit = _mlContext.Data.TrainTestSplit(data, testFraction: 0.2);

            // Define the training pipeline
            var pipeline = _mlContext.Transforms.Concatenate("Features", "SentimentText")
                .Append(_mlContext.Transforms.Text.FeaturizeText("SentimentText", "Features"))
                .Append(_mlContext.Transforms.Conversion.MapValueToKey("Label"))
                .Append(_mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(labelColumnName: "Label", maximumNumberOfIterations: 100))
                .Append(_mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

            var model = await Task.Run(() => pipeline.Fit(trainTestSplit.TrainSet));


            // Evaluate the model asynchronously
            var predictions = model.Transform(trainTestSplit.TestSet);
            var metrics = _mlContext.BinaryClassification.Evaluate(predictions, labelColumnName: "Label");
            Console.WriteLine($"Accuracy: {metrics.Accuracy}");

            // Save the model asynchronously
            await SaveModelAsync(model, data.Schema);
        }
    }
}

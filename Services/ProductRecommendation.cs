using Microsoft.ML;
using RMS.Web.MLModels;
using RMS.Data.Entities;
using RMS.Data.Repository;
using RMS.Web.Services;


namespace RMS.Web.Services
{
    public class ProductRecommendationService
    {
        private readonly PredictionEngine<ModelInput, ModelOutput> _predictionEngine;

        public ProductRecommendationService()
        {
            var mlContext = new MLContext();

            // Load the model
            var modelPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "MLModels", "model.zip");
            ITransformer mlModel = mlContext.Model.Load(modelPath, out var _);

            _predictionEngine = mlContext.Model.CreatePredictionEngine<ModelInput, ModelOutput>(mlModel);
        }

        public float PredictScore(uint userId, uint productId)
        {
            var input = new ModelInput
            {
                UserId = userId,
                ProductId = productId
            };

            var prediction = _predictionEngine.Predict(input);
            return prediction.Score;
        }
    }
}



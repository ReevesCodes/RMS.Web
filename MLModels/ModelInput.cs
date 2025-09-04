using Microsoft.ML.Data;

namespace RMS.Web.MLModels
{
    public class ModelInput
    {
        [KeyType(count: 1000)]
        public uint UserId { get; set; }

        [KeyType(count: 1000)]
        public uint ProductId { get; set; }

        public float Label { get; set; }
    }
}

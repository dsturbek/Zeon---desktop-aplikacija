using Newtonsoft.Json;

namespace ApiServiceLayer.DTOs
{
    public class NotificationCountResponse
    {
        [JsonProperty("totalNewCount")]
        public int TotalNewCount { get; set; }

        [JsonProperty("counts")]
        public NotificationCounts Counts { get; set; }
    }

    public class NotificationCounts
    {
        [JsonProperty("messages")]
        public int Messages { get; set; }

        [JsonProperty("feedbackTraining")]
        public int FeedbackTraining { get; set; }

        [JsonProperty("feedbackMeal")]
        public int FeedbackMeal { get; set; }
    }
}

using Newtonsoft.Json;
using System.Collections.Generic;

namespace ApiServiceLayer.DTOs
{
    public class NotificationResponse
    {
        [JsonProperty("totalNewCount")]
        public int TotalNewCount { get; set; }

        [JsonProperty("counts")]
        public NotificationCounts Counts { get; set; }

        [JsonProperty("lastSeenIds")]
        public LastSeenIds LastSeenIds { get; set; }

        [JsonProperty("notifications")]
        public NotificationData Notifications { get; set; }
    }

    public class LastSeenIds
    {
        [JsonProperty("messageId")]
        public int MessageId { get; set; }

        [JsonProperty("feedbackTrainingId")]
        public int FeedbackTrainingId { get; set; }

        [JsonProperty("feedbackMealId")]
        public int FeedbackMealId { get; set; }
    }

    public class NotificationData
    {
        [JsonProperty("messages")]
        public List<MessageNotification> Messages { get; set; } = new List<MessageNotification>();

        [JsonProperty("feedbackTraining")]
        public List<TrainingFeedbackNotification> FeedbackTraining { get; set; } = new List<TrainingFeedbackNotification>();

        [JsonProperty("feedbackMeal")]
        public List<MealFeedbackNotification> FeedbackMeal { get; set; } = new List<MealFeedbackNotification>();
    }

    public class MessageNotification
    {
        [JsonProperty("id_message")]
        public int IdMessage { get; set; }

        [JsonProperty("message_content")]
        public string MessageContent { get; set; }

        [JsonProperty("ConversationId")]
        public int ConversationId { get; set; }

        [JsonProperty("ClientId")]
        public int ClientId { get; set; }

        [JsonProperty("client_name")]
        public string ClientName { get; set; }

        [JsonProperty("client_username")]
        public string ClientUsername { get; set; }
    }

    public class TrainingFeedbackNotification
    {
        [JsonProperty("id_feedback")]
        public int IdFeedback { get; set; }

        [JsonProperty("feedback_message")]
        public string FeedbackMessage { get; set; }

        [JsonProperty("completed_sets")]
        public int CompletedSets { get; set; }

        [JsonProperty("completed_reps")]
        public int CompletedReps { get; set; }

        [JsonProperty("completed_weight")]
        public double? CompletedWeight { get; set; }

        [JsonProperty("ClientId")]
        public int ClientId { get; set; }

        [JsonProperty("client_name")]
        public string ClientName { get; set; }

        [JsonProperty("client_username")]
        public string ClientUsername { get; set; }

        [JsonProperty("exercise_name")]
        public string ExerciseName { get; set; }

        [JsonProperty("workout_name")]
        public string WorkoutName { get; set; }
    }

    public class MealFeedbackNotification
    {
        [JsonProperty("id_feedback_meal")]
        public int IdFeedbackMeal { get; set; }

        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("Planned_mealId")]
        public int PlannedMealId { get; set; }

        [JsonProperty("meal_date")]
        public string MealDate { get; set; }

        [JsonProperty("meal_type")]
        public string MealType { get; set; }

        [JsonProperty("food_plan_name")]
        public string FoodPlanName { get; set; }

        [JsonProperty("ClientId")]
        public int ClientId { get; set; }

        [JsonProperty("client_name")]
        public string ClientName { get; set; }

        [JsonProperty("client_username")]
        public string ClientUsername { get; set; }
    }
}

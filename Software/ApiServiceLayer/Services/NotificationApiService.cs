using ApiServiceLayer.DTOs;
using ApiServiceLayer.Storage;
using System.Threading.Tasks;

namespace ApiServiceLayer.Services
{
    public class NotificationApiService
    {
        private readonly ApiClient _apiClient;
        private readonly NotificationStateStorage _stateStorage;

        public NotificationApiService()
        {
            _apiClient = ApiClient.Instance;
            _stateStorage = new NotificationStateStorage();
        }

        public async Task<NotificationCountResponse> GetNotificationCountAsync(int trainerId)
        {
            var state = _stateStorage.Load(trainerId);

            var endpoint = $"/api/notifications/count?lastMessageId={state.LastMessageId}&lastFeedbackTrainingId={state.LastFeedbackTrainingId}&lastFeedbackMealId={state.LastFeedbackMealId}";

            return await _apiClient.GetAsync<NotificationCountResponse>(endpoint);
        }

        public async Task<NotificationResponse> GetNotificationsAsync(int trainerId)
        {
            var state = _stateStorage.Load(trainerId);

            var endpoint = $"/api/notifications/new?lastMessageId={state.LastMessageId}&lastFeedbackTrainingId={state.LastFeedbackTrainingId}&lastFeedbackMealId={state.LastFeedbackMealId}";

            return await _apiClient.GetAsync<NotificationResponse>(endpoint);
        }

        public void MarkNotificationsAsSeen(int trainerId, LastSeenIds lastSeenIds)
        {
            var state = new NotificationState
            {
                TrainerId = trainerId,
                LastMessageId = lastSeenIds.MessageId,
                LastFeedbackTrainingId = lastSeenIds.FeedbackTrainingId,
                LastFeedbackMealId = lastSeenIds.FeedbackMealId
            };

            _stateStorage.Save(state);
        }

        public void MarkMessageAsSeen(int trainerId, int messageId)
        {
            var state = _stateStorage.Load(trainerId);
            if (messageId > state.LastMessageId)
            {
                state.LastMessageId = messageId;
                _stateStorage.Save(state);
            }
        }

        public void MarkTrainingFeedbackAsSeen(int trainerId, int feedbackId)
        {
            var state = _stateStorage.Load(trainerId);
            if (feedbackId > state.LastFeedbackTrainingId)
            {
                state.LastFeedbackTrainingId = feedbackId;
                _stateStorage.Save(state);
            }
        }

        public void MarkMealFeedbackAsSeen(int trainerId, int feedbackId)
        {
            var state = _stateStorage.Load(trainerId);
            if (feedbackId > state.LastFeedbackMealId)
            {
                state.LastFeedbackMealId = feedbackId;
                _stateStorage.Save(state);
            }
        }

        public NotificationState GetLocalState(int trainerId)
        {
            return _stateStorage.Load(trainerId);
        }

        public async Task InitializeStateForNewUser(int trainerId)
        {
            try
            {
                var response = await GetNotificationsAsync(trainerId);
                if (response?.LastSeenIds != null)
                {
                    MarkNotificationsAsSeen(trainerId, response.LastSeenIds);
                }
            }
            catch
            {
                // Ako API poziv ne uspije počinjemo s nulama što znači da će korisnik
                // vidjeti sve obavijesti kao nove
            }
        }
    }
}

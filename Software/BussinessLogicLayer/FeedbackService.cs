using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogicLayer
{
    public class FeedbackService
    {
        private FeedbackRepository feedbackRepository;

        public FeedbackService()
        {
            feedbackRepository = new FeedbackRepository();
        }

        public async Task<List<Feedback_training>> GetAllTrainingFeedbacksAsync(int trainerId)
        {
            return await feedbackRepository.GetAllTrainingFeedbacksAsync(trainerId);
        }

        public async Task<List<Feedback_training>> GetAllTrainingFeedbacksByClientAsync(int clientId)
        {
            if(clientId <= 0)
            {
                throw new Exception("ID klijenta nije ispravan!");
            }

            return await feedbackRepository.GetAllTrainingFeedbackByClientAsnyc(clientId);
        }

        public async Task<List<Feedback_meal>> GetAllMealFeedbacksAsync(int trainerId)
        {
            return await feedbackRepository.GetAllMealFeedbacksAsync(trainerId);
        }

        public async Task<List<Feedback_meal>> GetAllMealFeedbacksByClientAsync(int clientId)
        {
            if (clientId <= 0)
            {
                throw new Exception("ID klijenta nije ispravan!");
            }

            return await feedbackRepository.GetAllMealFeedbackByClientAsnyc(clientId);
        }

    }
}

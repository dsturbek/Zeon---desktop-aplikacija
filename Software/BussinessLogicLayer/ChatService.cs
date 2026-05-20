using DataAccessLayer.Repositories;
using EntitiesLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BussinessLogicLayer
{
    public class ChatService
    {
        MessageRepository messageRepository = new MessageRepository();

        public List<Conversation> GetAllConversations(int trainderId)
        {
            return messageRepository.GetAllConversationsFromTrainer(trainderId);
        }

        public List<Message> GetConversationHistory(int conversationId)
        {
            return messageRepository.GetMessagesFromConversation(conversationId);
        }

        public Conversation GetOrCreateConversation(int trainerId, int clientId)
        {
            var existing = messageRepository.GetConversation(trainerId, clientId);
            if (existing != null)
                return existing;

            var conversation = new Conversation
            {
                TrainerId = trainerId,
                ClientId = clientId
            };

            return messageRepository.CreateConversation(conversation);
        }

        public void SendMessage(int conversationId, Message message)
        {
            if (string.IsNullOrWhiteSpace(message.message_content))
            {
                throw new Exception("Poruka ne može biti prazna!");
            }

            if (message.message_content.Length > 255)
            {
                throw new Exception("Poruka ne smije biti dulja od 255 znakova!");
            }

            message.ConversationId = conversationId;
            messageRepository.CreateMessage(message);
        }
    }
}

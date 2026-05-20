using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesLayer.Entities;

namespace DataAccessLayer.Repositories
{
    public class MessageRepository
    {
        public List<Conversation> GetAllConversationsFromTrainer(int trainerId)
        {
            using (var context = new ZeonDbContext())
            {
                return context.Conversations
                    .Include(c => c.Client)
                    .Include(c => c.Messages)
                    .Where(c => c.TrainerId == trainerId)
                    .ToList();
            }
        }

        public List<Message> GetMessagesFromConversation(int conversationId)
        {
            using (var context = new ZeonDbContext())
            {
                return context.Messages
                    .Include(m => m.Client)
                    .Include(m => m.Trainer)
                    .Where(m => m.ConversationId == conversationId)
                    .OrderBy(m => m.id_message)
                    .ToList();
            }
        }

        public Conversation CreateConversation(Conversation conversation)
        {
            using (var context = new ZeonDbContext())
            {
                context.Conversations.Add(conversation);
                context.SaveChanges();
                return conversation;
            }
        }

        public Conversation GetConversation(int trainerId, int clientId)
        {
            using (var context = new ZeonDbContext())
            {
                return context.Conversations
                    .Include(c => c.Client)
                    .Include(c => c.Messages)
                    .FirstOrDefault(c => c.TrainerId == trainerId && c.ClientId == clientId);
            }
        }

        public Message CreateMessage(Message message)
        {
            using (var context = new ZeonDbContext())
            {
                context.Messages.Add(message);
                context.SaveChanges();
                return message;
            }
        }
    }
}

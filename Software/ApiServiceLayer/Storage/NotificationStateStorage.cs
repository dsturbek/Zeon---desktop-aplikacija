using Newtonsoft.Json;
using System;
using System.IO;

namespace ApiServiceLayer.Storage
{
    public class NotificationState
    {
        public int TrainerId { get; set; }
        public int LastMessageId { get; set; }
        public int LastFeedbackTrainingId { get; set; }
        public int LastFeedbackMealId { get; set; }
    }

    public class NotificationStateStorage
    {
        private readonly string _storagePath;

        public NotificationStateStorage()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var zeonFolder = Path.Combine(appDataPath, "ZeonTrainer");

            if (!Directory.Exists(zeonFolder))
            {
                Directory.CreateDirectory(zeonFolder);
            }

            _storagePath = Path.Combine(zeonFolder, "notification_state.json");
        }

        public NotificationState Load(int trainerId)
        {
            try
            {
                if (File.Exists(_storagePath))
                {
                    var json = File.ReadAllText(_storagePath);
                    var state = JsonConvert.DeserializeObject<NotificationState>(json);

                    if (state != null && state.TrainerId == trainerId)
                    {
                        return state;
                    }
                }
            }
            catch
            {
                // tu se zapravo vraća default stanje
            }

            return new NotificationState
            {
                TrainerId = trainerId,
                LastMessageId = 0,
                LastFeedbackTrainingId = 0,
                LastFeedbackMealId = 0
            };
        }

        public void Save(NotificationState state)
        {
            try
            {
                var json = JsonConvert.SerializeObject(state, Formatting.Indented);
                File.WriteAllText(_storagePath, json);
            }
            catch
            {
                
            }
        }

        public void Clear()
        {
            try
            {
                if (File.Exists(_storagePath))
                {
                    File.Delete(_storagePath);
                }
            }
            catch
            {
                
            }
        }
    }
}

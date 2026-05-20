namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Message")]
    public partial class Message
    {
        [Key]
        public int id_message { get; set; }

        [StringLength(255)]
        public string message_content { get; set; }

        public int? ConversationId { get; set; }

        public int? SenderClientId { get; set; }

        public int? SenderTrainerId { get; set; }

        public virtual Client Client { get; set; }

        public virtual Conversation Conversation { get; set; }

        public virtual Trainer Trainer { get; set; }
    }
}

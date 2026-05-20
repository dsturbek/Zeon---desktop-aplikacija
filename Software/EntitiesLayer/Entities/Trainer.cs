namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Trainer")]
    public partial class Trainer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Trainer()
        {
            Clients = new HashSet<Client>();
            Conversations = new HashSet<Conversation>();
            Messages = new HashSet<Message>();
            Reports = new HashSet<Report>();
            Trainer_profile = new HashSet<Trainer_profile>();
            Workout_plan_template = new HashSet<Workout_plan_template>();
        }

        [Key]
        public int id_trainer { get; set; }

        [StringLength(255)]
        public string name_surname { get; set; }

        [StringLength(255)]
        public string email { get; set; }

        [StringLength(50)]
        public string username { get; set; }

        [StringLength(255)]
        public string password { get; set; }

        [Column(TypeName = "date")]
        public DateTime? employment_date { get; set; }

        [StringLength(50)]
        public string specialization { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Client> Clients { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Conversation> Conversations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Message> Messages { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Report> Reports { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Trainer_profile> Trainer_profile { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Workout_plan_template> Workout_plan_template { get; set; }
    }
}

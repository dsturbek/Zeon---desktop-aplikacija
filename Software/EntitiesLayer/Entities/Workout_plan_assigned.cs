namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Workout_plan_assigned
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Workout_plan_assigned()
        {
            Workout_plan_assigned_Workout = new HashSet<Workout_plan_assigned_Workout>();
        }

        [Key]
        public int id_workout_assigned { get; set; }

        [StringLength(50)]
        public string workout_assigned_name { get; set; }

        [Column(TypeName = "date")]
        public DateTime? date_start { get; set; }

        [Column(TypeName = "date")]
        public DateTime? date_end { get; set; }

        public int? ClientId { get; set; }

        public int? Workout_plan_templateId { get; set; }

        public virtual Client Client { get; set; }

        public virtual Workout_plan_template Workout_plan_template { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Workout_plan_assigned_Workout> Workout_plan_assigned_Workout { get; set; }
    }
}

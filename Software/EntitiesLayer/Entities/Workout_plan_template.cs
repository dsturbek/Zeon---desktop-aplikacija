namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Workout_plan_template
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Workout_plan_template()
        {
            Workout_plan_assigned = new HashSet<Workout_plan_assigned>();
            Workout_Workout_plan_template = new HashSet<Workout_Workout_plan_template>();
        }

        [Key]
        public int id_workout_template { get; set; }

        public int? TrainerId { get; set; }

        [StringLength(50)]
        public string workout_template_name { get; set; }

        public virtual Trainer Trainer { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Workout_plan_assigned> Workout_plan_assigned { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Workout_Workout_plan_template> Workout_Workout_plan_template { get; set; }
    }
}

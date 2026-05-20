namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Workout")]
    public partial class Workout
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Workout()
        {
            Exercise_Workout = new HashSet<Exercise_Workout>();
            Workout_plan_assigned_Workout = new HashSet<Workout_plan_assigned_Workout>();
            Workout_Workout_plan_template = new HashSet<Workout_Workout_plan_template>();
        }

        [Key]
        public int id_workout { get; set; }

        [StringLength(50)]
        public string workout_name { get; set; }

        [StringLength(50)]
        public string muscle_group { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Exercise_Workout> Exercise_Workout { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Workout_plan_assigned_Workout> Workout_plan_assigned_Workout { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Workout_Workout_plan_template> Workout_Workout_plan_template { get; set; }
    }
}

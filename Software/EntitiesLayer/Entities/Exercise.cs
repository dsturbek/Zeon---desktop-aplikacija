namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Exercise")]
    public partial class Exercise
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Exercise()
        {
            Exercise_Workout = new HashSet<Exercise_Workout>();
            Personal_record = new HashSet<Personal_record>();
        }

        [Key]
        public int id_exercise { get; set; }

        [StringLength(255)]
        public string exercise_name { get; set; }

        [StringLength(50)]
        public string muscle { get; set; }

        [StringLength(255)]
        public string video_url { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Exercise_Workout> Exercise_Workout { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Personal_record> Personal_record { get; set; }
    }
}

namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Planned_meal
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Planned_meal()
        {
            Feedback_meal = new HashSet<Feedback_meal>();
            Planned_meal_Food = new HashSet<Planned_meal_Food>();
        }

        [Key]
        public int id_planned_meal { get; set; }

        [Column(TypeName = "date")]
        public DateTime? date { get; set; }

        [StringLength(10)]
        public string meal_type { get; set; }

        public int? Food_planId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Feedback_meal> Feedback_meal { get; set; }

        public virtual Food_plan Food_plan { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Planned_meal_Food> Planned_meal_Food { get; set; }
    }
}

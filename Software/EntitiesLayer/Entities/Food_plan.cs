namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Food_plan
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Food_plan()
        {
            Planned_meal = new HashSet<Planned_meal>();
        }

        [Key]
        public int id_food_plan { get; set; }

        [StringLength(50)]
        public string food_plan_name { get; set; }

        [StringLength(255)]
        public string food_plan_description { get; set; }

        [Column(TypeName = "date")]
        public DateTime? date_start { get; set; }

        [Column(TypeName = "date")]
        public DateTime? date_end { get; set; }

        public int? ClientId { get; set; }

        public virtual Client Client { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Planned_meal> Planned_meal { get; set; }
    }
}

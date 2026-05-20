namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Food")]
    public partial class Food
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Food()
        {
            Food_Food_diary = new HashSet<Food_Food_diary>();
            Food_Recipe = new HashSet<Food_Recipe>();
            Planned_meal_Food = new HashSet<Planned_meal_Food>();
        }

        [Key]
        public int id_food { get; set; }

        [StringLength(50)]
        public string name { get; set; }

        public decimal? kCal { get; set; }

        public decimal? proteins { get; set; }

        public decimal? fat { get; set; }

        public decimal? carbohydrates { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Food_Food_diary> Food_Food_diary { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Food_Recipe> Food_Recipe { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Planned_meal_Food> Planned_meal_Food { get; set; }
    }
}

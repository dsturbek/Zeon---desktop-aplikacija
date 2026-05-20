namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Recipe")]
    public partial class Recipe
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Recipe()
        {
            Food_Recipe = new HashSet<Food_Recipe>();
        }

        [Key]
        public int id_recipe { get; set; }

        [StringLength(50)]
        public string recipe_name { get; set; }

        [StringLength(255)]
        public string recipe_description { get; set; }

        [StringLength(255)]
        public string instructions { get; set; }

        [NotMapped]
        public double TotalKcal { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Food_Recipe> Food_Recipe { get; set; }
    }
}

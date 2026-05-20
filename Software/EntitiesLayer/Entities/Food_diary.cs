namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Food_diary
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Food_diary()
        {
            Food_Food_diary = new HashSet<Food_Food_diary>();
        }

        [Key]
        public int id_food_diary { get; set; }

        public int? ClientId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? food_diary_date { get; set; }

        public virtual Client Client { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Food_Food_diary> Food_Food_diary { get; set; }
    }
}

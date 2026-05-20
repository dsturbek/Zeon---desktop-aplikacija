namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Feedback_meal
    {
        [Key]
        public int id_feedback_meal { get; set; }

        [StringLength(255)]
        public string comment { get; set; }

        public int? Planned_mealId { get; set; }

        public virtual Planned_meal Planned_meal { get; set; }
    }
}

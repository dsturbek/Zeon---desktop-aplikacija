namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Personal_record
    {
        [Key]
        public int id_pr { get; set; }

        public decimal? max_weight { get; set; }

        public int? max_reps { get; set; }

        public decimal? max_volument { get; set; }

        [Column(TypeName = "date")]
        public DateTime? date_achievement { get; set; }

        public int? Client_profileId { get; set; }

        public int? ExerciseId { get; set; }

        public virtual Client_profile Client_profile { get; set; }

        public virtual Exercise Exercise { get; set; }
    }
}

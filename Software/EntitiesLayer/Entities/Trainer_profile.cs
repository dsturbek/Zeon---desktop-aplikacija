namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Trainer_profile
    {
        [Key]
        public int id_trainer_profile { get; set; }

        [StringLength(255)]
        public string description { get; set; }

        public decimal? price { get; set; }

        public decimal? rating { get; set; }

        public int? number_clients { get; set; }

        public int? TrainerId { get; set; }

        public virtual Trainer Trainer { get; set; }
    }
}

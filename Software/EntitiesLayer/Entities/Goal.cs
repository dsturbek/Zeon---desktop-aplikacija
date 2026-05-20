namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Goal
    {
        [Key]
        public int id_goal { get; set; }

        [StringLength(50)]
        public string goal_name { get; set; }

        [StringLength(255)]
        public string goal_description { get; set; }

        public int? goal_cal { get; set; }

        public int? goal_water { get; set; }

        public int? ClientId { get; set; }

        public virtual Client Client { get; set; }
    }
}

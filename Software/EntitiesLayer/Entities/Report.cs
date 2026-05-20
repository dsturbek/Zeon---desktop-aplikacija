namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Report")]
    public partial class Report
    {
        [Key]
        public int id_report { get; set; }

        [StringLength(50)]
        public string report_name { get; set; }

        [StringLength(255)]
        public string report_content { get; set; }

        [Column(TypeName = "date")]
        public DateTime? report_date { get; set; }

        public int? TrainerId { get; set; }

        public int? ClientId { get; set; }

        public virtual Client Client { get; set; }

        public virtual Trainer Trainer { get; set; }
    }
}

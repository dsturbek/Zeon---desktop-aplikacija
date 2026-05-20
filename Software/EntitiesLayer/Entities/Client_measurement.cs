namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Client_measurement
    {
        [Key]
        public int id_measurement { get; set; }

        [Column(TypeName = "date")]
        public DateTime? measurement_date { get; set; }

        public decimal? height { get; set; }

        public decimal? weight { get; set; }

        public int? Client_profileId { get; set; }

        public virtual Client_profile Client_profile { get; set; }
    }
}

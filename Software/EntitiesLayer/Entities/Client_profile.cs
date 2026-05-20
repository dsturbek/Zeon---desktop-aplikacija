namespace EntitiesLayer.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Client_profile
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Client_profile()
        {
            Client_measurement = new HashSet<Client_measurement>();
            Personal_record = new HashSet<Personal_record>();
        }

        [Key]
        public int id_client_profile { get; set; }

        public int? ClientId { get; set; }

        public decimal? height { get; set; }

        public decimal? weight { get; set; }

        [StringLength(1)]
        public string gender { get; set; }

        [Column(TypeName = "date")]
        public DateTime? birth_date { get; set; }

        public virtual Client Client { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Client_measurement> Client_measurement { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Personal_record> Personal_record { get; set; }
    }
}

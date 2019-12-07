namespace WienerLinienApp.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Haltestelle")]
    public partial class Haltestelle
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Haltestelle()
        {
            Steigs = new HashSet<Steig>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int H_ID { get; set; }

        [Required]
        [StringLength(200)]
        public string H_Name { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Steig> Steigs { get; set; }
    }
}

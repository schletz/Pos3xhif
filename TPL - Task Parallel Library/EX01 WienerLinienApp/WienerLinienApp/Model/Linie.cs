namespace WienerLinienApp.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Linie")]
    public partial class Linie
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Linie()
        {
            Steigs = new HashSet<Steig>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int L_ID { get; set; }

        [Required]
        [StringLength(200)]
        public string L_Bezeichnung { get; set; }

        [Required]
        [StringLength(200)]
        public string L_Verkehrsmittel { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Steig> Steigs { get; set; }
    }
}

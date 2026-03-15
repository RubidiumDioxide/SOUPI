using System.ComponentModel.DataAnnotations;


namespace SOUPIShared.Models
{
    public class JobSequence
    {
        public Guid Id { get; set; }

        [Required]
        public Guid FirstJobId { get; set; }

        [Required]
        public Guid SecondJobId { get; set; }

        public virtual Job FirstJob { get; set; } = default!; 
        public virtual Job SecondJob { get; set; } = default!; 
    }
}

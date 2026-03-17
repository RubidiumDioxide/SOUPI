using System.ComponentModel.DataAnnotations;
using SOUPIShared.Models; 


namespace SOUPIShared.Dtos
{
    public class JobSequenceDto
    {
        public Guid Id { get; set; }
        [Required]
        public Guid FirstJobId { get; set; }
        [Required]
        public Guid SecondJobId { get; set; }

        public JobSequenceDto(JobSequence jobSequence)
        {
            Id = jobSequence.Id; 
            FirstJobId = jobSequence.FirstJobId; 
            SecondJobId = jobSequence.SecondJobId; 
        }

        public JobSequenceDto() { } 
    }
}

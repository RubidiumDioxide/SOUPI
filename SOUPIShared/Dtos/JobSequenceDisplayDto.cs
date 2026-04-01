using System.ComponentModel.DataAnnotations;
using SOUPIShared.Models; 


namespace SOUPIShared.Dtos
{
    public class JobSequenceDisplayDto
    {
        public Guid Id { get; set; }
        [Required]
        public Guid FirstJobId { get; set; }
        [Required]
        public string FirstJobTitle { get; set; }
        [Required]
        public Guid SecondJobId { get; set; }
        [Required]
        public string SecondJobTitle { get; set; }

        public JobSequenceDisplayDto (JobSequence jobSequence)
        {
            Id = jobSequence.Id; 
            FirstJobId = jobSequence.FirstJobId;
            FirstJobTitle = jobSequence.FirstJob.Title; 
            SecondJobId = jobSequence.SecondJobId;
            SecondJobTitle = jobSequence.SecondJob.Title;
        }

        public JobSequenceDisplayDto() { } 
    }
}

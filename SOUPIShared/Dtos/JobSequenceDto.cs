using SOUPIShared.Models; 


namespace SOUPIShared.Dtos
{
    public class JobSequenceDto
    {
        public Guid FirstJobId { get; set; }

        public Guid SecondJobId { get; set; }


        public JobSequenceDto(JobSequence jobSequence)
        {
            FirstJobId = jobSequence.FirstJobId; 
            SecondJobId = jobSequence.SecondJobId; 
        }

        public JobSequenceDto() { } 
    }
}

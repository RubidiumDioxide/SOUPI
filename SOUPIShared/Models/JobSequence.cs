namespace SOUPIShared.Models
{
    public class JobSequence
    {
        public Guid FirstJobId {  get; set; } 
        
        public Guid SecondJobId { get; set; }


        public virtual Job FirstJob { get; set; } = default!; 
        public virtual Job SecondJob { get; set; } = default!; 
    }
}

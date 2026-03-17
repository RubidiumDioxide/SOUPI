using SOUPIShared.Models; 


namespace SOUPIShared.Extensions
{
    public static class JobExtensions
    {
        public static bool IsSameLevel(this Job firstJob, Job secondJob)
        {
            return firstJob.ParentJobId == secondJob.ParentJobId;
        }
    }
}

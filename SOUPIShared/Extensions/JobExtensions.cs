using SOUPIShared.Dtos;
using SOUPIShared.Models; 


namespace SOUPIShared.Extensions
{
    public static class JobExtensions
    {
        public static bool IsSameLevel(this Job firstJob, Job secondJob)
        {
            return firstJob.ParentJobId == secondJob.ParentJobId;
        }

        public static void CopyContentProperties(this Job firstJob, Job secondJob)
        {
            firstJob.Title = secondJob.Title;
            firstJob.Body = secondJob.Body;
            firstJob.StartDateTime = secondJob.StartDateTime;
            firstJob.EndDateTime = secondJob.EndDateTime;
            firstJob.Progress = secondJob.Progress; 
            firstJob.Status = secondJob.Status;
        }

        public static void CopyContentProperties(this Job job, JobDto jobDto)
        {
            job.Title = jobDto.Title;
            job.Body = jobDto.Body;
            job.StartDateTime = jobDto.StartDateTime;
            job.EndDateTime = jobDto.EndDateTime; 
            job.Progress = jobDto.Progress; 
            job.Status = jobDto.Status;
        }
    }
}

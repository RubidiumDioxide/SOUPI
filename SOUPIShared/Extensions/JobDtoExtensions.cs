using SOUPIShared.Models; 
using SOUPIShared.Dtos;  


namespace SOUPIShared.Extensions
{
    public static class JobDtoExtensions
    {
        public static bool IsEquivalent(this JobDto jobDto, Job job)
        {
            return jobDto.Id == job.Id && 
                jobDto.ProjectId == job.ProjectId &&
                jobDto.CreatorId == job.CreatorId &&
                jobDto.Title == job.Title &&
                jobDto.Body == job.Body &&
                jobDto.StartDateTime == job.StartDateTime &&
                jobDto.EndDateTime == job.EndDateTime &&
                jobDto.Progress == job.Progress &&
                jobDto.CreationDateTime == job.CreationDateTime &&
                jobDto.Status == job.Status &&
                jobDto.ParentJobId == job.ParentJobId;
        }

        public static bool ArePropertiesEquivalent(this JobDto jobDto, Job job)
        {
            return jobDto.ProjectId == job.ProjectId &&
                jobDto.CreatorId == job.CreatorId &&
                jobDto.Title == job.Title &&
                jobDto.Body == job.Body &&
                jobDto.StartDateTime == job.StartDateTime &&
                jobDto.EndDateTime == job.EndDateTime &&
                jobDto.Progress == job.Progress &&
                jobDto.CreationDateTime == job.CreationDateTime &&
                jobDto.Status == job.Status &&
                jobDto.ParentJobId == job.ParentJobId; 
        }

        public static bool ArePropertiesEquivalent(this JobDto firstJobDto, JobDto secondJobDto)
        {
            return firstJobDto.ProjectId == secondJobDto.ProjectId &&
                firstJobDto.CreatorId == secondJobDto.CreatorId &&
                firstJobDto.Title == secondJobDto.Title &&
                firstJobDto.Body == secondJobDto.Body &&
                firstJobDto.StartDateTime == secondJobDto.StartDateTime &&
                firstJobDto.EndDateTime == secondJobDto.EndDateTime &&
                firstJobDto.Progress == secondJobDto.Progress &&
                firstJobDto.CreationDateTime == secondJobDto.CreationDateTime &&
                firstJobDto.Status == secondJobDto.Status &&
                firstJobDto.ParentJobId == secondJobDto.ParentJobId;
        }
    }
}

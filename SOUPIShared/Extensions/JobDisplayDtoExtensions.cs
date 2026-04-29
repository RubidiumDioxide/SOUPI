using SOUPIShared.Dtos;  


namespace SOUPIShared.Extensions
{
    public static class JobDisplayDtoExtensions
    {
        public static bool IsEquivalent(this JobDisplayDto firstJobDisplayDto, JobDisplayDto secondJobDisplayDto)
        {
            return firstJobDisplayDto.Id == secondJobDisplayDto.Id &&
                firstJobDisplayDto.ProjectId == secondJobDisplayDto.ProjectId &&
                firstJobDisplayDto.CreatorId == secondJobDisplayDto.CreatorId &&
                firstJobDisplayDto.Title == secondJobDisplayDto.Title &&
                firstJobDisplayDto.Body == secondJobDisplayDto.Body &&
                firstJobDisplayDto.StartDateTime == secondJobDisplayDto.StartDateTime &&
                firstJobDisplayDto.EndDateTime == secondJobDisplayDto.EndDateTime &&
                firstJobDisplayDto.Progress == secondJobDisplayDto.Progress &&
                firstJobDisplayDto.CreationDateTime == secondJobDisplayDto.CreationDateTime &&
                firstJobDisplayDto.Status == secondJobDisplayDto.Status &&
                firstJobDisplayDto.ParentJobId == secondJobDisplayDto.ParentJobId; 
        }

        public static bool IsEquivalent(this JobDisplayDto jobDisplayDto, GanttJobDto ganttJobDto)
        {
            try
            {
                return jobDisplayDto.Id == Guid.Parse(ganttJobDto.id) &&
                       jobDisplayDto.Title == ganttJobDto.name &&
                       jobDisplayDto.StartDateTime == ganttJobDto.start &&
                       jobDisplayDto.EndDateTime == ganttJobDto.end &&
                       jobDisplayDto.Progress == ganttJobDto.progress &&
                       jobDisplayDto.Dependencies == ganttJobDto.dependencies;
            }
            catch
            {
                return false;
            }
        }
    }
}

using SOUPIShared.Models; 


namespace SOUPIShared.Dtos
{
    public class GanttJob
    {
        public string id { get; set; } = "";
        public string name { get; set; } = "";
        public DateTime? start { get; set; }
        public DateTime? end { get; set; }
        public int progress { get; set; }
        public string? dependencies { get; set; } 

        public GanttJob(Job job) 
        {
            id = job.Id.ToString(); 
            name = job.Title; 
            start = job.StartDateTime; 
            end = job.EndDateTime; 
            progress = job.Progress; 
            dependencies = (job.PreviousJob != null)? job.PreviousJob.Id.ToString() : null; 
        }

        public GanttJob() { } 
    }
}

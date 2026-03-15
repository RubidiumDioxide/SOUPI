using SOUPIShared.Models; 


namespace SOUPIShared.Dtos
{
    public class GanttJobDto
    {
        public string id { get; set; } = "";
        public string name { get; set; } = "";
        public DateTime? start { get; set; }
        public DateTime? end { get; set; }
        public int progress { get; set; }
        public string? dependencies { get; set; } 


        public GanttJobDto(Job job) 
        {
            id = job.Id.ToString(); 
            name = job.Title; 
            start = job.StartDateTime; 
            end = job.EndDateTime; 
            progress = job.Progress;
            dependencies = string.Join(", ", job.PreviousJobSequences.Select(js => js.FirstJobId.ToString())); 
        }

        public GanttJobDto() { } 
    }
}

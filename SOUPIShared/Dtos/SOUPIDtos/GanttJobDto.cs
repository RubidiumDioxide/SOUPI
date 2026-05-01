using SOUPIShared.Models;


namespace SOUPIShared.Dtos.SOUPIDtos
{
    public class GanttJobDto
    {
        public string id { get; set; } = "";
        public string name { get; set; } = "";
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public int progress { get; set; }
        public string? dependencies { get; set; } 


        public GanttJobDto(JobDto jobDto) 
        {
            id = jobDto.Id.ToString(); 
            name = jobDto.Title; 
            start = jobDto.StartDateTime; 
            end = jobDto.EndDateTime; 
            progress = jobDto.Progress;
            dependencies = jobDto.Dependencies; 
        }

        public GanttJobDto() { } 
    }
}

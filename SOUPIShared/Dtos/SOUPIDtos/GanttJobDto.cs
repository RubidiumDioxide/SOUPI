namespace SOUPIShared.Dtos.SOUPIDtos
{
    public class GanttJobDto
    {
        public string id { get; set; } = "";
        public string name { get; set; } = "";
        public string start { get; set; } = default!;
        public string end { get; set; } = default!; 
        public int progress { get; set; }
        public List<string>? dependencies { get; set; }
        public string? custom_class { get; set; } 


        public GanttJobDto(JobDto jobDto) 
        {
            id = jobDto.Id.ToString(); 
            name = jobDto.Title; 
            start = jobDto.StartDateTime.ToString("yyyy-MM-dd"); ; 
            end = jobDto.EndDateTime.ToString("yyyy-MM-dd"); ; 
            progress = jobDto.Progress;
            dependencies = jobDto.Dependencies; 
            custom_class = jobDto.HasChildren? "no-progress-drag" : null;
        }

        public GanttJobDto() { } 
    }
}

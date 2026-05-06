using SOUPIShared.Misc;
using MudBlazor; 


namespace SOUPIShared.Extensions
{
    public static class JobStatusExtensions
    {
        public static Color GetStatusColor(this JobStatus status) => status switch
        {
            JobStatus.New => Color.Info,
            JobStatus.Working => Color.Warning,
            JobStatus.Completed => Color.Success,
            _ => Color.Default
        };

        public static string TranslateStatus(this JobStatus status) => status switch
        {
            JobStatus.New => "Новая",
            JobStatus.Working => "В процессе",
            JobStatus.Completed => "Завершена",
            _ => status.ToString()
        };
    }
}

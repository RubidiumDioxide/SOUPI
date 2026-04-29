using SOUPIShared.Models;
using System.ComponentModel.DataAnnotations;
using SOUPIShared.Attributes; 


namespace SOUPIShared.Dtos
{
    public class ActivityDisplayDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid AssignmentId { get; set; }

        [Required]
        [ValidGitHubUsername]
        public string TeamMemberLogin { get; set; } 

        [ValidCommitHash]
        public string? Commit { get; set; }

        [MaxLength(255, ErrorMessage = "Комментарий слишком длинный (максимум 255 символов)")]
        [ConsistsOfNumbersCyrillicLatin]
        public string? Comment { get; set; }


        public ActivityDisplayDto(Activity activity)
        {
            Id = activity.Id; 
            AssignmentId = activity.AssignmentId;
            TeamMemberLogin = activity.Assignment.TeamMember.User.Login;
            Commit = activity.Commit; 
            Comment = activity.Comment; 
        }

        public ActivityDisplayDto () { }
    }
}

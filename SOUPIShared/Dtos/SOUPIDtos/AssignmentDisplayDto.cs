using System.ComponentModel.DataAnnotations;
using SOUPIShared.Models;
using SOUPIShared.Attributes;


namespace SOUPIShared.Dtos.SOUPIDtos
{
    public class AssignmentDisplayDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid TeamMemberId { get; set; } = default!;

        [Required]
        [ValidGitHubUsername] 
        public string TeamMemberLogin { get; set; } = default!; 

        [Required]
        public Guid JobId { get; set; } = default!;

        [Required]
        [ConsistsOfNumbersCyrillicLatin]
        public string JobTitle { get; set; } = default!;

        [Required]
        public Guid ProjectId { get; set; } = default!;

        [Required]
        [ConsistsOfNumbersCyrillicLatin]
        public string ProjectTitle { get; set; } = default!;

        [MaxLength(255, ErrorMessage = "Комментарий слишком длинный (максимум 255 символов)")]
        [ConsistsOfNumbersCyrillicLatin]
        public string? Comment { get; set; }


        public AssignmentDisplayDto(Assignment assignment)
        {
            Id = assignment.Id; 
            TeamMemberId = assignment.TeamMemberId;
            TeamMemberLogin = assignment.TeamMember.User.Login; 
            JobId = assignment.JobId;
            JobTitle = assignment.Job.Title;
            ProjectId = assignment.Job.Project.Id;
            ProjectTitle = assignment.Job.Project.Title; 
            Comment = assignment.Comment; 
        }

        public AssignmentDisplayDto() { } 
    }
}

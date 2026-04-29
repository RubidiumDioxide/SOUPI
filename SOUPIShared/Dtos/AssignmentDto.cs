using System.ComponentModel.DataAnnotations;
using SOUPIShared.Models;
using SOUPIShared.Attributes; 


namespace SOUPIShared.Dtos
{
    public class AssignmentDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid TeamMemberId { get; set; } = default!;

        [Required]
        public Guid JobId { get; set; } = default!;

        [MaxLength(255, ErrorMessage = "Комментарий слишком длинный (максимум 255 символов)")]
        [ConsistsOfNumbersCyrillicLatin]
        public string? Comment { get; set; }


        public AssignmentDto(Assignment assignment)
        {
            Id = assignment.Id; 
            TeamMemberId = assignment.TeamMemberId; 
            JobId = assignment.JobId; 
            Comment = assignment.Comment; 
        }

        public AssignmentDto(AssignmentDto assignmentDto)
        {
            Id = assignmentDto.Id;
            TeamMemberId = assignmentDto.TeamMemberId;
            JobId = assignmentDto.JobId;
            Comment = assignmentDto.Comment;
        }

        public AssignmentDto(AssignmentDisplayDto assignmentDisplayDto)
        {
            Id = assignmentDisplayDto.Id;
            TeamMemberId = assignmentDisplayDto.TeamMemberId;
            JobId = assignmentDisplayDto.JobId;
            Comment = assignmentDisplayDto.Comment;
        }

        public AssignmentDto() { } 
    }
}

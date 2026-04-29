using SOUPIShared.Models;
using System.ComponentModel.DataAnnotations;
using SOUPIShared.Attributes;


namespace SOUPIShared.Dtos
{
    public class ActivityDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid AssignmentId { get; set; }

        [ValidCommitHash]
        public string? Commit { get; set; }

        [MaxLength(255, ErrorMessage = "Комментарий слишком длинный (максимум 255 символов)")]
        [ConsistsOfNumbersCyrillicLatin]
        public string? Comment { get; set; }


        public ActivityDto(Activity activity)
        {
            Id = activity.Id; 
            AssignmentId = activity.AssignmentId; 
            Commit = activity.Commit; 
            Comment = activity.Comment; 
        }
        public ActivityDto(ActivityDisplayDto activityDisplayDto)
        {
            Id = activityDisplayDto.Id;
            AssignmentId = activityDisplayDto.AssignmentId;
            Commit = activityDisplayDto.Commit;
            Comment = activityDisplayDto.Comment;
        }

        public ActivityDto() { }
    }
}

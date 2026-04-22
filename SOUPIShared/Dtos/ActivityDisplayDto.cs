using SOUPIShared.Models;
using System.ComponentModel.DataAnnotations;


namespace SOUPIShared.Dtos
{
    public class ActivityDisplayDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid AssignmentId { get; set; }

        [Required]
        public string TeamMemberLogin { get; set; } 

        [MaxLength(40, ErrorMessage = "Длина хэша коммита - 40 символов")]
        [MinLength(40, ErrorMessage = "Длина хэша коммита - 40 символов")]
        public string? Commit { get; set; }

        [MaxLength(255, ErrorMessage = "Комментарий слишком длинный (максимум 255 символов)")]
        public string? Comment { get; set; }


        public ActivityDisplayDto(Activity activity)
        {
            Id = activity.Id; 
            AssignmentId = activity.AssignmentId;
            TeamMemberLogin = activity.Assignment.TeamMember.User.Login;
            Commit = activity.Commit; 
            Comment = activity.Comment; 
        }
    }
}

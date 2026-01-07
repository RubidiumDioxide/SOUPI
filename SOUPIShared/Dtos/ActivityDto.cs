using System.ComponentModel.DataAnnotations;
using SOUPIShared.Models; 


namespace SOUPIShared.Dtos
{
    public class ActivityDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid AssignmentId { get; set; }

        [MaxLength(40, ErrorMessage = "Длина хэша коммита - 40 символов")]
        [MinLength(40, ErrorMessage = "Длина хэша коммита - 40 символов")]
        public string? Commit { get; set; }

        [MaxLength(255, ErrorMessage = "Комментарий слишком длинный (максимум 255 символов)")]
        public string? Comment { get; set; }
    

        public ActivityDto(Activity sctivity)
        {
            Id = sctivity.Id; 
            AssignmentId = sctivity.AssignmentId; 
            Commit = sctivity.Commit; 
            Comment = sctivity.Comment; 
        }

        public ActivityDto() { }
    }
}

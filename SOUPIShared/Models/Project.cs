using System.ComponentModel.DataAnnotations;


namespace SOUPIShared.Models 
{
    public class Project
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Поле названия обязательное")]
        [MaxLength(100, ErrorMessage = "Название слишком длинное (максимум 100 символов)")]
        [MinLength(1, ErrorMessage = "Название слишком короткое (минимум 1 символ)")]
        public string Name { get; set; } = null!;

        [MaxLength(255, ErrorMessage = "Описание слишком длинное (максимум 255 символов)")]
        public string? Description { get; set; }

        [MaxLength(255, ErrorMessage = "Название репозитория слишком длинное (максимум 255 символов)")]
        [MinLength(1, ErrorMessage = "Название репозитория слишком короткое (минимум 1 символ)")]
        public string? GithubRepository { get; set; }

        [Required]
        public Guid CreatorId { get; set; }

        [Required]
        public DateTime CreationDateTime { get; set; } = DateTime.Now;

        public string? Image { get; set; }


        public virtual User Creator { get; set; } = default!;
        public virtual List<TeamMember> TeamMembers { get; set; } = default!; 
        public virtual List<Notification> Notifications { get; set; } = default!; 
    }
}

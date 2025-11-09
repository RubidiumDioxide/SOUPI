using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;


namespace SOUPIShared.Models 
{
    public partial class Project
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Поле названия обязательное")]
        [MaxLength(100, ErrorMessage = "Название слишком длинное (максимум 50 символов)")]
        [MinLength(1, ErrorMessage = "Название слишком короткое (минимум 1 символ)")]
        public string Name { get; set; } = null!;

        [MaxLength(255, ErrorMessage = "Описание слишком длинное (максимум 100 символов)")]
        [MinLength(1, ErrorMessage = "Описание слишком короткое (минимум 1 символ)")]
        public string? Description { get; set; }

        public string? GithubRepository { get; set; }

        [Required]
        public Guid CreatorId { get; set; }

        public DateTime CreationDateTime { get; set; } = DateTime.Now;

        public string? Image { get; set; }


        public virtual User Creator { get; set; } = null!;
    }

}

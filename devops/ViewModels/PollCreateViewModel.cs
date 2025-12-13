using System.ComponentModel.DataAnnotations;

namespace DevOpsPollApp.ViewModels
{
    public class PollCreateViewModel
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string OptionsRaw { get; set; } = string.Empty;
    }
}

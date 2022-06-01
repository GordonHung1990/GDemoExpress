using System.ComponentModel.DataAnnotations;

namespace GDemoExpress.ViewModels
{
    public record PlayerAddViewModel
    {
        [Required]
        public string Account { get; init; } = default!;
        [Required]
        public string Password { get; init; } = default!;
        [Required]
        public string NickName { get; init; } = default!;
    }
}

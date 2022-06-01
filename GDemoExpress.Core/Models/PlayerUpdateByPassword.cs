namespace GDemoExpress.Core.Models
{
    public record PlayerUpdateByPassword
    {
        /// <summary>
        /// The player identifier
        /// </summary>
        public Guid PlayerId { get; set; }

        /// <summary>
        /// The password
        /// </summary>
        public string Password { get; set; } = default!;
    }
}

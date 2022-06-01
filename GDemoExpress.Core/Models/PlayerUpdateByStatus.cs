namespace GDemoExpress.Core.Models
{
    public record PlayerUpdateByStatus
    {
        /// <summary>
        /// The player identifier
        /// </summary>
        public Guid PlayerId { get; set; }
        /// <summary>
        /// The status
        /// </summary>
        public int Status { get; set; }
    }
}

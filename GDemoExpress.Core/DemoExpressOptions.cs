namespace GDemoExpress.Core
{
    public record DemoExpressOptions
    {
        /// <summary>
        /// The admin user
        /// </summary>
        public string AdminUser { get; init; } = default!;

        /// <summary>
        /// The admin password
        /// </summary>
        public string AdminPassword { get; init; } = default!;

        /// <summary>
        /// The admin cryptography key/
        /// </summary>
        public string AdminCryptographyKey { get; init; } = default!;

        /// <summary>
        /// The player password
        /// </summary>
        public string PlayerPassword { get; init; } = default!;

        /// <summary>
        /// The player cryptography key
        /// </summary>
        public string PlayerCryptographyKey { get; init; } = default!;
    }
}

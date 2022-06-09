namespace GDemoExpress.Core.Models
{
    public record PlayerData(
        Guid PlayerId,
        string Account,
        PlayerStatus Status,
        string? LastName,
        string? FullName,
        string? NickName,
        string? PhoneNumber,
        string? Mailbox,
        DateTimeOffset CreatedOn,
        DateTimeOffset UpdatedOn);
}

using GDemoExpress.Core;

namespace GDemoExpress.ViewModels
{
    public record PlayerViewModel(
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

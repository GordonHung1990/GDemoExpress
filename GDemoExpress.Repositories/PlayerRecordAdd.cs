using GDemoExpress.Core;

namespace GDemoExpress.Repositories
{
    internal record PlayerRecordAdd(
        Guid PlayerId,
        string Account,
        OperationType OperationType,
        string OldData,
        string NewData);
}

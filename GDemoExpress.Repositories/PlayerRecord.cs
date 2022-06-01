using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace GDemoExpress.Repositories
{
    public record PlayerRecord
    {
        /// <summary>
        /// The player record identifier
        /// </summary>
        [BsonId]
        public Guid PlayerRecordId { get; init; }

        /// <summary>
        /// The player identifier
        /// </summary>
        [BsonElement("player_id")]
        public Guid PlayerId { get; set; }

        /// <summary>
        /// The account
        /// </summary>
        [BsonElement("account")]
        [BsonRepresentation(BsonType.String)]
        public string Account { get; set; } = default!;

        /// <summary>
        /// The operation type
        /// </summary>
        [BsonElement("operation_type")]
        [BsonRepresentation(BsonType.Int32)]
        public int OperationType { get; set; }

        /// <summary>
        /// The old data
        /// </summary>
        [BsonDefaultValue("{}")]
        [BsonElement("old_data")]
        [BsonRepresentation(BsonType.String)]
        public string OldData { get; set; } = default!;

        /// <summary>
        /// Creates new data.
        /// </summary>
        [BsonElement("new_data")]
        [BsonRepresentation(BsonType.String)]
        public string NewData { get; set; } = default!;

        /// <summary>
        /// The operation on
        /// </summary>
        [BsonElement("operation_on")]
        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset OperationOn { get; set; }
    }
}

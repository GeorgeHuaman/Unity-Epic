using Postgrest.Attributes;
using Postgrest.Models;
using System;

[Table("user_topic_progress")]

public class UserTopicProgress : BaseModel
{
    [PrimaryKey("user_id", false)] public string UserId { get; set; }
    [PrimaryKey("topic_id", false)] public string TopicId { get; set; }
    [PrimaryKey("level", false)] public short Level { get; set; }

    [Column("progress")] public decimal Progress { get; set; }
    [Column("completed")] public bool Completed { get; set; }
    [Column("updated_at")] public DateTime UpdatedAt { get; set; }
}

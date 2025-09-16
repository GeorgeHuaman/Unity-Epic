using Postgrest.Attributes;
using Postgrest.Models;

[Table("topics")]
public class Topic : BaseModel
{
    [PrimaryKey("id", false)] public string Id { get; set; }
    [Column("slug")] public string Slug { get; set; }
    [Column("name")] public string Name { get; set; }
    [Column("levels")] public int Levels { get; set; }
}

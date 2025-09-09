using Postgrest.Attributes;
using Postgrest.Models;

[Table("profiles")]
public class Profile : BaseModel
{
    [PrimaryKey("id", false)]
    public string Id { get; set; } // UUID = auth.uid()

    [Column("username")]
    public string Username { get; set; }

    [Column("level")]
    public int Level { get; set; }
}

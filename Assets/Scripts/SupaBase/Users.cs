using Postgrest.Attributes;
using Postgrest.Models;

[Table("users")]
public class Users : BaseModel
{
    [PrimaryKey("id", false)]
    public string UID { get; set; } // UUID = auth.uid()

    [Column("email")]
    public string email { get; set; }

    [Column("full_name")]
    public string full_name { get; set; }
}

using Postgrest.Attributes;
using Postgrest.Models;

namespace MixBrev.Models
{
    // La clase UserAccount hereda de BaseModel, lo que le permite tener un ID autogenerado y otras propiedades comunes a los modelos de datos.
    [Table("User_accounts")]
    public class UserAccount : BaseModel
    {

    [PrimaryKey("fk_user_id", false)] // "false" porque el ID lo genera la DB o viene de fuera
    public string Id { get; set; }

    [Column("email")]
    public string Email { get; set; }

    [Column("Company")]
     public string Company { get; set; }

     [Column("first_name")]
        public string Name { get; set; }

     [Column("last_names")]
        public string last_names { get; set; }
 
    }
}

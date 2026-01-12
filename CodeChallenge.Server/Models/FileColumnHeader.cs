using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CodeChallenge.Server.Models
{
    public class FileColumnHeader
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FileColumnHeaderID { get; set; }
        public string ColumnHeaderName { get; set; }
    }
}

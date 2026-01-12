using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CodeChallenge.Server.Models
{
    public class TeamStats
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TeamStatsID { get; set; }
        public int Rank { get; set; }
        public string Team { get; set; }
        public string Mascot { get; set; }
        public string LastWinDate { get; set; }
        public double Percentage { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Ties { get; set; }
        public int Games { get; set; }
    } 
}

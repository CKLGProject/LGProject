using System;
using Postgrest.Attributes;
using Postgrest.Models;
namespace Data
{
    [Table("Ranking")]
    public class RankingModel : BaseModel
    {
        [PrimaryKey("id")] public int Id { get; set; }

        [Column("created_at")] public DateTime CreatedAt { get; set; }

        [Column("profile_nickname")] public string Nickname { get; set; }

        [Column("profile_image_type")] public int Thumbnail { get; set; }
        
        [Column("name")] public string UserName { get; set; }

        [Column("ci")] public string CI { get; set; }

        [Column("score")] public int Score { get; set; }
        [Column("class")] public int Class { get; set; }

        public override bool Equals(object obj)
        {
            return obj is RankingModel data && Id == data.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public RankingModel()
        {
            Nickname = $"갸오";
            UserName = "김갸오";
            CreatedAt = DateTime.Now;
            Thumbnail = 0;
            CI = Guid.NewGuid().ToString();
            Score = 0;
            Class = 1;
        }
    }
}

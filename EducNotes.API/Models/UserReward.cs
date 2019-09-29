using System;

namespace EducNotes.API.Models
{
    public class UserReward
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public int RewardId { get; set; }
        public Reward Reward { get; set; }
        public int RewardedById { get; set; }
        public User RewardedBy { get; set; }
        public DateTime RewardDate { get; set; }
        public string Reason { get; set; }
        public string Comment { get; set; }
    }

}
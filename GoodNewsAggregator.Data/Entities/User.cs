namespace GoodNewsAggregator.Data.Entities
{
    public class User : IBaseEntity
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public double PreferredRate { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
        public List<Comment> Comments { get; set; }
        
    }
    
}
namespace FlowDash.Domain.Entities
{
    public partial class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public DateTime ExpiresOn { get; set; }
        public bool IsRevoked { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; } = null!;
    }
}

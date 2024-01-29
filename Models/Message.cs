
namespace LessonFive.Models
{
    public partial class Message
    {
        public int Id { get; set; }

        public int? FromUserId { get; set; }
        public int? ToUserId { get; set; }

        public string Text { get; set; } = null!;

        public virtual User? ToUser { get; set; }
        public virtual User? FromUser { get; set; }

        public List<string> UnreadMessages { get; set; }
        public bool Received { get; set; }

    }
}

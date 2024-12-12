namespace AiComp.Domain.Entities
{
    public class User : BaseEntity
    {
        public string? Email { get; private set; }
        public string? Password { get; private set; }
        public bool IsConsented { get; set; } = default;        
        public Conversation? Conversation { get; set; }
        public Profile? Profile { get; set; }

        public List<MoodLog> MoodLogs = new List<MoodLog>();

        public List<MoodMessage> MoodMessages = new List<MoodMessage>();

        public User(string email) { Email = email; }

        public string AddPassword(string password) => Password = BCrypt.Net.BCrypt.HashPassword(password);

        public void AddMoodLog(MoodLog mood)
        {
            MoodLogs.Add(mood);
        }
        
        public void AddMoodMessage(MoodMessage moodMessage)
        {
            MoodMessages.Add(moodMessage);
        }

        public void AddConversation(Conversation converse)
        {
            Conversation = converse;
        }

        public void AddProfile(Profile profile)
        {
            Profile = profile;
        }

        public void UpdateEmail(string email)
        {
            Email = email;
        }

    }
}

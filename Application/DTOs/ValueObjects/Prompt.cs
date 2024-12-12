namespace AiComp.Application.DTOs.ValueObjects
{
    public class Prompt
    {
        public DateTime TimeCreated { get; set; }
        public string? ChatPromptToAi { get; set; }

        public Prompt(string response)
        {
            ChatPromptToAi = response;
            TimeCreated = DateTime.Now;
        }
    }
}

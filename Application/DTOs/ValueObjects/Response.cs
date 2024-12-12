namespace AiComp.Application.DTOs.ValueObjects
{
    public class Response
    {
        public DateTime TimeCreated { get; set; }
        public string? AiResponse {  get;  set; }

        public Response(string response) 
        { 
            AiResponse = response;
            TimeCreated = DateTime.Now;
        }
    }
}

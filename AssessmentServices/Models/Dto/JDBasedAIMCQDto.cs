namespace AssessmentServices.Models.Dto
{
    public class JDBasedAIMCQDto
    {
        public int Id { get; set; }
        public string JobId { get; set; } // Change to string
        public string Question { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
        public string CorrectOption { get; set; }
    }
}

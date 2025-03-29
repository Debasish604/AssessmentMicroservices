namespace AssessmentServices.Models.Dto
{
    public class AssessmentDto
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int CandidateId { get; set; }
        public string AssessmentName { get; set; }
        public int AssessmentSqnc { get; set; }
        public string Status { get; set; }
        public int? Score { get; set; }
    }
}

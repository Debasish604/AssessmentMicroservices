namespace AssessmentServices.Models.Entity
{
    public class Assessment
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int CandidateId { get; set; }
        public int AssessmentSqnc { get; set; }
        public string AssessmentName { get; set; }
        public string Status { get; set; }
        public int? Score { get; set; }
    }
}

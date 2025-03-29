namespace AssessmentServices.Models.Dto
{
    public class EvaluateRequestDto
    {
        public int JobId { get; set; }
        public int CandidateId { get; set; }

        public int AssessmentId { get; set; }
        public List<MCQAnswerDto> Data { get; set; }
    }

    public class MCQAnswerDto
    {
        public int Id { get; set; }
        public string SelectedOption { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace AssessmentServices.Models.Entity
{

    [Table("JobApplication", Schema = "ADANI_TALENT")]
    public class JobApplications
    {
        public int Id { get; set; }
        public int JobId { get; set; }
        public int CandidateId { get; set; }
        public string LatestStatus { get; set; }
        public string Score { get; set; }
    }
}

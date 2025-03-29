using System.ComponentModel.DataAnnotations.Schema;

namespace AssessmentServices.Models.Entity
{
    [Table("GenericThreshold", Schema = "ADANI_TALENT")]
    public class GenericThreshold
    {
        public int Id { get; set; }
        public string RuleName { get; set; }
        public string RuleValue { get; set; }
    }
}

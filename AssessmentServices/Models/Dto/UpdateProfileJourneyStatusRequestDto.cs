public class UpdateProfileJourneyStatusRequestDto
{
    public int JobId { get; set; } 
    public int CandidateId { get; set; } 
    public string ProfileJourney { get; set; } 
    public string Status { get; set; }
    public int? Score { get; set; } 
}


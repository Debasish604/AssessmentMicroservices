using System.Data;
using AssessmentServices.Data;
using AssessmentServices.Models.Dto;
using AssessmentServices.Models.Entity;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AssessmentServices.Controllers
{
    [Route("ASSESSMENTSERVICE")]
    [ApiController]
    public class AssessmentController : ControllerBase
    {
        private readonly AppDBContext _db;
        private ResponseDto _response;
        private IMapper _mapper;

        public AssessmentController(AppDBContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
        }

        // API 1: Fetch Assessment State
        [HttpGet("JOB/CANDIDATE/ASSESSMENTSTATE/{jobId:int}/{candidateId:int}")]
        public ResponseDto GetByJobAndCandidate(int jobId, int candidateId)
        {
            try
            {
                // Fetch all matching records
                List<Assessment> assessments = _db.JobAssessments
                    .Where(a => a.JobId == jobId && a.CandidateId == candidateId)
                    .OrderBy(a=>a.AssessmentSqnc)
                    .ToList();

                if (assessments == null || assessments.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found.";
                }
                else
                {
                    // Map the list to DTOs
                    _response.Result = _mapper.Map<IEnumerable<AssessmentDto>>(assessments);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        // API 2: Fetch AI MCQ Questions
        [HttpGet("ASSESSMENT/JOB/CANDIDATE/GETMCQ/{AID:int}/{jobId:int}/{CID:int}")]
        public ResponseDto GetAIMCQByJob(int AID, int jobId, int CID)
        {
            try
            {
                // Fetch data where JobId matches
                var mcqList = _db.Set<JDBasedAIMCQ>()
                                 .Where(m => m.JobId == jobId.ToString())
                                 .ToList();

                if (mcqList == null || mcqList.Count == 0)
                {
                    _response.IsSuccess = false;
                    _response.Message = "No records found for the given JobId.";
                }
                else
                {
                    // Map the entity list to DTO list
                    _response.Result = _mapper.Map<IEnumerable<JDBasedAIMCQDto>>(mcqList);
                }
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }

        // API 3: Evaluate MCQ
        [HttpPost("EvaluateMCQ")]
        public ResponseDto EvaluateMCQ([FromBody] EvaluateRequestDto request)
        {
            try
            {
                // Fetch MCQ data from the database
                var mcqData = _db.Set<JDBasedAIMCQ>()
                                 .Where(mcq => mcq.JobId == request.JobId.ToString())
                                 .ToList();

                if (mcqData == null || !mcqData.Any())
                {
                    _response.IsSuccess = false;
                    _response.Message = "No MCQ data found for the given JobId.";
                    return _response;
                }

                // Calculate the total matches
                int totalQuestions = request.Data.Count;
                int correctAnswers = 0;

                foreach (var item in request.Data)
                {
                    var question = mcqData.FirstOrDefault(mcq => mcq.Id == item.Id);

                    if (question != null && question.CorrectOption == item.SelectedOption)
                    {
                        correctAnswers++;
                    }
                }

                // Calculate percentage
                double percentage = (double)correctAnswers / totalQuestions * 100;

                // Fetch threshold value for "Shortlist Candidates"
                var threshold = _db.GenericThresholds
                                   .FirstOrDefault(t => t.RuleName == "Shortlist Candidates");

                if (threshold == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Threshold rule not found.";
                    return _response;
                }

                if (!double.TryParse(threshold.RuleValue, out double ruleValue))
                {
                    _response.IsSuccess = false;
                    _response.Message = $"Unable to parse RuleValue: {threshold.RuleValue}";
                    return _response;
                }

                // Determine pass or fail
                string status = percentage >= ruleValue ? "PASSED" : "FAILED";
                int? score = status == "PASSED" ? (int?)percentage : null;

                // Call the stored procedure
                using (var command = _db.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "CALL \"ADANI_TALENT\".\"UpdateProfileJourneyStatus\"(@jobid, @candidateid, @profilejourney, @status, @score)";
                    command.CommandType = CommandType.Text;

                    var jobidParam = command.CreateParameter();
                    jobidParam.ParameterName = "@jobid";
                    jobidParam.Value = request.JobId;
                    command.Parameters.Add(jobidParam);

                    var candidateidParam = command.CreateParameter();
                    candidateidParam.ParameterName = "@candidateid";
                    candidateidParam.Value = request.CandidateId;
                    command.Parameters.Add(candidateidParam);

                    var profilejourneyParam = command.CreateParameter();
                    profilejourneyParam.ParameterName = "@profilejourney";
                    profilejourneyParam.Value = "ASSESSMENT";
                    command.Parameters.Add(profilejourneyParam);

                    var statusParam = command.CreateParameter();
                    statusParam.ParameterName = "@status";
                    statusParam.Value = status;
                    command.Parameters.Add(statusParam);

                    var scoreParam = command.CreateParameter();
                    scoreParam.ParameterName = "@score";
                    scoreParam.Value = score ?? (object)DBNull.Value; // Set NULL if the score is not applicable
                    command.Parameters.Add(scoreParam);

                    _db.Database.OpenConnection();
                    command.ExecuteNonQuery();
                    _db.Database.CloseConnection();
                }

                // Prepare response with uppercase keys
                _response.IsSuccess = true;
                _response.Result = new
                {
                    C_ID = request.CandidateId,
                    J_ID = request.JobId,
                    EVENT = "ASSESSMENT",
                    STATUS = status,
                    SCORE = score
                };
                _response.Message = "Success";
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
            }

            return _response;
        }
        [HttpPost("CallUpdateProfileJourneyStatus")]
        public AssessmentServices.Models.Dto.ResponseDto CallUpdateProfileJourneyStatus([FromBody] UpdateProfileJourneyStatusRequestDto request)
        {
            var response = new AssessmentServices.Models.Dto.ResponseDto();

            try
            {
                // Call the stored procedure
                using (var command = _db.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = "CALL \"ADANI_TALENT\".\"UpdateProfileJourneyStatus\"(@jobid, @candidateid, @profilejourney, @status, @score)";
                    command.CommandType = CommandType.Text;

                    var jobidParam = command.CreateParameter();
                    jobidParam.ParameterName = "@jobid";
                    jobidParam.Value = request.JobId;
                    command.Parameters.Add(jobidParam);

                    var candidateidParam = command.CreateParameter();
                    candidateidParam.ParameterName = "@candidateid";
                    candidateidParam.Value = request.CandidateId;
                    command.Parameters.Add(candidateidParam);

                    var profilejourneyParam = command.CreateParameter();
                    profilejourneyParam.ParameterName = "@profilejourney";
                    profilejourneyParam.Value = request.ProfileJourney;
                    command.Parameters.Add(profilejourneyParam);

                    var statusParam = command.CreateParameter();
                    statusParam.ParameterName = "@status";
                    statusParam.Value = request.Status;
                    command.Parameters.Add(statusParam);

                    var scoreParam = command.CreateParameter();
                    scoreParam.ParameterName = "@score";
                    scoreParam.Value = request.Score.HasValue ? (object)request.Score.Value : DBNull.Value; // Set NULL if score is not provided
                    command.Parameters.Add(scoreParam);

                    _db.Database.OpenConnection();
                    command.ExecuteNonQuery();
                    _db.Database.CloseConnection();
                }

                // Set success response
                response.IsSuccess = true;
                response.Result = new
                {
                    JobId = request.JobId,
                    CandidateId = request.CandidateId,
                    ProfileJourney = request.ProfileJourney,
                    Status = request.Status,
                    Score = request.Score
                };
                response.Message = "Stored procedure executed successfully.";
            }
            catch (Exception ex)
            {
                // Handle exceptions
                response.IsSuccess = false;
                response.Message = $"An error occurred: {ex.Message}";
            }

            return response;
        }




    }
}

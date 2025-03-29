using AutoMapper;
using AssessmentServices.Models.Dto;
using AssessmentServices.Models.Entity;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AssessmentServices
{
    public class MapperConfig : Profile
    {
        public MapperConfig()
        {
            CreateMap<Assessment, AssessmentDto>().ReverseMap();
            CreateMap<JDBasedAIMCQ, JDBasedAIMCQDto>().ReverseMap();
        }
    }
}

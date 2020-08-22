using AutoMapper;
using BIMonTime.Data.Entities;
using BIMonTime.Data.Models;

namespace BIMonTime.Data.Mappings
{
    public class WorkdayMapping : Profile
    {
        public WorkdayMapping()
        {
            CreateMap<Workday, WorkdayModel>().ReverseMap();
        }
    }
}

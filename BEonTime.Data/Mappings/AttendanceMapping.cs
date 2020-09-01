using AutoMapper;
using BEonTime.Data.Entities;
using BEonTime.Data.Models;
using System;

namespace BEonTime.Data.Mappings
{
    public class AttendanceMapping : Profile
    {
        public AttendanceMapping()
        {
            CreateMap<Attendance, AttendanceDetailModel>()
                .ForMember(attmodel => attmodel.Status,
                map => map.MapFrom(work =>
                    Enum.GetName(typeof(WorkdayStatus), work.Status).ToSentence()));
        }
    }
}

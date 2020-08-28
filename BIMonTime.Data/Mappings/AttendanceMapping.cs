using AutoMapper;
using BIMonTime.Data.Entities;
using BIMonTime.Data.Models;
using System;

namespace BIMonTime.Data.Mappings
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

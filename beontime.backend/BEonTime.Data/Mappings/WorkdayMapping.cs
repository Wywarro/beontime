using AutoMapper;
using BEonTime.Data.Entities;
using BEonTime.Data.Models;
using System;
using System.Linq;

namespace BEonTime.Data.Mappings
{
    public class WorkdayMapping : Profile
    {
        public WorkdayMapping()
        {
            CreateMap<Workday, WorkdayListModel>()
                .ForMember(workmodel => workmodel.Status,
                map => map.MapFrom(work =>
                    Enum.GetName(typeof(WorkdayStatus), work.Status).ToSentence()))
                .ForMember(workmodel => workmodel.AttendancesCount,
                map => map.MapFrom(work => work.Attendances.Count));

            CreateMap<Workday, WorkdayDetailModel>()
                .ForMember(workmodel => workmodel.Status,
                map => map.MapFrom(work =>
                    Enum.GetName(typeof(WorkdayStatus), work.Status).ToSentence()));

            CreateMap<WorkdayCreateModel, Workday>()
                .ForMember(work => work.Status,
                map => map.MapFrom(workModel =>
                    (WorkdayStatus)Enum.Parse(typeof(WorkdayStatus), workModel.Status.Replace(" ", ""))));

            CreateMap<WorkdayUpdateModel, Workday>()
                .ForMember(work => work.Status,
                map => map.MapFrom(workModel =>
                    (WorkdayStatus)Enum.Parse(typeof(WorkdayStatus), workModel.Status.Replace(" ", ""))));
        }
    }

    public static class Extensions
    {
        public static string ToSentence(this string input)
        {
            return new string(input.SelectMany((c, i) =>
                i > 0 && char.IsUpper(c) ?
                new[] { ' ', c } :
                new[] { c })
                .ToArray());
        }
    }
}

using AutoMapper;
using BIMonTime.Data.Entities;
using BIMonTime.Data.Models;
using System;
using System.Linq;

namespace BIMonTime.Data.Mappings
{
    public class WorkdayMapping : Profile
    {
        public WorkdayMapping()
        {
            CreateMap<Workday, WorkdayDetailModel>()
                .ForMember(workmodel => workmodel.Status,
                map => map.MapFrom(work => 
                    Enum.GetName(typeof(WorkdayStatus), work.Status).ToSentence()));

            CreateMap<WorkdayCreateModel, Workday>();
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

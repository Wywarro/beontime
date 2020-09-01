using AutoMapper;
using BEonTime.Data.Entities;
using BEonTime.Data.Models;

namespace BEonTime.Data.Mappings
{
    public class BeOnTimeUserMapping : Profile
    {
        public BeOnTimeUserMapping()
        {
            CreateMap<RegistrationViewModel, BEonTimeUser>().ForMember(
                au => au.UserName, map => map.MapFrom(vm => vm.Email));

            CreateMap<CredentialsViewModel, BEonTimeUser>();
        }
    }
}

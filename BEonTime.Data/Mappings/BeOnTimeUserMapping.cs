using AutoMapper;
using BIMonTime.Data.Entities;
using BIMonTime.Data.Models;

namespace BIMonTime.Data.Mappings
{
    public class BeOnTimeUserMapping : Profile
    {
        public BeOnTimeUserMapping()
        {
            CreateMap<RegistrationViewModel, BeOnTimeUser>().ForMember(
                au => au.UserName, map => map.MapFrom(vm => vm.Email));
            
            CreateMap<CredentialsViewModel, BeOnTimeUser>();
        }
    }
}

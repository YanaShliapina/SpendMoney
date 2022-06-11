using AutoMapper;
using SpendMoney.Core.DTOs;
using SpendMoney.Core.Models;
using SpendMoney.ViewModels;

namespace SpendMoney.Mapping
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<LoginViewModel, LoginDto>();
            CreateMap<RegisterViewModel, RegisterDto>();
            CreateMap<RegisterDto, ApplicationUser>()
                .ForMember(x => x.NormalizedEmail,
                    q =>
                        q.MapFrom(x => x.Email.ToUpper()))

                .ForMember(x => x.NormalizedUserName,
                    q =>
                        q.MapFrom(x => x.UserName.ToUpper()))

                //.ForMember(x => x.PhoneNumber,
                //    q =>
                //        q.MapFrom(x => x.Phone))

                .ForMember(x => x.EmailConfirmed,
                    q =>
                        q.MapFrom(x => false))

                .ForMember(x => x.PhoneNumberConfirmed,
                    q =>
                        q.MapFrom(x => false))

                .ForMember(x => x.SecurityStamp,
                    q =>
                        q.MapFrom(x => Guid.NewGuid().ToString("D")))

                .ForMember(x => x.ProfileImage,
                    q =>
                        q.MapFrom(x => ""));
        }
    }
}

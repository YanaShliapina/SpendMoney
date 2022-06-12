using AutoMapper;
using SpendMoney.Core.Constants;
using SpendMoney.Core.DTOs;
using SpendMoney.Core.Entities;
using SpendMoney.Core.Models;
using SpendMoney.ViewModels;

namespace SpendMoney.Mapping
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<Transaction, TransactionDto>();
            CreateMap<UserMoneyAccount, UserAccountDto>()
                .ForMember(dest => dest.Amount, src => src.MapFrom(x => x.TransactionAccounts.Sum(s => s.Amount)))
                .ForMember(dest => dest.Image, src => src.MapFrom(x => string.Concat(ImageConstants.ROOT_PATH, x.Account.Image.Path)))
                .ForMember(dest => dest.Image, src => src.MapFrom(x => x.Account.Currency.ShortName));
            CreateMap<Image, ImageDto>();
            CreateMap<CreateCategoryRQ, Category>();
            CreateMap<CreateCategoryViewModel, CreateCategoryRQ>();
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.Image, src => src.MapFrom(x => string.Concat(ImageConstants.ROOT_PATH, x.Image.Path)))
                .ForMember(dest => dest.Amount, src => src.MapFrom(x => x.Transactions.Sum(s => s.Amount)));
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

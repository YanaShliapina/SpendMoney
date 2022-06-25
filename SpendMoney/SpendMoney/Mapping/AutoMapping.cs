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
            CreateMap<UserDreamDto, UserDreamViewModel>();
            CreateMap<UserDream, UserDreamDto>()
               .ForMember(dest => dest.Percentage, src => src.MapFrom(x => Math.Round((x.CurrentAmount / x.TargetAmount) * 100, 2)));
            CreateMap<CreateDreamRQ, UserDream>()
                .ForMember(dest => dest.StartDate, src => src.MapFrom(x => DateTime.Now.Date));
            CreateMap<CreateDreamViewModel, CreateDreamRQ>();
            CreateMap<ChangeCategoryViewModel, UpdateCategoryRQ>();
            CreateMap<CategoryDto, ChangeCategoryViewModel>();
            CreateMap<ChangeUserAccountViewModel, UpdateUserAccountRQ>();
            CreateMap<UserAccountDto, ChangeUserAccountViewModel>();
            CreateMap<CreateTransactionRQ, Transaction>()
                .ForMember(dest => dest.TransferAccountId, src => src.MapFrom(x => x.TransferToAccountId));
            CreateMap<CreateUserAccountViewModel, CreateUserAccountRQ>();
            CreateMap<CreateUserAccountRQ, UserMoneyAccount>();
            CreateMap<CreateUserAccountRQ, MoneyAccount>();
            CreateMap<Currency, CurrencyDto>();
            CreateMap<Transaction, TransactionDto>()
                .ForMember(dest => dest.TransactionId, src => src.MapFrom(x => x.Id))
                .ForMember(dest => dest.UserAccount, src => src.MapFrom(x => x.Account))
                .ForMember(dest => dest.TransactionType, src => src.MapFrom(x => x.TypeNavigation.InternalEnumValue));
            CreateMap<UserMoneyAccount, UserAccountDto>()
                .ForMember(dest => dest.Color, src => src.MapFrom(x => x.Account.Color))
                .ForMember(dest => dest.Name, src => src.MapFrom(x => x.Account.Name))
                .ForMember(dest => dest.Description, src => src.MapFrom(x => x.Account.Description))
                .ForMember(dest => dest.Amount, src => src.MapFrom(x => x.Amount))
                .ForMember(dest => dest.Image, src => src.MapFrom(x => string.Concat(ImageConstants.ROOT_PATH, x.Account.Image.Path)))
                .ForMember(dest => dest.CurrencyShortName, src => src.MapFrom(x => x.Account.Currency.ShortName))
                .ForMember(dest => dest.ImageId, src => src.MapFrom(x => x.Account.ImageId));
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

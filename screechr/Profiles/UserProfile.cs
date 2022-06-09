using AutoMapper;
using UserProfileDto = screechr.Models.UserProfile;

namespace screechr.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserProfileDto, Models.UserProfileWithoutToken>();

        }
    }
}

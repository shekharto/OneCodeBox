using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserManagement.JWT.Entities;
using UserManagement.JWT.Models.Users;

namespace UserManagement.JWT.Helpers
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            // JwtUser -> AuthenticateResponse
            CreateMap<JwtUser, AuthenticateResponse>();

            // RegisterRequest -> JwtUser
            CreateMap<RegisterRequest, JwtUser>();

            // UpdateRequest -> JwtUser
            CreateMap<UpdateRequest, JwtUser>()
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                        return true;
                    }

                    ));

        }
    }
}

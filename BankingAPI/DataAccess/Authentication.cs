using BankingAPI.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BankingAPI.DataAccess
{
    internal static class Authentication
    {
        internal static string GenerateJSONWebToken(User user, IConfiguration configuration)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                //new Claim("fullName", user.Username.ToString()),
                //new Claim("role",user.UserRole),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("userrole",user.UserRole),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //var descriptor = new SecurityTokenDescriptor
            //{
            //    Subject = new System.Security.Claims.ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, user.Email) }),
            //    Expires = DateTime.Now.AddMinutes(30),
            //    SigningCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256),
            //};

            var handler = new JwtSecurityTokenHandler();

            //var token = handler.CreateJwtSecurityToken(descriptor);
            var token = new JwtSecurityToken(configuration["Jwt:Issuer"],
                                             configuration["Jwt:Audience"],
                                             claims: claims,
                                             expires: DateTime.Now.AddMinutes(30),
                                             signingCredentials: credentials);

            return handler.WriteToken(token);
        }

        internal static User AuthenticateUser(User user)
        {
            User authUser = null;

            var users = new List<User>
            {
               new User(){Username = "Allan", Email = "ayagaa@yahoo.com", Password="Email1234$"},
               new User(){Username = "Tony", Email = "Tn@gmail.com", Password="Email2345%"},
               new User(){Username = "Reuben", Email = "Rn@gmail.com", Password = "Email3456&"}
            };

            if(users?.Count > 0)
            {
                authUser = users.Find(p => p.Email == user.Email && p.Password == user.Password);
            }

            return authUser;
        }

        internal static bool ValidateToken(string token, IConfiguration configuration)
        {
            string email = null;
            ClaimsPrincipal principal = GetPrincipal(token, configuration);

            if (principal == null) return false;

            ClaimsIdentity claimsIdentity = null;

            try
            {
                claimsIdentity = (ClaimsIdentity)principal.Identity;
            }
            catch (NullReferenceException)
            {
                return false;
            }

            Claim claim = claimsIdentity.FindFirst(ClaimTypes.Email);
            email = claim.Value;
            return (!string.IsNullOrEmpty(email)? true : false);
        }

        private static ClaimsPrincipal GetPrincipal(string token, IConfiguration configuration)
        {
            try
            {
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                JwtSecurityToken securityToken = (JwtSecurityToken)handler.ReadToken(token);

                if (securityToken == null) return null;

                byte[] key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);

                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                SecurityToken security;
                ClaimsPrincipal principal = handler.ValidateToken(token, parameters, out security);

                return principal;
            }
            catch (Exception)
            {
                return null;
            }

        }

    }
}

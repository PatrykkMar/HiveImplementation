using HiveGame.BusinessLogic.Models.AuthModels;
using HiveGame.BusinessLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace HiveGame.BusinessLogic.Utils
{
    public interface ITokenUtils
    {
        public string CreateToken(Player player);
        public string CreateToken(string playerId);
        public string CreateAdminToken(string password);
        public Player DecodePlayerToken(string jwt);
    }

    public class Roles
    {
        public const string Player = "Player";
        public const string Admin = "Admin";
    }

    public class TokenUtils : ITokenUtils
    {
        private readonly IConfiguration _config;
        public TokenUtils(IConfiguration config)
        {
            _config = config;
        }


        /// <summary>
        /// Creates a JWT token for player
        /// </summary>
        public string CreateToken(Player player)
        {
            var key = _config.GetValue<string>("Jwt:AuthKey");
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(type: "playerId",value: player.PlayerId),
                new Claim(type: ClaimTypes.Role, Roles.Player)
            };

            var tokenOptions = new JwtSecurityToken(
                expires: DateTime.Now.AddHours(1),
                claims: claims,
                signingCredentials: signingCredentials
                );
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenOptions);
            return tokenString;
        }

        public string CreateToken(string playerId)
        {
            var player = new Player()
            {
                PlayerId = playerId
            };
            return CreateToken(player);
        }


        /// <summary>
        /// Creates a JWT token for admin
        /// </summary>
        public string CreateAdminToken(string password)
        {
            var adminPassword = _config.GetValue<string>("Jwt:AdminPassword");

            if (password != adminPassword)
            {
                throw new UnauthorizedAccessException("Invalid admin password.");
            }


            var key = _config.GetValue<string>("Jwt:AuthKey");
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(type: ClaimTypes.Role, value: "Admin")
            };

            var tokenOptions = new JwtSecurityToken(
                expires: DateTime.Now.AddHours(2),
                claims: claims,
                signingCredentials: signingCredentials
            );
            return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        }

        public Player DecodePlayerToken(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var decodedValue = handler.ReadJwtToken(jwt.Substring(7));
            var claims = decodedValue.Claims;

            if(!(claims.FirstOrDefault(x => x.Type == ClaimTypes.Role)?.Value == Roles.Player))
            {
                throw new Exception("It's not player's token");
            }

            var datas = new Player()
            {
                PlayerId = claims.FirstOrDefault(x => x.Type == "playerId")?.Value,
                PlayerNick = claims.FirstOrDefault(x => x.Type == "playerNick")?.Value,
            };
            return datas;
        }
    }
}

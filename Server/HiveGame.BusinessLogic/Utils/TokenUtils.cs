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
        public Player DecodeToken(string jwt);
    }

    public class TokenUtils : ITokenUtils
    {
        private readonly IConfiguration _config;
        public TokenUtils(IConfiguration config)
        {
            _config = config;
        }


        /// <summary>
        /// Creates a JWT token
        /// </summary>
        public string CreateToken(Player player)
        {
            var key = _config.GetValue<string>("Jwt:AuthKey");
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(type: "playerId",value: player.PlayerId),
                new Claim(type: "gameId",value: player.GameId.ToString())
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

        public Player DecodeToken(string jwt)
        {
            var handler = new JwtSecurityTokenHandler();
            var decodedValue = handler.ReadJwtToken(jwt.Substring(7));
            var claims = decodedValue.Claims;
            var datas = new Player()
            {
                PlayerId = claims.FirstOrDefault(x => x.Type == "playerId").Value,
                GameId = long.Parse(claims.FirstOrDefault(x => x.Type == "gameId").Value)
            };
            return datas;
        }
    }
}

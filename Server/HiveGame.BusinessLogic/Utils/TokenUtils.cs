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

namespace HiveGame.BusinessLogic.Utils
{
    public interface ITokenUtils
    {
        public string CreateToken(Player player);
        public string GenerateRefreshToken();
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        public TokenDatas DecodeToken(string jwt);
    }

    public class TokenUtils
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
            throw new NotImplementedException(); //TODO
        }


        /// <summary>
        /// Returns principal from expired token
        /// </summary>
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            throw new NotImplementedException(); //TODO
        }

        public TokenDatas DecodeToken(string jwt)
        {
            throw new NotImplementedException(); //TODO
        }
    }
}

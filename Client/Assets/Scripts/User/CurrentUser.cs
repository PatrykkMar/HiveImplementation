using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using UnityEngine;

public class CurrentUser
{
    private static readonly object lockObject = new object();
    private static CurrentUser _instance;
    private TokenDatas _cachedTokenData;
    private const string TokenPrefix = "Bearer ";

    public string Token { get; set; }

    private CurrentUser() { }

    public static CurrentUser Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (lockObject)
                {
                    if (_instance == null)
                    {
                        Debug.LogWarning("Invalid or empty token");
                        _instance = new CurrentUser();
                    }
                }
            }
            return _instance;
        }
    }

    private TokenDatas DecodeToken()
    {
        if (string.IsNullOrEmpty(Token) || !Token.StartsWith(TokenPrefix))
        {
            Debug.LogWarning("Invalid or empty token");
            return null;
        }

        var handler = new JwtSecurityTokenHandler();
        var decodedValue = handler.ReadJwtToken(Token.Substring(TokenPrefix.Length));
        var claims = decodedValue.Claims;

        _cachedTokenData = new TokenDatas()
        {
            PlayerId = claims.FirstOrDefault(x => x.Type == "playerId")?.Value,
            GameId = long.TryParse(claims.FirstOrDefault(x => x.Type == "gameId")?.Value, out long gameId) ? gameId : (long?)null
        };

        Debug.Log("Decoded token data:");
        Debug.Log(_cachedTokenData);

        return _cachedTokenData;
    }

    private TokenDatas GetTokenData()
    {
        if (_cachedTokenData == null || Token == null)
        {
            DecodeToken();
        }
        return _cachedTokenData;
    }

    public long? GameId
    {
        get
        {
            return GetTokenData()?.GameId;
        }
    }

    public string PlayerId
    {
        get
        {
            Debug.Log("Getting data: " + Token + "AND!!!");
            return GetTokenData()?.PlayerId;
        }
    }
}

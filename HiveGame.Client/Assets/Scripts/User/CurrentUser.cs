using System.Collections;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using UnityEngine;

public class CurrentUser
{
    private TokenDatas _cachedTokenData;
    private const string TokenPrefix = "Bearer ";

    public string Token { get; set; }

    public bool HasToken
    {
        get
        {
            return !string.IsNullOrEmpty(Token);
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
            GameId = claims.FirstOrDefault(x => x.Type == "gameId")?.Value
        };

        Debug.Log("Decoded token data:");
        Debug.Log(_cachedTokenData);

        return _cachedTokenData;
    }

    private TokenDatas TokenDatas
    {
        get
        {
            return DecodeToken();
        }
    }

    public string GameId
    {
        get
        {
            return TokenDatas?.GameId;
        }
    }

    public string PlayerId
    {
        get
        {
            return TokenDatas?.PlayerId;
        }
    }
}

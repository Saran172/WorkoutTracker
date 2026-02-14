using Newtonsoft.Json;
using System.Security.Claims;
using WorkoutTracker.API.EncryptionServices;

namespace WorkoutTracker.API.DecrptionServices
{
    public class TokenDecryption : ITokenDecryption
    {
        public List<string> DecryptedToken(HttpContext httpContext)
        {

            var claimPrincipal = httpContext.User as ClaimsPrincipal;
            var encclaim = claimPrincipal.FindFirst("encryptedClaims")?.Value;
            if (encclaim != null)

            {
                var encryptionService = httpContext.RequestServices.GetRequiredService<IEnryptService>();
                var decrptedToken = encryptionService.Decryption(encclaim);
                var decryptcliam = JsonConvert.DeserializeObject<Dictionary<string, string>>(decrptedToken);
                if (decryptcliam != null & decryptcliam.ContainsKey("Id"))
                {
                    string Id = decryptcliam["Id"];
                    string UserCode = decryptcliam["UserCode"];

                    List<string> TokenValues = new List<string>
                    {
                       Id, UserCode
                    };
                    return TokenValues;
                }
                return new List<string> { "0" };

            }
            return new List<string> { "0" };
        }
    }
}

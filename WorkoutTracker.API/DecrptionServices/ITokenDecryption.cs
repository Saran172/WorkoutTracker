namespace WorkoutTracker.API.DecrptionServices
{
    public interface ITokenDecryption
    {
        public List<string> DecryptedToken(HttpContext context);
    }
}

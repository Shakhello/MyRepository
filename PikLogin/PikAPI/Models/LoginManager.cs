using System.Linq;

namespace PikAPI.Models
{
    public static class LoginManager
    {
        private readonly static string[] correctLogins = new string[] { "pikuser", "guestuser" };

        public static bool IsCorrectLogin(string login)
        {
            return correctLogins.Contains(login);
        }
    }
}
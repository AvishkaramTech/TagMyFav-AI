using Microsoft.Bot.Schema;

namespace TagMyFav_AI.Interfaces
{
    public interface ICards
    {
        Attachment Help(IConfiguration configuration);
        Attachment Login(string signInUrl);
        Attachment Logout();
        Attachment SignedInWelcome(string name);
        Attachment Welcome();
    }
}

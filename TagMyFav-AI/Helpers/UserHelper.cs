using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Builder;

namespace TagMyFav_AI.Helpers
{
    internal class UserHelper
    {
        internal async Task<string> GetSignedInUser(ITurnContext turnContext)
        {
            var user = await TeamsInfo.GetMemberAsync(turnContext, turnContext.Activity.From.Id);
            if (user == null)
            {
                return string.Empty;
            }
            return user.UserPrincipalName;
        }
    }
}

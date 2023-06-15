using Avish.Core.Interfaces.Services;
using Avish.Core.Model;
using Avish.Infra.Service;
using Microsoft.Bot.Builder;

namespace TagMyFav_AI.Helpers
{
    public class AuthHelper
    {
        public async Task<AppUser> GetUserDetails(IAvishAppContext appContext, string user, ITurnContext turnContext, ILogger logger, string method)
        {
            //string user = "nischit@avishdev1.onmicrosoft.com";            
            AuthenitcationService authenitcation = new AuthenitcationService(appContext);

            try
            {
                var response = await authenitcation.AuthenticateUserByUPNOrEmail(user);
                return response;
            }
            catch (Exception ex)
            {
                if (ex.Message == "Sequence contains no elements")
                {
                    logger.LogError($"user: {user} is not registered : event:---{method}");
                    //await turnContext.SendActivityAsync(MessageFactory.Text("user doesnot exist, Please Create an account"));
                    //throw new MsalClientException("User exception", $"user {user} desnot exist");
                    //var botAdapter = (BotFrameworkAdapter)turnContext.Adapter;
                    //await botAdapter.SignOutUserAsync(turnContext);
                    return null;
                }
                else
                {
                    logger.LogError(ex.Message + $"event:------------------>{method}", ex);
                    throw new ArgumentException(ex.Message);
                }
            }
        }
    }
}

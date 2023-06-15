using AdaptiveCards;
using Microsoft.Bot.Schema;

namespace TagMyFav_AI.Cards
{
    public class WelcomeCard
    {
        public Attachment Welcome(IConfiguration configuration)
        {
            var helpCard = new AdaptiveCard("1.0")
            {
                Body = new List<AdaptiveElement>
                {
                   new AdaptiveTextBlock
                   {
                       Text = "Hi, Welcome to TagMyFav.",
                       Color = AdaptiveTextColor.Dark,
                       Weight = AdaptiveTextWeight.Bolder,
                       Size = AdaptiveTextSize.Large
                   },
                  new AdaptiveTextBlock
                  {
                      Separator=true,
                      Wrap=true,
                      Weight=AdaptiveTextWeight.Default,
                      Size=AdaptiveTextSize.Medium,
                      Text="TagMyFav for Microsoft Teams brings you an easy bookmarking solution to make collaboration & communication easier. With TagMyFav app, save links from Teams chat, group chat, meetings or personal chat.You can not only save links but also links to conversation & messages within Teams."
                  },
                   new AdaptiveTextBlock
                   {
                       Separator = true,
                       Text = $"Find out more about [me]({configuration["AppConstants:TmfSite"]}) and [my policies]({configuration["AppConstants:Privacy"]})"
                   },
                   new AdaptiveTextBlock
                   {
                       Separator = true,
                       Text = $"In order to use this app, you must have an active TagMyFav account.Don't have an account? [Click here]({configuration["AppConstants:Signup_URL"]}) to signup using your Office 365 email.",
                       Wrap = true,
                   },
                   new AdaptiveImage
                   {
                       Type = "Image",
                       Url = new Uri("https://app.tagmyfav.com/img/bot_welcome.png"),
                       IsVisible = true,
                   },
                   new AdaptiveFactSet
                   {
                       Facts = new List<AdaptiveFact>
                       {
                           new AdaptiveFact
                           {
                               Title = "*",
                               Value = $"[Click here]({configuration["AppConstants:Signup_URL"]}) to sign-up",
                           },
                           new AdaptiveFact
                           {
                               Title = "*",
                               Value = $"[Click here]({configuration["AppConstants:Support"]}) for help",
                           }
                       }
                   }
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveOpenUrlAction
                    {
                        Url = new Uri($"{configuration["AppConstants:Youtube_link"]}"),
                        Title = "Tour"
                    }
                }
            };
            return new Attachment
            {
                Content = helpCard,
                ContentType = AdaptiveCard.ContentType
            };
        }
    }
}

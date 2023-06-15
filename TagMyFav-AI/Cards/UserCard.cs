using AdaptiveCards;
using Microsoft.Bot.Schema;

namespace TagMyFav_AI.Cards
{
    public class UserCard
    {
        
            public Attachment UserNotFound(IConfiguration _cofiguration)
            {
                var card = new AdaptiveCard("1.1")
                {
                    Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Weight = AdaptiveTextWeight.Bolder,
                        Text = "You need an active account to add bookmark!",
                        Wrap = true,
                        Color = AdaptiveTextColor.Default
                    },
                    //new AdaptiveTextBlock
                    //{
                    //    Weight = AdaptiveTextWeight.Lighter,
                    //    Text = "It looks like you don't have an account with us",
                    //    Color = AdaptiveTextColor.Good
                    //},
                    new AdaptiveTextBlock
                    {
                        Separator = true,
                        Weight = AdaptiveTextWeight.Default,
                        Text = $"[Click here]({_cofiguration["AppConstants:Signup_URL"]}) for a quick signup.",
                        Color = AdaptiveTextColor.Default
                    },
                    new AdaptiveTextBlock
                    {
                        Weight = AdaptiveTextWeight.Default,
                        Text = $"[Click here]({_cofiguration["AppConstants:Youtube_link"]}) to take a virtual tour of this.",
                        Color = AdaptiveTextColor.Default
                    }
                }
                };
                return new Attachment()
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = card,
                    Name = "Oops"
                };
            }
        
    }
}

using AdaptiveCards;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TagMyFav_AI.Interfaces;

namespace TagMyFav_AI.Cards
{
    public class TeamsCards:ICards
    {
        public Attachment Welcome()
        {
            var card = new AdaptiveCard("1.0")
            {
                Body = new List<AdaptiveElement>
                {
                   new AdaptiveTextBlock
                   {
                       Text = "tagmyfav bot has been Successfully Installed",
                       Weight = AdaptiveTextWeight.Bolder,
                       Color = AdaptiveTextColor.Attention,
                       IsVisible = true,
                   },                   
                },
                Actions = new List<AdaptiveAction>
                {
                    new AdaptiveOpenUrlAction
                    {
                        Url = new Uri("https://app.tagmyfav.com"),
                        Title = "Visit Us",
                    }
                }
            };
            return new Attachment { Content = card, ContentType = AdaptiveCard.ContentType};
        }

        public Attachment Help(IConfiguration configuration)
        {
            var helpCard = new AdaptiveCard("1.0")
            {
                Body = new List<AdaptiveElement>
                {
                   new AdaptiveTextBlock
                   {
                       Text = "TagMyFav help and support",
                       Color = AdaptiveTextColor.Dark,
                       Weight = AdaptiveTextWeight.Bolder
                   },
                   new AdaptiveTextBlock
                   {
                       Separator = true,
                       Text = "Please refer to the following links for help and support"
                   },
                   new AdaptiveFactSet
                   {
                       Facts = new List<AdaptiveFact>
                       {                          
                           new AdaptiveFact
                           {
                               Title = "*",
                               
                               Value = $"You can use this app in personal chat, Teams chat & manage your favourites from within Teams. [Check here]( {configuration["AppConstants:Youtube_link"]}) for getting started",
                           },    
                           
                           new AdaptiveFact
                           {
                               Title = "*",
                               Value = "[Click here](https://www.tagmyfav.com/support) for help",
                           },
                       }
                   }
                }
            };
            return new Attachment
            {
                Content = helpCard,
                ContentType = AdaptiveCard.ContentType
            };
        }
        public Attachment Login(string signInUrl)
        {
            var loginCard = new AdaptiveCard("1.0")
            {
                Body = new List<AdaptiveElement>
                {
                   new AdaptiveTextBlock
                   {
                       Text = "Please SignIn",
                       Weight = AdaptiveTextWeight.Bolder,
                       Color = AdaptiveTextColor.Dark
                   }
                },
                Actions = new List<AdaptiveAction>
                {
                   new AdaptiveSubmitAction
                   {
                       Title = "SignIn",
                     Data = new
                     {
                         msteams = new
                         {
                             type = "signin",
                             value = $"{signInUrl}"
                         }

                     }
                   }
                }
            };
            return new Attachment
            {
                Content = loginCard,
                ContentType = AdaptiveCard.ContentType
            };
        }
        public Attachment Logout()
        {
            var logoutCard = new AdaptiveCard("1.0")
            {
                Body = new List<AdaptiveElement>
                {
                   new AdaptiveTextBlock
                   {
                       Text = "You have been Logged out successfully",
                       Color = AdaptiveTextColor.Attention,
                       Weight = AdaptiveTextWeight.Bolder,
                       IsVisible = true
                   }
                }
            };

            return new Attachment
            {
                Content = logoutCard,
                ContentType = AdaptiveCard.ContentType
            };

        }

        public Attachment SignedInWelcome(string name)
        {
            var welcomeCard = new AdaptiveCard("1.0")
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Text = $"Welcome {name},",
                        Weight = AdaptiveTextWeight.Bolder,
                        Color = AdaptiveTextColor.Dark
                    },

                    new AdaptiveTextBlock
                    {
                        Separator  = true,
                        Text = "You can experience the following features of TMF App",
                        Weight = AdaptiveTextWeight.Bolder
                    },

                    new AdaptiveFactSet
                    {
                       Facts = new List<AdaptiveFact>
                       {
                           new AdaptiveFact
                           {
                               Title = "*",
                               Value = "Fact 1 of TagMyFav App"
                           },

                           new AdaptiveFact
                           {
                               Title = "*",
                               Value = "Fact 2 of TagMyFav App"
                           },

                           new AdaptiveFact
                           {
                               Title = "*",
                               Value = "Fact 3 of TagMyFav App"
                           },

                       }
                    }
                }
            };
            return new Attachment
            {
                Content = welcomeCard,
                ContentType = AdaptiveCard.ContentType
            };
        }

        
    }
}

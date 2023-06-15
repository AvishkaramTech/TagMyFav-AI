using AdaptiveCards;
using Avish.Common.OpenAi;
using Microsoft.Bot.Schema;

namespace TagMyFav_AI.Cards
{
    public class BookmarkCard
    {
        public static Attachment SuggestedBookmarks(List<UrlSuggestion> urlSuggestions)
        {
            var card = new AdaptiveCard(new AdaptiveSchemaVersion(1, 3))
            {
                Body = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        Text = "TagMyFav suggested links!",
                        Weight = AdaptiveTextWeight.Bolder,
                        Size = AdaptiveTextSize.Large,
                        HorizontalAlignment = AdaptiveHorizontalAlignment.Center
                    },

                }

            };
            foreach (var urlSuggestion in urlSuggestions)
            {
                card.Body.Add(CreateListItem(urlSuggestion));
            }
            return new Attachment
            {
                Content = card,
                ContentType = AdaptiveCard.ContentType
            };
        }
        private static AdaptiveContainer CreateListItem(UrlSuggestion suggestion)
        {
            var columnSet = new AdaptiveColumnSet
            {

                Columns = new List<AdaptiveColumn>
            {
            new AdaptiveColumn
            {

                Width = "stretch",
                SelectAction = new AdaptiveOpenUrlAction { Url = new Uri(suggestion.Link) },
                Items = new List<AdaptiveElement>
                {
                    new AdaptiveTextBlock
                    {
                        HorizontalAlignment = AdaptiveHorizontalAlignment.Left,
                        Text = suggestion.Title,
                        Weight = AdaptiveTextWeight.Bolder
                    },
                    new AdaptiveTextBlock
                    {
                        HorizontalAlignment = AdaptiveHorizontalAlignment.Left,
                        Text = suggestion.Link,
                        Wrap = true,
                    }
                }
            },
            new AdaptiveColumn
            {
                Width = "auto",
                VerticalContentAlignment = AdaptiveVerticalContentAlignment.Center,
                Items = new List<AdaptiveElement>
                {
                    new AdaptiveActionSet
                    {

                        Actions = new List<AdaptiveAction>
                        {

                            new AdaptiveSubmitAction
                            {

                                Title = "Add Bookmark",
                                Data = new {Title = suggestion.Title,Path=suggestion.Link }
                            }
                        },
                        Separator = true,
                        Spacing = AdaptiveSpacing.None,

                    }
                }
            }
        }
            };

            var container = new AdaptiveContainer
            {
                Items = new List<AdaptiveElement> { columnSet },
                Separator = true
            };

            return container;
        }
    }
}

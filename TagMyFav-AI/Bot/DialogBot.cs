using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using TagMyFav_AI.Cards;

namespace TagMyFav_AI.Bot
{
    public class DialogBot<T> : TeamsActivityHandler where T : Dialog
    {
        private readonly T _dialog;
        private readonly ConversationState _conversationState;
        private readonly IConfiguration _configuration;

        public DialogBot(T dialog, ConversationState conversationState,IConfiguration configuration)
        {
            _dialog = dialog;
            _conversationState = conversationState;
            _configuration = configuration;
        }
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            if (turnContext.Activity.Conversation.ConversationType == "personal" && turnContext.Activity.Name != "composeExtension/query")
            {
                await turnContext.SendActivityAsync(new Activity { Type = ActivityTypes.Typing }, cancellationToken);

            }
            await base.OnTurnAsync(turnContext, cancellationToken);

            // Save any state changes that might have occured during the turn.
            await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            // Run the Dialog with the new message Activity.
            await _dialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>("DialogState"), cancellationToken);
        }
        protected override async Task OnTeamsMembersAddedAsync(IList<TeamsChannelAccount> membersAdded, TeamInfo teamInfo, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id == turnContext.Activity.Recipient.Id)
                {
                    WelcomeCard welcome = new WelcomeCard();
                    // Send a message to introduce the bot to the team
                    if (member.Name == null)
                    {
                        await turnContext.SendActivityAsync(MessageFactory.Attachment(welcome.Welcome(_configuration)), cancellationToken);
                    }
                }
            }

        }
    }
}

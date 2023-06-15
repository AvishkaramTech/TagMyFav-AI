using Avish.Core.Model;
using Avishkaram.Core.Service;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration.UserSecrets;
using Newtonsoft.Json;
using System.Threading;
using Avish.Core.Interfaces.Services;
using Avish.Core.Interfaces.Repo;
using Avish.Common.Interfaces;
using Avish.Core.Interfaces;
using TagMyFav_AI.Helpers;
using TagMyFav_AI.Cards;
using TagMyFav_AI.Interfaces;
using Avish.Common.OpenAi;
using Microsoft.AspNetCore.Components.Forms;

namespace TagMyFav_AI.Bot
{
    public class MainDialog:ComponentDialog
    {
        private readonly IAvishAppContext _avishAppContext;
        private readonly IUserSettingsRepository _userSettingsRepository;
        private readonly ITeamsChannelRepository _teamsChannelRepository;
        private readonly IUrlParser _bookmarkService;
        private readonly IBookmarkRepository _bookmarkRepository;
        private readonly IConfiguration _configuration;
        private readonly ICategoryHelper _categoryHelper;
        private readonly ITeamsMembersRepository _membersRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IShareRepository _shareRepository;
        private readonly IOpenAiService _openAiService;
        private readonly ICards _cards;
        private readonly ILogger<MainDialog> _logger;

        public MainDialog(
            IAvishAppContext avishAppContext,
            IUserSettingsRepository userSettingsRepository,
            ITeamsChannelRepository teamsChannelRepository,
            IUrlParser bookmarkService,
            IBookmarkRepository bookmarkRepository,
            IConfiguration configuration,
            ICategoryHelper categoryHelper,
            ITeamsMembersRepository membersRepository,
            IUserRepository userRepository,
            ICategoryRepository categoryRepository,
            IShareRepository shareRepository,
            IOpenAiService openAiService,
            ICards cards,
            ILogger<MainDialog> logger
        )
        {
            _avishAppContext = avishAppContext;
            _userSettingsRepository = userSettingsRepository;
            _teamsChannelRepository = teamsChannelRepository;
            _bookmarkService = bookmarkService;
            _bookmarkRepository = bookmarkRepository;
            _configuration = configuration;
            _categoryHelper = categoryHelper;
            _membersRepository = membersRepository;
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
            _shareRepository = shareRepository;
            _openAiService = openAiService;
            _cards = cards;
            _logger = logger;
        }
        protected override async Task<DialogTurnResult> OnBeginDialogAsync(DialogContext innerDc, object options, CancellationToken cancellationToken = default(CancellationToken))
        {
            await ProcessDialogAsync(innerDc, cancellationToken);
            return await innerDc.EndDialogAsync();
            
        }
        private async Task ProcessDialogAsync(DialogContext innerDc,CancellationToken cancellationToken)
        {
            if (innerDc.Context.Activity.Type == ActivityTypes.Message && innerDc.Context.Activity.Value != null)
            {
                var toAdd = JsonConvert.DeserializeObject<Bookmark>(innerDc.Context.Activity.Value.ToString());
                UserHelper user = new UserHelper();
                string upn = await user.GetSignedInUser(innerDc.Context);

                if (string.IsNullOrEmpty(upn))
                {
                    await innerDc.Context.SendActivityAsync("invalid user");
                    return;
                }

                AuthHelper authHelper = new AuthHelper();
                var createdUser = await authHelper.GetUserDetails(_avishAppContext, upn, innerDc.Context, _logger, "Conversation");

                if (createdUser == null)
                {
                    UserCard notFound = new UserCard();
                    await innerDc.Context.SendActivityAsync(MessageFactory.Attachment(notFound.UserNotFound(_configuration)));
                    return;
                }

                var teamsChannelData = innerDc.Context.Activity.GetChannelData<TeamsChannelData>();

                int categoryID;

                if (teamsChannelData != null && teamsChannelData.Channel == null && teamsChannelData.Team == null && innerDc.Context.Activity.Conversation.ConversationType.ToLower() == "groupchat")
                {
                    var userSettings = await _userSettingsRepository.GetById(createdUser.UserID);
                    categoryID = (userSettings == null || userSettings.DefaultBrowserSession == 0) ? await _categoryHelper.CheckForCategory(createdUser.UserID) : userSettings.DefaultBrowserSession;
                    //channel
                }
                else if (teamsChannelData != null && teamsChannelData.Team != null && teamsChannelData.Channel != null)
                {
                    //teams
                    var teamDetails = await TeamsInfo.GetTeamDetailsAsync(innerDc.Context);
                    TeamsChannel teamsChannel = await _teamsChannelRepository.GetByChannelID(teamDetails.Id);

                    if (teamsChannel == null)
                    {
                        ChannelHelper channelHelper = new ChannelHelper();
                        await channelHelper.CheckAndCreateTeams(innerDc as ITurnContext, teamDetails, _membersRepository, _teamsChannelRepository, createdUser, _userRepository, _categoryRepository, _shareRepository, cancellationToken);
                        teamsChannel = await _teamsChannelRepository.GetByChannelID(teamDetails.Id);
                    }

                    categoryID = teamsChannel.CategoryID;
                    // var channelCategory = _categoryRepository.GetTeamsCategory(currentTeamsCategory.Id)
                }
                else
                {
                    var userSettings = await _userSettingsRepository.GetById(createdUser.UserID);
                    categoryID = (userSettings == null || userSettings.DefaultBrowserSession == 0) ? await _categoryHelper.CheckForCategory(createdUser.UserID) : userSettings.DefaultBrowserSession;
                    //one on one
                }

                var info = await _bookmarkService.GetUrlInfo(toAdd, createdUser.UserID, categoryID);
                info.CategoryID = categoryID;
                info.UserID = createdUser.UserID;
                info.CreatedBy = createdUser.UserID;
                info.Type = 1;

                if (info.IsDublicate)
                {
                    await innerDc.Context.SendActivityAsync("Selected bookmark already exists");
                    return;
                }

                await _bookmarkRepository.Add(info);
                await innerDc.Context.SendActivityAsync($"[{info.Title}]({info.Path}) added successfully.");
                return;
            }
            string inputText = StringHelper.RemoveHtmlTags(innerDc.Context.Activity.Text, _configuration["AppConstants:BotName"]);
            if (!string.IsNullOrEmpty(inputText))
            {   
                switch (inputText.ToLower())
                {
                    case "hi":
                    case "hello":
                        await innerDc.Context.SendActivityAsync(MessageFactory.Text($"Hi, I am TagMyFav. Find out more about [me]({_configuration["AppConstants:TmfSite"]})"));
                        break;

                    case "help":
                    case "how":
                        await innerDc.Context.SendActivityAsync(MessageFactory.Attachment(_cards.Help(_configuration)));
                        break;

                    default:
                        UserHelper user = new UserHelper();
                        string upn = await user.GetSignedInUser(innerDc.Context);

                        //if (string.IsNullOrEmpty(upn))
                        //{
                        //    await innerDc.Context.SendActivityAsync("invalid user");
                        //    return;
                        //}

                        //AuthHelper authHelper = new AuthHelper();
                        //var currentUser = await authHelper.GetUserDetails(_avishAppContext, upn, innerDc.Context, _logger, "Conversation");

                        //if (currentUser == null)
                        //{
                        //    UserCard userCard = new UserCard();
                        //    await innerDc.Context.SendActivityAsync(MessageFactory.Attachment(userCard.UserNotFound(_configuration)));
                        //    await innerDc.EndDialogAsync();
                        //    return;
                        //}

                        string aiClientResult = await _openAiService.MakeCompleition(inputText, PromptHelper.PromptType.Intent, upn);
                        aiClientResult = aiClientResult.Trim();

                        if (aiClientResult.ToLower() == "unknown")
                        {
                            aiClientResult = string.Empty;
                            aiClientResult = await _openAiService.MakeCompleition(inputText, PromptHelper.PromptType.QuestionAnswer, upn);
                            await innerDc.Context.SendActivityAsync(aiClientResult);
                        }
                        else if (aiClientResult.ToLower().Contains("link"))
                        {
                            aiClientResult = string.Empty;
                            aiClientResult = await _openAiService.MakeCompleition(inputText, PromptHelper.PromptType.TagMyFav, upn);
                            aiClientResult = StringHelper.TakeJsonResult(aiClientResult);

                            try
                            {
                                var urls = JsonConvert.DeserializeObject<List<UrlSuggestion>>(aiClientResult);
                                await innerDc.Context.SendActivityAsync(MessageFactory.Attachment(BookmarkCard.SuggestedBookmarks(urls)));
                            }
                            catch (Exception ex)
                            {
                                await innerDc.Context.SendActivityAsync("Cannot find any link");
                            }
                        }

                        break;
                }
            }

        }
    }
}

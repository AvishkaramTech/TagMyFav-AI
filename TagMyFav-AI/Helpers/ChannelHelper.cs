using Avish.Core.Interfaces.Repo;
using Avish.Core.Interfaces;
using Avish.Core.Model;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema.Teams;

namespace TagMyFav_AI.Helpers
{
    public class ChannelHelper
    {
        public async Task<List<int>> GetAllChannelMembersForATeam(TeamDetails teamDetails, ITurnContext turnContext, CancellationToken cancellationToken, IUserRepository _user)
        {
            string continuationToken = null;
            //var teamDetails = await TeamsInfo.GetTeamDetailsAsync(turnContext);
            var currentPage = await TeamsInfo.GetPagedMembersAsync(turnContext, 100, continuationToken, cancellationToken);
            continuationToken = currentPage.ContinuationToken;
            var members = new List<int>();
            do
            {

                foreach (var teamsMember in currentPage.Members)
                {
                    var currentUser = await _user.SearchByEmail(teamsMember.Email);
                    if (currentUser != null)
                    {
                        members.Add(Convert.ToInt32(currentUser.UserID));
                    }
                }
            }
            while (continuationToken != null);
            return members;
        }

        public async Task CheckAndCreateTeams
            (ITurnContext turnContext,
            TeamDetails teamDetails,
            ITeamsMembersRepository _teamsMember,
            ITeamsChannelRepository _teamsChannelRepository,
            AppUser appUser,
            IUserRepository _user,
            ICategoryRepository _categoryRepository,
            IShareRepository _share,
            CancellationToken cancellationToken)
        {
            var channels = await TeamsInfo.GetTeamChannelsAsync(turnContext).ConfigureAwait(false);
            //teamDetails = await TeamsInfo.GetTeamDetailsAsync(turnContext, turnContext.Activity.TeamsGetTeamInfo().Id);
            Team team = new Team();
            var teamCheck = await _teamsMember.CheckTeams(teamDetails.AadGroupId);
            List<int> members = new List<int>();
            Category addedCategory = new Category();
            User dummyUser = new User();
            if (!teamCheck)
            {
                dummyUser = await _user.GetDummyAdmin(appUser.OrgID);
                ChannelHelper channelHelper = new ChannelHelper();
                members = await channelHelper.GetAllChannelMembersForATeam(teamDetails, turnContext, cancellationToken, _user);
                Category newCategory = new Category();
                newCategory.Name = teamDetails.Name;
                newCategory.CreationDate = DateTime.Now;
                newCategory.IsActive = true;
                newCategory.Description = "Teams Shared Collection";
                newCategory.IsDeleted = false;
                newCategory.Icon = "<svg  viewBox=\"0 0 48 48\"><path fill=\"#1565C0\" d=\"M25,22h13l6,6V11c0-2.2-1.8-4-4-4H25c-2.2,0-4,1.8-4,4v7C21,20.2,22.8,22,25,22z\"/><path fill=\"#2196F3\" d=\"M23,19H10l-6,6V8c0-2.2,1.8-4,4-4h15c2.2,0,4,1.8,4,4v7C27,17.2,25.2,19,23,19z\"/><path fill=\"#FFA726\" d=\"M12 26A5 5 0 1 0 12 36 5 5 0 1 0 12 26zM36 26A5 5 0 1 0 36 36 5 5 0 1 0 36 26z\"/><g><path fill=\"#607D8B\" d=\"M20 42c0 0-2.2-4-8-4s-8 4-8 4v2h16V42zM44 42c0 0-2.2-4-8-4s-8 4-8 4v2h16V42z\"/></g></svg>";
                newCategory.Share = new Share() { IsShared = true };
                newCategory.CreatedBy = dummyUser.UserID;
                addedCategory = await _categoryRepository.Add(newCategory);
                team = await _teamsMember.AddTeamDetails(teamDetails.AadGroupId, teamDetails.Name, (int)addedCategory.ID);
                var result = await _share.AddMultipleShareToTeams(members, (int)addedCategory.ID, team.ID);
            }
            else
            {
                ChannelHelper channelHelper = new ChannelHelper();
                team = await _teamsMember.GetTeamInfo(teamDetails.AadGroupId);

                var existingMembers = await _teamsMember.AllTeamsMembers(teamDetails.AadGroupId);
                if (existingMembers.Count() == 0)
                {
                    members = await channelHelper.GetAllChannelMembersForATeam(teamDetails, turnContext, cancellationToken, _user);
                    await _share.AddMultipleShareToTeams(members, (team.CategoryID), team.ID);

                }
                //if (members.Count() == 0)
                //{
                //    members = await channelHelper.GetAllChannelMembersForATeam(teamDetails, turnContext, cancellationToken, _user);
                //}


            }

            IEnumerable<TeamsChannel> addedChannels = new List<TeamsChannel>();
            addedChannels = await _teamsChannelRepository.GetChannelsByTeamsGroupID(teamDetails.AadGroupId);
            bool fromPartner = false;
            foreach (var channel in addedChannels)
            {
                if (channel.ChannelName == "General" && string.IsNullOrEmpty(channel.ChannelID))
                {
                    var generalChannel = channels.FirstOrDefault(e => e?.Name == null);
                    await _teamsChannelRepository.UpdateChannelID(generalChannel.Id, channel.ID);
                    fromPartner = true;

                }
            }
            if (addedChannels.Count() == channels.Count())
            {

            }
            else
            {
                if (fromPartner)
                {
                    addedChannels = await _teamsChannelRepository.GetChannelsByTeamsGroupID(teamDetails.AadGroupId);
                }
                bool contains = true;
                List<TeamsChannel> teamsChannels = new List<TeamsChannel>();

                foreach (var channel in channels)
                {
                    TeamsChannel teamsChannel = new TeamsChannel();
                    contains = false;
                    foreach (var addedChannel in addedChannels)
                    {

                        if (addedChannel.ChannelID == channel.Id)
                        {
                            contains = true;
                        }
                    }
                    if (!contains)
                    {
                        teamsChannel.ChannelName = channel?.Name ?? "General";  // getting the current channel info
                        teamsChannel.ChannelID = channel.Id;
                        teamsChannel.TeamsID = team.ID;
                        teamsChannels.Add(teamsChannel);
                    }
                }
                if (dummyUser.UserID == 0)
                {
                    dummyUser = await _user.GetDummyAdmin(appUser.OrgID);
                }
                if (members.Count == 0)
                {
                    ChannelHelper channelHelper = new ChannelHelper();
                    members = await channelHelper.GetAllChannelMembersForATeam(teamDetails, turnContext, cancellationToken, _user);
                }

                if (addedCategory.ID == 0)
                {
                    addedCategory = await _teamsChannelRepository.GetTeamsDefaultCategry(teamDetails.AadGroupId);
                }
                foreach (var teamsChannel in teamsChannels)
                {
                    // var appUser = await authHelper.GetUserDetails(_avishAppContext, turnContext.Activity.From.AadObjectId, turnContext);                   
                    Category category = new Category();
                    category.CreatedBy = dummyUser.UserID;
                    category.ModifiedBy = appUser.UserID;
                    category.CreationDate = DateTime.Now;
                    category.ModifiedDate = DateTime.Now;
                    category.Name = teamsChannel.ChannelName;
                    category.IsActive = true;
                    category.IsDeleted = false;
                    category.Icon = "<svg  viewBox=\"0 0 48 48\"><path fill=\"#1565C0\" d=\"M25,22h13l6,6V11c0-2.2-1.8-4-4-4H25c-2.2,0-4,1.8-4,4v7C21,20.2,22.8,22,25,22z\"/><path fill=\"#2196F3\" d=\"M23,19H10l-6,6V8c0-2.2,1.8-4,4-4h15c2.2,0,4,1.8,4,4v7C27,17.2,25.2,19,23,19z\"/><path fill=\"#FFA726\" d=\"M12 26A5 5 0 1 0 12 36 5 5 0 1 0 12 26zM36 26A5 5 0 1 0 36 36 5 5 0 1 0 36 26z\"/><g><path fill=\"#607D8B\" d=\"M20 42c0 0-2.2-4-8-4s-8 4-8 4v2h16V42zM44 42c0 0-2.2-4-8-4s-8 4-8 4v2h16V42z\"/></g></svg>";
                    category.Share = new Share { IsShared = true };
                    category.ParentID = (int)addedCategory.ID;
                    var addedCatagory = await _categoryRepository.Add(category);
                    teamsChannel.CategoryID = (int)addedCatagory.ID;
                    var addedChannelInfo = await _teamsChannelRepository.Add(teamsChannel);
                    var result = await _share.AddMultipleShareToChannel(members, (int)addedCatagory.ID, addedChannelInfo.ID, true);
                }
            }
        }
    }
}

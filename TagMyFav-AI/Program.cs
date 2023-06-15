using Avish.Common.Interfaces;
using Avish.Common.OpenAi;
using Avish.Common.Services;
using Avish.Core.Interfaces;
using Avish.Core.Interfaces.Repo;
using Avish.Core.Interfaces.Services;
using Avish.Core.Model;
using Avish.Infra.Repo;
using Avish.Infra.Service;
using Avishkaram.Infra.Repository;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Options;
using System.Configuration;
using TagMyFav_AI.Adapters;
using TagMyFav_AI.Bot;
using TagMyFav_AI.Cards;
using TagMyFav_AI.Helpers;
using TagMyFav_AI.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var configuration = new ConfigurationBuilder()
       .AddJsonFile("appsettings.json", optional: false)
       .Build();

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Create the Bot Framework Authentication to be used with the Bot Adapter.
builder.Services.AddSingleton<BotFrameworkAuthentication, ConfigurationBotFrameworkAuthentication>();
builder.Services.AddSingleton<IStorage, MemoryStorage>();
builder.Services.AddSingleton<UserState>();
builder.Services.AddSingleton<ConversationState>();
builder.Services.AddSingleton<CloudAdapter, AdapterWithErrorHandler>();
builder.Services.AddSingleton<MainDialog>();
builder.Services.AddSingleton<IBotFrameworkHttpAdapter>(sp => sp.GetService<CloudAdapter>());
builder.Services.AddSingleton<IBot,DialogBot<MainDialog>>();
//injecting required services in DIContainer
var mySettings = new Config();
new ConfigureFromConfigurationOptions<Config>(configuration.GetSection("ConnectionStrings")).Configure(mySettings);
builder.Services.AddSingleton(mySettings);
builder.Services.AddSingleton<IAvishAppContext,AvishAppContext> ();
builder.Services.AddSingleton<IUserSettingsRepository,UserSettingsRepository> ();
builder.Services.AddSingleton<ITeamsChannelRepository,TeamsChannelRepository> ();
builder.Services.AddSingleton<IUrlParser,UrlParser> ();
builder.Services.AddSingleton<IBookmarkRepository,BookmarkRepository> ();
builder.Services.AddSingleton<ICategoryHelper,CategoryHelper> ();
builder.Services.AddSingleton<ITeamsMembersRepository,TeamsMembersRepository> ();
builder.Services.AddSingleton<IUserRepository,UserRepository> ();
builder.Services.AddSingleton<ICategoryRepository,CategoryRepository> ();
builder.Services.AddSingleton<IShareRepository,ShareRepository> ();
builder.Services.AddSingleton<IOpenAiService,OpenAiService> ();
builder.Services.AddSingleton<IOpenAiHelper,OpenAiHelper> ();
builder.Services.AddSingleton<ICards,TeamsCards> ();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

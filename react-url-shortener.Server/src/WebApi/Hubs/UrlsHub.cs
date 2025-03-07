using Microsoft.AspNetCore.SignalR;
using WebApi.Hubs.Clients;

namespace WebApi.Hubs;

public class UrlsHub : Hub<IUrlsClient>;
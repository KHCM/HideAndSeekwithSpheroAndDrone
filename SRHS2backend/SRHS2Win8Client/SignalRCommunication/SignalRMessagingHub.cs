using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNet.SignalR.Client;
using System.Diagnostics;

namespace SRHS2Win8Client
{
    public class SignalRMessagingHub : ISignalRHub
    {
        #region "Members"

        IHubProxy SignalRMapHub;
        IHubProxy SignalRGameHub;
        IHubProxy SignalRChatHub;
        IHubProxy SignalRGameScoreHub;
        IHubProxy SignalRObjSyncHub;

        // Use the specific port# for local server or actual URI if SignalR backend is hosted.     

        /*HubConnection mapConnection = new HubConnection("http://kaharri.azurewebsites.net/");
        HubConnection chatConnection = new HubConnection("http://kaharri.azurewebsites.net/");
        HubConnection gameConnection = new HubConnection("http://kaharri.azurewebsites.net/");
        HubConnection objConnection = new HubConnection("http://kaharri.azurewebsites.net/");
         */
        HubConnection gameConnection = new HubConnection("http://localhost:62127/");
        HubConnection mapConnection = new HubConnection("http://localhost:62127/");
        HubConnection chatConnection = new HubConnection("http://localhost:62127/");
        //HubConnection gameConnection = new HubConnection("http://localhost:62127/");
        HubConnection objConnection = new HubConnection("http://localhost:62127/");

        public event SignalRServerHandler SignalRServerNotification;

        #endregion

        #region "Constructor"

        public SignalRMessagingHub()
        {
            // Reference to SignalR Server Hub & Proxy.           
            SignalRGameHub = gameConnection.CreateHubProxy("gameHub");
            //SignalRMapHub = mapConnection.CreateHubProxy("MapHub");
            SignalRChatHub = chatConnection.CreateHubProxy("ChatHub");
            //SignalRGameScoreHub = gameConnection.CreateHubProxy("GameScoreHub");
            //SignalRObjSyncHub = objConnection.CreateHubProxy("ObjectSyncHub");
        }

        #endregion

        #region "Implementation"

        public async virtual void UserLogin(User tabletChatClient)
        {
            // Fire up SignalR Connection & join chatroom.  
            try
            {
                await gameConnection.Start();

                if (gameConnection.State == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
                {
                    //await SignalRGameHub.Invoke("Send", "client", "Client is Working");
                    await SignalRGameHub.Invoke("Login", tabletChatClient);
                    //await SignalRChatHub.Invoke("UserLogin", tabletChatClient);
                }
            }
            catch (Exception)
            {
                
                // Do some error handling. Could not connect to Sever Error.
                Debug.WriteLine("ERROR    !!!!!!!!!!!" );
            }

            // Listen to chat events on SignalR Server & wire them up appropriately.

            // Listen to chat events on SignalR Server & wire them up appropriately.
            SignalRGameHub.On<User, ServerMessage>("addNewUpdate", (user, message) =>
            {
                SignalREventArgs userArgs = new SignalREventArgs();
                userArgs.UserUpdate = user;
                userArgs.CustomServerMessage = message;

                // Raise custom event & let it bubble up.
                SignalRServerNotification(this, userArgs);
            });
            SignalRGameHub.On<string,string>("addNewMessageToPage", (message,word) =>
            {
                SignalREventArgs chatArgs = new SignalREventArgs();
                chatArgs.ChatMessageFromServer = message;
                chatArgs.ChatMessageFromServer = word;
                // Raise custom event & let it bubble up.
                SignalRServerNotification(this, chatArgs);
            });
            SignalRGameHub.On<User, List<Game>, List<User>, ServerMessage>("update", (user, agl, ul, sm) =>
            {
                SignalREventArgs uArgs = new SignalREventArgs();
                uArgs.UserUpdate = user;
                uArgs.CustomGameList = agl;
                uArgs.CustomAvailableOpponents = ul;
                uArgs.CustomServerMessage = sm; 
                // Raise custom event & let it bubble up.
                SignalRServerNotification(this, uArgs);
            });
            SignalRGameHub.On<Game, ServerMessage>("gameCreated", (g, sm) =>
            {
                SignalREventArgs gArgs = new SignalREventArgs();
                gArgs.CustomGameObject = g; 
                gArgs.CustomServerMessage = sm;
                // Raise custom event & let it bubble up.
                SignalRServerNotification(this, gArgs);
            });
            SignalRGameHub.On<User, Game, ServerMessage>("lobbyMessage", (u, g, sm) =>
            {
                SignalREventArgs gArgs = new SignalREventArgs();
                gArgs.UserUpdate = u; 
                gArgs.CustomGameObject = g;
                gArgs.CustomServerMessage = sm;
                // Raise custom event & let it bubble up.
                SignalRServerNotification(this, gArgs);
            });
            SignalRGameHub.On<Game, InGameMessage>("inGameMessage", (g, im) =>
            {
                SignalREventArgs gArgs = new SignalREventArgs();
                gArgs.CustomGameObject = g;
                gArgs.InGameActionMessageEvent = im;
                // Raise custom event & let it bubble up.
                SignalRServerNotification(this, gArgs);
            });
        }

        public async virtual void UserUpdate(User tabletChatMessage, ServerMessage s)
        {
            // Post message to Server Chatroom.
            await SignalRGameHub.Invoke("UpdateUser", tabletChatMessage, s);
        }

        public async virtual void CreateGame(User tabletChatClient, Game game)
        {
            // Leave the Server's Chatroom.
            await SignalRGameHub.Invoke("CreateGame", tabletChatClient, game);
        }

        public async virtual void JoinGame(User x, Game g)
        {
            // Post message to Server Chatroom.
            await SignalRGameHub.Invoke("JoinGame", x, g);
        }

        public async virtual void StartGame(Game game)
        {
            // Post message to Server Chatroom.
            await SignalRGameHub.Invoke("StartGame", game);
        }

        public async virtual void InGameMessageCall(Game game, InGameMessage im)
        {
            // Post message to Server Chatroom.
            await SignalRGameHub.Invoke("InGameMessageCall", game, im);
        }

        public async virtual void Test(string x, User y)
        {
            // Post message to Server Chatroom.
            await SignalRGameHub.Invoke("Test", x, App.Current.AppUser);
        }




        public async virtual void MapIt(MapClient tabletToMap)
        {
            // Fire up SignalR Connection & share location.  
            try
            {
                await mapConnection.Start();

                if (mapConnection.State == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
                {
                    await SignalRMapHub.Invoke("ShowClientOnMap", tabletToMap);
                }
            }
            catch (Exception)
            {
                // Do some error handling.
            }
        }

        public async virtual void UnMapIt(MapClient tabletToUnMap)
        {
            // Disconnect from Server & unmap.
            await SignalRMapHub.Invoke("RemoveClientFromMap", tabletToUnMap);
        }

        public async virtual void JoinChat(ChatClient tabletChatClient)
        {
            // Fire up SignalR Connection & join chatroom.  
            try
            {
                await chatConnection.Start();

                if (chatConnection.State == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)
                {
                    await SignalRChatHub.Invoke("JoinChatroom", tabletChatClient);
                }
            }
            catch (Exception)
            {
                // Do some error handling.
            }

            // Listen to chat events on SignalR Server & wire them up appropriately.
            SignalRChatHub.On<string>("addChatMessage", message =>
            {
                SignalREventArgs chatArgs = new SignalREventArgs();
                chatArgs.ChatMessageFromServer = message;

                // Raise custom event & let it bubble up.
                SignalRServerNotification(this, chatArgs);
            });
        }

        public async virtual void Chat(string tabletChatMessage)
        {
            // Post message to Server Chatroom.
            await SignalRChatHub.Invoke("PushChatMessageToClients", tabletChatMessage);
        }

        public async virtual void LeaveChat(ChatClient tabletChatClient)
        {
            // Leave the Server's Chatroom.
            await SignalRChatHub.Invoke("LeaveChatRoom", tabletChatClient);
        }

        public async virtual void FollowLiveGame()
        {
            try
            {
                await gameConnection.Start();
            }
            catch (Exception)
            {
                // Do some error handling.
            }

            // Listen to Game score updates from SignalR Server & wire them up appropriately.
            SignalRGameScoreHub.On<int, int>("pushScores", (teamAScore, teamBScore) =>
            {
                SignalREventArgs gameScoreArgs = new SignalREventArgs();
                gameScoreArgs.TeamAScore = teamAScore;
                gameScoreArgs.TeamBScore = teamBScore;

                // Raise custom event & let it bubble up.
                SignalRServerNotification(this, gameScoreArgs);
            });
        }

        public async virtual void StartObjectSync()
        {
            // Fire up SignalR Connection & start object sync.  
            try
            {
                await objConnection.Start();
            }
            catch (Exception)
            {
                // Do some error handling.
            }

            // Listen to object updates from SignalR Server & wire them up appropriately.
            SignalRObjSyncHub.On<CustomClass>("syncObject", customObject =>
            {
                SignalREventArgs objSyncArgs = new SignalREventArgs();
                objSyncArgs.CustomObject = customObject;

                // Raise custom event & let it bubble up.
                SignalRServerNotification(this, objSyncArgs);
            });
        }

        public async virtual void DoObjectSync(CustomClass objSyncFromTablet)
        {
            // Post message to Server for object sync from phone.
            await SignalRObjSyncHub.Invoke("SyncCustomObjectWithClients", objSyncFromTablet);
        }

        #endregion

        #region "Methods"

        public virtual void OnSignalRServerNotificationReceived(SignalREventArgs e)
        {
            if (SignalRServerNotification != null)
            {
                SignalRServerNotification(this, e);
            }
        }

        #endregion
    }
}

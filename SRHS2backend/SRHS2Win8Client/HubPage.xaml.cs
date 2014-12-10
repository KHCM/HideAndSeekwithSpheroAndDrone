using SRHS2Win8Client.Common;
using SRHS2Win8Client.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Hub Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=321224

namespace SRHS2Win8Client
{
    /// <summary>
    /// A page that displays a grouped collection of items.
    /// </summary>
    public sealed partial class HubPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        private List<Game> gameList = new List<Game>();
        private bool willJoin = false;


        ObservableDictionary spheroViewModel = new ObservableDictionary();

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }



        public HubPage()
        {
            this.InitializeComponent();
            App.Current.SignalRHub.SignalRServerNotification += new SignalRServerHandler(SignalRHub_SignalRServerNotification);
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;

            //ItemGridView.ItemsSource = storeData.Collection;
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private async void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            // TODO: Create an appropriate data model for your problem domain to replace the sample data
            var sampleDataGroup = await SampleDataSource.GetGroupAsync("Group-4");
            this.DefaultViewModel["Section3Items"] = sampleDataGroup;
            foreach (Game g in App.Current.AllGames){
                if (g.GameStatus < 4)
                    gameList.Add(g);

            }
            this.DefaultViewModel["SpheroSectionItems"] = gameList;
            

        }

        /// <summary>
        /// Invoked when a HubSection header is clicked.
        /// </summary>
        /// <param name="sender">The Hub that contains the HubSection whose header was clicked.</param>
        /// <param name="e">Event data that describes how the click was initiated.</param>
        void Hub_SectionHeaderClick(object sender, HubSectionHeaderClickEventArgs e)
        {
            HubSection section = e.Section;
            var group = section.DataContext;
            this.Frame.Navigate(typeof(SectionPage), ((SampleDataGroup)group).UniqueId);
        }

        /// <summary>
        /// Invoked when an item within a section is clicked.
        /// </summary>
        /// <param name="sender">The GridView or ListView
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            var itemId = ((SampleDataItem)e.ClickedItem).UniqueId;
            this.Frame.Navigate(typeof(ItemPage), itemId);
        }
        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
            // Wire-up to listen to custom event from SignalR Hub.

            //App.Current.OppUsers = new List<User>();

            //LoadingGames();

             if (App.Current.JustLoggedIn == true)
            {
                ServerMessage ss = new ServerMessage();
                ss.Action = "login";
                ss.Command = 0; 
                



                //Send Login to server
                //App.Current.SignalRHub.UserUpdate(App.Current.AppUser,ss);
               



                //gameList.Add(new Game("TG1", tu2, tu3, 1, 100));
                //gameList.Add(new Game("TG2", tu2, tu3, 1, 100));
                //gameList.Add(new Game("TG3", tu2, tu3, 1, 100));
                //gameList.Add(new Game("TG4", tu2, tu3, 1, 100));
                //spheroViewModel = gameList; 
                
            }
            else
            {
            }

        }

        private async void LoadingGames()
        {

            // Ready and waiting for other user
            var messageDialog = new MessageDialog("Loading Games and Opponents");
            // Add commands and set their command ids
            messageDialog.Commands.Add(new UICommand("Okay", (command) =>
            {
                ServerMessage s1m = new ServerMessage(0, "", "null");
                App.Current.SignalRHub.UserUpdate(App.Current.AppUser, s1m);
            },
            0));
           

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 1;

            // Show the message dialog and get the event that was invoked via the async operator
            var commandChosen = await messageDialog.ShowAsync();
        
        }

        protected async void SignalRHub_SignalRServerNotification(object sender, SignalREventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                //Debug.WriteLine("SEND, - SIMPLE CHAT WORKS- "+e.ChatMessageFromServer);
                if (e.CustomServerMessage != null)
                {
                    switch (e.CustomServerMessage.Command)
                    {
                        //Login
                        case 0:
                            //Update Game List
                            if (e.CustomServerMessage.Action == "login")
                            {
                                App.Current.AppUser = e.UserUpdate;
                                //Upload Games
                                App.Current.AllGames = e.CustomGameList;
                                App.Current.OppUsers = e.CustomAvailableOpponents;

                                
                            }

                            break;
                        case 1:
                            Debug.WriteLine(e.CustomServerMessage.Action + "MESSAGE- " + e.CustomServerMessage.Message);
                            break;
                        case 2:
                            if (e.CustomServerMessage.Action == "created")
                            {
                                //Add game to users game list
                                App.Current.AllGames.Add(e.CustomGameObject);
                                Debug.WriteLine("Created A Game " + e.ChatMessageFromServer);
                                //Frame.Navigate(typeof(LobbyPage));
                                //Ask if they want to join
                                OpenMessageDialog();
                            }
                            break;
                        case 3:
                            if (e.CustomServerMessage.Action == "join")
                            {
                                //This means you are the first , or second to join
                                // Message: User has joined the Game
                                // Message: User has left Game Lobby
                                
                                //chatDialog.Text += "\r\n" + e.ChatMessageFromServer;
                                Debug.WriteLine("JOIN WENT TO HUBPAGE");
                            }
                            if (e.CustomServerMessage.Action == "ready")
                            {

                            }
                            break; 
                        default:
                            break;
                    }
                    
                }
                // Add to local ChatRoom.
                //chatDialog.Text += "\r\n" + e.ChatMessageFromServer;
            });
        }

        private async void OpenMessageDialog()
        {
            var messageDialog = new MessageDialog("Do you want to Join the Game now?");
            // Add commands and set their command ids
            messageDialog.Commands.Add(new UICommand("Not Now", (command) =>
            {
                App.Current.CurrentGame = null;
            },
            0));
            messageDialog.Commands.Add(new UICommand("Join Game", (command) =>
            {
                App.Current.SignalRHub.JoinGame(App.Current.AppUser, App.Current.CurrentGame);
                ChangeListener();

                Frame.Navigate(typeof(LobbyPage));
            },
            1));

            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 1;

            // Show the message dialog and get the event that was invoked via the async operator
            var commandChosen = await messageDialog.ShowAsync();
        }

        private void ChangeListener()
        {
            // Unwire.
            App.Current.SignalRHub.SignalRServerNotification -= new SignalRServerHandler(SignalRHub_SignalRServerNotification);
        }

        private void CommandInvokedHandler(IUICommand command)
        {
            if (command.Label == "Join")
            {
                //Go to Lobby of current Game
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void test_Clicked(object sender, RoutedEventArgs e)
        {
            App.Current.SignalRHub.Test("from CLIENT! test click on main page",App.Current.AppUser);
        }

        private void createButton_Click(object sender, RoutedEventArgs e)
        {
            ChangeListener();
            Frame.Navigate(typeof(GameSettingsPage));
        }
    }
}

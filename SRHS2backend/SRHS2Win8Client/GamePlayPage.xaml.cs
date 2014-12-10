using SRHS2Win8Client.Common;
using System;
using System.Collections.Generic;
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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace SRHS2Win8Client
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class GamePlayPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        private int _hits;
        private int _time;
        private bool waiting = true; 
        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public GamePlayPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
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
            App.Current.SignalRHub.SignalRServerNotification += new SignalRServerHandler(SignalRHub_SignalRServerNotification);

        }
        protected async void SignalRHub_SignalRServerNotification(object sender, SignalREventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                //...has already started
                //upDate Game Object Start time
                //start timer
                //e.InGameActionMessageEvent(state)
                App.Current.CurrentGame = e.CustomGameObject;
                StateBlock.Text = "State: " + App.Current.CurrentGame.GameState;
                    //feed back from Ready 
                if (e.CustomGameObject.GameStatus < 4) //opp not ready
                {
                    waitingBlock.Text = "Waiting for Opponent...";
                }
                else
                {
                    //Make visible in Visible
                    image.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    image2.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    waitingBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    //Call Start if you were the last one ready

                }

                if (e.InGameActionMessageEvent.Action == "end")
                {
                    //Triger End event
                    if (App.Current.CurrentGame != null)
                    {
                        App.Current.CurrentGame = new Game();
                        EndGame(e.CustomGameObject);                       
                    } 

                }
                else
                {
                    switch (e.CustomGameObject.GameState)
                    {
                        //main state of game   
                        case 0:
                            if (e.InGameActionMessageEvent.Action == "start")
                            {
                                //STOP HERE!!!!!!!!
                                //if I'm a sphero display Warning
                                if (App.Current.AppUser.UserId == e.CustomGameObject.DronePlayer.UserId)
                                {
                                    NotificationBlock.Text = "One minute before Targeting systems are active.";

                                }

                                if (App.Current.AppUser.UserId == e.CustomGameObject.SpheroPlayer.UserId)
                                {
                                    NotificationBlock.Text = "You have One minute to hide!";
                                }

                            }
                            
                            
                            if (e.InGameActionMessageEvent.Action == "hit")
                            {
                                _hits = e.InGameActionMessageEvent.Hits;
                                if (App.Current.AppUser.UserId == e.CustomGameObject.DronePlayer.UserId)
                                {
                                    NotificationBlock.Text = "Hit Successful";

                                }
                                if (App.Current.AppUser.UserId == e.CustomGameObject.SpheroPlayer.UserId)
                                {
                                    NotificationBlock.Text = "You've been hit!";
                                }

                            }
                            if (e.InGameActionMessageEvent.Action == "lost")
                            {
                                if (App.Current.AppUser.UserId == e.CustomGameObject.DronePlayer.UserId)
                                {
                                    NotificationBlock.Text = "Lost Target!";

                                }
                                if (App.Current.AppUser.UserId == e.CustomGameObject.SpheroPlayer.UserId)
                                {
                                    NotificationBlock.Text = "Evaded the enemy";
                                }
                                

                            }
                            //Actions - update GameObject - start Time, targeted, end
                            break;
                        //targeted state
                        case 1:
                            if (e.InGameActionMessageEvent.Action == "targeted")
                            {
                                //if I'm a sphero display Warning
                                if (App.Current.AppUser.UserId == e.CustomGameObject.DronePlayer.UserId)
                                {
                                    NotificationBlock.Text = "Locked on Target";

                                }
                                if (App.Current.AppUser.UserId == e.CustomGameObject.SpheroPlayer.UserId)
                                {
                                    NotificationBlock.Text = "TARGETED!";
                                }
                            }
                            //Actions - fire, end(time), end(hits), lost

                            break;
                        //End State (show Winner message)
                        case 2:
                            //Current.Game = null

                            break;
                        default:
                            break;
                    }
                }
            });
        }

        private async void EndGame(Game g)
        {
            string wm;
            if (g.Winner.UserId == App.Current.AppUser.UserId)
            {
                wm = "Game Over, you WON!";
            }
            else
            {
                wm = "Game Over, you lost.";
            }
            var messageDialog = new MessageDialog(wm);
            // Add commands and set their command ids
            messageDialog.Commands.Add(new UICommand("Ok", (command) =>
            {
                App.Current.CurrentGame = null;
                Frame.Navigate(typeof(HubPage));
            },
            0));


            // Set the command that will be invoked by default
            messageDialog.DefaultCommandIndex = 0;

            // Show the message dialog and get the event that was invoked via the async operator
            var commandChosen = await messageDialog.ShowAsync();
        }


        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void SpheroTargeted_Button_Click(object sender, RoutedEventArgs e)
        {
            InGameMessage im = new InGameMessage();
            im.Action = "target";
            App.Current.SignalRHub.InGameMessageCall(App.Current.CurrentGame, im);

        }

        private void SpheroLost_Button_Click(object sender, RoutedEventArgs e)
        {
            InGameMessage im = new InGameMessage();
            im.Action = "lost";

            App.Current.SignalRHub.InGameMessageCall(App.Current.CurrentGame, im);

        }

        private void fire_Button_Click(object sender, RoutedEventArgs e)
        {
            InGameMessage im = new InGameMessage();
            im.Action= "fire";
            im.Hits = _hits;
            App.Current.SignalRHub.InGameMessageCall(App.Current.CurrentGame, im);

        }
    }
}

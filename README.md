HideAndSeekwithSpheroAndDrone
=============================

If you need more information go to kaharri.com where a detailed blog will be of what we did. 
Also We are still updating the code.

Sphero and Drone Hide & Seek

Overview:

Create a new twist on the classic game that uses both a Drone and a Sphero. Learn how to create a windows 8.1 project to talk between the Sphero and Drone. This is by no means an easy project to implement. If you are not familiar with backend networking and communication concepts as well as connecting with hardware, I suggest reading the documentation for each of the technologies first.

Before you start: * Get a Sphero: http://www.gosphero.com/ * Download the Sphero SDK: https://developer.gosphero.com/ * Get a Drone: http://ardrone2.parrot.com/ * Download the AR Drone SDK: https://projects.ardrone.org/ and https://github.com/ARDrone2Windows * Get Visual Studio: http://msdn.microsoft.com/en-us/vstudio/aa718325.aspx

Signal R – Backend

Create a new Project in web

Then, in the New ASP.NET Project window select MVC and Change Authentication to No Authentication. Click Create Project. Now you need to get the nugget package from the Package Manager.

Tools> Nuget Package Manager> Package Manager Console

Then type:

PM> install-package Microsoft.AspNet.SignalR

If you expand your Scripts folder you will see that the libraries for SignalR have been added.

Now Right-Click the Hubs folder, and click Add | New Item, select the Visual C# | Web | SignalR node in the Installed pane, select SignalR Hub Class (v2) from the options. And Create a new hub called GameHub.cs.

This Hub will be the host of your server and game logic. It will also be what all clients send their messages to.

Under our project create a new class called Startup.cs, by right clicking the project Add > Class.

Add the following code:

using Microsoft.Owin;

using Owin;

[assembly: OwinStartupAttribute(typeof(SRHS2backend.Startup))]

namespace SRHS2backend

{

public partial class Startup

{

public void Configuration(IAppBuilder app)

{

app.MapSignalR();

}

}

}

Now go to HomeController.cs in your Controllers folder.

Add the following code snippet:

public ActionResult Game()

{

return View();

}

Now this next part is mostly for seeing which messages are getting to the server and displaying them for the developers to see. We didn’t implement it in this code but there are sources online that show you the javascript and jQuery needed to print to the new view we just created her http://www.asp.net/signalr/overview/getting-started/tutorial-getting-started-with-signalr-and-mvc

Now, your gamehub.cs class is derived from SignalR’s Hub class. This allows you to access all the currently connected clients and the methods within those clients.
To understand it better, the CreateGame(User user, Game g) method would be called in the client code, and defined in GameHub. While the Clients.All.gameGreate(g, sm) is defined in the client side of the code.

The User, Game, and ServerMessage are classes that I created for this specific game. They hold information that is required by each User, Game, and ServerMessage. While AllGamesList and AvailableGames, are Lists that I create in GameHUb so the server can reference all the active and passive games currently in progress.



SIGNALR - FRONT END

Now we will make the front end that links with Signal R.

First we want to create a new blank Universal App.

Next we install the SignalR Client NuGet package for both the phone and the windows 8.1 project.

Now the way that the client interacts with the server code that was written is by connecting with the hub and sending messages through that hub.

For the SignalR portion of the client code create a new folder call SignalRCommunication, that contains the following classes: ISignalRHubs.cs, SignalREventArgs.cs, SignalRMessagingContainers.cs, and SignalRMessagingHub.cs.

The ISignalRHub is the interface for your SignalRMessagingHub.

And the SingnalREventArgs.cs acts as the interface allowing all parts of the project to access the messaging events.

The SignalRMessagingHub is where the connection is created between the server hub and client is initiated.

Now to connect with the Server Hub we need the following code:

#region "Implementation"

public async virtual void UserLogin(User tabletChatClient)

{

// Fire up SignalR Connection & join chatroom.

try

{

await gameConnection.Start();

if (gameConnection.State == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected)

{

await SignalRGameHub.Invoke("Login", tabletChatClient);

}

}

catch (Exception ex)

{

// Do some error handling. Could not connect to Sever Error.

Debug.WriteLine("Error: "+ ex.Message);

}

I put mine inside of the first User interaction with the server, so in this case when the User logs in.

For the Phone application if using the emulator you will need to use long polling since the phone emulator uses the PC’s identification number and a lot more work has to go into formatting your computer to run it.

This is the code you will need

#if WINDOWS_PHONE_APP

connection.Start(new LongPollingTransport());

#else

connection.Start();

#endif

Now we need to write code that will listen to events on the SignalR Server and wire them up appropriately
Calling the .On method is how our proxy hub listens to any messages the server passes to the clients.

Lets make this easier to understand by going through .On and explaining what each part is doing.

SignalRGameHub.On<Game, ServerMessage>("gameCreated", (g, sm) =>

{

SignalREventArgs gArgs = new SignalREventArgs();

gArgs.CustomGameObject = g;

gArgs.CustomServerMessage = sm;

// Raise custom event & let it bubble up.

SignalRServerNotification(this, gArgs);

});

The .On method can take in as many parameters as needed depending on which server call it’s defining On<x,y,z>.

Before you saw the server gamehub call Clients.All.gameCreated(); in the client code the quotes in SignalRGameHub.On method refer to which ever method we are listening to and then dictates what the client code should do based on the delegate following the On method.

The (g, sm) are the parameters that we defined earlier <x,y> in this case <Game, ServerMessage>. They are a part of a delegate that will create the SignalREventArgs, to parse out a gameObject and a ServerMessage. Then it raises a SignalRServerNotification(this, gArgs) that will trigger an event. You still need to write that event in other parts of your code.

We now need to write the method for the SignalRServerNotification(this, gArgs)

#region "Methods"

public virtual void OnSignalRServerNotificationReceived(SignalREventArgs e)

{

if (SignalRServerNotification != null)

{

SignalRServerNotification(this, e);

}

}

#endregion

Cool now on to defining calls made by the client that will be sent to the Server.
Make sure to define all the methods that will be sending information to the server. These async virtual methods will be called by the ISignalRHub. The quoted part “UpdateUser”, “CreateGame” and “JoinGame” refer to the methods on the server side GameHub, if the names are not exactly correct the server methods won’t invoke.

SingnalRMesagingContainers refers to the objects that you want to send through JSON to the server to manipulate. Meaning you would define your object classes within this .cs file. However if you already defined your models, in a models(or other name) folder, that will work too, in fact it’s preferred.

Referencing Gamehub

In order for your Signal R Hub to be accessed by all parts/pages of your project you will need to modify the App.xaml.cs in Shared.

public static new App Current

{

get { return Application.Current as App; }

}

Then inside the OnLaunched(LaunchActivatedEventArgs e) method add

App.Current.SignalRHub = new SignalRMessagingHub();

Inportant! If you are going to be passing objects through SignalR both projects need to have the exact same code for the objects.

Now when calling/listening to the SignalR hub from different pages of your app easy now that we have everything set up.

CALLING

LISTENING

Inorder to approipately handle the triggered SignalRServerNotifaction we need to add SignalRServerNorification for that page by referencing the App.Current.SignalRHub.SignalRServerNotification

When implementing the SignalRHub_SignalRServerNotification be sure to appropriately handle the dispatcher so the page can still be responsive when events are triggered.

In my code I use the CustomSererMessage to find which state the server is on versus the game. You can implement changing and checking game state however you think best suits your game.

Publishing your Server code

Eventually you want to publish your code to an azure website so anyone who downloads the application can connect to the server. Use the steps in this tutorial to do so: http://www.asp.net/signalr/overview/deployment/using-signalr-with-azure-web-sites 

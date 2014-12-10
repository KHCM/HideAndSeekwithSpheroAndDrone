using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SRHS2Win8Client
{
    public class ServerMessage
    {
        public ServerMessage()
        {

        }
        public ServerMessage(int command, string action, string message)
        {
            this.Command = command;
            this.Action = action;
            this.Message = message;
        }
        public int Command { get; set; }
        public string Message { get; set; }
        public string Action { get; set; }
    }
    public class Game
    {
        //CLIENT SIDE

        public Game()
        {

        }
        public Game(string gid, User testSpheroP1, User testDroneP2, int status, int time)
        {
            this.SpheroPlayer = testSpheroP1;
            this.DronePlayer = testDroneP2;
            this.GameStatus = status;
            this.MaxTime = time;
            this.GameId = gid;
        }
        public string Id { get; set; }
        [JsonProperty(PropertyName = "gameid")]
        public string GameId { get; set; }

        [JsonProperty(PropertyName = "datecreated")]
        public DateTime DateCreated { get; set; }
        public DateTime StartTime { get; set; }

        [JsonProperty(PropertyName = "gamestatus")]
        public int GameStatus { get; set; }
        public int GameState { get; set; }

        [JsonProperty(PropertyName = "spheroplayer")]
        public User SpheroPlayer { get; set; }

        [JsonProperty(PropertyName = "droneplayer")]
        public User DronePlayer { get; set; }

        [JsonProperty(PropertyName = "maxtime")]
        public int MaxTime { get; set; }

        [JsonProperty(PropertyName = "maxhits")]
        public int MaxHits { get; set; }

        [JsonProperty(PropertyName = "winner")]
        public User Winner { get; set; }

    }

    public class GamesList
    {
        public List<Game> SpheroGameList {get; set;}
    }

    //Client Side
    public class InGameMessage
    {
        public InGameMessage(string jsonMessage)
        {

            // Parse string
            // Fill in properties here
            // this.ID = whaever came from json
            // this.GameID = whatever else came from json

            InGameMessage gameObjectMessage = JsonConvert.DeserializeObject<InGameMessage>(jsonMessage);
            this.CurrentTime = (gameObjectMessage.CurrentTime);
            this.UserID = (gameObjectMessage.UserID);
            this.GameID = (gameObjectMessage.GameID);
            this.Action = (gameObjectMessage.Action);
            this.GameState = (gameObjectMessage.GameState);
            this.OpponentID = (gameObjectMessage.OpponentID);
            this.MaxHits = (gameObjectMessage.MaxHits);
            this.MaxTime = (gameObjectMessage.MaxTime);
            this.Hits = (gameObjectMessage.Hits);

        }
        public InGameMessage()
        {
            
        }
        //currentTime, userID, gameID, action, data);
        public DateTime CurrentTime { get; set; }
        public string UserID { get; set; }
        public string GameID { get; set; }
        public string Action { get; set; }
        //Data
        public int GameState { get; set; }
        public string OpponentID { get; set; }
        public int MaxHits { get; set; }
        public string MaxTime { get; set; }
        public int Hits { get; set; }
    }

    public class User
    {
        public User()
        {

        }
        public User(string UID, string uName)
        {
            this.UserId = UID;
            this.UserName = uName;
        }
        //ID | RANK | User Name | Password | Total Points | Games Won as Sphero | Games Won as  Drone | Games Lost as Sphero | Games Lost as Drone | 

        public string Id { get; set; }

        [JsonProperty(PropertyName = "userid")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "username")]
        public string UserName { get; set; }

        [JsonProperty(PropertyName = "score")]
        public int Score { get; set; }

        [JsonProperty(PropertyName = "datejoined")]
        public DateTime DateJoined { get; set; }

        [JsonProperty(PropertyName = "gwas")]
        public int GWAS { get; set; }

        [JsonProperty(PropertyName = "gwad")]
        public int GWAD { get; set; }

        [JsonProperty(PropertyName = "glas")]
        public int GLAS { get; set; }

        [JsonProperty(PropertyName = "glad")]
        public int GLAD { get; set; }

    }
    public class MapClient
    {
        public string ClientId { get; set; }
        public Location ClientLocation { get; set; }

        public class Location
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
        }
    }

    public class ChatClient
    {
        public string ClientId { get; set; }
        public string ChatUserName { get; set; }
    }

    public class GameScoreClient
    {
        public int TeamAScore { get; set; }
        public int TeamBScore { get; set; }
    }

    public class CustomClass
    {
        public string Property1 { get; set; }
        public int Property2 { get; set; }
        public bool Property3 { get; set; }
    }
}

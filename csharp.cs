using System;
using System.Dat*****mmon;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sfs2X;
using Sfs2*****re;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Util;
using WebSocketSharp;

class Program
{
    public static SmartFox sfs;

    static async Task Main()
    {
        await FetchLoginDataAsync();
        await FetchGameVersionAsync();

        // Perform login
        await PerformLoginAsync("username", "password");
        
        while (true)
        {
            // Process user input or perform other tasks
            Console.WriteLine("Enter a command:");
            string userInput = Console.ReadLine();

            // Handle user input or perform other tasks

            // You can also check for incoming server messages or events
            // and handle them accordingly
            sfs.ProcessEvents();
            //sfs.GetRoomByName(userInput);
            

            // Send user input to the server
            SendUserInputToServer(userInput);

        }
    }

    // Define a method to send user input to the server
    private static void SendUserInputToServer(string userInput)
    {
       // if (!sfs.MySelf.IsPlayer)
        //{
            // If not joined in a room, join a room
            // Replace "YourRoomName" with the actual room name or ID
       // }
        // You need to implement the logic to send 'userInput' to the server using SmartFoxServer
        // For example, you might use the SmartFoxServer API to send a message to the server
        sfs.Send(new PublicMessageRequest(userInput));
        
    }

    private static void JoinRoom(string roomName)
    {
        // Create a new request to join a room
        JoinRoomRequest joinRequest = new JoinRoomRequest(roomName);

        // Send the join room request to the server
        sfs.Send(joinRequest);
    }

    static async Task FetchLoginDataAsync()
    {
        using (HttpClient client = new HttpClient())
        {
            // Set the User-Agent header to simulate Microsoft Edge on Windows 10
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3 Edge/16.16299");

            try
            {
                string loginUrl = "https://game.aq.com/game/api/login/now";
                HttpResponseMessage response = await client.GetAsync(loginUrl);

                if (response.IsSuccessStatusCode)
                {
                    string loginData = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Login Data: " + loginData);
                }
                else
                {
                    Console.WriteLine("Failed to fetch login data. Status code: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }
    }

    static async Task FetchGameVersionAsync()
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3 Edge/16.16299");

            try
            {
                string versionUrl = "https://game.aq.com/game/api/data/gameversion?.asp";
                HttpResponseMessage response = await client.GetAsync(versionUrl);

                if (response.IsSuccessStatusCode)
                {
                    string versionData = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Game Version Data: " + versionData);

                    // Parse JSON data
                    dynamic jsonData = JsonConvert.DeserializeObject(versionData);

                    // Access variables
                    string sFile = jsonData.sFile + "?ver=" + new Random().NextDouble();
                    string sTitle = jsonData.sTitle;
                    string sBG = jsonData.sBG;
                    bool isEU = jsonData.isEU == "true";

                    Console.WriteLine("sFile: " + sFile);
                    Console.WriteLine("sTitle: " + sTitle);
                    Console.WriteLine("sBG: " + sBG);
                    Console.WriteLine("isEU: " + isEU);
                    Console.WriteLine("");
                    Console.WriteLine("ADVENTURE QUEST WORLDS - Console Edition");
                    Console.WriteLine("");
                    Console.WriteLine("Plays in your terminal!");

                    // You can set other variables or call functions based on the parsed data
                    // For example:
                    // this.LoadTitle(sFile, sTitle, sBG, isEU);
                }
                else
                {
                    Console.WriteLine("Failed to fetch game version data. Status code: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }
    }

    static async Task PerformLoginAsync(string username, string password)
    {
        using (HttpClient client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Safari/537.3 Edge/16.16299");

            try
            {
                string loginUrl = "https://game.aq.com/game/api/login/now?ran=" + new Random().NextDouble();
                string postData = $"option=1&user={Uri.EscapeDataString(username)}&pass={Uri.EscapeDataString(password)}";

                HttpResponseMessage response = await client.PostAsync(loginUrl, new StringContent(postData, Encoding.UTF8, "application/x-www-form-urlencoded"));

                if (response.IsSuccessStatusCode)
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    Console.WriteLine("Login Response: " + responseData);
                    ConnectToServerAsync(responseData);
                    
                    // Parse and handle the login response as needed
                }
                else
                {
                    Console.WriteLine("Failed to perform login. Status code: " + response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }
    }

    static async Task ConnectToServerAsync(string responseData)
    {
        // Initialize SmartFoxServer client
        sfs = new SmartFox();
        sfs.ThreadSafeMode = true; // Enable thread-safe mode for event handling

        // Subscribe to SmartFoxServer events
       
        // Configure SmartFoxServer connection settings
        ConfigData config = new ConfigData();
        config.Host = "socket4.aq.com";
        config.Port = 5588; // Use the appropriate port for your server
        config.BlueBox.UseHttps = false; // Set the desired BlueBox port
        config.Debug = true;
        config.BlueBox.IsActive = false;
        sfs.Logger.LoggingLevel = Sfs2X.Logging.LogLevel.DEBUG;
        sfs.Log.EnableConsoleTrace = true;
        sfs.Debug = true;
        
        sfs.SocketClient.AddEventListener(SFSEven*****NNECTION, OnConnection);
        sfs.SocketClient.AddEventListener(SFSEven*****NNECTION_LOST, OnConnectionLost);
        sfs.SocketClient.AddEventListener(SFSEvent.LOGIN, OnLogin);
        sfs.SocketClient.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
        sfs.SocketClient.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
        sfs.AddEventListener(SFSEven*****NNECTION, OnConnection);
        sfs.AddEventListener(SFSEven*****NNECTION_LOST, OnConnectionLost);
        sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
        sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
        sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);

        try { sfs.Connect("172.65.236.72", 5588);
            //Console.WriteLine(sfs.RoomList[0].Id);
            //Console.WriteLine(sfs.HttpUploadURI.ToString());
            sfs.SocketClien*****nnect("172.65.236.72", 5588);
            sfs.SocketClient.Log.EnableConsoleTrace = true;
            sfs.SetReconnectionSeconds(8);
            sfs.AddLogListener(Sfs2X.Logging.LogLevel.DEBUG, OnExtensionResponse);
            //Console.WriteLine(sfs.SocketClient.Log.ToString()) ;
            await Task.Delay(1000);
            
        }
        catch { Console.WriteLine("Failed to Connect 1"); }
        


       
        // Connect to the server
        try
        {
            //JoinRoom("battleon");
            Console.WriteLine(sfs.Debug);
            Console.WriteLine(sfs.SocketClient);
            if (sfs.IsConnected == true) {
                Console.WriteLine("Connected to SmartFoxServer! Attempting to join room..."); JoinRoom("battleontown");
                // Extract the data from the event
                //ISFSObject responseData = (SFSObject)["params"];

                // Log the data
               // LogServerResponse(responseData);


            }
            if (responseData != null)
            {
                

            }
            else
            {
                Console.WriteLine("Failed to connect to SmartFoxServer.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception during connection: " + ex.Message);
        }

        // Event handler for successful connection
        static void OnConnection(BaseEvent evt)
        {
            Console.WriteLine("Connected to SmartFoxServer!");
            // You can perform additional actions here upon successful connection
        }

        // Event handler for connection loss
        static void OnConnectionLost(BaseEvent evt)
        {
            Console.WriteLine("Connection lost to SmartFoxServer!");
            // You can attempt to reconnect or perform other actions upon connection loss
        }

        // Event handler for successful login
        static void OnLogin(BaseEvent evt)
        {
            Console.WriteLine("Successfully logged in!");
            // You can perform additional actions here upon successful login
        }

        // Event handler for login error
        static void OnLoginError(BaseEvent evt)
        {
            Console.WriteLine("Login error: " + evt.Params["errorMessage"]);
            // You can handle login errors here
        }

        

    }

    // Event handler for extension response
    public static void OnExtensionResponse(BaseEvent evt)
    {
        // Handle responses from server extensions here
        Console.WriteLine("Extension response received");

        // Extract the data from the event
        try
        {
            //ISFSObject responseData = (SFSObject)evt.Params["params"];
           // LogServerResponse(responseData);
           var er = evt.Clone();
           Console.WriteLine(er.ToString());
        }
        catch { }

        // Log the data
        

        // Process and handle the responseData as needed
    }

    // ... (rest of your code remains unchanged)

    // Don't forget to handle disconnection and cleanup when your application exits
    static void Cleanup()
    {
        if (sfs != null && sfs.IsConnected)
        {
            sfs.Disconnect();
        }
    }
    private static StringBuilder serverLog = new StringBuilder();
    // Method to log server 
    private static void LogServerResponse(ISFSObject responseData)
    {
        if (responseData != null)
        {
            // Convert the data to a string and log it
            string responseString = responseData.GetDump();
            Console.WriteLine("Server Response: " + responseString);

            // Append the response to the log
            serverLog.AppendLine(responseString);
        }
    }

}
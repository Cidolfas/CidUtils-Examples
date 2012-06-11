using UnityEngine;
using System.Collections;

public class NetworkingChaseWorldManager : NetworkingBase {
	
	// For ease of access
	public static NetworkingChaseWorldManager Instance;
	
	// Main GUI control enum
	public enum GUIDrawMode { None, MainMenu, DedicatedServer, Connected, ConnectionError }
	protected GUIDrawMode guiMode = GUIDrawMode.MainMenu;
	protected Rect guiRect = new Rect (0, 0, 400, 350);
	protected bool connecting = false;
	
	// Main Menu mode
	protected int guiMainMode = 0;
	protected string[] guiMainCategories = new string[] { "Player Settings", "Create Server", "Join Server" };
	protected string guiMainTitle = "Multiplayer Chase!";
	
	// Dedicated Server mode
	
	
	// Connected Options mode
	
	
	// Connection Error mode
	protected NetworkConnectionError connectionError;
	protected string connectionErrorMsg;
	
	// Use this for initialization
	void Start ()
	{
		Instance = this;
		
		GetServerPrefs ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	
	void OnGUI ()
	{
		switch (guiMode) {
		case GUIDrawMode.MainMenu:
			GUILayout.Window (0, guiRect, MainMenuWindow, guiMainTitle);
			break;
			
		case GUIDrawMode.DedicatedServer:
			GUILayout.Window (0, guiRect, DedicatedServerWindow, guiMainTitle);
			break;
			
		case GUIDrawMode.Connected:
			GUILayout.Window (0, guiRect, ConnectedWindow, guiMainTitle);
			break;
			
		case GUIDrawMode.ConnectionError:
			GUILayout.Window(0, guiRect, ConnectionErrorWindow, guiMainTitle);
			break;
			
		default:
			return;
		}
	}
	
	void MainMenuWindow (int windowID)
	{
		if (connecting) GUI.enabled = false;
		
		guiMainMode = GUILayout.Toolbar (guiMainMode, guiMainCategories);
		
		GUILayout.Space (15);
		
		switch (guiMainMode) {
		case 0:
			GUILayout.Label ("Enter your player name");
			m_playerName = GUILayout.TextField (m_playerName);
			
			if (Application.isWebPlayer == false && Application.isEditor == false) {
				GUILayout.Space (5);
				
				if (GUILayout.Button ("Exit Game", GUILayout.Height (60))) {
					Application.Quit ();	
				}
			}
			break;
			
		case 1:
			GUILayout.Label ("Enter a name for your server");
			m_serverName = GUILayout.TextField (m_serverName);
			
			GUILayout.Space (5);
			
			GUILayout.Label ("Server Port");
			m_serverPort = int.Parse (GUILayout.TextField (m_serverPort.ToString ()));
			
			GUILayout.Space (5);
			
			GUILayout.Label ("Max Players");
			m_serverPlayerLimit = int.Parse (GUILayout.TextField (m_serverPlayerLimit.ToString ()));
			
			GUILayout.Space (5);
			
			GUILayout.Label ("Put in a password for your server (optional)");
			m_serverPassword = GUILayout.TextField (m_serverPassword);
			
			GUILayout.Space (5);
			
			m_serverDedicated = GUILayout.Toggle (m_serverDedicated, "Dedicated Server?");
			
			GUILayout.Space (10);
			
			if (GUILayout.Button ("Start my server", GUILayout.Height (25))) {
				connecting = true;
				
				connectionError = StartServer ();
				
				switch (connectionError) {
				case NetworkConnectionError.NoError:
					break;
					
				default:
					connectionErrorMsg = connectionError.ToString();
					guiMode = GUIDrawMode.ConnectionError;
					break;
				}
			}
			break;
			
		case 2:
			GUILayout.Label ("Type in Server IP");
			m_joinServerIP = GUILayout.TextField (m_joinServerIP);
			
			GUILayout.Space (5);
			
			GUILayout.Label ("Server Port");
			m_joinServerPort = int.Parse (GUILayout.TextField (m_joinServerPort.ToString ()));
			
			GUILayout.Space (5);
			
			GUILayout.Label("Server Password");
			m_joinServerPassword = GUILayout.TextField (m_joinServerPassword);
			
			GUILayout.Space (10);
			
			if (GUILayout.Button ("Connect", GUILayout.Height (25))) {
				connecting = true;
				
				connectionError = JoinServerByIP ();
				
				Debug.Log(connectionError);
				
				switch (connectionError) {
				case NetworkConnectionError.NoError:
					break;
					
				default:
					connectionErrorMsg = connectionError.ToString();
					guiMode = GUIDrawMode.ConnectionError;
					break;
				}
			}
			break;
			
		default:
			break;
		}
		
		if (connecting) GUI.enabled = true;
	}
	
	void DedicatedServerWindow (int windowID)
	{
		GUILayout.Label ("Dedicated Server Controls");
		
		GUILayout.Space (5);
		
		GUILayout.Label ("Players Connected: " + Network.connections.Length.ToString ());
		
		GUILayout.Space (5);
		
		if (GUILayout.Button ("Close Server", GUILayout.Height (60))) {
			CloseServer ();
			guiMode = GUIDrawMode.MainMenu;
		}
	}
	
	void ConnectedWindow (int windowID)
	{
		if (Network.isServer) {
			GUILayout.Label ("You are host.");
			
			GUILayout.Space (5);
			
			int numPlayers = Network.connections.Length + 1;
			GUILayout.Label ("Players Connected: " + numPlayers);
			
			GUILayout.Space (5);
			
			if (GUILayout.Button ("Close Server", GUILayout.Height (60))) {
				CloseServer ();
				guiMode = GUIDrawMode.MainMenu;
			}
		} else {
			//GUILayout.Label ("Connected to server at " + Network.connections [0].ipAddress + ":" + Network.connections [0].port.ToString ());
			GUILayout.Label("Blah " + Network.connections.Length);
			
			GUILayout.Space (5);
			
			if (GUILayout.Button ("Leave Server", GUILayout.Height (60))) {
				QuitServer ();
				guiMode = GUIDrawMode.MainMenu;
			}
		}
	}
	
	void ConnectionErrorWindow (int windowID)
	{
		GUILayout.Label ("Error: " + connectionErrorMsg);
		
		if (GUILayout.Button ("Back to Menu", GUILayout.Height (60))) {
			guiMode = GUIDrawMode.MainMenu;
		}
	}
	
	// Network messages
	
	void OnServerInitialized()
	{
		if (m_serverDedicated) {
			guiMode = GUIDrawMode.DedicatedServer;
		} else {
			guiMode = GUIDrawMode.Connected;
		}
		
		connecting = false;
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info)
	{
		Debug.Log("OnDisconnectedFromServer " + info);
		
		if (info == NetworkDisconnection.LostConnection) {
			connectionErrorMsg = "Lost connection with server";
			guiMode = GUIDrawMode.ConnectionError;
		} else if (info == NetworkDisconnection.Disconnected) {
			connectionErrorMsg = "Server has closed connection";
			guiMode = GUIDrawMode.ConnectionError;
		}
	}
	
	void OnConnectedToServer()
	{
		guiMode = GUIDrawMode.Connected;
		connecting = false;
	}
	
	void OnFailedToConnect(NetworkConnectionError error)
	{
		Debug.Log("OnFailedToConnect " + error);
		
		connectionError = error;
		connectionErrorMsg = error.ToString();
		guiMode = GUIDrawMode.ConnectionError;
		
		connecting = false;
	}
	
}

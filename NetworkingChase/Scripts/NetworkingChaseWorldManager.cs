using UnityEngine;
using System.Collections;

public class NetworkingChaseWorldManager : NetworkingBase {
	
	// For ease of access
	public static NetworkingChaseWorldManager Instance;
	
	// Main GUI control enum
	public enum GUIDrawMode { None, MainMenu, DedicatedServer, Connected, ConnectionError }
	protected GUIDrawMode guiMode = GUIDrawMode.MainMenu;
	protected Rect guiRect = new Rect (0, 0, 400, 350);
	
	// Main Menu mode
	protected int guiMainMode = 0;
	protected string[] guiMainCategories = new string[] { "Player Settings", "Create Server", "Join Server" };
	protected string guiMainTitle = "Multiplayer Chase!";
	
	// Dedicated Server mode
	
	
	// Connected Options mode
	
	
	// Connection Error mode
	protected NetworkConnectionError connectionError;
	
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
			
		default:
			return;
		}
	}
	
	void MainMenuWindow (int windowID)
	{
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
				connectionError = StartServer ();
				
				switch (connectionError) {
				case NetworkConnectionError.NoError:
					if (m_serverDedicated) {
						guiMode = GUIDrawMode.DedicatedServer;
					} else {
						guiMode = GUIDrawMode.Connected;
					}
					break;
					
				default:
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
			
			if (GUILayout.Button ("Connect", GUILayout.Height (25))) {
				connectionError = JoinServerByIP ();
				
				switch (connectionError) {
				case NetworkConnectionError.NoError:
					guiMode = GUIDrawMode.Connected;
					break;
					
				default:
					guiMode = GUIDrawMode.ConnectionError;
					break;
				}
			}
			break;
			
		default:
			break;
		}
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
			GUILayout.Label ("Connected to server at " + Network.connections [0].ipAddress + ":" + Network.connections [0].port.ToString ());
			
			GUILayout.Space (5);
			
			if (GUILayout.Button ("Leave Server", GUILayout.Height (60))) {
				QuitServer ();
				guiMode = GUIDrawMode.MainMenu;
			}
		}
	}
	
	void ConnectionErrorWindow (int windowID)
	{
		GUILayout.Label ("Connection Error: " + connectionError);
		
		if (GUILayout.Button ("Back to Menu", GUILayout.Height (60))) {
			guiMode = GUIDrawMode.MainMenu;
		}
	}
	
}

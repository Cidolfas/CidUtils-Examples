using UnityEngine;
using System.Collections;

public class NetworkingChaseWorldManager : NetworkingBase {
	
	// For ease of access
	public static NetworkingChaseWorldManager Instance;
	
	// Main GUI control enum
	public enum GUIDrawMode { None, MainMenu, DedicatedServer, Connected }
	protected GUIDrawMode guiMode = GUIDrawMode.MainMenu;
	protected Rect guiRect = new Rect (0, 0, 400, 280);
	
	// Main Menu mode
	protected int guiMainMode = 0;
	protected string[] guiMainCategories = new string[] { "Player Settings", "Create Server", "Join Server" };
	protected string guiMainTitle = "Multiplayer Chase!";
	
	// Dedicated Server mode
	
	
	// Connected Options mode
	
	
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
			
			GUILayout.Space (10);
			
			if (GUILayout.Button ("Start my server", GUILayout.Height (25))) {
				StartServer ();
				guiMode = GUIDrawMode.DedicatedServer;
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
				JoinServerByIP ();
				guiMode = GUIDrawMode.Connected;
			}
			break;
			
		default:
			return;
		}
	}
	
	void DedicatedServerWindow (int windowID)
	{
		if (GUILayout.Button ("Close Server", GUILayout.Height (60))) {
			CloseServer ();
			guiMode = GUIDrawMode.MainMenu;
		}
	}
	
	void ConnectedWindow (int windowID)
	{
		if (GUILayout.Button ("Leave Server", GUILayout.Height (60))) {
			QuitServer ();
			guiMode = GUIDrawMode.MainMenu;
		}
	}
	
}

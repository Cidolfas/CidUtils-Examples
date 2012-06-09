using UnityEngine;
using System.Collections;

public class NetworkingChaseWorldManager : NetworkingBase {
	
	// For ease of access
	public static NetworkingChaseWorldManager Instance;
	
	// Main GUI control enum
	public enum GUIDrawMode { None, MainMenu }
	protected GUIDrawMode guiMode = GUIDrawMode.MainMenu;
	
	// Main Menu modes
	public enum GUIMainMenuMode { Choose, Create, Join }
	protected GUIMainMenuMode guiMainMode = GUIMainMenuMode.Choose;
	protected Rect guiMainRect = new Rect(0, 0, 400, 280);
	protected string guiMainTitle = "Multiplayer Chase!";
	protected int guiMainButtonHeight = 60;
	
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
			GUILayout.Window (0, guiMainRect, MainMenuWindow, guiMainTitle);
			break;
			
		default:
			return;
		}
	}
	
	void MainMenuWindow (int windowID)
	{
		GUILayout.Space (15);
		
		switch (guiMainMode) {
		case GUIMainMenuMode.Choose:
			if (GUILayout.Button ("Setup a server", GUILayout.Height (guiMainButtonHeight))) {
				guiMainMode = GUIMainMenuMode.Create;	
			}
			
			GUILayout.Space (10);
			
			if (GUILayout.Button ("Connect to a server", GUILayout.Height (guiMainButtonHeight))) {
				guiMainMode = GUIMainMenuMode.Join;
			}
			
			GUILayout.Space (10);
			
			if (Application.isWebPlayer == false && Application.isEditor == false) {
				if (GUILayout.Button ("Exit Game", GUILayout.Height (guiMainButtonHeight))) {
					Application.Quit ();	
				}
			}
			break;
			
		case GUIMainMenuMode.Create:
			GUILayout.Label ("Enter a name for your server");
			m_serverName = GUILayout.TextField (m_serverName);
			
			GUILayout.Space (5);
			
			GUILayout.Label ("Server Port");
			m_serverPort = int.Parse (GUILayout.TextField (m_serverPort.ToString ()));
			
			GUILayout.Space (10);
			
			if (GUILayout.Button ("Start my own server", GUILayout.Height (30))) {
				StartServer ();
				guiMainMode = GUIMainMenuMode.Choose;
				guiMode = GUIDrawMode.None;
			}
			
			if (GUILayout.Button ("Go Back", GUILayout.Height (30))) {
				guiMainMode = GUIMainMenuMode.Choose;	
			}
			break;
			
		case GUIMainMenuMode.Join:
			GUILayout.Label ("Enter your player name");
			m_playerName = GUILayout.TextField (m_playerName);
			
			GUILayout.Space (5);
			
			GUILayout.Label ("Type in Server IP");
			m_joinServerIP = GUILayout.TextField (m_joinServerIP);
			
			GUILayout.Space (5);
			
			GUILayout.Label ("Server Port");
			m_joinServerPort = int.Parse (GUILayout.TextField (m_joinServerPort.ToString ()));
			
			GUILayout.Space (5);
			
			if (GUILayout.Button ("Connect", GUILayout.Height (25))) {
				JoinServerByIP ();
				guiMainMode = GUIMainMenuMode.Choose;
				guiMode = GUIDrawMode.None;
			}
			
			GUILayout.Space (5);
			
			if (GUILayout.Button ("Go Back", GUILayout.Height (25))) {
				guiMainMode = GUIMainMenuMode.Choose;
			}
			break;
			
		default:
			return;
		}
	}
	
}

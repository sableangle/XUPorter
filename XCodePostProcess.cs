using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.XCodeEditor;
using System.Xml;
#endif
using System.IO;

public static class XCodePostProcess
{
    #if UNITY_EDITOR
    [PostProcessBuild (200)]
    public static void OnPostProcessBuild (BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS) {
            Debug.LogWarning ("Target is not iPhone. XCodePostProcess will not run");
            return;
        }

        string path = Path.GetFullPath (pathToBuiltProject);

        // Create a new project object from build target
        XCProject project = new XCProject (pathToBuiltProject);
		 
        // Find and run through all projmods files to patch the project.
        // Please pay attention that ALL projmods files in your project folder will be excuted!
        string[] files = Directory.GetFiles (Application.dataPath, "*.projmods", SearchOption.AllDirectories);
        foreach (string file in files) {
            project.ApplyMod (file);
        }

        project.AddOtherLinkerFlags("-ObjC");
		project.AddOtherLinkerFlags("-all_load");

		project.SetBitcode ("NO");
        // edit plist
        EditorPlist(path);

        //edit code
        EditorCode(path);

        // Finally save the xcode project
        project.Save ();



    }

    private static void EditorPlist(string filePath)
    {
		string FacebookAppId = PlayerPrefs.GetString ("FBAppId");
		string FacebookDisplayName = PlayerPrefs.GetString ("FacebookDisplayName");
		string facebookFans = PlayerPrefs.GetString ("facebookFans");
		string facebookShare = PlayerPrefs.GetString ("facebookShare");

		string GoogleClientId = PlayerPrefs.GetString ("GoogleId");
		string Google_REVERSED_ClientId = PlayerPrefs.GetString ("Google_REVERSED_ClientId");

		string E758_game_name = PlayerPrefs.GetString ("E758_game_name");
		string E758_game_id = PlayerPrefs.GetString ("E758_game_id");
		string E758_fac_id = PlayerPrefs.GetString ("E758_fac_id");
		string E758_fac_key = PlayerPrefs.GetString ("E758_fac_key");
		string Game_Web_Url = PlayerPrefs.GetString ("Game_Web_Url");
		string com_hagame_sdk_fb_share_desc = PlayerPrefs.GetString ("com_hagame_sdk_fb_share_desc");


        XCPlist list =new XCPlist(filePath);
        string PlistAdd =  
		@"
		<key>FacebookAppID</key>
			<string>"+FacebookAppId+@"</string>
		<key>CFBundleURLTypes</key>
        <array>
            <dict>
	            <key>CFBundleURLSchemes</key>
	            <array>
					<string>fb"+ FacebookAppId +@"</string>
	            </array>
            </dict>
			<dict>
				<key>CFBundleURLSchemes</key>
				<array>
					<string>"+ PlayerSettings.iPhoneBundleIdentifier +@"</string>
				</array>
			</dict>
			<dict>
				<key>CFBundleURLSchemes</key>
				<array>
					<string>"+ Google_REVERSED_ClientId +@"</string>
				</array>
			</dict>
        </array>

		<key>LSApplicationQueriesSchemes</key>
		<array>
			<string>fbapi</string>
			<string>fb-messenger-api</string>
			<string>fbauth2</string>
			<string>fbshareextension</string>
			<string>com.hagame.Sdk</string>
			<string>com.googleusercontent.apps.1234567890-abcdefghijklmnopqrstuvwxyz</string>
			<string>com-google-gidconsent-google</string>
			<string>com-google-gidconsent-youtube</string>
			<string>com-google-gidconsent</string>
			<string>com.google.gppconsent.2.4.1</string>
			<string>com.google.gppconsent.2.4.0</string>
			<string>com.google.gppconsent.2.3.0</string>
			<string>com.google.gppconsent.2.2.0</string>
			<string>com.google.gppconsent</string>
			<string>googlechrome</string>
			<string>googlechrome-x-callback</string>
			<string>hasgplus4</string>
		</array>

		<key>com_hagame_sdk_fb_share_desc</key>
		<string>"+com_hagame_sdk_fb_share_desc+@"</string>
		<key>E758_game_name</key>
		<string>"+E758_game_name+@"</string>
		<key>E758_game_id</key>
		<string>"+E758_game_id+@"</string>
		<key>E758_fac_id</key>
		<string>"+E758_fac_id+@"</string>
		<key>E758_fac_key</key>
		<string>"+E758_fac_key+@"</string>
		<key>facebookFans</key>
		<string>"+facebookFans+@"</string>
		<key>facebookShare</key>
		<string>"+facebookShare+@"</string>
		<key>Game_Web_Url</key>
		<string>"+Game_Web_Url+@"</string>
		<key>GoogleClientID</key>
		<string>"+GoogleClientId+@"</string>
		<key>NSPhotoLibraryUsageDescription</key>
		<string>照片將記錄您的自動註冊帳號密碼</string>";
        
        list.AddKey(PlistAdd);
       
        list.Save();

    }

	private static void EditorCode(string filePath)
    {

		XClass UnityAppController = new XClass(filePath + "/Classes/UnityAppController.mm");

		//Add header
		UnityAppController.WriteBelow("#include \"PluginBase/AppDelegateListener.h\"","#include <E758SdkFramework/ThirdPartyLoginController.h>");


		UnityAppController.AddInFunction("(BOOL)application:(UIApplication*)application didFinishLaunchingWithOptions:(NSDictionary*)launchOptions","[ThirdPartyLoginController ThirdPartyLoginInit:application :launchOptions];");

		UnityAppController.ChangeFunctionReturn("(BOOL)application:(UIApplication*)application openURL:(NSURL*)url sourceApplication:(NSString*)sourceApplication annotation:(id)annotation","return [ThirdPartyLoginController ThirdPartyLoginCallback :application :url :sourceApplication :annotation ];");

    }

    #endif
}

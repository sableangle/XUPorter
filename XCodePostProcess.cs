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
	//All code Below is only for demo the function
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
		

        XCPlist list =new XCPlist(filePath);
        string PlistAdd =  
		@"
		

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
		</array>";
        
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

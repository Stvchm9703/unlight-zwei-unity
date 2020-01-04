using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
#if (UNITY_EDITOR) 
public class CCAssetBuildWindow : EditorWindow {
    string RootString = "Hello World";
    bool groupEnabled;
    Vector2 scrollPos;
    public string AssetBuildFolder;

    public string status_asset_path;

    // Add menu named "My Window" to the Window menu
    [MenuItem ("Window/Unlight Zwei/CCAssetBuild")]
    static void Init () {
        // Get existing open window or if none, make a new one:
        CCAssetBuildWindow window = (CCAssetBuildWindow) EditorWindow.GetWindow (typeof (CCAssetBuildWindow));
        window.Show ();
    }

    void OnGUI () {
        GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
        RootString = EditorGUILayout.TextField ("Text Field", RootString);

        if (GUILayout.Button ("Set Root CC asset")) {
            RootString = EditorUtility.OpenFolderPanel ("Load png Textures", "", "");
        }

        if (GUILayout.Button ("Add CC asset")) {
            AssetBuildFolder = EditorUtility.OpenFolderPanel ("Load png Textures", "", "");
        }
        List<string> files = new List<string> ();
        if (AssetBuildFolder != "") {
            files = new List<string> (Directory.GetFiles (AssetBuildFolder));
            EditorGUILayout.BeginVertical ();
            scrollPos = EditorGUILayout.BeginScrollView (scrollPos);
            foreach (var t in files) {
                GUILayout.Label (t);
            }
            EditorGUILayout.EndScrollView ();
            if (files.Count > 0) {
                if (GUILayout.Button ("build CC asset")) {
                    buildStreamAsset (files);
                }
            }
        }
        if (GUILayout.Button ("dry build CC asset")) {
            buildStreamAssetAll ();
        }
        if (GUILayout.Button ("Create Status Asset")) {
            status_asset_path = EditorUtility.OpenFolderPanel ("Load png Textures", "", "");
            buildStatusStreamAsset (status_asset_path);
        }

        // return;
    }

    void buildStreamAsset (List<string> filepath) {
        Debug.Log ("Copy asset move ");
        var header_name = new DirectoryInfo (filepath[0]).Parent.Name;
        Debug.Log (header_name);
        var resx_path = Path.Combine (Directory.GetCurrentDirectory (), "Assets", "Data", header_name);
        Debug.Log (resx_path);
        // 
        Directory.CreateDirectory (resx_path);
        foreach (var t in filepath) {
            var tw = Path.GetFileName (t);
            Debug.Log (tw);
            System.IO.File.Copy (t, Path.Combine (resx_path, tw), true);
        }
        AssetDatabase.Refresh ();
        var assetBundleBuilds = new AssetBundleBuild[1];
        assetBundleBuilds[0].assetBundleName = header_name;
        assetBundleBuilds[0].assetBundleVariant = "ab";

        List<string> t0 = new List<string> ();

        for (int k = 0; k < filepath.Count; k++) {
            var tw = Path.GetFileName (filepath[k]);
            t0.Add ("Assets/Data/" + header_name + "/" + tw);
            Debug.Log (Path.Combine (resx_path, tw));
        }
        assetBundleBuilds[0].assetNames = t0.ToArray ();

       ULZ_BuildAssetBundles(assetBundleBuilds);

    }
    void buildStreamAssetAll () {

        Debug.Log ("Copy asset move ");
        var root_path = new DirectoryInfo (RootString);
        DirectoryInfo[] diArr = root_path.GetDirectories ();
        // {}
        var assetBundleBuilds = new AssetBundleBuild[diArr.Length];

        for (int fat = 0; fat < diArr.Length; fat++) {
            var filepath = diArr[fat].GetFiles ();
            Debug.Log (diArr[fat].Name);
            var header_name = diArr[fat].Name;

            var resx_path = Path.Combine (Directory.GetCurrentDirectory (), "Assets", "Data", header_name);
            Directory.CreateDirectory (resx_path);
            foreach (var t in filepath) {
                System.IO.File.Copy (t.ToString (), Path.Combine (resx_path, t.Name), true);
            }

            // var assetBundleBuilds = new AssetBundleBuild[1];
            assetBundleBuilds[fat].assetBundleName = header_name;
            assetBundleBuilds[fat].assetBundleVariant = "ab";
            List<string> t0 = new List<string> ();

            for (int k = 0; k < filepath.Length; k++) {
                var tw = (filepath[k].Name);
                t0.Add ("Assets/Data/" + header_name + "/" + tw);
                Debug.Log (Path.Combine (resx_path, tw));
            }
            assetBundleBuilds[fat].assetNames = t0.ToArray ();
        }
        AssetDatabase.Refresh ();

        ULZ_BuildAssetBundles (assetBundleBuilds);
    }

    void buildStatusStreamAsset (string filepath) {
        Debug.Log ("Copy asset move ");
        var header_name = new DirectoryInfo (filepath).Name;
        Debug.Log (header_name);

        var assetBundleBuilds = new AssetBundleBuild[1];
        assetBundleBuilds[0].assetBundleName = header_name;
        assetBundleBuilds[0].assetBundleVariant = "ab";

        var files = new List<string> (Directory.GetFiles (filepath));
        List<string> t0 = new List<string> ();
        for (int k = 0; k < files.Count; k++) {
            if (Path.GetExtension (Path.GetExtension (files[k])) != ".meta") {
                t0.Add ("Assets/Data/" + header_name + "/" + Path.GetFileName (files[k]));
            }
        }
        // Debug.Log (t0);
        assetBundleBuilds[0].assetNames = t0.ToArray ();
        ULZ_BuildAssetBundles (assetBundleBuilds);
    }

    void ULZ_BuildAssetBundles (AssetBundleBuild[] abs) {
        BuildPipeline.BuildAssetBundles (
            Path.Combine (Application.streamingAssetsPath, "android"),
            abs,
            BuildAssetBundleOptions.None,
            BuildTarget.Android);

        BuildPipeline.BuildAssetBundles (
            Path.Combine (Application.streamingAssetsPath, "win", "x64"),
            abs,
            BuildAssetBundleOptions.None,
            BuildTarget.StandaloneWindows64);

        BuildPipeline.BuildAssetBundles (
            Path.Combine (Application.streamingAssetsPath, "win", "x86"),
            abs,
            BuildAssetBundleOptions.None,
            BuildTarget.StandaloneWindows);

        BuildPipeline.BuildAssetBundles (
            Path.Combine (Application.streamingAssetsPath, "mac"),
            abs,
            BuildAssetBundleOptions.None,
            BuildTarget.StandaloneOSX);
    }
}

#endif
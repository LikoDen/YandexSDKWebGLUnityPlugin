#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO.Compression;
using System.IO;
using UnityEditor.SceneManagement;

namespace YandexSDK
{
    public class YandexSDKBuilder : EditorWindow
    {      

        [MenuItem("Yandex SDK/Initialize SDK")]
        public static void InitializeYandexSDKGameobject()
        {
           
            GameObject go = ObjectFactory.CreateGameObject("YandexSDK", typeof(YaSDK));
           
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        }


        [MenuItem("Yandex SDK/Yandex SDK Builder")]
        public static void ShowWindow()
        {
            YandexSDKBuilder window;

            window =  GetWindow<YandexSDKBuilder>("Yandex SDK Builder");
            window.minSize = new Vector2(450,250);
            window.maxSize = new Vector2(450, 250);
        }
        private string path = null;
        private string gameTitle = null;
        private string buildPath = null;
        private string indexData = null;

        [System.Obsolete]
        private void OnGUI()
        {

            GUILayout.Space(10);

            if(GUILayout.Button("Select Path", GUILayout.Width(100)))
            {
                SelectPath();
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Directory path:", GUILayout.Width(100));
            GUILayout.Label(path); // GUILayout.TextArea(path, GUILayout.Width(300), GUILayout.Height(20));
           
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Insert game title",GUILayout.Width(100));
            gameTitle = GUILayout.TextArea(gameTitle, GUILayout.Width(200), GUILayout.Height(20));
            GUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (GUILayout.Button("Build", GUILayout.Width(100)))
            {
                if (path == null || gameTitle == null)
                {
                    Debug.LogError("Path or Title is null");
                    return;
                }
                buildPath = Path.Combine(path, gameTitle);

                Scene[] scenes = SceneManager.GetAllScenes();
                string[] scenesName = new string[scenes.Length];

                for (int i = 0; i < scenes.Length; i++)
                {
                    scenesName[i] = scenes[i].path;
                }

           

                BuildPipeline.BuildPlayer(scenesName, buildPath, BuildTarget.WebGL, BuildOptions.None);
                IntegrationServices(Path.Combine(buildPath + "/index.html"));
                ZipFile.CreateFromDirectory(buildPath,  buildPath +".zip");
            }
        }
        private void SelectPath()
        {
           path = EditorUtility.OpenFolderPanel("Select build folder", "", "");
          
        }
        private void IntegrationServices(string buildPath)
        {
            
            indexData = null;

            #region Read
           
            try
            {
                using (StreamReader sr = new StreamReader(buildPath))
                {
                    indexData = sr.ReadToEnd();
                }
            }
            catch (System.Exception e)
            {

                Debug.LogError(e.Message);
            }
            #endregion
            indexData = IntegrationText(indexData, "<head>", Template.tempaltes[3]);
            indexData = IntegrationText(indexData, "<head>", Template.tempaltes[2]);
            
            indexData = IntegrationText(indexData, "<head>", Template.tempaltes[1]);
            indexData = IntegrationText(indexData, "<head>", Template.tempaltes[0]);
            
            indexData = IntegrationText(indexData, "then((unityInstance) => {", Template.tempaltes[4]);
            


            #region Write

            try
            {
                using (StreamWriter sw = new StreamWriter(buildPath))
                {
                    sw.Write(indexData);
                }
            }
            catch (System.Exception e)
            {

                Debug.LogError(e);
            }
            #endregion

            System.Diagnostics.Process.Start("explorer.exe", "/select," + buildPath.Replace(@"/", @"\"));
            
        }

        private string IntegrationText(string data, string tag, string integrationText)
        {
            return data = data.Insert(data.LastIndexOf(tag) + tag.Length, integrationText +"\n");
        }

    }
    public class Template
    {
        public static string[] tempaltes = new string[] { "<script src='https://yandex.ru/games/sdk/v2'></script>",
            "<script> YaGames.init().then(ysdk => {sdk = ysdk;}); </script>",
          "<script>function showFullScreenAdv() { sdk.adv.showFullscreenAdv({callbacks: {onClose: function(wasShown) {window.unityInstance.SendMessage('YandexSDK','OnInterstitialShown');}, onError: function(error) {window.unityInstance.SendMessage('YandexSDK','OnInterstitialError', error); }}}); }</script>",
          "<script>function showRewardedAdv(placement) {  sdk.adv.showRewardedVideo({callbacks: {onOpen: () => {window.unityInstance.SendMessage('YandexSDK','OnRewardedOpen',placement);}, onRewarded: () => {window.unityInstance.SendMessage('YandexSDK', 'OnRewarded', placement);}, onClose: () => {window.unityInstance.SendMessage('YandexSDK', 'OnRewardedClose', placement);}, onError: (e) => {window.unityInstance.SendMessage('YandexSDK', 'OnRewardedError', placement);}} });} </script>",
        "window.unityInstance = unityInstance;"
        };
    }

   
}
#endif

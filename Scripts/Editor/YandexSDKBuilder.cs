#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.IO.Compression;
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

            window = GetWindow<YandexSDKBuilder>("Yandex SDK Builder");
            window.minSize = new Vector2(450, 250);
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

            if (GUILayout.Button("Select Path", GUILayout.Width(100)))
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
            GUILayout.Label("Insert game title", GUILayout.Width(100));
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

                int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCountInBuildSettings;
                string[] scenes = new string[sceneCount];
                for (int i = 0; i < sceneCount; i++)
                {
                    scenes[i] = UnityEngine.SceneManagement.SceneUtility.GetScenePathByBuildIndex(i);
                }



                BuildPipeline.BuildPlayer(scenes, buildPath, BuildTarget.WebGL, BuildOptions.None);
                ZipFile.CreateFromDirectory(buildPath, buildPath + ".zip");
            }
        }
        private void SelectPath()
        {
            path = EditorUtility.OpenFolderPanel("Select build folder", "", "");

        }
    }
}
#endif

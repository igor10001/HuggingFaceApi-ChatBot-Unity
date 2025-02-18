using System.IO;
using UnityEditor;
using UnityEngine;

namespace HuggingFace.API.Editor {
    [InitializeOnLoad]
    public class HuggingFaceAPIConfiguration : EditorWindow {
        private string statMessage = string.Empty;

        private static APIConfig config;
       

      

        static HuggingFaceAPIConfiguration() {
            EditorApplication.update += CheckConfig;
        }

        private static void CheckConfig() {
            EditorApplication.update -= CheckConfig;
            LoadOrCreateConfig();
            if (string.IsNullOrEmpty(config.apiKey)) {
                ShowWindow();
            }
        }

        private static void LoadOrCreateConfig() {
            string resourcesPath = "Assets/Resources";
            if (!AssetDatabase.IsValidFolder(resourcesPath)) {
                AssetDatabase.CreateFolder("Assets", "Resources");
            }
            string configPath = $"{resourcesPath}/HuggingFaceAPIConfig.asset";
            config = AssetDatabase.LoadAssetAtPath<APIConfig>(configPath);
            if (config == null) {
                config = ScriptableObject.CreateInstance<APIConfig>();
                AssetDatabase.CreateAsset(config, configPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        [MenuItem("Window/Hugging Face API Configuration")]
        public static void ShowWindow() {
            GetWindow<HuggingFaceAPIConfiguration>("Hugging Face API Configuration");
        }

        private void OnGUI() {

            EditorGUILayout.LabelField("Hugging Face API Setup", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            string apiKey = EditorGUILayout.TextField("API Key", config.apiKey);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(config, "Change API Key");
                config.SetAPIKey(apiKey);
                EditorUtility.SetDirty(config);
            }

            if (GUILayout.Button("Test API Key")) {
                statMessage = "<color=white>Waiting for API response...</color>";
                Repaint();
                HuggingFaceAPI.TestAPIKey(apiKey, OnSuccess, OnError);
            }

            EditorGUILayout.LabelField("Status:", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(statMessage, new GUIStyle());

            GUILayout.Space(10);
            EditorGUILayout.LabelField("Task URL", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            for (int i = 0; i < config.taskEndpoints.Count; i++) {
                TaskURL url = config.taskEndpoints[i];
                EditorGUI.BeginChangeCheck();
                string newEndpoint = EditorGUILayout.TextField(url.taskName, url.endpoint);
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(config, "Change Task Endpoint");
                    config.taskEndpoints[i] = new TaskURL(url.taskName, newEndpoint);
                    EditorUtility.SetDirty(config);
                }
            }

            EditorGUI.indentLevel--;

          

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Settings", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            var content = new GUIContent("Use Backup URL", "If the primary endpoint fails, the API will try to use the backup endpoints.");
            bool useBackupEndpoints = EditorGUILayout.Toggle(content, config.useBackupEndpoints);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(config, "Change Use Backup Endpoints");
                config.SetUseBackupEndpoints(useBackupEndpoints);
                EditorUtility.SetDirty(config);
            }

            EditorGUI.BeginChangeCheck();
            content = new GUIContent("Wait for Model", "If true, the API will wait for the model to load if necessary. If false, the API will send the request immediately and return an error if the model is not loaded.");
            bool waitForModel = EditorGUILayout.Toggle(content, config.waitForModel);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(config, "Change Wait for Model");
                config.SetWaitForModel(waitForModel);
                EditorUtility.SetDirty(config);
            }

            EditorGUI.BeginChangeCheck();
            content = new GUIContent("Max Timeout", "The maximum time to wait for a response from the API. If the API does not respond within this time, the request will be cancelled. Set to 0 to disable timeout.");
            float maxTimeout = EditorGUILayout.FloatField(content, config.maxTimeout);
            if (EditorGUI.EndChangeCheck()) {
                Undo.RecordObject(config, "Change Max Timeout");
                config.SetMaxTimeout(maxTimeout);
                EditorUtility.SetDirty(config);
            }

            GUILayout.Space(10);


           

            EditorGUI.EndDisabledGroup();
           
        }

       

        

        private void OnSuccess(string response) {
            statMessage = "<color=#5cb85c>API key is valid!</color>";
        }

        private void OnError(string error) {
            statMessage = $"<color=#d9534f>{error}</color>";
        }
    }
}
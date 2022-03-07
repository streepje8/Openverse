using Openverse.Core;
using Openverse.ScriptableObjects;
using RiptideNetworking;
using RiptideNetworking.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Openverse.NetCode { 
    public class Metaserver : Singleton<Metaserver>
    {
        public OpenverseSettings settings;
        public Server server;
        private GameObject playerPrefab;

        public void OnValidate()
        {
            #if UNITY_EDITOR
                if(settings == null) //Try to find the existing settings
                    settings = (OpenverseSettings)AssetDatabase.LoadAssetAtPath("Assets/Openverse/OpenvereSettings.asset", typeof(OpenverseSettings));
                if(settings == null) //Create a new settings object if none is found
                {
                    settings = ScriptableObject.CreateInstance<OpenverseSettings>();
                    AssetDatabase.CreateAsset(settings, "Assets/Openverse/OpenverseSettings.asset");
                    AssetDatabase.SaveAssets();
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = settings;
                }
            #endif
        }

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            playerPrefab = settings.playerPrefab;
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 90; //Make the server run fast
            #if UNITY_EDITOR
                        RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
            #else
                Console.Title = "Openverse Server";
                Console.Clear();
                Application.SetStackTraceLogType(UnityEngine.LogType.Log, StackTraceLogType.None);
                RiptideLogger.Initialize(Debug.Log, true);
            #endif

            server = new Server();
            server.ClientConnected += NewPlayerConnected;
            server.ClientDisconnected += PlayerLeft;
            server.Start(settings.ServerPort, settings.playerLimit);
        }

        private void FixedUpdate()
        {
            server.Tick();
        }

        private void OnApplicationQuit()
        {
            server.Stop();

            server.ClientConnected -= NewPlayerConnected;
            server.ClientDisconnected -= PlayerLeft;
        }

        private void NewPlayerConnected(object sender, ServerClientConnectedEventArgs e)
        {
            foreach (OpenversePlayer player in OpenversePlayer.List.Values)
            {
                if (player.Id != e.Client.Id)
                    player.SendSpawn(e.Client.Id);
            }
        }

        private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
        {
            if(OpenversePlayer.List.ContainsKey(e.Id))
                Destroy(OpenversePlayer.List[e.Id].gameObject);
        }
    }
}
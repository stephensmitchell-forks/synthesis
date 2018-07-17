﻿using Synthesis.States;
using Synthesis.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Synthesis.Network
{
    public class MultiplayerNetwork : NetworkManager
    {
        // TODO: Add event listeners for connection.
        
        public enum ConnectionStatus
        {
            Connected,
            Disconnected,
            Failed
        }
        
        public MultiplayerState State { get; set; }

        public int ConnectionID { get; private set; }

        public bool Host { get; private set; }

        public event EventHandler<ConnectionStatus> ConnectionStatusChanged;

        //bool awaitingFieldLoad;

        private void Start()
        {
            Host = false;
            //client = new NetworkClient();
            //awaitingFieldLoad = false;
        }

        public override void OnStartHost()
        {
            base.OnStartHost();
            Host = true;
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            ConnectionID = conn.connectionId;
            ConnectionStatusChanged?.Invoke(this, ConnectionStatus.Connected);
            //ClientScene.Ready(conn);
            //ClientScene.AddPlayer(0);
        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            if (conn.lastError != NetworkError.Ok)
                ConnectionStatusChanged?.Invoke(this, ConnectionStatus.Failed);
        }

        public override void OnStartClient(NetworkClient client)
        {
            //if (host)
            //{
            //    if (!State.LoadField(PlayerPrefs.GetString("simSelectedField"), host))
            //    {
            //        AppModel.ErrorToMenu("Could not load field: " + PlayerPrefs.GetString("simSelectedField") + "\nHas it been moved or deleted?)");
            //        return;
            //    }
            //}
            //else
            //{
            //    awaitingFieldLoad = true;
            //}
        }

        private void Update()
        {
            //if (awaitingFieldLoad && Resources.FindObjectsOfTypeAll<NetworkElement>().Length > 1)
            //{
            //    awaitingFieldLoad = false;

            //    if (!State.LoadField(PlayerPrefs.GetString("simSelectedField"), host))
            //    {
            //        AppModel.ErrorToMenu("Could not load field: " + PlayerPrefs.GetString("simSelectedField") + "\nHas it been moved or deleted?)");
            //        return;
            //    }
            //}
        }
    }
}
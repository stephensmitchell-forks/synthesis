﻿using Synthesis.FSM;
using Synthesis.GUI;
using Synthesis.Input;
using Synthesis.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

namespace Assets.Scripts.GUI
{
    /// <summary>
    /// This state controls the emulation toolbar and interface to starting emulation robot code.
    /// </summary>
    public class EmulationToolbarState : State
    {
        EmulationDriverStation emulationDriverStation;

        GameObject canvas;
        GameObject tabs;
        GameObject emulationToolbar;

        GameObject helpMenu;
        GameObject overlay;
        Text helpBodyText;

        public override void Start()
        {
            emulationDriverStation = StateMachine.SceneGlobal.GetComponent<EmulationDriverStation>();

            canvas = GameObject.Find("Canvas");
            tabs = Auxiliary.FindObject(canvas, "Tabs");
            emulationToolbar = Auxiliary.FindObject(canvas, "EmulationToolbar");

            helpMenu = Auxiliary.FindObject(canvas, "Help");
            overlay = Auxiliary.FindObject(canvas, "Overlay");
            helpBodyText = Auxiliary.FindObject(canvas, "BodyText").GetComponent<Text>();

            Button helpButton = Auxiliary.FindObject(helpMenu, "CloseHelpButton").GetComponent<Button>();
            helpButton.onClick.RemoveAllListeners();
            helpButton.onClick.AddListener(CloseHelpMenu);
        }

        /// <summary>
        /// Selects robot code and starts VM. 
        /// </summary>
        public void OnSelectRobotCodeButtonClicked()
        {

            string[] selectedFiles = SFB.StandaloneFileBrowser.OpenFilePanel("Robot Code", "C:\\", "", false);
            if (selectedFiles.Length != 1)
            {
                UnityEngine.Debug.Log("No files selected for robot code upload");
            }
            else
            {
                SSHClient.UserProgram userProgram = new SSHClient.UserProgram(selectedFiles[0]);
                if (userProgram.type == SSHClient.UserProgram.UserProgramType.JAVA) // TODO remove this once support is added
                {
                    emulationDriverStation.ShowJavaNotSupportedPopUp();
                }
                else
                {
                    SSHClient.SCPFileSender(userProgram);
                }
            }
            { }
        }

        /// <summary>
        /// Opens the Synthesis Driver Station for emulation
        /// </summary>
        public void OnDriverStationButtonClicked()
        {
            emulationDriverStation.OpenDriverStation();
        }

        public void OnStartRobotCodeButtonClicked()
        {
            emulationDriverStation.ToggleRobotCodeButton();
            //Serialization.RestartThreads("10.140.148.66");
        }

        #region Help Button and Menu
        public void OnHelpButtonClicked()
        {
            helpMenu.SetActive(true);

            // Used to change the text of emulation help menu
            helpBodyText.GetComponent<Text>().text = "\n\nSelect Code: Select the user program file to upload. Uploading may take a couple seconds." +
                "\n\nDriver Station: Access an FRC driver station-like tool to manipulate robot running state." +
                "\n\nStart Code / Stop Code: Run or kill user program in VM. It may take a second to start the user program." +
                "\n\nVM Connection status: Shows SSH connection status to VM. Running user program is disabled until connection is established.";

            Auxiliary.FindObject(helpMenu, "Type").GetComponent<Text>().text = "EmulationToolbar";
            overlay.SetActive(true);
            tabs.transform.Translate(new Vector3(300, 0, 0));
            foreach (Transform t in emulationToolbar.transform)
            {
                if (t.gameObject.name != "HelpButton") t.Translate(new Vector3(300, 0, 0));
                else t.gameObject.SetActive(false);
            }

            if (PlayerPrefs.GetInt("analytics") == 1)
            {
                Analytics.CustomEvent("Emulation Help Button Pressed", new Dictionary<string, object> //for analytics tracking
                {
                });
            }

        }

        internal static Serialization s;

        private void CloseHelpMenu()
        {
            helpMenu.SetActive(false);
            overlay.SetActive(false);
            tabs.transform.Translate(new Vector3(-300, 0, 0));
            foreach (Transform t in emulationToolbar.transform)
            {
                if (t.gameObject.name != "HelpButton") t.Translate(new Vector3(-300, 0, 0));
                else t.gameObject.SetActive(true);
            }
        }
        #endregion
    }
}
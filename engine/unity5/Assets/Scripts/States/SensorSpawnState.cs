﻿using BulletUnity;
using Synthesis.Configuration;
using Synthesis.DriverPractice;
using Synthesis.FEA;
using Synthesis.Field;
using Synthesis.FSM;
using Synthesis.GUI;
using Synthesis.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Synthesis.States
{
    public class SensorSpawnState : State
    {
        private GameObject sensor;
        private DynamicCamera.CameraState lastCameraState;
        private GameObject moveArrows;

        #region help ui variables
        GameObject ui;
        GameObject helpMenu;
        GameObject toolbar;
        GameObject overlay;
        #endregion

        public SensorSpawnState(GameObject sensor)
        {
            this.sensor = sensor;
        }
        // Use this for initialization
        public override void Start()
        {
            #region init
            ui = GameObject.Find("ResetSensorSpawnpointUI");
            helpMenu = Auxiliary.FindObject(ui, "Help");
            toolbar = Auxiliary.FindObject(ui, "ResetStateToolbar");
            overlay = Auxiliary.FindObject(ui, "Overlay");
            #endregion

            moveArrows = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs\\MoveArrows"));
            moveArrows.name = "IndicatorMoveArrows";
            moveArrows.transform.parent = sensor.transform;
            moveArrows.transform.rotation = sensor.transform.rotation;
            moveArrows.transform.localPosition = UnityEngine.Vector3.zero;

            moveArrows.GetComponent<MoveArrows>().Translate = (translation) =>
                sensor.transform.Translate(translation, Space.World);

            StateMachine.SceneGlobal.Link<SensorSpawnState>(moveArrows);

            DynamicCamera dynamicCamera = UnityEngine.Camera.main.transform.GetComponent<DynamicCamera>();
            lastCameraState = dynamicCamera.ActiveState;

            dynamicCamera.SwitchCameraState(new DynamicCamera.ConfigurationState(dynamicCamera, sensor));

            Button resetButton = GameObject.Find("ResetButton").GetComponent<Button>();
            resetButton.onClick.RemoveAllListeners();
            resetButton.onClick.AddListener(ResetSpawn);
            Button helpButton = GameObject.Find("HelpButton").GetComponent<Button>();
            helpButton.onClick.RemoveAllListeners();
            helpButton.onClick.AddListener(HelpMenu);
            Button returnButton = GameObject.Find("ReturnButton").GetComponent<Button>();
            returnButton.onClick.RemoveAllListeners();
            returnButton.onClick.AddListener(ReturnToMainState);
            Button closeHelp = Auxiliary.FindObject(helpMenu, "CloseHelpButton").GetComponent<Button>();
            closeHelp.onClick.RemoveAllListeners();
            closeHelp.onClick.AddListener(CloseHelpMenu);
        }

        // Update is called once per frame
        public override void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Return))
            {
                ReturnToMainState();
            }
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                ReturnToMainState();
            }
        }
        private void ReturnToMainState()
        {
            if (helpMenu.activeSelf) CloseHelpMenu();
            DynamicCamera dynamicCamera = UnityEngine.Camera.main.transform.GetComponent<DynamicCamera>();
            dynamicCamera.SwitchCameraState(lastCameraState);
            GameObject.Destroy(moveArrows);
            StateMachine.PopState();
        }
        private void ResetSpawn()
        {
            sensor.transform.localPosition = sensor.name.Contains("Camera") ? new Vector3(0f, 0.5f, 0f) : new Vector3(0f, 0.2f, 0f);
        }
        private void HelpMenu()
        {
            helpMenu.SetActive(true);
            overlay.SetActive(true);
            toolbar.transform.Translate(new Vector3(100, 0, 0));
            foreach (Transform t in toolbar.transform)
            {
                if (t.gameObject.name != "HelpButton") t.Translate(new Vector3(100, 0, 0));
                else t.gameObject.SetActive(false);
            }
        }
        private void CloseHelpMenu()
        {
            helpMenu.SetActive(false);
            overlay.SetActive(false);
            toolbar.transform.Translate(new Vector3(-100, 0, 0));
            foreach (Transform t in toolbar.transform)
            {
                if (t.gameObject.name != "HelpButton") t.Translate(new Vector3(-100, 0, 0));
                else t.gameObject.SetActive(true);
            }
        }
    }
}

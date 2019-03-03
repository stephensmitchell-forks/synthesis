using Synthesis.Input;
using Synthesis.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Synthesis.GUI
{
    class RobotIOGUI : MonoBehaviour
    {
        public static RobotIOGUI Instance { get; private set; }

        GameObject canvas;
        GameObject robotIOPanel;
        GameObject robotOutputPanel;
        GridLayoutGroup robotOutputGrid;

        // Sprites for emulation coloring details
        public Sprite HighlightColor;
        public Sprite DefaultColor;
        public Sprite EnableColor;
        public Sprite DisableColor;

        private class RobotOutputs
        {
            public List<GameObject> pwmHdrs;

            public RobotOutputs()
            {
                pwmHdrs = new List<GameObject>();
            }
        }

        private RobotOutputs robotOutputs;

        private void Start()
        {
            canvas = GameObject.Find("Canvas");
            robotIOPanel = Auxiliary.FindObject(canvas, "RobotIOGUI");
            robotOutputPanel = Auxiliary.FindObject(robotIOPanel, "RobotOutputPanel");
            robotOutputGrid = robotOutputPanel.GetComponent<GridLayoutGroup>();

            robotOutputs = new RobotOutputs();

            var outputInstance = OutputManager.Instance;

            GameObject textObjectPrefab = Auxiliary.FindObject(robotOutputPanel, "TextPrefab");
            textObjectPrefab.SetActive(false);

            for (int i = 0; i < outputInstance.Roborio.PwmHdrs.Length; i++)
            {
                robotOutputs.pwmHdrs.Add(GameObject.Instantiate(textObjectPrefab, robotOutputPanel.transform));
                robotOutputs.pwmHdrs[i].SetActive(true);
            }

            // TODO

            Update();
        }

        public void UpdateOutputs()
        {
            var outputInstance = OutputManager.Instance;

            for (int i = 0; i < outputInstance.Roborio.PwmHdrs.Length; i++)
            {
                robotOutputs.pwmHdrs[i].GetComponent<Text>().text = "PWM HDR " + i.ToString() + ": " + outputInstance.Roborio.PwmHdrs[i].ToString();
            }

            // TODO
        }

        public void UpdateInputs()
        {
            var inputInstance = InputManager.Instance;

            // TODO
        }

        public void Update()
        {
            UpdateOutputs();
        }

        private void Awake()
        {
            Instance = this;
        }

        /// <summary>
        /// Opens the emulated roborio IO view
        /// </summary>
        public void ToggleOpen()
        {
            if (robotIOPanel.activeSelf == true)
            {
                robotIOPanel.SetActive(false);
                InputControl.freeze = false;
            }
            else
            {
                robotIOPanel.SetActive(true);
                InputControl.freeze = true;
            }
        }
    }
}

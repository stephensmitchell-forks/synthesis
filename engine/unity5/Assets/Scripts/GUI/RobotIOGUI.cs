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
            public GameObject pwmHdrHeader;
            public List<GameObject> pwmHdrs;
            public GameObject canMotorControllerHeader;
            public List<GameObject> canMotorControllers;

            public RobotOutputs()
            {
                pwmHdrs = new List<GameObject>();
                canMotorControllers = new List<GameObject>();
            }
        }

        private RobotOutputs robotOutputs;

        private void Start() // TODO reinstance when new user program is uploaded?
        {
            canvas = GameObject.Find("Canvas");
            robotIOPanel = Auxiliary.FindObject(canvas, "RobotIOGUI");
            robotOutputPanel = Auxiliary.FindObject(robotIOPanel, "RobotOutputPanel");
            robotOutputGrid = robotOutputPanel.GetComponent<GridLayoutGroup>();

            robotOutputs = new RobotOutputs();

            var outputInstance = OutputManager.Instance;

            GameObject textObjectPrefab = Auxiliary.FindObject(robotOutputPanel, "TextPrefab");
            textObjectPrefab.SetActive(false);

            robotOutputs.pwmHdrHeader = GameObject.Instantiate(textObjectPrefab, robotOutputPanel.transform);
            robotOutputs.pwmHdrHeader.SetActive(true);
            robotOutputs.pwmHdrHeader.GetComponent<Text>().text = "PWM Headers";

            for (int i = 0; i < outputInstance.Roborio.PwmHdrs.Length; i++)
            {
                robotOutputs.pwmHdrs.Add(GameObject.Instantiate(textObjectPrefab, robotOutputPanel.transform));
                robotOutputs.pwmHdrs[i].SetActive(true);
            }

            robotOutputs.canMotorControllerHeader = GameObject.Instantiate(textObjectPrefab, robotOutputPanel.transform);
            robotOutputs.canMotorControllerHeader.GetComponent<Text>().text = "Active CAN Motor Controllers";

            for (int i = 0; i < outputInstance.Roborio.CANDevices.Length; i++)
            {    
                robotOutputs.canMotorControllers.Add(GameObject.Instantiate(textObjectPrefab, robotOutputPanel.transform));
                if (outputInstance.Roborio.CANDevices[i].id != -1) // Check if in use
                {
                    robotOutputs.canMotorControllers[i].SetActive(true); 
                    robotOutputs.canMotorControllerHeader.SetActive(true); // Only show header if any are active
                }
            }

            // TODO

            Update();
        }

        public void UpdateOutputs()
        {
            var outputInstance = OutputManager.Instance;

            for (int i = 0; i < outputInstance.Roborio.PwmHdrs.Length; i++)
            {
                robotOutputs.pwmHdrs[i].GetComponent<Text>().text = i.ToString() + ": " + outputInstance.Roborio.PwmHdrs[i].ToString();
            }
            for (int i = 0; i < outputInstance.Roborio.CANDevices.Length; i++)
            {
                if (outputInstance.Roborio.CANDevices[i].id != -1) // Check if in use
                {
                    robotOutputs.canMotorControllers[i].SetActive(true);
                    robotOutputs.canMotorControllerHeader.SetActive(true); // Only show header if any are active
                    
                    robotOutputs.canMotorControllers[i].GetComponent<Text>().text = outputInstance.Roborio.CANDevices[i].id.ToString() + ": " + outputInstance.Roborio.CANDevices[i].speed.ToString() + "(Inverted: " + outputInstance.Roborio.CANDevices[i].inverted.ToString() + ")";
                } else
                {
                    robotOutputs.canMotorControllers[i].SetActive(false);
                    robotOutputs.canMotorControllerHeader.SetActive(false);
                }
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

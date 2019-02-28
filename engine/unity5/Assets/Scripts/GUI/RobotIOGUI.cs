using Synthesis.Input;
using Synthesis.Utils;
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

        private void Start()
        {
            canvas = GameObject.Find("Canvas");
            robotIOPanel = Auxiliary.FindObject(canvas, "RobotIOGUI");
            robotOutputPanel = Auxiliary.FindObject(robotIOPanel, "RobotOutputPanel");
            robotOutputGrid = robotOutputPanel.GetComponent<GridLayoutGroup>();

            var outputInstance = OutputManager.Instance;

            GameObject prefab = Auxiliary.FindObject(robotIOPanel, "TextPrefab");
            prefab.SetActive(false);

            for (int i = 0; i < outputInstance.Roborio.PwmHdrs.Length; i++)
            {
                GameObject textObject = Instantiate(prefab);
                Text text = textObject.GetComponent<Text>();
                text.text = "PWM HDR " + i.ToString() + ": " + outputInstance.Roborio.PwmHdrs[i].ToString();
                textObject.SetActive(true);
                textObject.transform.SetParent(robotOutputPanel.transform);
            }
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

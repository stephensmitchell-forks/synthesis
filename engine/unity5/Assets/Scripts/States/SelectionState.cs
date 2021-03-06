﻿using Synthesis.FSM;
using UnityEngine.SceneManagement;

namespace Synthesis.States
{
    public class SelectionState : State
    {
        /// <summary>
        /// Opens the default simulator tab when the main simulator button is pressed.
        /// </summary>
        public void OnMainSimulatorButtonClicked()
        {
            StateMachine.PushState(new DefaultSimulatorState());
        }

        /// <summary>
        /// Opens the mix and match tab when the mix and match button is pressed.
        /// </summary>
        public void OnMixAndMatchButtonClicked()
        {
            StateMachine.PushState(new MixAndMatchState());
        }

        /// <summary>
        /// Launches the multiplayer scene when the network multiplayer button is pressed.
        /// </summary>
        public void OnMultiplayerButtonClicked()
        {
            SceneManager.LoadScene("MultiplayerScene");
        }

        /// <summary>
        /// Returns back to the <see cref="HomeTabState"/>.
        /// </summary>
        public void OnBackButtonClicked()
        {
            StateMachine.ChangeState(new HomeTabState());
        }
    }
}

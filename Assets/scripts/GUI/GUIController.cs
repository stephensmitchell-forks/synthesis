﻿using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;

/// <summary>
/// The GUI sidebar access to overlay windows, etc.
/// Useful functions are AddAction and AddWindow.
/// </summary>
class GUIController
{
    #region Style
    /// <summary>
    /// The sidebar fade time, seconds.
    /// </summary>
    private const float GUI_SHOW_TIME = 0.5f;
    /// <summary>
    /// The padding for the sidebar content, pixels.
    /// </summary>
    private static readonly Vector2 GUI_SIDEBAR_PADDING = new Vector2(10, 25);
    /// <summary>
    /// The height of a sidebar entry.
    /// </summary>
    private const float GUI_SIDEBAR_ENTRY_HEIGHT = 45f;
    /// <summary>
    /// The space between sidebar entries.
    /// </summary>
    private const float GUI_SIDEBAR_ENTRY_PADDING_Y = 5;
    
    // Objects to allow rendering of GUI boxes with black backgrounds.
    #region make it black
    private Texture2D _black;
    private GUIStyle _blackBox;
    public Texture2D Black
    {
        get
        {
            if (_black == null)
            {
                _black = new Texture2D(1, 1);
                _black.SetPixel(0, 0, new Color(0, 0, 0));
                _black.Apply();
            }
            return _black;
        }
    }
    public GUIStyle BlackBoxStyle
    {
        get
        {
            if (_blackBox == null)
            {
                _blackBox = new GUIStyle(GUI.skin.box);
                _blackBox.normal.background = Black;
            }
            return _blackBox;
        }
    }
    #endregion
    #endregion

    /// <summary>
    /// All the entries on this sidebar.
    /// </summary>
    private KeyValuePair<string, Action>[] entries;
    /// <summary>
    /// All the overlay windows that are linked to this sidebar.
    /// </summary>
    private List<OverlayWindow> windows = new List<OverlayWindow>();

    /// <summary>
    /// Current intensity of the sidebar, [0-1].
    /// </summary>
    private float guiFadeIntensity = 0;
    /// <summary>
    /// Is the sidebar visible.
    /// </summary>
    public bool guiVisible = false;

    /// <summary>
    /// Escape key state last time OnGUI was called.
    /// </summary>
    private bool keyDebounce = false;
    /// <summary>
    /// Does the sidebar width need recalculating.
    /// </summary>
    private volatile bool recalcWidth = false;
    /// <summary>
    /// The current sidebar width, pixels.  This is dynamically calculated.
    /// </summary>
    private float sidebarWidth = 100f;

    /// <summary>
    /// Creates a GUI sidebar with an exit button.
    /// </summary>
    public GUIController()
    {
		AddWindow ("Exit", new DialogWindow ("Exit?", new string[] {"Yes", "No"}),
			(object o) =>
		{
			if ((int) o == 1) {
				Application.Quit();
			}
		});

		recalcWidth = true;
    }

    /// <summary>
    /// Adds an overlay window to the sidebar.
    /// </summary>
    /// <param name="caption">The title of the sidebar entry</param>
    /// <param name="window">The window to control</param>
    /// <param name="onReturn">Optional callback on window close</param>
    public void AddWindow(string caption, OverlayWindow window, Action<object> onReturn)
    {
		onReturn = null;
        windows.Add(window);
        AddAction(caption, () =>
        {
            bool state = window.Active;
            foreach (OverlayWindow win in windows)
            {
                win.Active = false;
            }
            window.Active = !state;
        });
        if (onReturn != null)
            window.OnComplete += onReturn;
    }

    /// <summary>
    /// Adds an entry to the sidebar.
    /// </summary>
    /// <param name="caption">The title of the entry</param>
    /// <param name="act">The action to execute when the entry is pressed</param>
    public void AddAction(string caption, Action act)
    {
        if (entries == null || entries.Length == 0)
        {
            entries = new KeyValuePair<string, Action>[1] { new KeyValuePair<string, Action>(caption, act) };
            return;
        }
        var res = new KeyValuePair<string, Action>[entries.Length + 1];
        if (entries.Length > 1)
            Array.Copy(entries, res, entries.Length - 1);
        res[res.Length - 2] = new KeyValuePair<string, Action>(caption, act);
        res[res.Length - 1] = entries[entries.Length - 1];
        entries = res;
        recalcWidth = true;
    }

    /// <summary>
    /// Executes the action with the given title in the sidebar
    /// </summary>
    /// <param name="caption">The sidebar title</param>
    public void DoAction(string caption)
    {
        foreach (var v in entries)
        {
            if (v.Key.Equals(caption))
            {
                v.Value();
                break;
            }
        }
    }

    /// <summary>
    /// Renders the overlay.
    /// </summary>
    public void Render()
    {
        bool windowVisible = false;
        #region windowVisible
        foreach (OverlayWindow window in windows)
        {
            if (window.Active)
            {
                windowVisible = true;
                break;
            }
        }
        #endregion

        #region calculate width
        GUIStyle btnStyle = new GUIStyle(GUI.skin.GetStyle("Button"));
        btnStyle.fontSize *= 3;
        if (recalcWidth)
        {
            recalcWidth = false;
            float width = -1;
            foreach (var btn in entries)
            {
                width = Math.Max(btnStyle.CalcSize(new GUIContent(btn.Key)).x, width);
            }
            sidebarWidth = width + 2 * GUI_SIDEBAR_PADDING.x;
        }
        #endregion

        #region hotkeys
        {
            bool escPressed = Input.GetKeyDown(KeyCode.Escape);
            if (escPressed && !keyDebounce)
            {
                if (guiVisible && windowVisible)
                {
                    foreach (OverlayWindow window in windows)
                    {
                        window.Active = false;
                    }
                }
                else
                {
                    guiVisible = !guiVisible;
                }
                if (!guiVisible)
                {
                    foreach (OverlayWindow window in windows)
                    {
                        window.Active = true;
                    }
                }
            }
            keyDebounce = escPressed;
        }
        #endregion

        guiFadeIntensity += (guiVisible ? 1f : -1f) * Time.deltaTime / GUI_SHOW_TIME;
        guiFadeIntensity = Mathf.Clamp01(guiFadeIntensity);

        // Dims the background
        if (guiFadeIntensity > 0)
        {
            GUI.backgroundColor = new Color(1, 1, 1, 0.45f * guiFadeIntensity);
            GUI.Box(new Rect(-10, -10, Screen.width + 20, Screen.height + 20), "", BlackBoxStyle);
        }

        UserMessageManager.Render();

        if (guiFadeIntensity > 0)
        {
            GUI.BeginGroup(new Rect((1f - guiFadeIntensity) * -sidebarWidth, 0, sidebarWidth, Screen.height));

            // Render sidebar
            {
                GUI.backgroundColor = new Color(1, 1, 1, 0.9f);
                GUI.Box(new Rect(-1, -10, sidebarWidth + 2, Screen.height + 20), "", BlackBoxStyle);
            }

            #region Render entries
            float y = GUI_SIDEBAR_PADDING.y;
            foreach (var btn in entries)
            {
                if (GUI.Button(new Rect(GUI_SIDEBAR_PADDING.x, y, sidebarWidth - GUI_SIDEBAR_PADDING.x * 2, GUI_SIDEBAR_ENTRY_HEIGHT), btn.Key, btnStyle) && !windowVisible)
                {
                    btn.Value();
                }
                y += GUI_SIDEBAR_ENTRY_HEIGHT + GUI_SIDEBAR_ENTRY_PADDING_Y;
            }
            GUI.EndGroup();
            #endregion

            foreach (OverlayWindow window in windows)
            {
                window.Render();
            }
        }
    }
}
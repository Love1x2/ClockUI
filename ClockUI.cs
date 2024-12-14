using System;
using System.Runtime.InteropServices;
using HutongGames.PlayMaker;
using MSCLoader;
using UnityEngine;

namespace ClockUI
{
    public class ClockUI : Mod
    {
        public override string ID => "ClockUI";
        public override string Name => "ClockUI";
        public override string Author => "Love1x2";
        public override string Version => "1.0";
        public override string Description => "A simple digital clock in the style of the original game's UI.";

        public override void ModSetup()
        {
            SetupFunction(Setup.PostLoad, Mod_PostLoad);
            SetupFunction(Setup.OnGUI, Mod_OnGUI);
            SetupFunction(Setup.Update, Mod_Update);
            SetupFunction(Setup.ModSettings, Mod_Settings);
        }

        private string time;

        private GUIStyle customStyle;
        private GUIStyle customShadowStyle;

        private SettingsCheckBox CheckBoxToggleMod;
        
        private void Mod_Settings()
        {
            CheckBoxToggleMod = Settings.AddCheckBox(this, "CheckBoxToggleMod", "Toggle mod", true);
        }

        private void Mod_PostLoad()
        {
            
            // Main text style.
            customStyle = new GUIStyle();
            customStyle.fontSize = 23;
            customStyle.normal.textColor = new Color(0.847f, 0.847f, 0.0f);
            customStyle.fontStyle = FontStyle.BoldAndItalic;

            // Main text shadow style.
            customShadowStyle = new GUIStyle();
            customShadowStyle.fontSize = 23;
            customShadowStyle.normal.textColor = Color.black;
            customShadowStyle.fontStyle = FontStyle.BoldAndItalic;

            _sun = GameObject.Find("SUN/Pivot");
            _rot = _sun.GetComponent<PlayMakerFSM>().FsmVariables.FindFsmFloat("Rotation");

            _transH = GameObject.Find("SuomiClock/Clock/hour/NeedleHour").transform;
            _transM = GameObject.Find("SuomiClock/Clock/minute/NeedleMinute").transform;

        }
        private void Mod_OnGUI()
        {
            
            int textPosX = 346;
            int textPosY = 37;

            // Text shadow offset.
            Vector2 shadowOffset = new Vector2(2, 2);

            if(CheckBoxToggleMod.GetValue())
            {
                // Draws the shadow of the main clock text.
                GUI.Label(new Rect(textPosX + shadowOffset.x, textPosY + shadowOffset.y, 100, 100), time, customShadowStyle);

                // Draws the main text of the clock.
                GUI.Label(new Rect(textPosX, textPosY, 100, 100), time, customStyle);
            }
                
        }
        private void Mod_Update()
        {
            // Updates the clock every frame.
            time = string.Format("{0:0}:{1:00}", Hour24, Minute);
        }


        private GameObject _sun;
        private FsmFloat _rot;
        private Transform _transH, _transM;

        // Based on the Sun's rotation, returns whether it's the afternoon.
		public bool IsAfternoon => (_rot.Value > 330.0f || _rot.Value <= 150.0f);

        // Hour in day, 0 to 12 float.
        public float Hour12F => ((360.0f - _transH.localRotation.eulerAngles.y) / 30.0f + 2.0f) % 12;
        
        // Hour in day, 0 to 24 float.
        public float Hour24F => IsAfternoon ? Hour12F + 12.0f : Hour12F;
        
        // Minute in hour, 0 to 60 float.
        public float MinuteF => (360.0f - _transM.localRotation.eulerAngles.y) / 6.0f;
        
        // Second in minute, 0 to 60 float.
        public float SecondF => (MinuteF * 60) % 60;

        // Hour in day, 0 to 11 integer.
        public int Hour12 => Mathf.FloorToInt(Hour12F);
        
        // Hour in day, 0 to 23 integer.
        public int Hour24 => Mathf.FloorToInt(Hour24F);
        
        // Minute in hour, 0 to 59 integer.
        public int Minute => Mathf.FloorToInt(MinuteF);
        
        // Second in minute, 0 to 59 integer.
        public int Second => Mathf.FloorToInt(SecondF);

        // The angle of the hour hand.
        public float AngleHour => _transH.localRotation.eulerAngles.y;
        
        // The angle of the minute hand.
        public float AngleMinute => _transM.localRotation.eulerAngles.y;
        
        // The Sun's current angle.
        public float AngleSun => _rot.Value;

    }
}

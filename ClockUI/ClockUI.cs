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
        public override string Version => "1.2";
        public override string Description => "A simple digital clock in the style of the original game's UI.";

        public override void ModSetup()
        {
            SetupFunction(Setup.PostLoad, Mod_PostLoad);
            SetupFunction(Setup.OnGUI, Mod_OnGUI);
            SetupFunction(Setup.Update, Mod_Update);
            SetupFunction(Setup.ModSettings, Mod_Settings);
        }

        private string time24;
        private string time12;
        private string AmPm;
        private string[] timeFormats = { "24 Hour", "12 Hour" };

        private float textPosX;
        private float textPosY;
        private float rectSizeW = 100;
        private float rectSizeH = 100;

        private int fontSize = 23;

        private GUIStyle customStyle;
        private GUIStyle customShadowStyle;

        private SettingsCheckBox ToggleModCheckBox;
        private SettingsCheckBox WristwatchCheckBox;
        private SettingsDropDownList TimeFormatList;
        private SettingsSliderInt xPosSlider;
        private SettingsSliderInt yPosSlider;
        private SettingsSliderInt fontSizeSlider;

        private GameObject wristwatch;
        private PlayMakerFSM wristwatchPlayMaker;
        private bool wristwatchIsOwned;


        private void Mod_Settings()
        {
            ToggleModCheckBox = Settings.AddCheckBox(this, "ToggleModCheckBox", "Toggle mod", true);
            WristwatchCheckBox = Settings.AddCheckBox(this, "WristwatchCheckBox", "Requires wristwatch", false);
            TimeFormatList = Settings.AddDropDownList(this, "TimeFormatList", "Time Format", timeFormats);
            xPosSlider = Settings.AddSlider(this, "xPosSlider", "X Position", 0, 500, 305);
            yPosSlider = Settings.AddSlider(this, "yPosSlider", "Y Position", 0, 500, 39);
            fontSizeSlider = Settings.AddSlider(this, "fontSizeSlider", "Font Size", 0, 50, 23);
        }

        private void Mod_PostLoad()
        {

            wristwatch = GameObject.Find("Wristwatch");
            wristwatchPlayMaker = wristwatch.GetComponent<PlayMakerFSM>();
            
            
            // Main text style.
            customStyle = new GUIStyle();
            
            customStyle.normal.textColor = new Color(0.847f, 0.847f, 0.0f);
            customStyle.fontStyle = FontStyle.BoldAndItalic;
            customStyle.alignment = TextAnchor.UpperRight;

            // Main text shadow style.
            customShadowStyle = new GUIStyle();
            
            customShadowStyle.normal.textColor = Color.black;
            customShadowStyle.fontStyle = FontStyle.BoldAndItalic;
            customShadowStyle.alignment = TextAnchor.UpperRight;

            _sun = GameObject.Find("SUN/Pivot");
            _rot = _sun.GetComponent<PlayMakerFSM>().FsmVariables.FindFsmFloat("Rotation");

            _transH = GameObject.Find("SuomiClock/Clock/hour/NeedleHour").transform;
            _transM = GameObject.Find("SuomiClock/Clock/minute/NeedleMinute").transform;

        }
        private void Mod_OnGUI()
        {
            if(ToggleModCheckBox.GetValue() && WristwatchCheckBox.GetValue() && wristwatchIsOwned)
            {
                if (TimeFormatList.GetSelectedItemIndex() == 0)
                {
                    DrawClock24();
                }
                else
                {
                    DrawClock12();
                }
            }
            else if(ToggleModCheckBox.GetValue() && WristwatchCheckBox.GetValue() == false)
            {
                if (TimeFormatList.GetSelectedItemIndex() == 0)
                {
                    DrawClock24();
                }
                else
                {
                    DrawClock12();
                }
            }
                
        }
        private void Mod_Update()
        {
            // Updates the clock every frame.
            time24 = string.Format("{0:0}:{1:00}", Hour24, Minute);
            time12 = string.Format("{0:0}:{1:00} {2}", Hour12, Minute, AmPm);

            wristwatchIsOwned = wristwatchPlayMaker.FsmVariables.GetFsmBool("Owned").Value;

            if (IsAfternoon)
            {
                AmPm = "PM";
            }
            else
            {
                AmPm = "AM";
            }

            textPosX = xPosSlider.GetValue();
            textPosY = yPosSlider.GetValue();
            fontSize = fontSizeSlider.GetValue();

            customStyle.fontSize = fontSize;
            customShadowStyle.fontSize = fontSize;
        }

        private void DrawClock12()
        {
            // Text shadow offset.
            Vector2 shadowOffset = new Vector2(2, 2);

            // Draws the shadow of the main clock text.
            GUI.Label(new Rect(textPosX + shadowOffset.x, textPosY + shadowOffset.y, rectSizeW, rectSizeH), time12, customShadowStyle);

            // Draws the main text of the clock.
            GUI.Label(new Rect(textPosX, textPosY, rectSizeW, rectSizeH), time12, customStyle);
        }

        private void DrawClock24()
        {
            // Text shadow offset.
            Vector2 shadowOffset = new Vector2(2, 2);

            // Draws the shadow of the main clock text.
            GUI.Label(new Rect(textPosX + shadowOffset.x, textPosY + shadowOffset.y, rectSizeW, rectSizeH), time24, customShadowStyle);

            // Draws the main text of the clock.
            GUI.Label(new Rect(textPosX, textPosY, rectSizeW, rectSizeH), time24, customStyle);
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

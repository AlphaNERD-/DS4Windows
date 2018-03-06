using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS4Windows
{
    public class MouseLikeAnalog
    {
        private DS4State state = new DS4State();
        private DS4Device device;
        private int deviceNum;

        public MouseLikeAnalog(int deviceID, DS4Device d)
        {
            device = d;
            deviceNum = deviceID;
        }

        public void SixAxisMoved(object sender, SixAxisEventArgs e)
        {
            if (Global.UseSAforMouseLikeAnalog[deviceNum] && Global.GyroSensitivity[deviceNum] > 0)
            {
                bool triggeractivated = true;
                int i = 0;
                string[] ss = Global.SATriggers[deviceNum].Split(',');
                if (!string.IsNullOrEmpty(ss[0]))
                    foreach (string s in ss)
                        if (!(int.TryParse(s, out i) && getDS4ControlsByName(i)))
                            triggeractivated = false;
                if (triggeractivated)
                    calculateStickDelta();
                device.getCurrentState(state);
            }
        }

        private void calculateStickDelta()
        {

        }

        private bool getDS4ControlsByName(int key)
        {
            switch (key)
            {
                case -1: return true;
                case 0: return state.Cross;
                case 1: return state.Circle;
                case 2: return state.Square;
                case 3: return state.Triangle;
                case 4: return state.L1;
                case 5: return state.L2 > 127;
                case 6: return state.R1;
                case 7: return state.R2 > 127;
                case 8: return state.DpadUp;
                case 9: return state.DpadDown;
                case 10: return state.DpadLeft;
                case 11: return state.DpadRight;
                case 12: return state.L3;
                case 13: return state.R3;
                case 14: return state.Touch1;
                case 15: return state.Touch2;
                case 16: return state.Options;
                case 17: return state.Share;
                case 18: return state.PS;
            }
            return false;
        }
    }
}

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

        private double hRemainder = 0.0, vRemainder = 0.0;
        private double stickH = 0.0, stickV = 0.0;

        public MouseLikeAnalog(int deviceID, DS4Device d)
        {
            device = d;
            deviceNum = deviceID;
        }

        public void SixAxisMoved(object sender, SixAxisEventArgs e)
        {
            stickH = 0;
            stickV = 0;

            if (Global.GyroMode[deviceNum] == 1 && Global.GyroSensitivity[deviceNum] > 0)
            {
                bool triggeractivated = true;
                int i = 0;
                string[] ss = Global.SATriggers[deviceNum].Split(',');
                if (!string.IsNullOrEmpty(ss[0]))
                    foreach (string s in ss)
                        if (!(int.TryParse(s, out i) && getDS4ControlsByName(i)))
                            triggeractivated = false;
                if (triggeractivated)
                    calculateStickDelta(e);
                device.getCurrentState(state);
            }
        }

        private void calculateStickDelta(SixAxisEventArgs arg)
        {
            int deltaX = 0, deltaY = 0;
            deltaX = -arg.sixAxis.accelX;
            deltaY = -arg.sixAxis.accelY;
            //Console.WriteLine(arg.sixAxis.deltaX);

            double coefficient = Global.GyroSensitivity[deviceNum] / 100f;
            //Collect rounding errors instead of losing motion.
            double xMotion = coefficient * deltaX;
            xMotion += hRemainder;
            int xAction = (int)xMotion;
            hRemainder += xMotion - xAction;
            hRemainder -= (int)hRemainder;
            double yMotion = coefficient * deltaY;
            yMotion += vRemainder;
            int yAction = (int)yMotion;
            vRemainder += yMotion - yAction;
            vRemainder -= (int)vRemainder;
            if (Global.GyroInvert[deviceNum] == 2 || Global.GyroInvert[deviceNum] == 3)
                xAction *= -1;
            if (Global.GyroInvert[deviceNum] == 1 || Global.GyroInvert[deviceNum] == 3)
                yAction *= -1;
            if (yAction != 0 || xAction != 0)
            {
                stickH = xAction;
                stickV = yAction;
            }
        }

        public DS4State GetStickDelta(DS4State cState)
        {
            if (Global.GyroMode[deviceNum] == 1 && Global.GyroSensitivity[deviceNum] > 0)
            {
                cState.RX = (byte)(cState.RX + (byte)stickH);
                cState.RY = (byte)(cState.RY + (byte)stickV);
            }

            return cState;
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

using System;
using Tiny2D;
using Unity.Entities;

namespace Assets.Scripts.Runtime

{
    // Format Object to string for JSON, class from scratch because a JSON Lib for tiny doesn't exist yet
    public static class ToStringUtils
    {
        public static string FormatStep(StepStimulus actualStep)
        {
            string r = "{"
                + @"""index""" + ": " + actualStep.index + ", "
                + @"""indexImg""" + ": " + actualStep.indexImg + ", "
                + @"""isFood""" + ": " + ToStringUtils.GetBool(actualStep.isFood) + ", "
                + @"""isStopSignal""" + ": " + ToStringUtils.GetBool(actualStep.isStopSignal) + ", "
                + @"""isTimeOut""" + ": " + ToStringUtils.GetBool(actualStep.isTimeOut) + ", "
                + @"""isSuccess""" + ": " + ToStringUtils.GetBool(actualStep.isSuccess) + ", "
                + @"""timeStartStimulus""" + ": " + actualStep.timeStartStimulus + ", "
                + @"""timeEndStimulus""" + ": " + actualStep.timeEndStimulus + ", "
                + @"""reactionTime""" + ": " + actualStep.reactionTime + ", "
                + @"""ssd""" + ": " + actualStep.ssd + ", "
                + @"""ssrt""" + ": " + actualStep.ssrt
            + "}";

            return r;
        }

        public static string FormatSession(SSTSession session, string sessionName, DynamicBuffer<StepStimulus> steps, int sessionNum)
        {
            string r = "";

            if (sessionNum == 0) {
                r += "{"
                    + @"""date""" + ": " + (char)34 + DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year + (char)34 + ", "
                    + @"""sessions""" + ": " + "[";
            }

            r += "{"
                + @"""sessionName""" + ": " + (char)34 + sessionName + (char)34 + ", "
                + @"""isFoodLeft""" + ": " + ToStringUtils.GetBool(session.isFoodLeft) + ", "
                + @"""steps""" + ": " + "[";

            for (int i = 0; i < steps.Length; i++) {
                r += FormatStep(steps[i]);
                if (i != (steps.Length - 1)) // if it's not the last step add ,
                    r += ",";
            }

            r += "]}";

            if (sessionNum != 3) { // If it's not the last session add ,
                r += ",";
            } else {
                r += "]}";
            }

            return r.ToString();
        }

        public static string GetBool(bool isTrue)
        {
            if (isTrue) {
                return "true";
            } else {
                return "false";
            }
        }
    }
}

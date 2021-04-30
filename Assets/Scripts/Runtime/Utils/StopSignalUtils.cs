
using Tiny2D;
using Unity.Tiny;

namespace Assets.Scripts.Runtime
{
    public static class StopSignalUtils
    {
        public static float GetNextSSD(SSTSession session, bool actualStepRight)
        {
            float nextSSD = 0f;
            // Adjust the ssd for the session
            if (session.nbrOfTrue > session.nbrOfFalse)
                nextSSD = session.actualSSD + Const.STEP_SSD;

            if (session.nbrOfTrue < session.nbrOfFalse)
                nextSSD = session.actualSSD - Const.STEP_SSD;

            // Clamp the value
            if (session.actualSSD >= Const.MAX_SSD)
                nextSSD = Const.MAX_SSD;

            if (session.actualSSD <= Const.MIN_SSD)
                nextSSD = Const.MIN_SSD;

            Debug.Log("Nbr of true in session: " + session.nbrOfTrue + " + Nbr of false in session: " + session.nbrOfFalse + " / Nbr of steps: " + session.nbrOfStep);
            Debug.Log("Previous SSD: " + session.actualSSD + "/ Next SSD: " + nextSSD);
            return nextSSD;
        }
    }
}


using Tiny2D;

namespace Assets.Scripts.Runtime
{
    public static class StopSignalUtils
    {
        // New method
        public static float GetNextSSD(SSTSession session, bool actualStepRight)
        {
            float r = session.actualSSD;
            if (actualStepRight) {
                r += Const.STEP_SSD;
            } else {
                r -= Const.STEP_SSD;
            }

            if (r >= Const.MAX_SSD) r = Const.MAX_SSD;
            if (r <= Const.MIN_SSD) r = Const.MIN_SSD;

            return r;
        }

        // Old method
        /* public static float GetNextSSD(SSTSession session, bool actualStepRight)
            {
                Debug.Log("Session actual SSD: " + session.actualSSD);

            float nextSSD = session.actualSSD; // init next SSD with actual, if nbr False == nbr True, SSD doesn't change

            // Adjust the ssd for the session
            if (session.nbrOfTrueStop > session.nbrOfFalseStop) {
                nextSSD = session.actualSSD + Const.STEP_SSD;
                if(session.actualSSD >= Const.MAX_SSD)
                    nextSSD = Const.MAX_SSD;
                Debug.Log("nbr true > nbr false, nextssd: " + nextSSD);
            }

            if (session.nbrOfTrueStop < session.nbrOfFalseStop) { 
                nextSSD = session.actualSSD - Const.STEP_SSD;
                if(session.actualSSD <= Const.MIN_SSD)
                    nextSSD = Const.MIN_SSD;
                Debug.Log("nbr true < nbr false, nextssd: " + nextSSD);
            }

            Debug.Log("Nbr of true stop in session: " + session.nbrOfTrueStop + " + Nbr of false stop in session: " + session.nbrOfFalseStop);
            Debug.Log("Previous SSD: " + session.actualSSD + " / Next SSD: " + nextSSD);
            return nextSSD;
        }*/
    }
}

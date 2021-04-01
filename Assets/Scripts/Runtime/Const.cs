
namespace Assets.Scripts.Runtime
{
    static class Const
    {

        // Basics timing SSD
        public static float SPAWN_DELAY = 2f;
        public static float RESPONSE_DELAY = 0.6f;
        public static float STEP_SSD = 0.05f;
        public static float BASE_SSD = 0.25f;
        public static float MIN_SSD = 0.1f;
        public static float MAX_SSD = 0.45f;

        public static float MODIFIED_SST = 0.25f;

        public static string NAME_TRAINING1 = "Training 1";
        public static string NAME_TRAINING2 = "Training 2";
        public static string NAME_SESSION1 = "Session 1";
        public static string NAME_SESSION2 = "Session 2";

        public static string FOOD_LEFT = "Food is left";
        public static string FOOD_RIGHT = "Food is right";

        public static string FOLDER_IMG = "Sprites/";
        public static string FOLDER_FOOD = "Food/00";
        public static string FOLDER_NON_FOOD = "NoFood/10";
        public static string EXT_PNG = ".jpg";

        // Stimulus images
        public static int NBR_IMAGE = 64;
        public static int NBR_STIMULUS_TRAINING = 6;
        public static int NBR_STIMULUS_BLOCK = 64; // 64

        // Houses & Road spanning
        public static float SPAWN_DELAY_HOUSE = 1f;
        public static float SPAWN_DELAY_NATURE = 0.5f;
        public static float SPAWN_DELAY_ROAD = 1f;
        public static int NBR_HOUSES_LEFT = 5;
        public static int NBR_HOUSES_RIGTH = 5;
        public static int NBR_NATURE = 4;

        public static float CHANGE_DELAY_SCENE = 2f;

        public static void SetNewSST(float n)
        {
            MODIFIED_SST = n;
        }
    }
}

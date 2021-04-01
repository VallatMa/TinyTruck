
namespace Assets.Scripts.Runtime

{
    // Work with Touch and Mouse
    public static class ImageUtils
    {
        // Get the path for the image
        public static string GetPathImg(int index, bool isFood)
        {
            // Load the image
            string imageName = Const.FOLDER_IMG;

            if (isFood)
                imageName += Const.FOLDER_FOOD;
            else
                imageName += Const.FOLDER_NON_FOOD;

            if (index < 10)
                imageName += 0;

            imageName += index;// + Const.EXT_PNG;

            return imageName;
        }
    }
}

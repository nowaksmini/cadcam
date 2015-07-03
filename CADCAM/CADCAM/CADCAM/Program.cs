namespace CADCAM
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (CadCamGame game = new CadCamGame())
            {
                game.Run();
            }
        }
    }
#endif
}


using System;
using TTengine.Util;

namespace Pixie1
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            try
            {
                using (PixieGame game = new PixieGame())
                {
                    game.Run();
                }
            }
            catch (Exception ex)
            {
                MsgBox.Show("FEIG! (Fatal Error In Game)",
                  "Fatal Error - if you want you can notify the author.\n" + ex.Message + "\n" + ex.ToString());                
            }

        }
    }
#endif
}


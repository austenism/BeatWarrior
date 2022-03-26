using System;
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Text;

namespace Gaming
{
    public static class Constants {
        public static int BOARDHEIGHT = 768;
        public static int BOARDWIDTH = 768;
        public static int BORDERSIZE = 80;

        public static int GAMEHEIGHT = BOARDHEIGHT + (2 * BORDERSIZE);
        public static int GAMEWIDTH  = BOARDWIDTH  + (2 * BORDERSIZE);
    }
}

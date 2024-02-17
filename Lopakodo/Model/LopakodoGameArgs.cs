using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lopakodo.Model
{
    public class LopakodoGameArgs : EventArgs
    {
        private Int32 gameTime;
        private Boolean Won;
        public Int32 GameTime { get { return gameTime; } }

        public LopakodoGameArgs() { }
        public Boolean IsWon { get { return Won; } }
        public LopakodoGameArgs(Boolean won, Int32 GameTime)
        {
            gameTime = GameTime;
            Won = won;
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imagenation
{
    internal class DrawRequest
    {
        internal string Path;
        internal long DestroyAt;
        internal bool ResetOnWipe;
        internal int x;
        internal int y;

        public DrawRequest(string path, long lifetime, bool resetOnWipe, int x, int y)
        {
            Path = path;
            DestroyAt = Environment.TickCount64 + lifetime;
            ResetOnWipe = resetOnWipe;
            this.x = x;
            this.y = y;
        }
    }
}

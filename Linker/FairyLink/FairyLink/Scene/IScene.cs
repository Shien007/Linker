using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FairyLink.Device;

namespace FairyLink.Scene
{
    interface IScene
    {
        void Initialize();
        void Update();
        void Draw(Renderer renderer);
        void Shutdown();

        bool IsEnd();
        Scene Next();
    }
}

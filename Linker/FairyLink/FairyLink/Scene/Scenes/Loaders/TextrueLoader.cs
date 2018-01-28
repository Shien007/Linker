using FairyLink.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FairyLink.Scene.Scenes.Loaders
{
    class TextrueLoader : Loader
    {
        private Renderer renderer;

        public TextrueLoader(Renderer renderer, string[,] resources)
            : base(resources)
        {
            this.renderer = renderer;
            Initialize();
        }

        public override void Update()
        {
            endFlag = true;
            if (counter < maxNum) {
                renderer.LoadTextures(resources[counter, 0], resources[counter, 1]);
                counter++;
                endFlag = false;
            }
        }
    }
}

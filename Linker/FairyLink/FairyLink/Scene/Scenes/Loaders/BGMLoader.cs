using FairyLink.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FairyLink.Scene.Scenes.Loaders
{
    class BGMLoader : Loader
    {
        private Sound sound;
        public BGMLoader(Sound sound, string[,] resources)
            : base(resources)
        {
            this.sound = sound;
            Initialize();
        }

        public override void Update()
        {
            endFlag = true;
            if (counter < maxNum) {
                sound.LoadBGM(resources[counter, 0], resources[counter, 1]);
                counter++;
                endFlag = false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FairyLink.Scene.Scenes
{
    abstract class Loader
    {
        protected string[,] resources;
        protected int counter;
        protected int maxNum;
        protected bool endFlag;

        public Loader(string[,] resources) {
            this.resources = resources;
        }

        public void Initialize() {
            counter = 0;
            endFlag = false;
            maxNum = 0;
            if (resources != null) {
                maxNum = resources.GetLength(0);
            }
        }

        public int Count {
            get { return maxNum; }
        }

        public int CurrentCount {
            get { return counter; }
        }

        public bool IsEnd { get { return endFlag; } }

        public abstract void Update();
    }
}

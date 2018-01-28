using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FairyLink.Utility
{
    class Timer
    {
        private float currentTime;
        private float lmitTime;
        private bool isTime;

        public Timer(float second) {
            lmitTime = second * 60;
            currentTime = second * 60;
            isTime = false;
        }

        public void Initialize() {
            currentTime = lmitTime;
            isTime = false;
        }

        //時間がなくなる前にカウントダウンする
        public void Update() {
            if (isTime) { return; }
            currentTime--;
            if (currentTime <= 0) {
                isTime = true;
                currentTime = 0;
            }
        }

        public bool IsTime{
            get { return isTime; }
        }

        public float nowTime {
            get { return currentTime; }
        }


    }
}

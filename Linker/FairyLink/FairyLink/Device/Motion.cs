using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FairyLink.Utility;

namespace FairyLink.Device
{
    class Motion
    {
        private Range range;
        private Timer timer;
        private int motionNumber;
        private bool roll;

        private Dictionary<int, Rectangle> rectangles = new Dictionary<int, Rectangle>();

        public Motion() {
            Initialize(new Range(0, 0), new Timer(0.5f));
        }

        public void Initialize(Range range, Timer timer) {
            roll = true;
            this.range = range;
            this.timer = timer;
            motionNumber = range.First;
        }

        //時間だったら、次の画像を表示する
        public void Update() {
            if (range.IsOutOfRange()) { return; }
            timer.Update();
            if (timer.IsTime) {
                MotionUpdate();
                timer.Initialize();
            }
        }

        //表示したい画像の長方形情報を入れる
        public void Add(int index, Rectangle rect) {
            if (rectangles.ContainsKey(index)) {
                return;
            }
            rectangles.Add(index, rect);
        }

        //次の画像にチェンジする
        private void MotionUpdate() {
            motionNumber++;
            if (range.IsOutOfRange(motionNumber)) {
                if (!roll) { motionNumber = range.End; }
                else { motionNumber = range.First; }
            }
        }

        //描画したい画像の長方形情報を出す
        public Rectangle DrawRange_L() {
            return rectangles[motionNumber];
        }

        public Rectangle DrawRange_R() {
            int motionNumber2 = 3 - 2*(motionNumber % 4) + motionNumber ;
            return rectangles[motionNumber2];
        }

        public bool Roll {
            get { return roll; }
            set { roll = value; }
        }

    }
}

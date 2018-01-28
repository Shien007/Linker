using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using FairyLink.Utility;

namespace FairyLink.Device
{
    class EffectMotion
    {
        private Range range;
        private Timer timer;
        private int effectNumber;
        private bool roll;

        private Dictionary<int, Rectangle> rectangles = new Dictionary<int, Rectangle>();

        public EffectMotion() {
            Initialize(new Range(0, 0), new Timer(0.5f));
        }

        public void Initialize(Range range, Timer timer) {
            roll = false;
            this.range = range;
            this.timer = timer;
            effectNumber = range.First;
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
            effectNumber++;
            if (range.IsOutOfRange(effectNumber)) {
                if (!roll) { effectNumber = range.End; }
                else { effectNumber = range.First; }
            }
        }

        //描画したい画像の長方形情報を出す
        public Rectangle DrawRange_L() {
            return rectangles[effectNumber];
        }

        public Rectangle DrawRange_R() {
            int effectNumber2 = 3 - 2*(effectNumber % 4) + effectNumber ;
            return rectangles[effectNumber2];
        }

        public bool Roll {
            get { return roll; }
            set { roll = value; }
        }

        internal FairyLink.Actor.Character Character
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

    }
}

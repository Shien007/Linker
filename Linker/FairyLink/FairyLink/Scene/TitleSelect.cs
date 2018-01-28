using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FairyLink.Device;

namespace FairyLink.Scene
{
    class TitleSelect
    {
        private Vector2 startText;
        private Vector2 operateText;
        private Vector2 staffText;
        private bool isDown;        //選択肢を揺れる（下方向かどうか）
        private bool isSelect;
        private bool selectEnd;     //選択終わりかどうか
        private float velocity;     //揺れるスピード
        private InputState inputState;
        private int x;              //選択肢（１、startText　２、operateText　３、staffText）
        private Vector2 beam1;      //ビームの射出する座標（選択肢１）
        private Vector2 beam2;      //ビームの射出する座標（選択肢２）
        private Vector2 beam3;      //ビームの射出する座標（選択肢３）
        public TitleSelect(InputState inputState) {
            startText = new Vector2(380, 420);
            operateText = new Vector2(380, 500);
            staffText = new Vector2(380, 580);
            this.inputState = inputState;
            isDown = true;
            beam1 = new Vector2(320, 440);
            beam2 = new Vector2(320, 515);
            beam3 = new Vector2(320, 595);

            x = 1;
            velocity = 0;
            isSelect = false;
            selectEnd = false;
    }

        public void Initialize() {
            x = 1;
            isSelect = false;
            selectEnd = false;
            beam1 = new Vector2(320, 440);
            beam2 = new Vector2(320, 515);
            beam3 = new Vector2(320, 595);
        }

        public void Update() {
            Select();   //選択する
            Move();     //選択肢によって選択肢を揺れる
            ToNext();   //選択肢によってbeamを射出する処理
        }

        public void Move() {
            if (isDown) { velocity += 0.5f; }
            else { velocity -= 0.5f; }
            if ( Math.Abs(velocity) > 2) { isDown = !isDown; }

            if (x == 1) {
                startText.Y += velocity;
                operateText = new Vector2(380, 500);
                staffText = new Vector2(380, 580);

            }
            else if (x == 2) {
                operateText.Y += velocity;
                startText = new Vector2(380, 420);
                staffText = new Vector2(380, 580);
            }
            else {
                staffText.Y += velocity;
                startText = new Vector2(380, 420);
                operateText = new Vector2(380, 500);
            }
        }

        public void Select() {
            if (isSelect) { return; }
            if(inputState.WasDown(Keys.Down) || 
                inputState.WasDown(Buttons.DPadDown) ||
                inputState.WasDown(Buttons.LeftThumbstickDown))
            {
                if (x == 3) { return; }
                x++;
            }
            else if(inputState.WasDown(Keys.Up) || 
                inputState.WasDown(Buttons.DPadUp)||
                inputState.WasDown(Buttons.LeftThumbstickUp)) {
                if (x == 1) { return; }
                x--;
            }
        }

        public void ToNext() {
            if (isSelect) { 
                switch (x) {
                    case 1:
                        beam1.X += 12;
                        break;
                    case 2:
                        beam2.X += 12;
                        break;
                    case 3:
                        beam3.X += 12;
                        break;
                }
            }
            //演出終わると、次のシーンに行く
            if (beam1.X > 640 || beam2.X > 640 || beam3.X > 640) { selectEnd = true; }
        }

        public int GetSelect {
            get { return x; }
        }

        public bool IsSelectEnd {
            get { return selectEnd; }
        }

        public bool IsSelected {
            get { return isSelect; }
            set { isSelect = value; }
        }


        public void Draw(Renderer renderer) {
            renderer.DrawTexture("startText", startText);
            renderer.DrawTexture("operateText", operateText);
            renderer.DrawTexture("staffText", staffText);
            //選択肢によって、beamを描画する
            switch (x) {
                case 1:
                    renderer.DrawTexture("beam", beam1);
                    break;
                case 2:
                    renderer.DrawTexture("beam", beam2);
                    break;
                case 3:
                    renderer.DrawTexture("beam", beam3);
                    break;
            }
        }
    }
}

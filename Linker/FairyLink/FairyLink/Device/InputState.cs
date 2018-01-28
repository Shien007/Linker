using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace FairyLink.Device
{
    class InputState
    {
        private KeyboardState currentKey;
        private KeyboardState previousKey;
        private GamePadState currentPad;
        private GamePadState previousPad;

        public InputState() { }

        public void Update() {
            var keyState = Keyboard.GetState();
            var buttonState = GamePad.GetState(PlayerIndex.One);
            UpdateKey(keyState);
            UpdateButton(buttonState);
        }

        public void UpdateKey(KeyboardState keyState) {
            previousKey = currentKey;
            currentKey = keyState;
        }
        public void UpdateButton(GamePadState padState)
        {
            previousPad = currentPad;
            currentPad = padState;
        }

        /// <summary>
        /// 押しっぱなし
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsDown(Keys key){
            return currentKey.IsKeyDown(key);
        }

        public bool IsDown(Buttons button) {
            return currentPad.IsButtonDown(button);
        }



        /// <summary>
        /// 押して離す
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool WasDown(Keys key) {
            bool current = currentKey.IsKeyDown(key);
            bool previous = previousKey.IsKeyDown(key);
            return current && !previous;
        }

        public bool WasDown(Buttons button) {
            bool current = currentPad.IsButtonDown(button);
            bool previous = previousPad.IsButtonDown(button);
            return current && !previous;
        }

    }
}

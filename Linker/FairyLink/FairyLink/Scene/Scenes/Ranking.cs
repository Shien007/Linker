using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FairyLink.Device;
using FairyLink.Utility;

using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

namespace FairyLink.Scene.Scenes
{
    class Ranking : IScene
    {
        private Renderer renderer;
        private Sound sound;
        private InputState inputState;
        private Rankings rankings;
        private int[,] nowRanking;
        private List<int[,]> playerNames;
        private int[,] keyboard;
        private int timer;
        private bool inputEnd;
        private int count;

        private bool isEnd;
        
        public Ranking(GameDevice gameDevice)
        {
            renderer = gameDevice.GetRenderer;
            sound = gameDevice.GetSound;
            inputState = gameDevice.GetInputState;
            rankings = gameDevice.GetRankings;
            nowRanking = rankings.GetRankings;
            inputEnd = false;
            isEnd = false;
            timer = 0;
            count = -1;

            playerNames = new List<int[,]>();
            for (int i = 0; i < 3; i++) {
                playerNames.Add(new int[5, 2] { { 20, 20 }, { 20, 20 }, { 20, 20 }, { 20, 20 }, { 20, 20 } });
            }

            keyboard = new int[5, 11];
            keyboard[0, 0] = 1;
        }

        public void Initialize() {
            isEnd = false;
            inputEnd = false;
            keyboard = new int[5, 11];
            keyboard[0, 0] = 1;
            count = -1;

            playerNames.Clear();
            for (int i = 0; i < 3; i++) {
                playerNames.Add(new int[5, 2] { { 20, 20 }, { 20, 20 }, { 20, 20 }, { 20, 20 }, { 20, 20 } });
            }
        }

        public void Update()
        {
            timer++;
            sound.PlayBGM("gameover");
            if (inputState.WasDown(Keys.Enter) ||
                inputState.WasDown(Buttons.A) ||
                inputState.WasDown(Buttons.Start))
            {
                if (!inputEnd) {
                    NameInput();
                }
                else {
                    sound.PlaySE("cancel");
                    isEnd = true;
                }
                
            }

            inputStageUpdate();
        }


        public void NameInput() {
            if (Cursor()[0] == 4 && Cursor()[1] == 10)
            {
                if (count < 4)
                {
                    for (int i = 4; i > count; i--) {
                        playerNames[0][i, 0] = 3;
                        playerNames[0][i, 1] = 11;
                    }
                }
                rankings.PlayerName = playerNames;
                rankings.RankingSave();
                inputEnd = true;
            }
            else if (Cursor()[0] == 4 && Cursor()[1] == 9)
            {
                if (count == -1) { return; }
                playerNames[0][count, 0] = 3;
                playerNames[0][count, 1] = 11;
                count--;
            }
            else {
                if (count == -1) { count++; }
                playerNames[0][count, 0] = Cursor()[0];
                playerNames[0][count, 1] = Cursor()[1];
                if (count == 4) { return; }
                count++;
            }
        }


        public int[] Cursor() {
            int kx = 0, ky = 0;
            for (int j = 0; j < keyboard.GetLength(0); j++)
            {
                for (int i = 0; i < keyboard.GetLength(1); i++)
                {
                    if (keyboard[j, i] == 1)
                    {
                        kx = i;
                        ky = j;
                    }
                }
            }
            int[] cursor = new int[2] { ky, kx };
            return cursor;
        }

        public void inputStageUpdate() {
            if (inputState.WasDown(Keys.Up) ||
                inputState.WasDown(Buttons.DPadUp) ||
                inputState.WasDown(Buttons.LeftThumbstickUp))
            {
                for (int j = 0; j < keyboard.GetLength(0); j++)
                {
                    for (int i = 0; i < keyboard.GetLength(1); i++)
                    {
                        if (keyboard[j, i] == 1)
                        {
                            if (j - 1 < 0) { return; }
                            keyboard[j, i] = 0;
                            keyboard[j - 1, i] = 1;
                            return;
                        }
                    }
                }
            }
            else if (inputState.WasDown(Keys.Down) ||
                inputState.WasDown(Buttons.DPadDown) ||
                inputState.WasDown(Buttons.LeftThumbstickDown))
            {
                for (int j = 0; j < keyboard.GetLength(0); j++)
                {
                    for (int i = 0; i < keyboard.GetLength(1); i++)
                    {
                        if (keyboard[j, i] == 1)
                        {
                            if (j + 1 >= keyboard.GetLength(0)) { return; }
                            keyboard[j, i] = 0;
                            keyboard[j + 1, i] = 1;
                            return;
                        }
                    }
                }
            }
            else if (inputState.WasDown(Keys.Left) ||
                    inputState.WasDown(Buttons.DPadLeft) ||
                    inputState.WasDown(Buttons.LeftThumbstickLeft))
            {
                for (int j = 0; j < keyboard.GetLength(0); j++)
                {
                    for (int i = 0; i < keyboard.GetLength(1); i++)
                    {
                        if (keyboard[j, i] == 1)
                        {
                            if (i - 1 < 0) { return; }
                            keyboard[j, i] = 0;
                            keyboard[j, i - 1] = 1;
                            return;
                        }
                    }
                }
            }
            else if (inputState.WasDown(Keys.Right) ||
                    inputState.WasDown(Buttons.DPadRight) ||
                    inputState.WasDown(Buttons.LeftThumbstickRight))
            {
                for (int j = 0; j < keyboard.GetLength(0); j++)
                {
                    for (int i = 0; i < keyboard.GetLength(1); i++)
                    {
                        if (keyboard[j, i] == 1)
                        {
                            if (i + 1 >= keyboard.GetLength(1)) { return; }
                            keyboard[j, i] = 0;
                            keyboard[j, i + 1] = 1;
                            return;
                        }
                    }
                }
            }
        }


        public void Draw(Renderer renderer)
        {
            if (!inputEnd) {
                DrawInputName(renderer);
            }
            else {
                DrawRanking(renderer);
                DrawName(renderer);
            }
        }

        public void DrawInputName(Renderer renderer) {
            if (timer < 20)
            {
                renderer.DrawTexture("RankInPut1", Vector2.Zero);
            }
            else {
                if (timer >= 40) { timer = 0; }
                renderer.DrawTexture("RankInPut2", Vector2.Zero);
            }

            Vector2 barPosition = new Vector2(350, 240);
            for (int i = 0; i < 5; i++) {
                renderer.DrawTexture("bar", barPosition);
                barPosition.X += 60;
            }

            int kx = 0, ky = 0;
            for (int j = 0; j < keyboard.GetLength(0); j++)
            {
                for (int i = 0; i < keyboard.GetLength(1); i++)
                {
                    if (keyboard[j, i] == 1) {
                        kx = i;
                        ky = j;
                    }
                }
            }

            Vector2 cursorPosition = new Vector2(95 + kx * 77, 285 + ky * 60);
            renderer.DrawTexture("cursor", cursorPosition);

            Vector2 namePosition = new Vector2(360, 180);

            for (int j = 0; j <= count; j++) {
                renderer.DrawTexture("keyboard", namePosition, new Rectangle(playerNames[0][j,1] * 26, playerNames[0][j,0] * 39, 26, 39));
                namePosition.X += 60;
            }
        }

        public void DrawName(Renderer renderer) {

            Vector2 playerNamePosition = new Vector2(140, 305);
            playerNames = rankings.PlayerNameData;
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 5; i++)
                {
                    renderer.DrawTexture("keyboard", playerNamePosition,
                        new Rectangle(playerNames[j][i,1] * 26, playerNames[j][i,0] * 39, 26, 39)
                        );
                    playerNamePosition.X += 40;
                }

                playerNamePosition.X = 140;
                playerNamePosition.Y += 110;
            }
        }

        public void DrawRanking(Renderer renderer) {
            if (timer < 20) {
                renderer.DrawTexture("Ranking1", Vector2.Zero);
            }
            else {
                if (timer >= 40) { timer = 0; }
                renderer.DrawTexture("Ranking2", Vector2.Zero);
            }
            renderer.DrawNumber("number", new Vector2(405, 310), rankings.TimeCalculat((nowRanking[0, 1]))[0]);
            renderer.DrawNumber("number", new Vector2(580, 310), rankings.TimeCalculat((nowRanking[0, 1]))[1]);
            renderer.DrawNumber("number", new Vector2(810, 310), nowRanking[0, 0]);

            renderer.DrawNumber("number", new Vector2(405, 420), rankings.TimeCalculat((nowRanking[1, 1]))[0]);
            renderer.DrawNumber("number", new Vector2(580, 420), rankings.TimeCalculat((nowRanking[1, 1]))[1]);
            renderer.DrawNumber("number", new Vector2(810, 420), nowRanking[1, 0]);

            renderer.DrawNumber("number", new Vector2(405, 530), rankings.TimeCalculat((nowRanking[2, 1]))[0]);
            renderer.DrawNumber("number", new Vector2(580, 530), rankings.TimeCalculat((nowRanking[2, 1]))[1]);
            renderer.DrawNumber("number", new Vector2(810, 530), nowRanking[2, 0]);
        }


        public void Shutdown() { }

        public bool IsEnd() { return isEnd; }
        public Scene Next()
        {
            Initialize();
            return Scene.TITLE;
        }

    }
}

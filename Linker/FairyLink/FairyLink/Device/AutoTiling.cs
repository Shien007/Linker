using FairyLink.Def;
using FairyLink.Scene;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FairyLink.Device
{
    class AutoTiling
    {

        private int size;
        private int width;
        private int[,] mapData;
        private List<Rectangle[,]> rectData;
        private List<Rectangle> tiles;
        private Stage stage;

        public AutoTiling(Stage stage)
        {
            size = 32;
            width = 16;
            this.stage = stage;
            mapData = stage.MapData;
            rectData = new List<Rectangle[,]>();
            tiles = new List<Rectangle>();
            Initialize();
        }


        public void Initialize()
        {
            for (int i = 0; i < 6; i++) {
                for (int j = 0; j < 16; j++) {
                    tiles.Add(new Rectangle(j * size, i * size, size, size));
                }
            }

            for (int i = 0; i < StageMap.XMax * StageMap.YMax; i++) {
                rectData.Add(new Rectangle[2, 2] {
                    { new Rectangle(),new Rectangle()},
                    { new Rectangle(),new Rectangle()},
                });
            }
            Check();
        }


        public void Check()
        {
            for (int i = 0; i < mapData.GetLength(0); i++) {
                for (int j = 0; j < mapData.GetLength(1); j++) {
                    CheckTilingRound(i, j);
                }
            }
        }


        public Point MapPosition(Vector2 position)
        {
            int x = (int)position.X / (size * 2);
            int y = (int)position.Y / (size * 2);
            return new Point(x, y);
        }

        private bool isInGameMap(int x, int y)
        {
            return x >= 0 && x < mapData.GetLength(1) &&
                    y >= 0 && y < mapData.GetLength(0);
        }

        private bool[,] isEqualTile(int x, int y)
        {
            bool[,] check = new bool[3, 3];
            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    int targetX = x + j - 1;
                    int targetY = y + i - 1;
                    check[i, j] = (isInGameMap(targetX, targetY) && mapData[y, x] == mapData[targetY, targetX]);
                }
            }
            return check;
        }

        public void CheckTilingRound(int y, int x)
        {
            Rectangle topL = new Rectangle();
            Rectangle topR = new Rectangle();
            Rectangle bottomL = new Rectangle();
            Rectangle bottomR = new Rectangle();

            if (mapData[y, x] >= 0)
            {
                bool[,] check = isEqualTile(x, y);
                int offset = mapData[y, x] / (width * 6) + mapData[y, x] * 2 % width;

                topL = tiles[3 * width + 1 + offset];
                topR = tiles[3 * width + 2 + offset];
                bottomL = tiles[4 * width + 1 + offset];
                bottomR = tiles[4 * width + 2 + offset];


                //左上
                if (!check[0, 0]) { topL = tiles[0 + offset]; }
                //右上
                if (!check[0, 2]) { topR = tiles[1 + offset]; }
                //左下
                if (!check[2, 0]) { bottomL = tiles[width + offset]; }
                //右下
                if (!check[2, 2]) { bottomR = tiles[width + 1 + offset]; }

                //上
                if (!check[0, 1])
                {
                    topL = tiles[2 * width + 1 + offset];
                    topR = tiles[2 * width + 2 + offset];
                    if (!check[1, 2])   //右
                    { topR = tiles[3 + offset]; }
                    if (!check[1, 0])   //左
                    { topL = tiles[2 + offset]; }
                }
                //下
                if (!check[2, 1])
                {
                    bottomL = tiles[5 * width + 1 + offset];
                    bottomR = tiles[5 * width + 2 + offset];
                    if (!check[1, 0])   //左
                    { bottomL = tiles[width + 2 + offset]; }
                    if (!check[1, 2])   //右
                    { bottomR = tiles[width + 3 + offset]; }
                }
                //右
                if (!check[1, 2])
                {
                    if (check[0, 1])   //上
                    { topR = tiles[3 * width + 3 + offset]; }
                    if (check[2, 1])   //下
                    { bottomR = tiles[4 * width + 3 + offset]; }
                }
                //左
                if (!check[1, 0])
                {
                    if (check[0, 1])   //上
                    { topL = tiles[3 * width + offset]; }
                    if (check[2, 1])   //下
                    { bottomL = tiles[4 * width + offset]; }
                }
            }


            int num = y * mapData.GetLength(1) + x;
            rectData[num] = new Rectangle[2, 2] {
                    { topL,topR},
                    { bottomL,bottomR},
            };
        }

        public void Draw(Renderer renderer) {
            for (int i = 0; i < mapData.GetLength(0); i++) {
                for (int j = 0; j < mapData.GetLength(1); j++)
                {
                    int num = i * mapData.GetLength(1) + j;
                    int tx = j * size * 2;
                    int ty = i * size * 2;
                    renderer.DrawTexture("MapChip", stage.GetScreenPosition(new Vector2(tx, ty)), rectData[num][0, 0]);
                    renderer.DrawTexture("MapChip", stage.GetScreenPosition(new Vector2(tx + size, ty)), rectData[num][0, 1]);
                    renderer.DrawTexture("MapChip", stage.GetScreenPosition(new Vector2(tx, ty + size)), rectData[num][1, 0]);
                    renderer.DrawTexture("MapChip", stage.GetScreenPosition(new Vector2(tx + size, ty + size)), rectData[num][1, 1]);
                }
            }
        }


    }
}

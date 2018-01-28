using System.Collections.Generic;
using System.Text;
using System.IO;
using FairyLink.Def;

namespace FairyLink.Scene
{
    class StageLoader
    {
        private int[,] mapData;
        private int[,] gimmickData;

        public StageLoader() { }

        public int[,] MapLoad(int stageNumber) {
            try {
                StreamReader streamReader = new StreamReader("Content/CSV/Stage" + stageNumber + "_stage.csv", Encoding.GetEncoding("Shift_JIS"));

                List<string> lines = new List<string>();
                string[] splitLine = null;
                while (streamReader.Peek() >= 0) {
                    lines.Add(streamReader.ReadLine());
                }
                splitLine = lines[0].Split(',');
                mapData = new int[StageMap.YMax, StageMap.XMax];

                for (int j = 0; j < StageMap.YMax; j++) {
                    splitLine = lines[j].Split(',');
                    for (int i = 0; i < StageMap.XMax; i++) {
                        mapData[j, i] = int.Parse(splitLine[i]);
                    }
                }
                streamReader.Close();
            }
            catch (FileNotFoundException ffe) {
                mapData = new int[0, 0];
            }
            return mapData;
        }

        public int[,] GimmickLoad(int stageNumber) {
            try {
                StreamReader streamReader = new StreamReader("Content/CSV/Stage" + stageNumber + "_gimmick.csv", Encoding.GetEncoding("Shift_JIS"));

                List<string> lines = new List<string>();
                string[] splitLine = null;
                while (streamReader.Peek() >= 0) {
                    lines.Add(streamReader.ReadLine());
                }
                splitLine = lines[0].Split(',');
                gimmickData = new int[StageMap.YMax, StageMap.XMax];

                for (int j = 0; j < StageMap.YMax; j++) {
                    splitLine = lines[j].Split(',');
                    for (int i = 0; i < StageMap.XMax; i++) {
                        gimmickData[j, i] = int.Parse(splitLine[i]);
                    }
                }
                streamReader.Close();
            }
            catch (FileNotFoundException ffe) {
                gimmickData = new int[0, 0];
            }
            return gimmickData;
        }




    }
}

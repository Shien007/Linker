using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FairyLink.Utility
{
    class Rankings
    {
        private int deathCount;
        private int playTime;
        private int[,] rankings;
        private List<int[,]> playerName;
        private List<int[,]> playerNameData;


        public Rankings() {
            deathCount = 0;
            playTime = 0;
            rankings = new int[4, 2];
            playerName = new List<int[,]>();
            playerNameData = new List<int[,]>();
            for (int i = 0; i < 3; i++) {
                playerName.Add(new int[5, 2] { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } });
                playerNameData.Add(new int[5, 2] { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } });
            }
            
        }

        public void Initialize() {
            deathCount = 0;
            playTime = 0;
            playerName = new List<int[,]>();
            playerNameData = new List<int[,]>();
            for (int i = 0; i < 3; i++)
            {
                playerName.Add(new int[5, 2] { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } });
                playerNameData.Add(new int[5, 2] { { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 }, { 0, 0 } });
            }
        }

        public void LoadRankingData()
        {
            FileStream fileStream = new FileStream("Content/Csv/rankings.csv", FileMode.OpenOrCreate);
            StreamReader streamReader = new StreamReader(fileStream, Encoding.GetEncoding("Shift_JIS"));

            List<string> lines = new List<string>();
            string[] splitLine = new string[12];

            for (int i = 0; i < 3; i++) {
                lines.Add(streamReader.ReadLine());
                splitLine = lines[i].Split(',');

                for (int j = 0; j < 5; j++) {
                    playerNameData[i][j, 0] =int.Parse(splitLine[j * 2]);
                    playerNameData[i][j, 1] = int.Parse(splitLine[j * 2 + 1]);
            }

                rankings[i, 0] = int.Parse(splitLine[10]);
                rankings[i, 1] = int.Parse(splitLine[11]);
            }
            
            streamReader.Close();
            fileStream.Close();
        }
        
        public void RankingSave()
        {
            LoadRankingData();
            FileStream fileStream = new FileStream("Content/Csv/rankings.csv", FileMode.OpenOrCreate);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.GetEncoding("Shift_JIS"));

            

            for (int i = 0; i < 3; i++) {
                if (playTime < rankings[i, 1]) {
                    for (int j = 2; j > i; j--) {
                        rankings[j, 0] = rankings[j - 1, 0];
                        rankings[j, 1] = rankings[j - 1, 1];
                        playerNameData[j] = playerNameData[j - 1];
                    }
                    playerNameData[i] = playerName[0];
                    rankings[i, 0] = deathCount;
                    rankings[i, 1] = playTime;
                    break;
                }
            }

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 5; j++) {
                    streamWriter.Write(playerName[i][j, 0] + "," + playerName[i][j, 1] + ",");
                }
                
                streamWriter.WriteLine(rankings[i, 0] + "," + rankings[i, 1]);
            }
            streamWriter.Close();
            fileStream.Close();
        }


        public int[] TimeCalculat(int time) {
            int second = time % 60;
            int minute = time / 60;
            int[] times = new int[2] { minute, second };
            return times;
        }



        public int DeathCount {
            get { return deathCount; }
            set { deathCount = value; }
        }

        public int PlayTime {
            get { return playTime; }
            set { playTime = value;}
        }

        public List<int[,]> PlayerName
        {
            set { playerName = value; }
        }

        public List<int[,]> PlayerNameData {
            get { return  playerNameData; }
        }

        public int[,] GetRankings {
            get { return rankings; }
        }

    }
}

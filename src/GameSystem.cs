using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{

    public abstract class GameSystem
    {
        protected GameBase Game;
        public GameSystem(GameBase g) { Game = g; }
        public abstract void Exit();
        public abstract bool SaveData(string filename, string data);
        public abstract bool LoadData(string filename, out string data);
        public abstract void HideNav();
        public abstract Platform GetPlatform();
    }

    public class GameData
    {
        GameBase GameBase;
        public GameData(GameBase b)
        {
            GameBase = b;
        }

        public float HighScore { get; set; } = 0;

        public void Save()
        {
            try
            {
                GameBase.GameSystem.SaveData("Save.txt", HighScore.ToString());
            }
            catch (Exception ex)
            {
                GameBase.Log(ex.ToString());
            }
        }
        public void Load()
        {
            try
            {
                HighScore = 0;
                string str;
                if (GameBase.GameSystem.LoadData("Save.txt", out str))
                {
                    string[] strs = str.Split(',');
                    if (strs.Length == 1)
                    {
                        float hs = 0;
                        Single.TryParse(strs[0], out hs);
                        HighScore = hs;
                    }
                }
            }
            catch (Exception ex)
            {
                GameBase.Log(ex.ToString());
            }
        }
    }
}

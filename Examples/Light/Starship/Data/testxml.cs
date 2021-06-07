using System;
using System.Collections.Generic;
using System.Text;

namespace Starship.Data
{
    public class Leaderboard
    {
        public List<Score> Scores;
        public Leaderboard()
        {
            Scores = new List<Score>();
        }
    }
    public class Score
    {
        public double topTime;

        public Score(double scoreTime)
        {
            topTime = scoreTime;
            time = DateTime.Now;
        }

        public Score()
        {
            time = DateTime.Now;
        }

        public DateTime time;
    }

}

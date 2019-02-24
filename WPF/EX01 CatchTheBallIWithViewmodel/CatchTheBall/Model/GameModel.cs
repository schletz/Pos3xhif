using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CatchTheBall.Model
{
    public class GameModel
    {
        // Wir setzen ein Singleton Pattern um, das bedeutet dass wir nur
        // 1 Instanz von GameModel erzeugen können.
        public static GameModel Instance { get; } = new GameModel();

        private GameModel() { }

        private int clickCount, score;
        public const int MAX_CLICKS = 10;
        public int ClickCount
        {
            get => clickCount;
            set { if (!Finished) clickCount = value; }
        }
        public int Score
        {
            get => score;
            set { score = Finished ? score : value; }
        }
        public bool Finished => ClickCount == MAX_CLICKS;

        public void NewGame()
        {
            clickCount = score = 0;
        }

    }
}

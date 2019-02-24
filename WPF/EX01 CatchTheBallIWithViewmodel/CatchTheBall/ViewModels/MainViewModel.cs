using CatchTheBall.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CatchTheBall.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        // Die einzige Instanz des GameModel (Singleton Pattern)
        private readonly GameModel gameModel = GameModel.Instance;
        public int ClickCount
        {
            get => gameModel.ClickCount;
            set
            {
                // Damit am Anfang nicht 0 verarbeitet wird.
                if (value != gameModel.ClickCount)
                {
                    gameModel.ClickCount = value;
                    // Nicht vergessen, dass nach jedem Klick die Bindings
                    // neu zu lesen sind!
                    PropertyChanged(
                        this, 
                        new PropertyChangedEventArgs(nameof(ClickCount)));
                    PropertyChanged(
                        this,
                        new PropertyChangedEventArgs(nameof(Finished)));
                }
            }
        }

        public int Score
        {
            get => gameModel.Score;
            set
            {
                if (value != gameModel.Score)
                {
                    gameModel.Score = value;
                    PropertyChanged(
                        this,
                        new PropertyChangedEventArgs(nameof(Score)));
                }
            }
        }
        public bool Finished => gameModel.Finished;

        /// <summary>
        /// Methode für den Klick auf Neues Spiel.
        /// </summary>
        public ICommand NewGameCommand => new RelayCommand(
            () =>
            {
                gameModel.NewGame();
                // NewGame setzt diese 3 Werte neu. Nicht schön, eine Alternative
                // wäre auf GameModel mit den Bindings direkt zuzugreifen.
                PropertyChanged(
                    this,
                    new PropertyChangedEventArgs(nameof(ClickCount)));
                PropertyChanged(
                    this,
                    new PropertyChangedEventArgs(nameof(Score)));
                PropertyChanged(
                    this,
                    new PropertyChangedEventArgs(nameof(Finished)));
            });
        // Vom Interface INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
    }
}

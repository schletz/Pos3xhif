using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using CashApp.Model;

namespace CashApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Einzige Instanz der TransactionLog Klasse.
        /// </summary>
        private static TransactionLog transactionLog;
        /// <summary>
        /// Einzige Instanz der Statemachine von MainWindow.
        /// </summary>
        private readonly Stateless.StateMachine<States, Triggers> stateMachine;

        /// <summary>
        /// Initialisiert die State Machine und die Commands.
        /// </summary>
        public MainViewModel()
        {
            stateMachine = new Stateless.StateMachine<States, Triggers>(States.EnterAmount);
            InitStateMachine();

            // Damit nicht bei jedem get ein neuer Command erstellt wird, weisen wir sie im
            // Konstruktor zu.
            AmountOkCommand = CreateStateChangingCommand(Triggers.AmountOkPressed);
            ConfirmPaymentCommand = CreateStateChangingCommand(Triggers.ConfirmPaymentPressed);
            PinAgainCommand = CreateStateChangingCommand(Triggers.PinAgainPressed);
            PinNumberCommand = CreatePinNumberCommand();
        }

        /// <summary>
        /// Vom Interface INotifyPropertyChanged; aktualisiert die GUI.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Die verschiedenen States, die MainWindow einnehmen kann.
        /// </summary>
        public enum States
        {
            EnterAmount, EnterPin, ConfirmPayment, PinError
        }

        /// <summary>
        /// Die Trigger, die zum Erreichen der States führen.
        /// </summary>
        public enum Triggers
        {
            AmountOkPressed, ConfirmPaymentPressed, PinAgainPressed, LastPinNumberEntered
        }

        /// <summary>
        /// Lazy Initialization der TransactionLog Klasse. Ist hier nicht notwendig, aber im Falle
        /// einer Datenbank macht das mehr Sinn, erst zu initialisieren, wenn das Model auch wirklich
        /// gebraucht wird.
        /// </summary>
        public static TransactionLog TransactionLog => transactionLog ?? (transactionLog = new TransactionLog());
        /// <summary>
        /// Der zu zahlende Betrag, der am Anfang eingegeben wird.
        /// </summary>
        public decimal? Amount { get; set; }
        /// <summary>
        /// OK Button nach der Eingabe des zu zahlenden Betrages.
        /// </summary>
        public ICommand AmountOkCommand { get; private set; }
        /// <summary>
        /// OK Button nach der erfolgreichen Eingabe des PINS.
        /// </summary>
        public ICommand ConfirmPaymentCommand { get; private set; }
        /// <summary>
        /// Der eingegebene PIN.
        /// </summary>
        public int Pin { get; private set; }
        /// <summary>
        /// OK Button nach fehlerhafter Eingabe des Pins.
        /// </summary>
        public ICommand PinAgainCommand { get; private set; }
        /// <summary>
        /// Zahlenbuttons
        /// </summary>
        public ICommand PinNumberCommand { get; private set; }
        /// <summary>
        /// Aktueller State, damit in XAML der Converter für die Sichtbarkeit genutzt werden kann.
        /// </summary>
        public States State => stateMachine.State;
        /// <summary>
        /// Erstellt den Handler für die Zahlenbuttons. Dabei wird die gedrückte Zahl (als 
        /// CommandParameter in XAML) zum PIN dazugezählt.
        /// </summary>
        /// <returns></returns>
        private ICommand CreatePinNumberCommand()
        {
            return new RelayCommand(
            (param) =>
            {
                if (param == null) { return; }
                Pin = Pin * 10 + int.Parse(param.ToString());
                if (Pin >= 1000)
                {
                    stateMachine.Fire(Triggers.LastPinNumberEntered);
                }
            },
            (param) => stateMachine.CanFire(Triggers.LastPinNumberEntered)
            );
        }

        /// <summary>
        /// Handler für die OK Buttons. Meist wird der State durch einen Button verändert. Diese
        /// Methode liefert generisch ein RelayCommand zurück. Die Buttons sind nur aktiv, wenn
        /// der Trigger auch im aktuellen State ausgelöst werden kann.
        /// </summary>
        /// <param name="trigger"></param>
        /// <returns></returns>
        private ICommand CreateStateChangingCommand(Triggers trigger)
        {
            return new RelayCommand(
                (param) => stateMachine.Fire(trigger),
                (param) => stateMachine.CanFire(trigger));
        }

        /// <summary>
        /// Initialisiert die erlaubten Übergange der States, indem sie immer die Information
        /// Aktueller State + Trigger = Neuer State
        /// speichert.
        /// </summary>
        private void InitStateMachine()
        {
            stateMachine.Configure(States.EnterAmount)
                .OnEntry(() =>
                {
                    // Den zu zahlenden Betrag wieder auf 0 setzen.
                    SetProperty(nameof(Amount), default(decimal?));
                })
                .Permit(Triggers.AmountOkPressed, States.EnterPin);

            stateMachine.Configure(States.EnterPin)
                .OnEntry(() =>
                {
                    // Den PIN wieder leer machen.
                    SetProperty(nameof(Pin), default(int));
                })
                // Bedingter Übergang, wenn der PIN gültig ist (1234) wird zu ConfirmPayment
                // gewechselt.
                .PermitIf(Triggers.LastPinNumberEntered, States.ConfirmPayment, () => Pin == 1234)
                .PermitIf(Triggers.LastPinNumberEntered, States.PinError, () => Pin != 1234);

            stateMachine.Configure(States.ConfirmPayment)
                .Permit(Triggers.ConfirmPaymentPressed, States.EnterAmount)
                // Wird die Zahlung bestätigt, wird ein Log Eintrag im Model erstellt.
                .OnExit(() => TransactionLog.Transactions.Add(new Transaction
                    {
                        Amount = Amount ?? 0
                    }));

            stateMachine.Configure(States.PinError)
                .Permit(Triggers.PinAgainPressed, States.EnterPin);

            stateMachine.OnTransitioned((t) =>
            {
                // Wird der State verändert, benachrichtigt dies alle Controls, 
                // dessen Sichtbarkeit vom State abhängen.
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(State)));
            });
        }

        /// <summary>
        /// Setzt den Wert eines Properties und ruft PropertyChanged auf, sodass das Binding 
        /// aktualisiert wird. Leider gibt es keine bessere Lösung als Reflection oder das 
        /// händische aufrufen von PropertyChanged in jedem set.
        /// </summary>
        /// <param name="name">Name des Properties (nameof verwenden!)</param>
        /// <param name="value">Neuer Wert des Properties.</param>
        private void SetProperty(string name, object value)
        {
            PropertyInfo prop = GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance)
                ?? throw new ArgumentException($"Property {name} not found.");
            prop.SetValue(this, value);
            PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}

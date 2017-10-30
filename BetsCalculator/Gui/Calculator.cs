using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gui
{
    public class Calculator : INotifyPropertyChanged
    {
        double _maxBet = 1000;
        double _coef1 = 1;
        double _coef2 = 1;
        double _bet1;
        double _bet2;
        double _profit1;
        double _profit2;
        double _profit1Pc;
        double _profit2Pc;

        public event PropertyChangedEventHandler PropertyChanged;

        public double Coef1
        {
            get
            {
                return _coef1;
            }
            set
            {
                if (_coef1 != value)
                {
                    _coef1 = value;
                    OnCoef1Changed();
                }
            }
        }

        public double Coef2
        {
            get
            {
                return _coef2;
            }
            set
            {
                if (_coef2 != value)
                {
                    _coef2 = value;
                    OnCoef1Changed();
                }
            }
        }

        public double Bet1 { get => _bet1; set => _bet1 = value; }
        public double Bet2 { get => _bet2; set => _bet2 = value; }
        public double Profit1 { get => _profit1; set => _profit1 = value; }
        public double Profit2 { get => _profit2; set => _profit2 = value; }
        public double Profit1Pc { get => _profit1Pc; set => _profit1Pc = value; }
        public double Profit2Pc { get => _profit2Pc; set => _profit2Pc = value; }
        public double MaxBet { get => _maxBet; set => _maxBet = value; }

        public double ForkCoef
        {
            get
            {
                try
                {
                    return 1 / Coef1 + 1 / Coef2;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public double ForkPercent
        {
            get
            {
                try
                {
                    return 100 / ForkCoef - 100;
                }
                catch
                {
                    return 0;
                }
            }
        }

        public double SumCoef
        {
            get
            {
                return Coef1 + Coef2;
            }
        }

        public double SumBet
        {
            get
            {
                return Bet1 + Bet2;
            }
        }

        public void Calculate()
        {
            if (ForkPercent == 0)
                return;

            var mb = MaxBet * 2;
            Bet1 = MaxBet * Coef2 / SumCoef;
            Bet2 = MaxBet * Coef1 / SumCoef;
            Profit1 = Coef1 * Bet1;
            Profit2 = Coef2 * Bet2;
            Profit1Pc = (Profit1 - SumBet) / SumBet * 100;
            Profit2Pc = (Profit2 - SumBet) / SumBet * 100;

            OnPropertyChanged(nameof(Bet1));
            OnPropertyChanged(nameof(Bet2));
            OnPropertyChanged(nameof(Profit1));
            OnPropertyChanged(nameof(Profit2));
            OnPropertyChanged(nameof(Profit1Pc));
            OnPropertyChanged(nameof(Profit2Pc));
        }

        void OnCoef1Changed()
        {
            Calculate();
        }

        void OnCoef2Changed()
        {
            Calculate();
        }

        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

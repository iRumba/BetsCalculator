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
        public double Profit1
        {
            get
            {
                return Bet1 * Coef1;
            }
        }

        public double Profit2
        {
            get
            {
                return Bet2 * Coef2;
            }
        }

        public double Profit1Pc
        {
            get
            {
                return (Profit1 - SumBet) / SumBet * 100;
            }
        }

        public double Profit2Pc
        {
            get
            {
                return (Profit2 - SumBet) / SumBet * 100;
            }
        }

        public double MaxBet
        {
            get
            {
                return _maxBet;
            }
            set
            {
                if (_maxBet != value)
                {
                    _maxBet = value;
                    OnMaxBetChanged();
                }
            }
        }

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

            ChangeBets();
            //Bet1 = MaxBet * Coef2 / SumCoef;
            //Bet2 = MaxBet * Coef1 / SumCoef;
            //Profit1 = Coef1 * Bet1;
            //Profit2 = Coef2 * Bet2;
            //Profit1Pc = (Profit1 - SumBet) / SumBet * 100;
            //Profit2Pc = (Profit2 - SumBet) / SumBet * 100;

            OnPropertyChanged(nameof(Bet1));
            OnPropertyChanged(nameof(Bet2));
            OnPropertyChanged(nameof(Profit1));
            OnPropertyChanged(nameof(Profit2));
            OnPropertyChanged(nameof(Profit1Pc));
            OnPropertyChanged(nameof(Profit2Pc));
            OnPropertyChanged(nameof(ForkCoef));
        }

        //double NormalizeDig(double dig)
        //{
        //    var trunc = Math.Truncate(dig);
        //    if 
        //}

        void ChangeBets()
        {
            var b1 = Bet1;
            var b2 = Bet2;
            var bestRatio = Coef1 / Coef2;
            Tuple<int, int> bestBets = null;
            var currentRatioDif = double.MaxValue;

            var bets = GetAllBets((int)MaxBet).Where(b => b.Item1 * Coef1 >= (b.Item1 + b.Item2) && b.Item2 * Coef2 >= (b.Item1 + b.Item2));
            foreach (var bet in bets)
            {
                Bet1 = bet.Item1;
                Bet2 = bet.Item2;

                if (Profit1Pc == 0 || Profit2Pc == 0)
                    continue;

                var currentRatio = Profit2Pc / Profit1Pc;

                if (currentRatioDif >= Math.Abs(currentRatio - bestRatio))
                {
                    currentRatioDif = Math.Abs(currentRatio - bestRatio);
                    bestBets = bet;
                }
            }

            if (bestBets == null)
            {
                Bet1 = 0;
                Bet2 = 0;
            }
            else
            {
                Bet1 = bestBets.Item1;
                Bet2 = bestBets.Item2;
            }
        }

        IEnumerable<int> GetBeautyDigits(int max)
        {
            var incrs = new int[] { 50, 100, 500, 1000 };
            var current = 50;
            var incrIndex = 0;
            while (current <= max)
            {
                yield return current;
                if (incrIndex < 3)
                {
                    var preNext = current + incrs[incrIndex];
                    var preNext2 = current + incrs[incrIndex + 1];
                    if (Rang(preNext2) > Rang(preNext))
                        incrIndex++;
                }
                current += incrs[incrIndex];
            }
        }

        IEnumerable<Tuple<int,int>> GetAllBets(int max)
        {
            foreach (var bet1 in GetBeautyDigits(max))
                foreach (var bet2 in GetBeautyDigits(max))
                    yield return new Tuple<int, int>(bet1, bet2);
        }

        int Rang(int dig)
        {
            return (dig == 0) ? 1 : (int)Math.Ceiling(Math.Log10(Math.Abs(dig) + 0.5));
        }

        void OnCoef1Changed()
        {
            Calculate();
        }

        void OnCoef2Changed()
        {
            Calculate();
        }

        void OnMaxBetChanged()
        {
            Calculate();
        }

        void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}

using System;

namespace AlgorithmsOptimizationMethodsRGR
{
    public readonly struct Fraction : IComparable<Fraction>, IEquatable<Fraction>
    {
        public readonly long Numerator;
        public readonly long Denominator;

        public Fraction(long numerator, long denominator = 1)
        {
            if (denominator == 0)
            {
                throw new DivideByZeroException();
            }

            if (denominator < 0)
            {
                numerator = -numerator;
                denominator = -denominator;
            }

            long gcd = GCD(Math.Abs(numerator), denominator);
            Numerator = numerator / gcd;
            Denominator = denominator / gcd;
        }

        private static long GCD(long a, long b)
        {
            while (b != 0)
            {
                long temp = b;
                b = a % b;
                a = temp;
            }

            return a;
        }

        public static Fraction Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new Fraction(0);
            }

            string[] parts = input.Trim().Split('/');

            if (parts.Length == 1)
            {
                return new Fraction(long.Parse(parts[0]));
            }

            return new Fraction(long.Parse(parts[0]), long.Parse(parts[1]));
        }

        public override string ToString() => (Denominator == 1 || Denominator == 0) ? Numerator.ToString() : $"{Numerator}/{Denominator}";

        public static Fraction operator +(Fraction a, Fraction b) => new Fraction(a.Numerator * b.Denominator + b.Numerator * a.Denominator, a.Denominator * b.Denominator);

        public static Fraction operator -(Fraction a) => new Fraction(-a.Numerator, a.Denominator);

        public static Fraction operator -(Fraction a, Fraction b) => a + (-b);

        public static Fraction operator *(Fraction a, Fraction b) => new Fraction(a.Numerator * b.Numerator, a.Denominator * b.Denominator);

        public static Fraction operator /(Fraction a, Fraction b) => new Fraction(a.Numerator * b.Denominator, a.Denominator * b.Numerator);

        public static bool operator ==(Fraction a, Fraction b) => a.Equals(b);

        public static bool operator !=(Fraction a, Fraction b) => !(a == b);

        public static bool operator >(Fraction a, Fraction b) => a.Numerator * b.Denominator > b.Numerator * a.Denominator;

        public static bool operator <(Fraction a, Fraction b) => a.Numerator * b.Denominator < b.Numerator * a.Denominator;

        public static bool operator >=(Fraction a, Fraction b) => a.Numerator * b.Denominator >= b.Numerator * a.Denominator;

        public static bool operator <=(Fraction a, Fraction b) => a.Numerator * b.Denominator <= b.Numerator * a.Denominator;

        public bool Equals(Fraction other) => Numerator == other.Numerator && Denominator == other.Denominator;

        public override bool Equals(object? obj) => obj is Fraction other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Numerator, Denominator);

        public int CompareTo(Fraction other)
        {
            if (this < other)
            {
                return -1;
            }

            if (this > other)
            {
                return 1;
            }

            return 0;
        }
    }
}
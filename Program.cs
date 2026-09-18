using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FuzzyCoffeeMaker
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Sugeno Fuzzy Logic Coffee Maker Demo ===");

            // Define crisp inputs
            double waterVolume = 350.0;     // ml (Range: 100ml to 500ml)
            double strengthPreference = 7.5; // Scale 1 to 10 (1 = Mild, 10 = Strong)

            Console.WriteLine($"\nCrisp Inputs Water: {waterVolume}ml, Desired Strength: {strengthPreference}/10");

            // 1. FUZZIFICATION (Input Memberships)
            // Water Volume: Small, Large
            double waterSmall = TriangularMembership(waterVolume, 50, 100, 300);
            double waterLarge = TriangularMembership(waterVolume, 200, 400, 600);

            // Strength Preference: Mild, Strong
            double strengthMild = TriangularMembership(strengthPreference, 0, 2, 6);
            double strengthStrong = TriangularMembership(strengthPreference, 4, 8, 10);

            Console.WriteLine("\n--- Fuzzification Results ---");
            Console.WriteLine($"Water Volume   [Small: {waterSmall:F2}, Large: {waterLarge:F2}]");
            Console.WriteLine($"Strength Pref  [Mild: {strengthMild:F2}, Strong: {strengthStrong:F2}]");

            // 2. RULE EVALUATION (Sugeno-style min-inference)
            // Rule 1: IF Water is Small AND Strength is Mild, THEN Brew is Short
            // Rule 2: IF Water is Small AND Strength is Strong, THEN Brew is Medium
            // Rule 3: IF Water is Large AND Strength is Mild, THEN Brew is Medium
            // Rule 4: IF Water is Large AND Strength is Strong, THEN Brew is Long

            double rule1_strength = Math.Min(waterSmall, strengthMild);
            double rule2_strength = Math.Min(waterSmall, strengthStrong);
            double rule3_strength = Math.Min(waterLarge, strengthMild);
            double rule4_strength = Math.Min(waterLarge, strengthStrong);

            Console.WriteLine("\n--- Rule Evaluation Strengths ---");
            Console.WriteLine($"Rule 1 (Short Brew): {rule1_strength:F2}");
            Console.WriteLine($"Rule 2 (Medium Brew): {rule2_strength:F2}");
            Console.WriteLine($"Rule 3 (Medium Brew): {rule3_strength:F2}");
            Console.WriteLine($"Rule 4 (Long Brew): {rule4_strength:F2}");

            // 3. DEFUZZIFICATION (Sugeno Weighted Average)
            // Output centers (Constants for Brew Time in seconds)
            double brewShort = 60.0;  // 1 minute
            double brewMedium = 120.0; // 2 minutes
            double brewLong = 180.0;   // 3 minutes

            // Calculate the weighted average based on rule strengths
            double numerator = (rule1_strength * brewShort) +
                               (rule2_strength * brewMedium) +
                               (rule3_strength * brewMedium) +
                               (rule4_strength * brewLong);

            double denominator = rule1_strength + rule2_strength + rule3_strength + rule4_strength;

            double crispOutput = 0.0;
            if (denominator > 0)
            {
                crispOutput = numerator / denominator;
            }

            Console.WriteLine("\n--- Defuzzification Result ---");
            Console.WriteLine($"Calculated Crisp Brew Time: {crispOutput:F1} seconds");

            Console.WriteLine("\nPress any key to start brewing...");
            Console.ReadLine();
        }

        // Helper function for Triangular Membership Function
        // Parameters: x (input value), a (left foot), b (peak), c (right foot)
        static double TriangularMembership(double x, double a, double b, double c)
        {
            if (x <= a || x >= c)
                return 0.0;

            if (x == b)
                return 1.0;

            if (x > a && x < b)
                return (x - a) / (b - a);

            return (c - x) / (c - b);
        }
    }
}
}
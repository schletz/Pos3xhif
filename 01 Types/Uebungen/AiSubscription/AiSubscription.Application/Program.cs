using System;
using System.Collections.Generic;
using TestHelpers;
using static TestHelpers.ProgramChecker;

namespace AiSubscription.Application
{
    internal class Program
    {
        /// <summary>
        /// A discount that is not part of the model. It shows that Subscription works with every IDiscount.
        /// </summary>
        private class HalfPriceDiscount : IDiscount
        {
            public string Code => "HALF";
            public decimal Apply(decimal amount) => amount / 2;
        }

        // Every check creates its own objects inside the lambda.
        // An unfinished class therefore fails only its checks and cannot stop the program.
        private static FlatPlan Flat(string name = "Pro", decimal monthlyFee = 20, int fairUseCredits = 1000) =>
            new FlatPlan(name: name, monthlyFee: monthlyFee, fairUseCredits: fairUseCredits);

        private static MeteredPlan Metered(string name = "Basic", decimal monthlyFee = 10, int includedCredits = 500, decimal pricePerCredit = 0.02M) =>
            new MeteredPlan(name: name, monthlyFee: monthlyFee, includedCredits: includedCredits, pricePerCredit: pricePerCredit);

        private static Subscription Sub(Plan plan) => new Subscription(customer: "Anna", plan: plan);

        private static void Main(string[] args)
        {
            Console.WriteLine("Checking class implementation");
            CheckAndWrite(() => typeof(Plan).IsAbstract, "Plan is abstract.");
            CheckAndWrite(() => typeof(Plan).HasProtectedConstructor(typeof(string), typeof(decimal)), "Plan has a protected constructor.");
            CheckAndWrite(() => typeof(Plan).IsImmutable(), "Plan is immutable.");
            CheckAndWrite(() =>
                typeof(Plan).GetMethod(nameof(Plan.CalculateCost))?.IsAbstract == true
                && typeof(Plan).GetMethod(nameof(Plan.CalculateCost))?.ReturnType == typeof(decimal), "Plan.CalculateCost() is an abstract method that returns decimal.");
            CheckAndWrite(() => !typeof(FlatPlan).HasDefaultConstructor() && typeof(FlatPlan).IsImmutable(), "FlatPlan has no default constructor and is immutable.");
            CheckAndWrite(() => !typeof(MeteredPlan).HasDefaultConstructor() && typeof(MeteredPlan).IsImmutable(), "MeteredPlan has no default constructor and is immutable.");
            CheckAndWrite(() => typeof(IDiscount).IsInterface, "IDiscount is an interface.");
            CheckAndWrite(() =>
                typeof(IDiscount).IsAssignableFrom(typeof(PercentDiscount))
                && typeof(IDiscount).IsAssignableFrom(typeof(FixedDiscount))
                && typeof(IDiscount).IsAssignableFrom(typeof(StudentDiscount)), "PercentDiscount, FixedDiscount and StudentDiscount implement IDiscount.");
            CheckAndWrite(() => !typeof(Subscription).HasDefaultConstructor() && typeof(Subscription).IsImmutable(), "Subscription has no default constructor and is immutable.");
            CheckAndWrite(() => typeof(Subscription).PropertyHasType<IReadOnlyList<IDiscount>>(nameof(Subscription.Discounts)), "Subscription.Discounts is IReadOnlyList<IDiscount>.");
            CheckAndWrite(() => typeof(Subscription).PropertyHasType<IReadOnlySet<string>>(nameof(Subscription.DiscountCodes)), "Subscription.DiscountCodes is IReadOnlySet<string>.");
            CheckAndWrite(() => typeof(Subscription).PropertyHasType<IReadOnlyDictionary<string, int>>(nameof(Subscription.CreditsPerMonth)),
                "Subscription.CreditsPerMonth is IReadOnlyDictionary<string, int>.");

            Console.WriteLine("Checking constructors");
            CheckAndWrite(() =>
            {
                var p = new FlatPlan(name: "Pro", monthlyFee: 20, fairUseCredits: 1000);
                return p.Name == "Pro" && p.MonthlyFee == 20 && p.FairUseCredits == 1000;
            }, "Constructor of FlatPlan sets the values.");
            CheckAndWrite(() =>
            {
                var p = new MeteredPlan(name: "Basic", monthlyFee: 10, includedCredits: 500, pricePerCredit: 0.02M);
                return p.Name == "Basic" && p.MonthlyFee == 10 && p.IncludedCredits == 500 && p.PricePerCredit == 0.02M;
            }, "Constructor of MeteredPlan sets the values.");
            CheckAndWrite(() =>
            {
                Plan plan = Flat();
                var s = Sub(plan);
                return s.Customer == "Anna" && s.Plan == plan && s.Discounts.Count == 0;
            }, "Constructor of Subscription sets the values. Discounts is empty.");
            CheckAndWrite(() => Throws(() => Flat(name: "")) && Throws(() => Metered(monthlyFee: -1)), "Plan throws an ArgumentException if name is empty or monthlyFee is negative.");
            CheckAndWrite(() => Throws(() => Flat(fairUseCredits: 0)), "FlatPlan throws an ArgumentException if fairUseCredits is less than 1.");
            CheckAndWrite(() => Throws(() => Metered(includedCredits: 0)), "MeteredPlan throws an ArgumentException if includedCredits is less than 1.");

            Console.WriteLine("Checking FlatPlan and MeteredPlan");
            CheckAndWrite(() => Flat().CalculateCost(1000) == 20, "FlatPlan costs MonthlyFee if credits are equal to FairUseCredits.");
            CheckAndWrite(() => Flat().CalculateCost(1001) == 30, "FlatPlan costs 150 % of MonthlyFee above FairUseCredits.");
            CheckAndWrite(() => Metered().CalculateCost(800) == 16, "MeteredPlan charges every credit above IncludedCredits.", 2);
            CheckAndWrite(() => Metered().CalculateCost(500) == 10 && Metered().CalculateCost(0) == 10, "MeteredPlan costs only MonthlyFee up to IncludedCredits.", 2);
            CheckAndWrite(() => Flat(name: "Pro").ToString() == "Plan: Pro", "Plan.ToString() returns the name.");
            CheckAndWrite(() => Metered(name: "Basic", includedCredits: 500).ToString() == "Plan: Basic - 500 credits included", "MeteredPlan.ToString() adds the included credits.", 2);

            Console.WriteLine("Checking discounts");
            CheckAndWrite(() =>
                new PercentDiscount(percent: 1).Code == "PERCENT"
                && new FixedDiscount(value: 1).Code == "FIXED"
                && new StudentDiscount().Code == "STUDENT", "The codes are PERCENT, FIXED and STUDENT.");
            CheckAndWrite(() => new FixedDiscount(value: 5).Apply(12) == 7 && new FixedDiscount(value: 5).Apply(3) == 0, "FixedDiscount subtracts the value. The result is never negative.");
            CheckAndWrite(() => new PercentDiscount(percent: 20).Apply(50) == 40, "PercentDiscount reduces the amount by the percentage.", 2);
            CheckAndWrite(() => new StudentDiscount().Apply(15) == 7.5M && new StudentDiscount().Apply(20) == 10, "StudentDiscount returns 50 % of the amount.", 2);
            CheckAndWrite(() => new StudentDiscount().Apply(40) == 30, "StudentDiscount reduces the amount by 10 EUR at most.", 2);

            Console.WriteLine("Checking Subscription.AddDiscount()");
            CheckAndWrite(() =>
            {
                var s = Sub(Flat());
                return s.Discounts is not List<IDiscount> && s.DiscountCodes is not HashSet<string> && s.CreditsPerMonth is not Dictionary<string, int>;
            }, "Discounts, DiscountCodes and CreditsPerMonth do not return the internal collections.");
            CheckAndWrite(() =>
            {
                var s = Sub(Flat());
                IDiscount d = new StudentDiscount();
                return s.AddDiscount(d) && s.Discounts.Count == 1 && s.Discounts[0] == d && s.DiscountCodes.Contains("STUDENT");
            }, "AddDiscount() adds the discount and the code and returns true.");
            CheckAndWrite(() =>
            {
                var s = Sub(Flat());
                s.AddDiscount(new PercentDiscount(percent: 10));
                return !s.AddDiscount(new PercentDiscount(percent: 50)) && s.Discounts.Count == 1 && s.DiscountCodes.Count == 1;
            }, "AddDiscount() returns false if the code already exists.", 2);
            CheckAndWrite(() =>
            {
                var s = Sub(Flat());
                IDiscount d = new FixedDiscount(value: 1);
                s.AddDiscount(new StudentDiscount());
                return s.AddDiscount(d) && s.Discounts.Count == 2 && s.Discounts[1] == d;
            }, "AddDiscount() adds a second discount at the end of the list.");

            Console.WriteLine("Checking usage and invoices");
            CheckAndWrite(() =>
            {
                var s = Sub(Metered());
                return s.CreditsPerMonth.Count == 0 && s.TotalAmount == 0 && s.MostExpensiveMonth is null;
            }, "Without usage CreditsPerMonth is empty, TotalAmount is 0 and MostExpensiveMonth is null.");
            CheckAndWrite(() =>
            {
                var s = Sub(Metered());
                s.AddUsage("2026-10", 300);
                s.AddUsage("2026-11", 500);
                s.AddUsage("2026-11", 300);
                return s.CreditsPerMonth.Count == 2 && s.CreditsPerMonth["2026-10"] == 300 && s.CreditsPerMonth["2026-11"] == 800;
            }, "AddUsage() adds the credits to the month.", 2);
            CheckAndWrite(() =>
            {
                var s = Sub(Metered());
                s.AddUsage("2026-10", 300);
                return Throws(() => s.AddUsage("2026-10", 0)) && s.CreditsPerMonth["2026-10"] == 300;
            }, "AddUsage() throws an ArgumentException if credits is less than 1.", 2);
            CheckAndWrite(() =>
            {
                var s = Sub(Metered());
                s.AddUsage("2026-11", 800);
                return s.CalculateInvoice("2026-11") == 16 && s.CalculateInvoice("2026-12") == 10;
            }, "CalculateInvoice() uses the plan. A month without usage costs CalculateCost(0).", 2);
            CheckAndWrite(() =>
            {
                var s1 = Sub(Flat(monthlyFee: 100));
                s1.AddDiscount(new PercentDiscount(percent: 20));
                s1.AddDiscount(new FixedDiscount(value: 10));
                var s2 = Sub(Flat(monthlyFee: 100));
                s2.AddDiscount(new HalfPriceDiscount());
                return s1.CalculateInvoice("2026-11") == 70 && s2.CalculateInvoice("2026-11") == 50;
            }, "CalculateInvoice() applies the discounts in the order of the list and works with every IDiscount.", 2);
            CheckAndWrite(() =>
            {
                var s = Sub(Metered());
                s.AddDiscount(new PercentDiscount(percent: 50));
                s.AddUsage("2026-10", 300);
                s.AddUsage("2026-11", 800);
                return s.TotalAmount == 13;
            }, "TotalAmount is the sum of the invoices of all months with usage.", 2);
            CheckAndWrite(() =>
            {
                var s = Sub(Metered());
                s.AddUsage("2026-10", 300);
                s.AddUsage("2026-11", 800);
                s.AddUsage("2026-12", 600);
                return s.MostExpensiveMonth == "2026-11";
            }, "MostExpensiveMonth returns the month with the highest invoice.", 2);
            WriteSummary();
        }

        /// <summary>
        /// Returns true if the action throws an ArgumentException.
        /// </summary>
        private static bool Throws(Action action)
        {
            try { action(); }
            catch (ArgumentException) { return true; }
            return false;
        }
    }
}

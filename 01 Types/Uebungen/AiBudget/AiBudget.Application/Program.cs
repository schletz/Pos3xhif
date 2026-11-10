using System;
using System.Collections.Generic;
using TestHelpers;
using static TestHelpers.ProgramChecker;

namespace AiBudget.Application
{
    internal class Program
    {
        /// <summary>
        /// A rule that is not part of the model. It shows that BudgetAccount works with every IBudgetRule.
        /// </summary>
        private class NoSecretProjectRule : IBudgetRule
        {
            public string Name => "NoSecretProject";
            public bool Allows(AiRequest request, decimal spent) => request.Project != "Secret";
        }

        // Every check creates its own objects inside the lambda.
        // An unfinished class therefore fails only its checks and cannot stop the program.
        private static ChatRequest Chat(string project = "Website", string modelName = "TextBot", int inputTokens = 10000, int outputTokens = 5000) =>
            new ChatRequest(modelName: modelName, project: project, inputTokens: inputTokens, outputTokens: outputTokens);

        private static ImageRequest Image(string project = "Website", string modelName = "PixelGen", int images = 3, bool isHighResolution = false) =>
            new ImageRequest(modelName: modelName, project: project, images: images, isHighResolution: isHighResolution);

        private static BudgetAccount Account() => new BudgetAccount(owner: "Anna");

        private static void Main(string[] args)
        {
            Console.WriteLine("Checking class implementation");
            CheckAndWrite(() => typeof(AiRequest).IsAbstract, "AiRequest is abstract.");
            CheckAndWrite(() => typeof(AiRequest).HasProtectedConstructor(typeof(string), typeof(string)), "AiRequest has a protected constructor.");
            CheckAndWrite(() => typeof(AiRequest).IsImmutable(), "AiRequest is immutable.");
            CheckAndWrite(() =>
                typeof(AiRequest).PropertyHasType<decimal>(nameof(AiRequest.Cost))
                && typeof(AiRequest).IsAbstractProperty(nameof(AiRequest.Cost)), "AiRequest.Cost is an abstract decimal property.");
            CheckAndWrite(() => !typeof(ChatRequest).HasDefaultConstructor() && typeof(ChatRequest).IsImmutable(), "ChatRequest has no default constructor and is immutable.");
            CheckAndWrite(() => !typeof(ImageRequest).HasDefaultConstructor() && typeof(ImageRequest).IsImmutable(), "ImageRequest has no default constructor and is immutable.");
            CheckAndWrite(() => typeof(IBudgetRule).IsInterface, "IBudgetRule is an interface.");
            CheckAndWrite(() =>
                typeof(IBudgetRule).IsAssignableFrom(typeof(MonthlyLimitRule))
                && typeof(IBudgetRule).IsAssignableFrom(typeof(MaxRequestCostRule))
                && typeof(IBudgetRule).IsAssignableFrom(typeof(NoImagesRule)), "MonthlyLimitRule, MaxRequestCostRule and NoImagesRule implement IBudgetRule.");
            CheckAndWrite(() => !typeof(BudgetAccount).HasDefaultConstructor() && typeof(BudgetAccount).IsImmutable(), "BudgetAccount has no default constructor and is immutable.");
            CheckAndWrite(() =>
                typeof(BudgetAccount).PropertyHasType<IReadOnlyList<AiRequest>>(nameof(BudgetAccount.Requests))
                && typeof(BudgetAccount).PropertyHasType<IReadOnlyList<IBudgetRule>>(nameof(BudgetAccount.Rules)), "BudgetAccount.Requests and Rules are IReadOnlyList.");
            CheckAndWrite(() => typeof(BudgetAccount).PropertyHasType<IReadOnlySet<string>>(nameof(BudgetAccount.UsedModels)), "BudgetAccount.UsedModels is IReadOnlySet<string>.");
            CheckAndWrite(() => typeof(BudgetAccount).PropertyHasType<IReadOnlyDictionary<string, decimal>>(nameof(BudgetAccount.CostPerProject)),
                "BudgetAccount.CostPerProject is IReadOnlyDictionary<string, decimal>.");

            Console.WriteLine("Checking constructors");
            CheckAndWrite(() =>
            {
                var r = new ChatRequest(modelName: "TextBot", project: "Website", inputTokens: 100, outputTokens: 50);
                return r.ModelName == "TextBot" && r.Project == "Website" && r.InputTokens == 100 && r.OutputTokens == 50;
            }, "Constructor of ChatRequest sets the values.");
            CheckAndWrite(() =>
            {
                var r = new ImageRequest(modelName: "PixelGen", project: "App", images: 2, isHighResolution: true);
                return r.ModelName == "PixelGen" && r.Project == "App" && r.Images == 2 && r.IsHighResolution;
            }, "Constructor of ImageRequest sets the values.");
            CheckAndWrite(() =>
            {
                var a = Account();
                return a.Owner == "Anna" && a.Requests.Count == 0 && a.Rules.Count == 0;
            }, "Constructor of BudgetAccount sets the values. Requests and Rules are empty.");
            CheckAndWrite(() => Throws(() => Chat(modelName: "")) && Throws(() => Image(project: "")), "AiRequest throws an ArgumentException if modelName or project is empty.");
            CheckAndWrite(() => Throws(() => Chat(inputTokens: 0)) && Throws(() => Chat(outputTokens: 0)), "ChatRequest throws an ArgumentException if a token count is less than 1.");
            CheckAndWrite(() => Throws(() => Image(images: 0)), "ImageRequest throws an ArgumentException if images is less than 1.");

            Console.WriteLine("Checking ChatRequest and ImageRequest");
            CheckAndWrite(() => Image(images: 3).Cost == 0.12M, "3 images with normal resolution cost 0.12 EUR.");
            CheckAndWrite(() => Image(images: 3, isHighResolution: true).Cost == 0.30M, "3 images with high resolution cost 0.30 EUR.");
            CheckAndWrite(() => Chat(inputTokens: 10000, outputTokens: 5000).Cost == 3.00M, "A chat with 10000 input and 5000 output tokens costs 3.00 EUR.", 2);
            CheckAndWrite(() => Chat(inputTokens: 100, outputTokens: 50).Cost == 0.05M, "A chat with 100 input and 50 output tokens costs the minimum price of 0.05 EUR.", 2);
            CheckAndWrite(() => Image(project: "App").ToString() == "PixelGen (App)", "AiRequest.ToString() returns ModelName and Project.");
            CheckAndWrite(() => Chat(inputTokens: 100, outputTokens: 50).ToString() == "TextBot (Website) - 150 tokens", "ChatRequest.ToString() adds the number of tokens.", 2);

            Console.WriteLine("Checking budget rules");
            CheckAndWrite(() =>
                new MonthlyLimitRule(limit: 1).Name == "MonthlyLimit"
                && new MaxRequestCostRule(maxCost: 1).Name == "MaxRequestCost"
                && new NoImagesRule().Name == "NoImages", "The names are MonthlyLimit, MaxRequestCost and NoImages.");
            CheckAndWrite(() => new NoImagesRule().Allows(Chat(), 0) && !new NoImagesRule().Allows(Image(), 0), "NoImagesRule allows chats and rejects images.");
            CheckAndWrite(() =>
                new MaxRequestCostRule(maxCost: 3.00M).Allows(Chat(), 100)
                && !new MaxRequestCostRule(maxCost: 2.99M).Allows(Chat(), 0), "MaxRequestCostRule compares the cost of the request with MaxCost.", 2);
            CheckAndWrite(() => new MonthlyLimitRule(limit: 10).Allows(Chat(), 7.00M), "MonthlyLimitRule allows a request that reaches the limit exactly.", 2);
            CheckAndWrite(() => !new MonthlyLimitRule(limit: 10).Allows(Chat(), 7.01M), "MonthlyLimitRule rejects a request above the limit.", 2);

            Console.WriteLine("Checking BudgetAccount.Submit()");
            CheckAndWrite(() =>
            {
                var a = Account();
                return a.Requests is not List<AiRequest> && a.Rules is not List<IBudgetRule> && a.UsedModels is not HashSet<string>;
            }, "Requests, Rules and UsedModels do not return the internal collections.");
            CheckAndWrite(() =>
            {
                var a = Account();
                var r = Chat();
                return a.Submit(r) && a.Requests.Count == 1 && a.Requests[0] == r;
            }, "Submit() adds the request and returns true.");
            CheckAndWrite(() =>
            {
                var a = Account();
                var r = Chat();
                a.Submit(r);
                return !a.Submit(r) && a.Requests.Count == 1;
            }, "Submit() returns false if the request is already in the list.", 2);
            CheckAndWrite(() =>
            {
                var a = Account();
                var r1 = Chat();
                var r2 = Image();
                a.Submit(r1);
                return a.Submit(r2) && a.Requests.Count == 2 && a.Requests[1] == r2;
            }, "Submit() adds a second request at the end of the list.");

            Console.WriteLine("Checking rules and costs in BudgetAccount");
            CheckAndWrite(() =>
            {
                var a = Account();
                return a.Spent == 0 && a.CostPerProject.Count == 0 && a.UsedModels.Count == 0;
            }, "Without requests Spent is 0, CostPerProject and UsedModels are empty.");
            CheckAndWrite(() =>
            {
                var a = Account();
                a.Submit(Chat());
                a.Submit(Image());
                return a.Spent == 3.12M;
            }, "Spent is the sum of all costs.", 2);
            CheckAndWrite(() =>
            {
                var a = Account();
                a.Submit(Chat(project: "Website"));
                a.Submit(Image(project: "Website"));
                a.Submit(Image(project: "App", isHighResolution: true));
                return a.CostPerProject.Count == 2 && a.CostPerProject["Website"] == 3.12M && a.CostPerProject["App"] == 0.30M;
            }, "CostPerProject has one entry for every project with the sum of the costs.", 2);
            CheckAndWrite(() =>
            {
                var a = Account();
                a.Submit(Chat(modelName: "TextBot"));
                a.Submit(Chat(modelName: "TextBot"));
                a.Submit(Image(modelName: "PixelGen"));
                return a.UsedModels.Count == 2 && a.UsedModels.Contains("TextBot") && a.UsedModels.Contains("PixelGen");
            }, "UsedModels contains every model only once.", 2);
            CheckAndWrite(() =>
            {
                var a = Account();
                a.AddRule(new NoSecretProjectRule());
                bool rejected = !a.Submit(Chat(project: "Secret", modelName: "SecretBot"));
                return rejected && a.Requests.Count == 0 && a.Spent == 0 && a.UsedModels.Count == 0 && a.Submit(Chat());
            }, "Submit() does not save a rejected request and works with every IBudgetRule.", 2);
            CheckAndWrite(() =>
            {
                var a = Account();
                a.AddRule(new MonthlyLimitRule(limit: 5));
                return a.Submit(Chat()) && !a.Submit(Chat()) && a.Submit(Image()) && a.Spent == 3.12M;
            }, "Submit() passes the current value of Spent to the rules.", 2);
            CheckAndWrite(() =>
            {
                var a = Account();
                a.AddRule(new NoImagesRule());
                a.AddRule(new MaxRequestCostRule(maxCost: 0.20M));
                return a.FirstBlockingRule(Image()) == "NoImages"
                    && a.FirstBlockingRule(Chat()) == "MaxRequestCost"
                    && a.FirstBlockingRule(Chat(inputTokens: 100, outputTokens: 50)) is null
                    && a.Requests.Count == 0;
            }, "FirstBlockingRule() returns the name of the first blocking rule or null.", 2);
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

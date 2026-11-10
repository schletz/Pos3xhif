using System;
using System.Collections.Generic;
using TestHelpers;
using static TestHelpers.ProgramChecker;

namespace AiAgentFleet.Application
{
    internal class Program
    {
        /// <summary>
        /// A policy that is not part of the model. It shows that AgentFleet works with every IRunPolicy.
        /// </summary>
        private class NoSecretTeamPolicy : IRunPolicy
        {
            public string Name => "NoSecretTeam";
            public bool Allows(AiAgent agent, int runs) => agent.Team != "Secret";
        }

        // Every check creates its own objects inside the lambda.
        // An unfinished class therefore fails only its checks and cannot stop the program.
        private static ResearchAgent Research(string name = "Scout", string team = "Marketing", int searches = 20) =>
            new ResearchAgent(name: name, team: team, searches: searches);

        private static CodingAgent Coding(string name = "Builder", string team = "Backend", int minutes = 12) =>
            new CodingAgent(name: name, team: team, minutes: minutes);

        private static AgentFleet Fleet() => new AgentFleet(company: "Acme");

        private static void Main(string[] args)
        {
            Console.WriteLine("Checking class implementation");
            CheckAndWrite(() => typeof(AiAgent).IsAbstract, "AiAgent is abstract.");
            CheckAndWrite(() => typeof(AiAgent).HasProtectedConstructor(typeof(string), typeof(string)), "AiAgent has a protected constructor.");
            CheckAndWrite(() => typeof(AiAgent).IsImmutable(), "AiAgent is immutable.");
            CheckAndWrite(() =>
                typeof(AiAgent).PropertyHasType<decimal>(nameof(AiAgent.CostPerRun))
                && typeof(AiAgent).IsAbstractProperty(nameof(AiAgent.CostPerRun)), "AiAgent.CostPerRun is an abstract decimal property.");
            CheckAndWrite(() => !typeof(ResearchAgent).HasDefaultConstructor() && typeof(ResearchAgent).IsImmutable(), "ResearchAgent has no default constructor and is immutable.");
            CheckAndWrite(() => !typeof(CodingAgent).HasDefaultConstructor() && typeof(CodingAgent).IsImmutable(), "CodingAgent has no default constructor and is immutable.");
            CheckAndWrite(() => typeof(IRunPolicy).IsInterface, "IRunPolicy is an interface.");
            CheckAndWrite(() =>
                typeof(IRunPolicy).IsAssignableFrom(typeof(MaxRunsPolicy))
                && typeof(IRunPolicy).IsAssignableFrom(typeof(MaxCostPolicy))
                && typeof(IRunPolicy).IsAssignableFrom(typeof(OnlyResearchPolicy)), "MaxRunsPolicy, MaxCostPolicy and OnlyResearchPolicy implement IRunPolicy.");
            CheckAndWrite(() => !typeof(AgentFleet).HasDefaultConstructor() && typeof(AgentFleet).IsImmutable(), "AgentFleet has no default constructor and is immutable.");
            CheckAndWrite(() =>
                typeof(AgentFleet).PropertyHasType<IReadOnlyList<AiAgent>>(nameof(AgentFleet.Agents))
                && typeof(AgentFleet).PropertyHasType<IReadOnlyList<IRunPolicy>>(nameof(AgentFleet.Policies)), "AgentFleet.Agents and Policies are IReadOnlyList.");
            CheckAndWrite(() => typeof(AgentFleet).PropertyHasType<IReadOnlySet<string>>(nameof(AgentFleet.Teams)), "AgentFleet.Teams is IReadOnlySet<string>.");
            CheckAndWrite(() => typeof(AgentFleet).PropertyHasType<IReadOnlyDictionary<string, int>>(nameof(AgentFleet.RunsPerAgent)),
                "AgentFleet.RunsPerAgent is IReadOnlyDictionary<string, int>.");

            Console.WriteLine("Checking constructors");
            CheckAndWrite(() =>
            {
                var a = new ResearchAgent(name: "Scout", team: "Marketing", searches: 8);
                return a.Name == "Scout" && a.Team == "Marketing" && a.Searches == 8;
            }, "Constructor of ResearchAgent sets the values.");
            CheckAndWrite(() =>
            {
                var a = new CodingAgent(name: "Builder", team: "Backend", minutes: 12);
                return a.Name == "Builder" && a.Team == "Backend" && a.Minutes == 12;
            }, "Constructor of CodingAgent sets the values.");
            CheckAndWrite(() =>
            {
                var f = Fleet();
                return f.Company == "Acme" && f.Agents.Count == 0 && f.Policies.Count == 0;
            }, "Constructor of AgentFleet sets the values. Agents and Policies are empty.");
            CheckAndWrite(() => Throws(() => Research(name: "")) && Throws(() => Coding(team: "")), "AiAgent throws an ArgumentException if name or team is empty.");
            CheckAndWrite(() => Throws(() => Research(searches: 0)), "ResearchAgent throws an ArgumentException if searches is less than 1.");
            CheckAndWrite(() => Throws(() => Coding(minutes: 0)), "CodingAgent throws an ArgumentException if minutes is less than 1.");

            Console.WriteLine("Checking ResearchAgent and CodingAgent");
            CheckAndWrite(() => Research(searches: 20).CostPerRun == 1.00M, "A research agent with 20 searches costs 1.00 EUR.");
            CheckAndWrite(() => Research(searches: 21).CostPerRun == 0.63M, "A research agent with 21 searches costs 0.63 EUR.");
            CheckAndWrite(() => Coding(minutes: 12).CostPerRun == 1.80M, "A coding agent with 12 minutes costs 1.80 EUR.", 2);
            CheckAndWrite(() => Coding(minutes: 5).CostPerRun == 1.00M, "A coding agent with 5 minutes costs the minimum price of 1.00 EUR.", 2);
            CheckAndWrite(() => Research().ToString() == "Scout (Marketing)", "AiAgent.ToString() returns Name and Team.");
            CheckAndWrite(() => Coding().ToString() == "Builder (Backend) - 12 min", "CodingAgent.ToString() adds the minutes.", 2);

            Console.WriteLine("Checking run policies");
            CheckAndWrite(() =>
                new MaxRunsPolicy(maxRuns: 1).Name == "MaxRuns"
                && new MaxCostPolicy(maxCost: 1).Name == "MaxCost"
                && new OnlyResearchPolicy().Name == "OnlyResearch", "The names are MaxRuns, MaxCost and OnlyResearch.");
            CheckAndWrite(() => new OnlyResearchPolicy().Allows(Research(), 0) && !new OnlyResearchPolicy().Allows(Coding(), 0), "OnlyResearchPolicy allows research agents and rejects coding agents.");
            CheckAndWrite(() =>
                new MaxCostPolicy(maxCost: 1.80M).Allows(Coding(), 100)
                && !new MaxCostPolicy(maxCost: 1.79M).Allows(Coding(), 0), "MaxCostPolicy compares the cost of a run with MaxCost.", 2);
            CheckAndWrite(() => new MaxRunsPolicy(maxRuns: 3).Allows(Research(), 2), "MaxRunsPolicy allows a run below MaxRuns.", 2);
            CheckAndWrite(() => !new MaxRunsPolicy(maxRuns: 3).Allows(Research(), 3), "MaxRunsPolicy rejects a run if MaxRuns is reached.", 2);

            Console.WriteLine("Checking AgentFleet.Register()");
            CheckAndWrite(() =>
            {
                var f = Fleet();
                return f.Agents is not List<AiAgent> && f.Policies is not List<IRunPolicy> && f.Teams is not HashSet<string> && f.RunsPerAgent is not Dictionary<string, int>;
            }, "Agents, Policies, Teams and RunsPerAgent do not return the internal collections.");
            CheckAndWrite(() =>
            {
                var f = Fleet();
                var a = Research();
                return f.Register(a) && f.Agents.Count == 1 && f.Agents[0] == a && f.RunsPerAgent[a.Name] == 0;
            }, "Register() adds the agent with 0 runs and returns true.");
            CheckAndWrite(() =>
            {
                var f = Fleet();
                f.Register(Research(name: "Scout"));
                return !f.Register(Coding(name: "Scout")) && f.Agents.Count == 1;
            }, "Register() returns false if the name already exists.", 2);
            CheckAndWrite(() =>
            {
                var f = Fleet();
                var a2 = Coding(name: "Builder", team: "Backend");
                f.Register(Research(name: "Scout", team: "Backend"));
                return f.Register(a2) && f.Agents.Count == 2 && f.Agents[1] == a2 && f.Teams.Count == 1 && f.Teams.Contains("Backend");
            }, "Register() adds a second agent at the end of the list. Teams contains every team only once.");

            Console.WriteLine("Checking runs and costs");
            CheckAndWrite(() =>
            {
                var f = Fleet();
                return f.TotalCost == 0 && f.RunsPerAgent.Count == 0 && f.Teams.Count == 0;
            }, "Without agents TotalCost is 0, RunsPerAgent and Teams are empty.");
            CheckAndWrite(() =>
            {
                var f = Fleet();
                var a = Research();
                f.Register(a);
                return f.Run(a) && f.Run(a) && f.RunsPerAgent[a.Name] == 2;
            }, "Run() counts the runs of the agent and returns true.", 2);
            CheckAndWrite(() =>
            {
                var f = Fleet();
                return !f.Run(Research()) && f.RunsPerAgent.Count == 0;
            }, "Run() returns false if the agent is not registered.", 2);
            CheckAndWrite(() =>
            {
                var f = Fleet();
                var secret = Research(name: "Spy", team: "Secret");
                var scout = Research(name: "Scout", team: "Marketing");
                f.Register(secret);
                f.Register(scout);
                f.AddPolicy(new NoSecretTeamPolicy());
                return !f.Run(secret) && f.RunsPerAgent["Spy"] == 0 && f.Run(scout);
            }, "Run() does not count a rejected run and works with every IRunPolicy.", 2);
            CheckAndWrite(() =>
            {
                var f = Fleet();
                var a = Research();
                f.Register(a);
                f.AddPolicy(new MaxRunsPolicy(maxRuns: 2));
                return f.Run(a) && f.Run(a) && !f.Run(a) && f.RunsPerAgent[a.Name] == 2;
            }, "Run() passes the current number of runs to the policies.", 2);
            CheckAndWrite(() =>
            {
                var f = Fleet();
                var scout = Research();
                var builder = Coding();
                f.Register(scout);
                f.Register(builder);
                f.Run(scout);
                f.Run(scout);
                f.Run(builder);
                return f.TotalCost == 3.80M;
            }, "TotalCost is the sum of runs x CostPerRun of all agents.", 2);
            CheckAndWrite(() =>
            {
                var f = Fleet();
                var coding = Coding();
                var cheapResearch = Research(name: "Cheap", searches: 20);
                var expensiveResearch = Research(name: "Expensive", searches: 40);
                f.Register(coding);
                f.Register(cheapResearch);
                f.Register(expensiveResearch);
                f.AddPolicy(new OnlyResearchPolicy());
                f.AddPolicy(new MaxCostPolicy(maxCost: 1.00M));
                return f.FirstBlockingPolicy(coding) == "OnlyResearch"
                    && f.FirstBlockingPolicy(expensiveResearch) == "MaxCost"
                    && f.FirstBlockingPolicy(cheapResearch) is null
                    && f.RunsPerAgent[cheapResearch.Name] == 0;
            }, "FirstBlockingPolicy() returns the name of the first blocking policy or null.", 2);
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

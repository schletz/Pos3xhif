using System.Globalization;
using System.Linq.Expressions;
using ExpressionParser;

Console.WriteLine("Tokens of 2 * (3 + 4):");
foreach (Token token in new Tokenizer("2 * (3 + 4)").Tokenize())
{
    Console.WriteLine($"  {token}");
}
Console.WriteLine();

string[] examples =
[
    "2 * (3 + 4)",
    "2 * 3 + 4",
    "1 - 2 - 3",
    "-(2 + 3) * 4",
    "7 / 2 - -1.5",
    "2 * (3 + 4",
    "2 + * 3",
    "2 3",
    "4 % 2",
    "3. + 1",
];
foreach (string example in examples)
{
    Run(example);
}

Console.WriteLine();
Console.WriteLine("Compiled with System.Linq.Expressions:");
string[] compilerExamples = ["2 * (3 + 4)", "-(2 + 3) * 4", "7 / 2 - -1.5"];
foreach (string source in compilerExamples)
{
    Expr ast = Parser.Parse(source);
    Expression tree = ExpressionCompiler.ToExpression(ast);
    Func<double> compiled = ExpressionCompiler.Compile(ast);
    double compiledValue = compiled();
    double evaluatedValue = Evaluator.Evaluate(ast);
    Console.WriteLine($"  {tree,-24} Compiled: {Format(compiledValue),-5} Evaluator: {Format(evaluatedValue)}");
}

Console.WriteLine();
Console.WriteLine("Enter an expression (empty line ends):");
while (true)
{
    Console.Write("> ");
    string? line = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(line))
    {
        break;
    }
    Run(line);
}

// Parses and evaluates one expression and prints the AST and the value or the error.
static void Run(string source)
{
    try
    {
        Expr ast = Parser.Parse(source);
        double value = Evaluator.Evaluate(ast);
        Console.WriteLine($"{source,-14} AST: {ast,-22} Value: {Format(value)}");
    }
    catch (ParseException e)
    {
        Console.WriteLine($"{source,-14} Error at position {e.Position}: {e.Message}");
    }
}

// Formats a number with a decimal point, independent of the current culture.
static string Format(double value) => value.ToString(CultureInfo.InvariantCulture);

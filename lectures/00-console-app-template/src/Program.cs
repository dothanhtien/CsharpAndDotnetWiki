// Sample - Console App
// Run: dotnet run (from this src/ folder)

Console.WriteLine("Hello from the sample lecture!");

var numbers = new[] { 1, 2, 3, 4, 5 };
var sum = numbers.Sum();
Console.WriteLine($"Sum of {string.Join(" + ", numbers)} = {sum}");

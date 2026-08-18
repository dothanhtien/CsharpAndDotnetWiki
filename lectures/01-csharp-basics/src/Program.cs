// C# Basics - sample project
// Run: dotnet run (from this src/ folder)

// ---- 1. Variables & data types ----
string name = "Alice";
int age = 25;
double height = 1.68;
bool isStudent = false;
char grade = 'A';

Console.WriteLine($"{name} is {age} years old, {height}m tall, grade {grade}, student: {isStudent}");

// var lets the compiler infer the type from the right-hand side.
var favoriteNumber = 7;
Console.WriteLine($"favoriteNumber is inferred as {favoriteNumber.GetType().Name}");

Console.WriteLine();

// ---- 2. Constants ----
const double Pi = 3.14159; // must be assigned here, and can never change again
double circleArea = Pi * 2 * 2;
Console.WriteLine($"Area of a circle with radius 2 = {circleArea}");

Console.WriteLine();

// ---- 3. Value types vs. reference types ----
int x = 10;
int y = x; // copies the VALUE - x and y are independent
y = 99;
Console.WriteLine($"value types:     x={x}, y={y}"); // x is still 10

var box1 = new int[] { 1, 2, 3 };
var box2 = box1; // copies the REFERENCE - both point to the same array
box2[0] = 99;
Console.WriteLine($"reference types: box1[0]={box1[0]}, box2[0]={box2[0]}"); // both show 99

Console.WriteLine();

// ---- 4. Type conversion & casting ----
int wholeNumber = 42;
double asDouble = wholeNumber; // implicit: int -> double never loses data

double preciseNumber = 3.99;
int truncated = (int)preciseNumber; // explicit cast: double -> int can lose data (3, not 4)

string numericText = "123";
int parsed = int.Parse(numericText); // throws if the text isn't a valid number

string badText = "not a number";
bool ok = int.TryParse(badText, out int result); // never throws - returns false instead
Console.WriteLine($"implicit={asDouble}, explicit cast={truncated}, Parse={parsed}, TryParse ok={ok} result={result}");

Console.WriteLine();

// ---- 5. Operators ----
int a = 10, b = 3;
Console.WriteLine($"{a} + {b} = {a + b}");
Console.WriteLine($"{a} - {b} = {a - b}");
Console.WriteLine($"{a} * {b} = {a * b}");
Console.WriteLine($"{a} / {b} = {a / b}"); // integer division -> 3
Console.WriteLine($"{a} % {b} = {a % b}"); // remainder -> 1

var counter = 0;
counter += 5;  // same as counter = counter + 5
counter++;     // same as counter = counter + 1
Console.WriteLine($"counter after += 5 and ++: {counter}");

bool hasDiscount = true, isMember = false;
Console.WriteLine($"hasDiscount && isMember = {hasDiscount && isMember}");
Console.WriteLine($"hasDiscount || isMember = {hasDiscount || isMember}");

Console.WriteLine();

// ---- 6. String interpolation & formatting ----
decimal price = 1234.5m;
Console.WriteLine($"Plain:      {price}");
Console.WriteLine($"Currency:   {price:C}");   // culture-dependent currency symbol
Console.WriteLine($"Fixed(2):   {price:F2}");  // always 2 decimal places
Console.WriteLine($"Padded:     [{age,5}]");   // right-align in a 5-char field
Console.WriteLine(string.Format("Composite: {0} is {1} years old", name, age));

Console.WriteLine();

// ---- 7. Conditionals & switch ----
int score = 78;
string rank = score >= 90 ? "Excellent"
    : score >= 70 ? "Good"
    : score >= 50 ? "Average"
    : "Needs improvement";
Console.WriteLine($"Score {score} -> {rank}");

// Classic switch statement.
switch (grade)
{
    case 'A':
        Console.WriteLine("Grade A: outstanding.");
        break;
    case 'B':
        Console.WriteLine("Grade B: good.");
        break;
    default:
        Console.WriteLine("Grade: keep improving.");
        break;
}

// Modern switch expression - more compact, always produces a value.
string gradeLabel = grade switch
{
    'A' => "outstanding",
    'B' => "good",
    _ => "keep improving",
};
Console.WriteLine($"Grade label: {gradeLabel}");

Console.WriteLine();

// ---- 8. Nullable types & null handling ----
int? maybeAge = null; // int can't normally be null; int? (Nullable<int>) can
maybeAge ??= 18;      // "if null, assign this" - the null-coalescing assignment operator
Console.WriteLine($"maybeAge after ??=: {maybeAge}");

string? maybeName = null;
Console.WriteLine($"maybeName?.Length: {maybeName?.Length}"); // null-conditional: skips the call, result is null
Console.WriteLine($"maybeName ?? \"Unknown\": {maybeName ?? "Unknown"}"); // null-coalescing: fallback value

Console.WriteLine();

// ---- 9. Loops ----
Console.Write("for loop:      ");
for (var i = 1; i <= 5; i++)
{
    Console.Write($"{i} ");
}
Console.WriteLine();

Console.Write("while loop:    ");
var count = 5;
while (count > 0)
{
    Console.Write($"{count} ");
    count--;
}
Console.WriteLine();

Console.Write("do-while loop: ");
var n = 0;
do
{
    Console.Write($"{n} "); // runs at least once, even though the condition is already false
} while (n++ < 0);
Console.WriteLine();

Console.Write("foreach loop:  ");
foreach (var letter in "ABC")
{
    Console.Write($"{letter} ");
}
Console.WriteLine();

Console.WriteLine();

// ---- 10. Arrays & collections ----
var numbers = new[] { 1, 2, 3, 4, 5 };
var sum = numbers.Sum();
Console.WriteLine($"Sum of {string.Join(" + ", numbers)} = {sum}");

var fruits = new List<string> { "apple", "banana", "cherry" };
fruits.Add("date");
Console.WriteLine($"Fruits: {string.Join(", ", fruits)}");

// A 2D (rectangular) array: fixed rows x columns.
int[,] grid = { { 1, 2 }, { 3, 4 } };
Console.WriteLine($"grid[1,0] = {grid[1, 0]}");

// A jagged array: an array of arrays, each row can have a different length.
int[][] jagged = { new[] { 1 }, new[] { 2, 3 }, new[] { 4, 5, 6 } };
Console.WriteLine($"jagged row lengths: {string.Join(", ", jagged.Select(row => row.Length))}");

Console.WriteLine();

// ---- 11. Methods ----
Console.WriteLine($"Square(6) = {Square(6)}");
Console.WriteLine($"Greet(\"Bob\") = {Greet("Bob")}");
Console.WriteLine($"Greet(\"Bob\", \"Hi\") = {Greet("Bob", "Hi")}"); // overload with optional parameter
Console.WriteLine($"Add(1, 2, 3, 4) = {Add(1, 2, 3, 4)}"); // params: any number of arguments

static int Square(int n) => n * n;

static string Greet(string person, string greeting = "Hello") => $"{greeting}, {person}!";

static int Add(params int[] values) => values.Sum();

Console.WriteLine();

// ---- 12. A simple class ----
var dog = new Animal("Rex", "Dog");
dog.Describe();

class Animal
{
    public string Name { get; }
    public string Species { get; }

    public Animal(string name, string species)
    {
        Name = name;
        Species = species;
    }

    public void Describe() => Console.WriteLine($"{Name} is a {Species}.");
}

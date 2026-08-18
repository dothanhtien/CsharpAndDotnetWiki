// C# Collections - sample project
// Run: dotnet run (from this src/ folder)

// ---- 1. Arrays (recap) ----
// Fixed size, fastest possible indexed access, but can't grow or shrink.
int[] fixedScores = { 90, 85, 77 };
Console.WriteLine($"Array:      [{string.Join(", ", fixedScores)}], length={fixedScores.Length}");

Console.WriteLine();

// ---- 2. List<T> ----
// A resizable, ordered, indexable collection - the default choice for "a bunch of items".
var names = new List<string> { "Alice", "Bob" };
names.Add("Carol");
names.Insert(1, "Dave");     // insert at a specific index - shifts later elements
names.Remove("Bob");         // remove by value
Console.WriteLine($"List:       [{string.Join(", ", names)}], Contains(\"Carol\")={names.Contains("Carol")}");
Console.WriteLine($"List[0]:    {names[0]} (indexed access, like an array)");

Console.WriteLine();

// ---- 3. Dictionary<TKey, TValue> ----
// Key -> value lookups in (amortized) O(1), unordered.
var ages = new Dictionary<string, int>
{
    ["Alice"] = 30,
    ["Bob"] = 25,
};
ages["Carol"] = 28;                          // add or overwrite by key
ages["Alice"] = 31;                          // overwrite an existing key

if (ages.TryGetValue("Bob", out var bobAge)) // never throws, unlike ages["Missing"]
{
    Console.WriteLine($"Dictionary: Bob is {bobAge}");
}

foreach (var (personName, personAge) in ages)
{
    Console.WriteLine($"  {personName} -> {personAge}");
}

Console.WriteLine();

// ---- 4. HashSet<T> ----
// Unique values, (amortized) O(1) Contains - no duplicates, no guaranteed order.
var uniqueTags = new HashSet<string> { "csharp", "dotnet" };
var addedNew = uniqueTags.Add("csharp"); // already present - returns false, set unchanged
Console.WriteLine($"HashSet:    [{string.Join(", ", uniqueTags)}], adding a duplicate returned {addedNew}");

var setA = new HashSet<int> { 1, 2, 3 };
var setB = new HashSet<int> { 2, 3, 4 };
var intersection = new HashSet<int>(setA);
intersection.IntersectWith(setB);
Console.WriteLine($"Intersection of {{{string.Join(",", setA)}}} and {{{string.Join(",", setB)}}} = {{{string.Join(",", intersection)}}}");

Console.WriteLine();

// ---- 5. Queue<T> (FIFO - First In, First Out) ----
var ticketQueue = new Queue<string>();
ticketQueue.Enqueue("ticket-1");
ticketQueue.Enqueue("ticket-2");
ticketQueue.Enqueue("ticket-3");
Console.WriteLine($"Queue peek: {ticketQueue.Peek()} (doesn't remove it)");
Console.WriteLine($"Queue serves: {ticketQueue.Dequeue()}, then {ticketQueue.Dequeue()}");
Console.WriteLine($"Queue remaining: [{string.Join(", ", ticketQueue)}]");

Console.WriteLine();

// ---- 6. Stack<T> (LIFO - Last In, First Out) ----
var undoStack = new Stack<string>();
undoStack.Push("type 'Hello'");
undoStack.Push("type ' World'");
undoStack.Push("delete last word");
Console.WriteLine($"Stack peek: {undoStack.Peek()} (the most recent action)");
Console.WriteLine($"Undo: {undoStack.Pop()}, then undo: {undoStack.Pop()}");
Console.WriteLine($"Stack remaining: [{string.Join(", ", undoStack)}]");

Console.WriteLine();

// ---- 7. LinkedList<T> ----
// A doubly-linked list - O(1) insertion/removal at a known node, but O(n) indexed access.
var timeline = new LinkedList<string>();
var noon = timeline.AddLast("12:00 lunch");
timeline.AddAfter(noon, "13:00 meeting");
timeline.AddFirst("09:00 stand-up");
Console.WriteLine($"LinkedList: [{string.Join(", ", timeline)}]");

Console.WriteLine();

// ---- 8. Collection interfaces ----
// Accepting the narrowest interface a method actually needs makes it work
// with any collection - List<T>, arrays, HashSet<T>, the result of a LINQ query...
PrintAll(names);                    // List<T> implements IEnumerable<string>
PrintAll(fixedScores.Select(s => s.ToString())); // works on any IEnumerable<T>, including LINQ results

static void PrintAll(IEnumerable<string> items) => Console.WriteLine($"PrintAll:   {string.Join(" | ", items)}");

Console.WriteLine();

// ---- 9. LINQ basics ----
var products = new List<Product>
{
    new("Laptop", 1500m, Category: "Electronics"),
    new("Mouse", 25m, Category: "Electronics"),
    new("Desk", 300m, Category: "Furniture"),
    new("Chair", 150m, Category: "Furniture"),
    new("Monitor", 400m, Category: "Electronics"),
};

var electronicsOver100 = products
    .Where(p => p.Category == "Electronics" && p.Price > 100)   // filter
    .OrderBy(p => p.Price)                                       // sort ascending
    .Select(p => $"{p.Name} (${p.Price})")                       // project to a new shape
    .ToList();
Console.WriteLine($"LINQ Where/OrderBy/Select: {string.Join(", ", electronicsOver100)}");

var totalValue = products.Sum(p => p.Price);
var mostExpensive = products.OrderByDescending(p => p.Price).First();
var cheapest = products.MinBy(p => p.Price);
var anyOverThousand = products.Any(p => p.Price > 1000);
var allPositive = products.All(p => p.Price > 0);
var countByCategory = products.GroupBy(p => p.Category).Select(g => $"{g.Key}={g.Count()}");

Console.WriteLine($"Sum: {totalValue:C}, Max: {mostExpensive.Name}, Min: {cheapest?.Name}");
Console.WriteLine($"Any > 1000: {anyOverThousand}, All positive: {allPositive}");
Console.WriteLine($"Count by category: {string.Join(", ", countByCategory)}");

// Deferred execution: the query isn't run until it's enumerated (by ToList/foreach/etc).
var query = products.Where(p => LogAndCheck(p)); // nothing printed yet
Console.WriteLine("Query built, not yet run...");
var materialized = query.ToList(); // NOW the predicate runs for each item
Console.WriteLine($"Matched {materialized.Count} product(s) after enumeration.");

static bool LogAndCheck(Product p)
{
    Console.WriteLine($"  evaluating {p.Name}");
    return p.Price > 200;
}

// A record makes a concise, immutable data-holder with value-based equality -
// a natural fit for elements stored in a collection.
record Product(string Name, decimal Price, string Category);

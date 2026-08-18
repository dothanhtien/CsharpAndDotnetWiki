# C# Collections

A detailed tour of the collection types in `System.Collections.Generic`: `List<T>`, `Dictionary<TKey,TValue>`, `HashSet<T>`, `Queue<T>`, `Stack<T>`, `LinkedList<T>`, the interfaces that tie them together, and the LINQ methods used to query them. Every concept below has a runnable counterpart in [src/Program.cs](./src/Program.cs).

> This lecture builds on [01-csharp-basics](../01-csharp-basics/README.md), which introduces arrays and `List<T>` briefly. Here we go much deeper, and cover the rest of the built-in collection types.

## Goals

- Know when to reach for `List<T>` vs. an array vs. the other collection types.
- Use `Dictionary<TKey,TValue>` for key → value lookups, safely (`TryGetValue`).
- Use `HashSet<T>` for uniqueness checks and set operations (union, intersection, difference).
- Use `Queue<T>` (FIFO) and `Stack<T>` (LIFO) for order-sensitive processing.
- Know what `LinkedList<T>` trades off against `List<T>`, and when that trade is worth it.
- Understand the collection interfaces (`IEnumerable<T>`, `ICollection<T>`, `IList<T>`) and why accepting the narrowest one makes methods more reusable.
- Write LINQ queries with `Where`, `Select`, `OrderBy`, `First`/`FirstOrDefault`, `Any`/`All`, `Sum`/`Min`/`Max`, and `GroupBy`.
- Understand deferred execution - why a LINQ query doesn't run until you enumerate it.
- Compare collections by their time complexity for the operations you actually use.

## 1. Arrays - recap

An array is fixed-size and offers the fastest possible indexed access, but can't grow or shrink after creation:

```csharp
int[] fixedScores = { 90, 85, 77 };
fixedScores.Length; // 3, and it can never change
```

Arrays are the right tool when the size is genuinely fixed (a 3x3 game board, RGB values). For almost everything else - a collection whose size changes over the program's lifetime - reach for `List<T>` instead.

## 2. `List<T>`

`List<T>` is a resizable, ordered, indexable collection - the default choice for "a bunch of items of the same type."

```csharp
var names = new List<string> { "Alice", "Bob" };
names.Add("Carol");        // append at the end - O(1) amortized
names.Insert(1, "Dave");   // insert at an index - O(n), shifts later elements
names.Remove("Bob");       // remove by value - O(n), searches then shifts
names[0];                  // indexed access - O(1), like an array
names.Contains("Carol");   // O(n) - checks every element
```

Internally, `List<T>` is backed by an array that automatically resizes (doubling in size) as needed - that's why `Add` is fast on average even though the underlying array occasionally has to be reallocated and copied.

## 3. `Dictionary<TKey, TValue>`

A dictionary stores key → value pairs and gives (amortized) **O(1)** lookup, insertion, and removal by key - dramatically faster than searching a `List<T>` for a matching item when you have a natural key.

```csharp
var ages = new Dictionary<string, int>
{
    ["Alice"] = 30,
    ["Bob"] = 25,
};

ages["Carol"] = 28; // adds a new key
ages["Alice"] = 31; // overwrites the existing value for an existing key

if (ages.TryGetValue("Bob", out var bobAge))
{
    Console.WriteLine(bobAge);
}
```

Prefer `TryGetValue` over indexing (`ages["Missing"]`) whenever the key might not exist - indexing throws a `KeyNotFoundException`, while `TryGetValue` just returns `false`. A dictionary has no guaranteed iteration order; never rely on the order you get from `foreach`.

## 4. `HashSet<T>`

A set stores unique values with no duplicates and (amortized) **O(1)** `Contains` - much faster than `List<T>.Contains`, which has to scan every element.

```csharp
var uniqueTags = new HashSet<string> { "csharp", "dotnet" };
bool added = uniqueTags.Add("csharp"); // false - "csharp" was already present, set is unchanged
```

`HashSet<T>` also implements classic set algebra directly:

```csharp
var setA = new HashSet<int> { 1, 2, 3 };
var setB = new HashSet<int> { 2, 3, 4 };

var intersection = new HashSet<int>(setA);
intersection.IntersectWith(setB); // {2, 3} - in both

var union = new HashSet<int>(setA);
union.UnionWith(setB);            // {1, 2, 3, 4} - in either

var difference = new HashSet<int>(setA);
difference.ExceptWith(setB);      // {1} - in setA but not setB
```

Use a `HashSet<T>` whenever the question you're asking is "have I seen this before?" or "is this in the collection?" - it turns an O(n) scan into an O(1) lookup.

## 5. `Queue<T>` (FIFO)

A queue processes items in the order they arrived - **F**irst **I**n, **F**irst **O**ut. Think of a real line of people waiting.

```csharp
var ticketQueue = new Queue<string>();
ticketQueue.Enqueue("ticket-1");
ticketQueue.Enqueue("ticket-2");

ticketQueue.Peek();    // "ticket-1" - look at the front without removing it
ticketQueue.Dequeue(); // "ticket-1" - remove and return the front
```

Typical uses: task/job queues, breadth-first traversal, anything modeling "process in arrival order."

## 6. `Stack<T>` (LIFO)

A stack processes items in reverse order of arrival - **L**ast **I**n, **F**irst **O**ut. Think of a stack of plates.

```csharp
var undoStack = new Stack<string>();
undoStack.Push("type 'Hello'");
undoStack.Push("type ' World'");

undoStack.Peek(); // "type ' World'" - look at the top without removing it
undoStack.Pop();  // "type ' World'" - remove and return the top
```

Typical uses: undo/redo history, expression evaluation, depth-first traversal, backtracking, the call stack itself.

## 7. `LinkedList<T>`

`LinkedList<T>` is a doubly-linked list: each node knows its neighbors. Inserting or removing a node you already have a reference to is **O(1)**, regardless of where it is in the list - `List<T>.Insert`/`Remove` in the middle is O(n) because it has to shift every following element.

```csharp
var timeline = new LinkedList<string>();
var noon = timeline.AddLast("12:00 lunch");
timeline.AddAfter(noon, "13:00 meeting"); // O(1) - we already have the node
timeline.AddFirst("09:00 stand-up");
```

The trade-off: `LinkedList<T>` has **no indexed access** (`list[5]` doesn't exist - reaching the 5th node means walking 5 links, O(n)), uses more memory per element (each node stores two extra pointers), and is generally slower to iterate than `List<T>` due to poor CPU cache locality. In practice, `List<T>` is the better default almost always; reach for `LinkedList<T>` only when you specifically need frequent insertion/removal at arbitrary, already-known positions.

## 8. Collection interfaces

The generic collections all implement a shared hierarchy of interfaces:

```
IEnumerable<T>  - can be iterated with foreach (read-only, forward-only)
    ↑
ICollection<T>  - + Count, Add, Remove, Contains
    ↑
IList<T>        - + indexed access (this[int]), Insert, RemoveAt
```

`Dictionary<TKey,TValue>` and `HashSet<T>` implement `ICollection<T>` but not `IList<T>` (no meaningful index). `Queue<T>` and `Stack<T>` are deliberately narrower still - they only expose the operations that make sense for FIFO/LIFO access.

Writing a method against the **narrowest interface it actually needs** makes it work with anything that satisfies that interface - a list, an array, a set, or the result of a LINQ query:

```csharp
static void PrintAll(IEnumerable<string> items) => Console.WriteLine(string.Join(" | ", items));

PrintAll(names);                                   // a List<string>
PrintAll(fixedScores.Select(s => s.ToString()));   // a LINQ query result - also an IEnumerable<string>
```

If `PrintAll` had instead required `List<string>`, the second call wouldn't compile - a needless restriction, since the method never uses anything beyond what `IEnumerable<T>` already provides.

## 9. LINQ basics

LINQ (Language Integrated Query) adds query-style methods to any `IEnumerable<T>` - filtering, projecting, sorting, and aggregating without hand-written loops.

```csharp
record Product(string Name, decimal Price, string Category);

var products = new List<Product>
{
    new("Laptop", 1500m, "Electronics"),
    new("Mouse", 25m, "Electronics"),
    new("Desk", 300m, "Furniture"),
};

var result = products
    .Where(p => p.Category == "Electronics" && p.Price > 100)  // filter
    .OrderBy(p => p.Price)                                      // sort ascending
    .Select(p => $"{p.Name} (${p.Price})")                      // project to a new shape
    .ToList();                                                  // materialize into a List<string>
```

Common LINQ operations, grouped by purpose:

| Purpose           | Methods                                                                  |
| ----------------- | ------------------------------------------------------------------------ |
| Filter            | `Where`                                                                  |
| Project (reshape) | `Select`                                                                 |
| Sort              | `OrderBy`, `OrderByDescending`, `ThenBy`                                 |
| Pick one          | `First`, `FirstOrDefault`, `Single`, `SingleOrDefault`, `MinBy`, `MaxBy` |
| Test              | `Any`, `All`, `Contains`                                                 |
| Aggregate         | `Count`, `Sum`, `Average`, `Min`, `Max`, `Aggregate`                     |
| Group             | `GroupBy`                                                                |
| Materialize       | `ToList`, `ToArray`, `ToDictionary`, `ToHashSet`                         |

`First` throws if nothing matches; `FirstOrDefault` returns `default` (e.g. `null` for a reference type) instead - prefer `FirstOrDefault` whenever "not found" is a normal, expected outcome rather than a bug.

### Deferred execution

Most LINQ methods (`Where`, `Select`, `OrderBy`, ...) are **lazy**: building the query does not run it. The query only actually executes when you enumerate it - with `foreach`, or a materializing call like `ToList()`/`ToArray()`/`Count()`.

```csharp
var query = products.Where(p => LogAndCheck(p)); // nothing runs yet - just describes the query
Console.WriteLine("Query built, not yet run...");
var materialized = query.ToList();               // NOW the predicate runs for every element
```

This matters in practice: if you build a query once and enumerate it twice, the whole pipeline runs twice; and if the underlying collection changes between building the query and enumerating it, the second enumeration sees the changed data. Call `.ToList()` once when you need a stable snapshot.

## Choosing the right collection

| Need                                                          | Use                       | Typical complexity                                   |
| ------------------------------------------------------------- | ------------------------- | ---------------------------------------------------- |
| Ordered items, indexed access, resizable                      | `List<T>`                 | Add: O(1)\*, index: O(1), insert/remove middle: O(n) |
| Fixed-size, fastest possible indexing                         | Array (`T[]`)             | Index: O(1), can't resize                            |
| Key → value lookup                                            | `Dictionary<TKey,TValue>` | Get/Add/Remove: O(1)\*                               |
| Uniqueness / set operations                                   | `HashSet<T>`              | Contains/Add/Remove: O(1)\*                          |
| Process in arrival order                                      | `Queue<T>`                | Enqueue/Dequeue: O(1)                                |
| Process most-recent-first                                     | `Stack<T>`                | Push/Pop: O(1)                                       |
| Frequent insert/remove at known positions, no indexing needed | `LinkedList<T>`           | Insert/remove at a node: O(1), index: O(n)           |

\*O(1) for dictionaries/sets/list-append is "amortized" - occasionally an internal resize costs O(n), but averaged over many operations it works out to O(1).

## Common pitfalls

- **Using `List<T>.Contains` in a hot loop** on a large list - it's O(n) per call; if you're checking membership repeatedly, use a `HashSet<T>` instead.
- **Indexing a `Dictionary<TKey,TValue>` for a key that might not exist** - `ages["Missing"]` throws `KeyNotFoundException`; use `TryGetValue` or check `ContainsKey` first.
- **Relying on dictionary/set iteration order** - it's not guaranteed and can change; if order matters, sort explicitly or use an ordered structure like `List<T>` or `SortedDictionary<TKey,TValue>`.
- **Modifying a collection while iterating it** with `foreach` throws `InvalidOperationException` - collect the changes you want to make, then apply them after the loop (or iterate over a copy with `.ToList()`).
- **Enumerating a LINQ query multiple times** without realizing it re-runs the whole pipeline each time - call `.ToList()` once if you need to reuse the results.
- **Reaching for `LinkedList<T>` by default** because it "sounds efficient" - for most workloads `List<T>` is faster in practice due to cache locality; only use `LinkedList<T>` when you specifically need O(1) insertion/removal at already-known nodes.

## Exercises

1. Given a `List<int>` with duplicates, use a `HashSet<int>` to produce the distinct values, then compare the result to LINQ's own `.Distinct()`.
2. Build a `Dictionary<string, List<string>>` grouping a list of `(city, name)` tuples by city, without using `GroupBy` - then redo it with `GroupBy` and compare.
3. Simulate a simple print queue: `Enqueue` five jobs, `Dequeue` them one at a time, printing each as it's served.
4. Implement "balanced parentheses" checking (`"(()())"` → true, `"(()"` → false) using a `Stack<char>`.
5. Write a LINQ query over a `List<Product>` that returns the name of the cheapest product in each category, using `GroupBy` followed by `Select`.

## Running the project

```bash
cd lectures/02-csharp-collections/src
dotnet run
```

## Notes

- See [src/Program.cs](./src/Program.cs) for the full runnable sample covering every section above, in the same order.
- OOP concepts referenced here (like the `record Product`) are covered in depth in [03-csharp-oop](../03-csharp-oop/README.md).

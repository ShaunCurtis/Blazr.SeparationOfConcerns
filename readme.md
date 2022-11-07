# Building Blazor Applications

In any application it's all too easy to fall into the "Quick" solution trap.  What we forget is the "and Dirty" bit, which we soon discover as we expand the functionality and scope of what we've coded.

The right way to code it seems to complicated, so we cut corners.  Blazor is no different: many of the simple code examples do just that.  In "keeping it simple" they promote a mode of coding that doesn't stand up to the complexities of a real world application and will get you into deep trouble if you apply them in more complex applications.

The out-of-the-box template `Counter` page is a good example.  No one would code the solution I propose below in real life - it's too complex.  However, it does provide a simple requirement that we can use to demonstrate various coding techniques,  practices and patterns that are applicable in more complex scenarios.
 
Here's the standard page:

```csharp
@page "/counter"
<PageTitle>Counter</PageTitle>

<h1>Counter</h1>
<p role="status">Current count: @currentCount</p>
<button class="btn btn-primary" @onclick="IncrementCount">Click me</button>

@code {
    private int currentCount = 0;

    private void IncrementCount()
        => currentCount++;
}
```

So what's wrong?  An awful lot: there's data (`private int currentCount`), the state of that data and the presentation of that data all mixed up in a single class: the `Counter` component.  There's no "separation of concerns" and application of SOLID coding principles.

## Separate Out the Data

We can separate the data by building a Counter class - *Dco = Data Class Object*.

```csharp
public class CounterDco
{
    public int Counter { get; set; }
}
```

This doesn't solve thew statw problem.  How do we know when a value has changed?

We can do this using a separate method and making the property setter private:

```csharp
public class CounterDco
{
    public int Counter { get; private set; }

    public void IncrementCounter()
    {
        this.Counter++;
        this.NotifyStateChanged();
    }

    public void NotifyStateChanged()
    { }
}
```

Or directly in the setter:

```csharp
public class CounterDco
{
    private int _counter;
    public int Counter
    {
        get => _counter;
        set
        {
            if (_counter != value)
            {
                _counter = value;
                this.NotifyFieldChanged();
            }
        }
    }

    public void NotifyFieldChanged()
    { }
}
```

However we are only see if a field value has changed, not the actual state - has it changed from the original or the last time we save it.

To do that we to know the original state.  Step forward Record -  aka *Immutable Objects*.

If we define our data class like this:

```csharp
public record CounterDro(
    int Counter
    );
```

It's immutable, no one can change it.  And we can compare two `CounterDro` objects where `Counter` is `2` and the comparator will return true.

So, having created an immutable Counter object how do we edit it?

## Managing State

In the code above there's only one property, and it always increments, so the `FieldChanged` event can be used as a proxy for a change in state of the object.  That isn't normally the case.  Data objects have multiple properties that can change to new values or revert to original values.

There are frameworks for managing state - Fluxor works well with Blazor.  Here I'm going to demonstrate a relatively simple methodology.

### CounterState

```csharp
public class CounterState
```
A property and private field for each editable property.

```csharp
    private int _counter;
    public int Counter
    {
        get => _counter;
        set => SetAndNotifyIfChanged(ref _counter, value, "Counter");
    }
```


A field to hold the record provided in the Ctor new method and a method to load the data from the provided record.  The actual load is separated out as we'll be using it again.

```csharp
    private CounterDro BaseRecord = default!;

    public CounterState1(CounterDro record)
        => this.Load(record);

    public void Load(CounterDro record)
    {
        this.BaseRecord = record with { };
        Counter = record.Counter;
        this.NotifyStateMayHaveChanged(true);
    }
```

A method to build a record based on the current property values

```csharp
    public CounterDro AsRecord() => new(
        Counter: this.Counter
        );
```

A Property pair to get and track state.  This uses record equality checking.

```csharp
    private bool _wasDirty;
    public bool IsDirty 
        => BaseRecord?.Equals(AsRecord()) 
            ?? this.AsRecord() is not null;
```

Two events and notification methods for field and state change:

```csharp
    public event EventHandler<string>? FieldChanged;
    public event EventHandler<bool>? StateChanged;

    private void NotifyStateMayHaveChanged()
    {
        var isDirty = this.IsDirty;
        if (_wasDirty != isDirty)
        {
            _wasDirty = isDirty;
            this.StateChanged?.Invoke(this, this.IsDirty);
        }
    }
```

A method to detect change:

```csharp
    protected void SetAndNotifyIfChanged<TType>(ref TType? currentValue, TType? value, string fieldName)
    {
        if (!currentValue?.Equals(value) ?? value is not null)
        {
            currentValue = value;
            this.NotifyFieldChanged(fieldName);
            this.NotifyStateMayHaveChanged();
        }
    }
```

And two methods to update the state:

```csharp
    public void Reset()
        => this.Load(BaseRecord);

    public void Update()
        => this.Load(AsRecord());
```


### Abstracting Common Functionality

Hopefully you can see patterns that applied to all simnilar objects.

We can define a base class to hold this boilerplate code.  The class is abstract so it can't be used directly and can enforce certain methods are implemented in child classes.

```csharp
public abstract class StateBase<TRecord>
{
    protected TRecord BaseRecord = default!;

    public event EventHandler<string>? FieldChanged;
    public event EventHandler<bool>? StateChanged;

    public abstract TRecord AsRecord();
    public abstract void Reset();
    public abstract void Update();

    private bool _wasDirty;
    public bool IsDirty 
        => BaseRecord?.Equals(AsRecord()) 
            ?? this.AsRecord() is not null;

    protected void SetAndNotifyIfChanged<TType>(ref TType? currentValue, TType? value, string fieldName)
    {
        if (!currentValue?.Equals(value) ?? value is not null)
        {
            currentValue = value;
            this.NotifyFieldChanged(fieldName);
            this.NotifyStateMayHaveChanged();
        }
    }

    protected void NotifyFieldChanged(string fieldName)
        => FieldChanged?.Invoke(this, fieldName);

    protected void NotifyStateMayHaveChanged(bool force = false)
    {
        var isDirty = this.IsDirty;
        if (_wasDirty != isDirty || force)
        {
            _wasDirty = isDirty;
            this.StateChanged?.Invoke(this, this.IsDirty);
        }
    }
}
```
#### Null Coalescing and Conditional Operators

For those not fully conversant with Null coalescing, modern C# offers more concise language for dealing with null.  You don't often need to write `if (x == null) ....` anymore. 

While

```
!currentValue?.Equals(value)
```

checks if the two values are not equal, `currentValue` could be null and throw an exception.  

The `?` - the "Null conditional operator" - returns `null` if the object tested is null.  Everything to the right of the operator - `.Equals(value)` - is not evaluated.

That's solves one problem, but this is a boolean check and a return value of null with throw an error.  We therefore apply the second "Null Coalescing" operator `??`.

```
?? value is not null
```

This will return the right side of the statement (after `??`) if the left side evaluates to null.

In this case we know `currentValue` is null - we wouldn't be doing the evaluation if it wasn't - so if value is not null, it has changed and we return the result - `true`.

You will see this coding style used throughout the code.

## Data/State/UI Separation

At this point we've build objects to represent the data and state of our countere data.  We now need to separate these from the component.  For this we define a *View* service.  The *View* is responsible for data management, the component/components are responsible for the presentation and user interaction with the data.

We can define the `CounterViewService` like this.  It provides two methods to get and save the counter data for an undefined store.

```csharp
public class CounterViewService
{
    public readonly CounterState StateContext = new CounterState(new CounterDro(0));

    private readonly IDataService _counterDataService;

    public CounterViewService(IDataService counterDataService)
        => _counterDataService = counterDataService;

    public async Task GetCounterAsync()
    {
        var result = await _counterDataService.ReadAsync<CounterDro>(new RecordQueryRequest<CounterDro>("Counter"));
        this.StateContext.Load(result.Record ?? new CounterDro(0));
    }

    public async Task SaveCounterAsync()
    {
        var request = new CommandRequest<CounterDro>(
            StorageName: "Counter", 
            Record: this.StateContext.AsRecord());

        var result = await _counterDataService.SaveAsync<CounterDro>(request);
    }

    public async Task Increment()
    {
        StateContext.Counter++;
        await SaveCounterAsync();
    }
}
```

The `IDataService` is defined as follows.  We'll look at the actual implementation shortly.

```Csharp
public interface IDataService
{
    public ValueTask<CommandResult> SaveAsync<TRecord>(CommandRequest<TRecord> request);
    public ValueTask<RecordQueryResult<TRecord>> ReadAsync<TRecord>(RecordQueryRequest<TRecord> request);
}
```

The class demonstrates some important concepts:

1. Abstraction - we separate the data persistance out though an interface.  `CounterViewService` injects the `IDataService` defined in the service container.  It doesn't care if the implementation loaded is session base storage, `LocalStorage`, a SQL database or a remote store.
2. CQS - Command/Query Separation.  Operatons are either:
     1. *Commands* - that change state - defined by a *CommandRequests* that return a simple status response in a *CommandResult*.
    2. *Queries* -that get data but don't change state - defined by a *QueryRequest* and return data and status in a *QueryResult*.
    
The request and result objects are records. 



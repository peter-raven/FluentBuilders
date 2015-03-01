Build Buddy
===========

Build Buddy is a lightweight library for creating [Test Data Builders](http://www.natpryce.com/articles/000714.html) to be used in your tests.

By using a BuildBuddy-backed Test Data Builder inside your test, you will be able to make setups like this:

```csharp
Product product = new ProductBuilder()
    .WithName("My product")
    .WithCategory("My category")
    .WithPrice(100)
    .Create();
```

A builder uses a base class from the BuildBuddy library and can take advantage of a few convenience helpers. You will be able to easily create advanced builders that handle complex types, child collections and nesting like this:

```csharp
Order order = new OrderBuilder()
    .WithOrderLines(ol => ol.AddMany(3))
    .WithCustomer(c => c
        .WithCountry("Denmark")
        .WithType("B2B"))
    .SubmittedToOrderProcessingService()
    .Persisted()
    .Create();
```

The real beauty of test data builders is that by using the right conventions, you will get very focused test setups that are easy to write and easy to read.

```csharp
// Automatically creates parent order, associated product, etc.
OrderLine orderLine = new OrderLineBuilder()
    .WithAmount(100)
    .Create();

// ... Now test something related to amount being above 99.
// The builder ensures that everything else on the order line
// is set up correctly so it doesn't disturb your test.
```  

##Getting started

To start using BuildBudy, fork this repository or use nuget:

    install-package buildbuddy

##Test Data Building Approaches

For Test Data Builders to work efficiently, they should always follow one basic rule:

**A builder used in it's simplest form must always return a unique and valid object**.

For example a ProductBuilder used like this:

```csharp
Product product = new ProductBuilder().Create();
```
Should return a Product with valid, random defaults for all it's properties.

This way you only need to opt-in with the settings of your builder that are relevant for your test.

##Creating a builder

To create a builder, inherit from the Builder base class and implement the Build method:

```csharp
public class ProductBuilder : Builder<Product>
{
    public override Product Build(int seed)
    {
        var product = new Product
        {
            Name = Generate.RandomString(20),
            Category = Generate.RandomString(15),
            Description = Generate.LoremIpsum(50)
            // ... Give valid defaults to all properties
        };
        return product;
    }
}
```

##Adding fluent syntax opt-in methods

For each property you need to control in your tests, add a method to opt-in and set this property. Use the `OptInWith` helper method, and let it return the builder itself to enable a fluent syntax for setting up your builder:

```csharp
public class ProductBuilder : Builder<Product>
{
    public ProductBuilder WithName(string name)
    {
        OptInWith(x => x.Name, name);
        return this;
    }

    public override Product Build(int seed)
    {
        var product = new Product
        {
            Name = OptInFor(x => x.Name, () => Generate.RandomString(20)),
            Category = Generate.RandomString(15),
            Description = Generate.LoremIpsum(50)
            // ... Give valid defaults to all properties
        };
        return product;
    }
}
```

Notice how the Build method uses the opt-in if present using another helper method `OptInFor`, otherwise it sets a default value. This way the valid-object principle is kept.

##Nesting builders
Notice how this builder exampe uses a nested builder to create a related entity (customer) if a specific one was not supplied (using the `BuildUsing` base method). This way if the builder was not set up with .WithCustomer, it will create a default one itself, ensuring that a valid Order object is created.

```csharp
public class OrderBuilder : Builder<Order>
{
    public OrderBuilder WithCustomer(Customer customer)
    {
        OptInWith(x => x.Customer, customer);
        return this;
    }

    public override Order Build(int seed)
    {
        var order = new Order
        {
            Customer = OptInFor(x => x.Customer, () => BuildUsing<CustomerBuilder>())
        };
        return order;
    }
}
```

##Opt-in on nested builders

Another good practice is to allow opt-in on the nested builders, so you can do:

```csharp
Order myOrder = new OrderBuilder()
    .WithCustomer(c => c.OutsideEU()) // Order should have a customer that is outside EU, everything else will be valid defaults.
    .Create();
```

To do this, add an optional Action parameter to your method:

```csharp
public class OrderBuilder : Builder<Order>
{
    public OrderBuilder WithCustomer(Action<CustomerBuilder> opt == null)
    {
        OptInWith<CustomerBuilder>(x => x.Customer, opt);
        return this;
    }

    public override Order Build(int seed)
    {
        var order = new Order
        {
            Customer = OptInFor(x => x.Customer, () => BuildUsing<CustomerBuilder>())
        };
        return order;
    }
}
```

##Handling collections
It is possible to achieve builder behavior like this:
```csharp
var myOrder = BuildUsing<OrderBuilder>
    .WithOrderLines(x => x.AddOne()) // build a default order line and add it
    .WithOrderLines(x => x.AddOne(myOrderLine)) // add my specific order line
    .WithOrderLines(x => x.AddOne(y => y.WithAmount(10))) // build an order line with the specified opt-in for the order line builder
    .Create()
```

For this you need to use the CollectionBuilder:

```csharp
public class OrderBuilder : Builder<Order>
{
    private CollectionBuilder<OrderLine, OrderLineBuilder> _orderLineBuilders;

    public OrderBuilder()
    {
        _orderLineBuilders = new CollectionBuilder<OrderLine, OrderLineBuilder>(this);
    }

    // ...

    public OrderBuilder WithOrderLines(Action<CollectionBuilder<OrderLine, OrderLineBuilder>> opts)
    {
        opts(_orderLineBuilders);
    }

    public override Order Build(int seed)
    {
        var order = new Order();
        // ...

        order.OrderLines.AddRange(_orderLineBuilders.CreateAll());
        return order;
    }
}
```

##Persistence
It is possible to support persistence, which can be useful in integration tests.

```csharp
var product = new ProductBuilder(myPersistenceSession)
    .Persisted()
    .Create() // Now the instance is in the database also :-)
```

To support persistence, inherit the from PersistingBuilder class. This extended base class will extend the build flow with persistence. It will go through these steps:

1. Create the instance by calling Build method
2. Persist the instance by calling Save method
3. Do any optional post-persist operations by invoking method PostPersist.

The persisting and post-persist parts are only invoked if the builder has the property Persist set, this can also be done by chaining .Persisted() to the fluent syntax.

The PersistingBuilder does not know how objects are persisted, it only ensures the correct flow, so you have to provide the details in the builder. Usage example (NHibernate):

```csharp
public class ProductBuilder : PersistingBuilder<Product>
{
    private ISession _session;

    public ProductBuilder(ISession session)
    {
        _session = session;
    }

    // ... Add fluent opt-in methods ...

    public override Product Build(int seed)
    {
        var product = new Product
        {
            Name = Generate.RandomString(20),
            // ... Give valid defaults to all properties
        };
        return product;
    }

    public override Save(Product product)
    {
        _session.Save(product);
        _session.Flush();
    }

    public override void PostPersist(Product product)
    {
        // Do anything that can only be done after persisting, fx. add child entities that require
        // parent entity to be persisted first.
    }
}
```

##Dependency injection
In integration tests you often want the builders to support more advanced scenarios, where part of the building could include calling services, do messaging and using persistence. Put all dependencies of your builder in it's constructor, and instantiate the builder classes through your IoC of choice. Most IoC containers support doing like this even without any registration:

```csharp
// Instantiates an OrderBuilder, and resolves any of it's constructor parameters using the IoC.
Order myOrder = MyIocOfChoice.Resolve<OrderBuilder>()
    .Create();
```

Each builder has a convention for how to create nested builders. It is controlled by the `BuilderFactoryConvention` property, which always defaults to use a `SimpleBuilderFactory` that will just create a new instance of the nested builder, and throw if the nested builder requires any parameter in it's constructor. To use your IoC of choice for nested builders, create a new convention:

```csharp
public class MyBuilderFactory : IBuilderFactory
{
    public T Create<T>() where T : IBuilder
    {
        return MyIocOfChoice.Resolve<T>();
    }
}
```

... And put it to use in your builder:

```csharp
myOrderBuilder.BuilderFactoryConvention.UseFactory(new MyBuilderCreator());
```

A typical approach is to combine all dependency injection in a common method in your test base class like this:

```csharp
public T BuildUsing<T>()
{
    T builder = MyIocOfChoice.Resolve<T>();
    builder.BuilderFactoryConvention.UseFactory(new MyBuilderCreator());
    return builder;
}
```

Now you can use `BuildUsing<SomeBuilder>()` anywhere in your tests, and have all builder instantiation controlled by your IoC.

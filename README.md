# RulesChain
[![Build Status](https://dev.azure.com/RulesChain/RulesChain/_apis/build/status/lutticoelho.RulesChain?branchName=master)](https://dev.azure.com/RulesChain/RulesChain/_build/latest?definitionId=1&branchName=master)

RulesChain is a small lib that simplify writing business rules in .NET environment. Its based on Rules Design Pattern and Chain of Responsability Pattern and works like ASP.Net Core middlewares.

## Table of Contents ##

- [How to use](#how-to-use)
    - [Create your first Rule](#create-your-first-rule)
    - [Create your first Rule Chain](#create-your-first-rule-chain)
    - [Create your first Rule Context](#create-your-first-rule-context)
    - [Execute your Rule Chain](#execute-your-rule-chain)
- [License](#license)
- [Contributing](#contributing)

## How to use ##

### Create your first Rule ###

Imagine that you are developing an e-commerce app and you have to apply automatic discounts for each order before finish the sale.

The first discount is based on user birthday. If user buy something on their birthday you should aply 10% discount on each item, but if the user already have another discount applied to the order you should keep only the biggest discount.

````

    public class BirthdayDiscountRule : Rule<ApplyDiscountContext>
    {
        public FakeBirthdayDiscountRule(Rule<ApplyDiscountContext> next) : base(next)
        {}

        public override ApplyDiscountContext Run(ApplyDiscountContext context)
        {
            // Gets 10% of discount;
            var birthDayDiscount = context.Context.Items.Sum(i => i.Price * 0.1M);
            context = _next.Invoke(context);

            // Only apply birthday disccount if the discount applied by the other rules are smaller than this
            if (birthDayDiscount > context.DiscountApplied)
                context.DiscountApplied = birthDayDiscount;

            return context;
        }

        public override bool ShouldRun(ApplyDiscountContext context)
        {
            var dayAndMonth = context.Context.ClientBirthday.ToString("ddMM");
            var todayDayAndMonth = DateTime.Now.ToString("ddMM");
            return dayAndMonth == todayDayAndMonth;
        }
    }
   
````

Another discount will be applied if this is the first order of the client. If so, client will have 5% discount. And again it will only be applied if the user didn't get another bigger discount.

````

    public class FirstOrderDiscount : Rule<ApplyDiscountContext>
    {
        public FirstOrderDiscount(Rule<ApplyDiscountContext> next) : base(next)
        {}

        public override ApplyDiscountContext Run(ApplyDiscountContext context)
        {
            // Gets 5% of discount;
            var myDiscount = context.Context.Items.Sum(i => i.Price * 0.05M);
            context = _next.Invoke(context) ?? context;

            // Only apply first order disccount if the discount applied by the other rules are smaller than this
            if (myDiscount > context.DiscountApplied)
                context.DiscountApplied = myDiscount;

            return context;
        }

        public override bool ShouldRun(ApplyDiscountContext context)
        {
            return context.Properties["IsFirstOrder"]?.ToBool() ?? false;
        }
    }

````

As you saw every rule contains two methods `ShoulRun` and `Run`. 

`ShoulRun` method is the pre condition to execute the Rule. If it returns false, the `Run` method will never run.

`Run` method is the concrete logic that will be aplied for your context. And if neccessery it will call the next Rule on the chain using the `_next.Invoke(context)` statement. 

Now that you have all your bussiness rules done, you need to put it on a chain of execution.

### Create your first Rule Chain ###

````
     var chain = new RuleChain<ApplyDiscountContext>()
                .Use<FakeBirthdayDiscountRule>()
                .Use<FirstOrderDiscount>()
                .Build();
````
When you call the build method, your chain will be created with all your rules in the same sequence that you defined on `.Use<>`

### Create your first Rule Context ###

Prepare your Rule context with the neccessery variables:

````
    public class ApplyDiscountContext : IRuleContext<ShoppingCart>
    {
        public ApplyDiscountContext()
        {
        }

        public ApplyDiscountContext(ShoppingCart shoppingCart)
        {
            Properties = new ConcurrentDictionary<string, object>();
            Context = shoppingCart;
        }

        public IDictionary<string, object> Properties { get; }

        public ShoppingCart Context { get; set; }

        public decimal DiscountApplied { get; set; }
    }
    
    var context = new ApplyDiscountContext
            {
                Context = new ShoppingCart
                {
                    CilentName = "John Doe",
                    Items = new List<ShopItem>()
                {
                    {new ShopItem() {Name = "Item 1", Price = 100}},
                    {new ShopItem() {Name = "Item 2", Price = 900}}
                },
                    ClientBirthday = new DateTime(1986, 8, 16)
                }
            };
            
    context.Properties["IsFirstOrder"] = true;
    
````

### Execute your Rule Chain ###

Now just execute the `Invoke`method of your chain. 

````

var result = chain.Invoke(context);
Console.WriteLine("Discount = {0}", result.DiscountApplied);

````


## License ##

RulesChain is Open Source software and released under the MIT license.

## Contributing ##

For more information on contributing to RulesChain please found me on LinkedIn. (It's the unique social network that I use today)


# RulesChain
![Nuget](https://img.shields.io/nuget/dt/RulesChain)![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/lutticoelho/ruleschain)![GitHub commits since latest release (by SemVer)](https://img.shields.io/github/commits-since/lutticoelho/ruleschain/latest/develop?sort=semver)[![Build Status](https://dev.azure.com/RulesChain/RulesChain/_apis/build/status/lutticoelho.RulesChain?branchName=master)](https://dev.azure.com/RulesChain/RulesChain/_build/latest?definitionId=1&branchName=master)![Azure DevOps coverage (branch)](https://img.shields.io/azure-devops/coverage/lutticoelho/ruleschain/1/develop)![GitHub issues](https://img.shields.io/github/issues/lutticoelho/ruleschain)![GitHub Repo stars](https://img.shields.io/github/stars/lutticoelho/ruleschain)![GitHub](https://img.shields.io/github/license/lutticoelho/ruleschain)

RulesChain is a small Rules Engine lib that simplify writing business rules in .NET environment. Its based on Rules Design Pattern and Chain of Responsibility Pattern and works like Asp.Net Core middlewares.


## Table of Contents ##

- [How to install](#how-to-install)
- [How to use](#how-to-use)
    - [Create your first Rule](#create-your-first-rule)
    - [Create your first Rule Chain](#create-your-first-rule-chain)
    - [Create your first Rule Context](#create-your-first-rule-context)
    - [Execute your Rule Chain](#execute-your-rule-chain)
- [License](#license)
- [Contributing](#contributing)

## How to install ##

Nuget Package Manager
> Install-Package RulesChain

.Net cli
> dotnet add package RulesChain

## How to use ##

### Create your first Rule ###

Imagine that you are developing an e-commerce app and you have to apply automatic discounts for each order before finish the sale.

The first discount is based on user birthday. If user buy something on their birthday you should apply 10% discount on each item, but if the user already have another discount applied to the order you should keep only the biggest discount.

````c#

    public class BirthdayDiscountRule : Rule<ApplyDiscountContext>
    {
        public BirthdayDiscountRule(RuleHandlerDelegate<ApplyDiscountContext> next) : base(next)
        {}

        public override async Task Run(ApplyDiscountContext context)
        {
            // Gets 10% of discount;
            var birthDayDiscount = context.Context.Items.Sum(i => i.Price * 0.1M);
            await Next(context);

            // Only apply birthday disccount if the discount applied by the other rules are smaller than this
            if (birthDayDiscount > context.DiscountApplied)
                context.DiscountApplied = birthDayDiscount;
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

````c#

    public class FirstOrderDiscount : Rule<ApplyDiscountContext>
    {
        public FirstOrderDiscount(RuleHandlerDelegate<ApplyDiscountContext> next) : base(next)
        {}

        public override async Task Run(ApplyDiscountContext context)
        {
            // Gets 5% of discount;
            var myDiscount = context.Context.Items.Sum(i => i.Price * 0.05M);
            await Next(context);

            // Only apply first order disccount if the discount applied by the other rules are smaller than this
            if (myDiscount > context.DiscountApplied)
                context.DiscountApplied = myDiscount;
        }

        public override bool ShouldRun(ApplyDiscountContext context)
        {
            return context.Properties["IsFirstOrder"]?.ToBool() ?? false;
        }
    }

````

As you saw every rule contains two methods `ShoulRun` and `Run`. 

`ShoulRun` method is the pre condition to execute the Rule. If it returns false, the `Run` method will never run.

`Run` method is the concrete logic that will be applied for your context. And if necessary it will call the next Rule on the chain using the `await Next(context)` statement. 

Now that you have all your business rules done, you need to put it on a chain of execution.

### Create your first Rule Chain ###

````C#
     var chain = new RuleChain<ApplyDiscountContext>()
                .Use<BirthdayDiscountRule>()
                .Use<FirstOrderDiscount>()
                .Build();
````
When you call the build method, your chain will be created with all your rules in the same sequence that you defined on `.Use<>`. The result of `.Build()` method is a `RuleHandlerDelegate<T>`.

### Create your first Rule Context ###

Prepare your Rule context with the necessary variables:

````c#
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

Now just call the `chain` created earlier. 

````c#

chain(context);
Console.WriteLine("Discount = {0}", context.DiscountApplied);

````


## License ##

RulesChain is Open Source software and released under the MIT license.

## Examples

[Asp.Net Rules Chain Sample](http://bit.ly/ruleschain-sample) - This project contains a sample about how to use RulesChain and how easy is to write unit tests for each Rule.

[.Net Fiddle](http://bit.ly/RulesChain-DotNetFiddle)  - If you prefer to test RulesChain without download or clone the Sample repository this fiddle contains a console application with three rules chained and you can fork it and change whatever you want and see how it affects the `ApplyDiscount` result.

## Contributing ##

If you found any bug or have any suggestion, feel free to open an issue here on GitHub and I'll be glad to help you.

For more information on contributing to RulesChain please found me on [LinkedIn](http://bit.ly/linkedin-lutticoelho). (It's the only social network that I use today)


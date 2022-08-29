using System.Globalization;
using System.Reflection;
using AutoFixture;
using AutoFixture.Kernel;
using AutoFixtureWithGrpc;
using Google.Protobuf.Collections;

var fixture = new Fixture();

fixture.Behaviors.Add(new RepeatedFieldBehavior());
var message = fixture.Create<HelloRequest>();
Console.WriteLine(message.LuckyNumbers.Count);


public class RepeatedFieldBehavior : ISpecimenBuilderTransformation
{
    public RepeatedFieldBehavior() : this(RepeatedFieldPropertiesSpecification.DefaultPropertyQuery)
    {
    }

    public RepeatedFieldBehavior(IPropertyQuery propertyQuery)
    {
        PropertyQuery = propertyQuery;
    }

    public IPropertyQuery PropertyQuery { get; }

    public ISpecimenBuilderNode Transform(ISpecimenBuilder? builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        return new Postprocessor(
            builder,
            new RepeatedFieldPropertiesCommand(PropertyQuery),
            new AndRequestSpecification(
                new RepeatedFieldPropertiesSpecification(PropertyQuery),
                new OmitFixtureSpecification()));
    }
}

public class RepeatedFieldPropertiesCommand : ISpecimenCommand
{
    public RepeatedFieldPropertiesCommand()
        : this(RepeatedFieldPropertiesSpecification.DefaultPropertyQuery)
    {
    }

    public RepeatedFieldPropertiesCommand(IPropertyQuery propertyQuery)
    {
        PropertyQuery = propertyQuery;
    }

    public IPropertyQuery PropertyQuery { get; }

    public void Execute(object specimen, ISpecimenContext context)
    {
        if (specimen == null)
        {
            throw new ArgumentNullException(nameof(specimen));
        }

        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        Type specimenType = specimen.GetType();
        foreach (PropertyInfo? pi in PropertyQuery.SelectProperties(specimenType))
        {
            IMethod? addRangeMethod = new InstanceMethodQuery(pi.GetValue(specimen), nameof(RepeatedField<object>.AddRange))
                .SelectMethods()
                .SingleOrDefault();
            if (addRangeMethod == null)
            {
                continue;
            }

            Type parameterType = addRangeMethod.Parameters.Single().ParameterType;
            Type itemsType = parameterType.GenericTypeArguments[0];
            object[] valuesToAdd = CreateMany(context, itemsType).ToArray();
            var a = Array.CreateInstance(itemsType, valuesToAdd.Length);
            for (var i = 0; i < valuesToAdd.Length; ++i)
            {
                a.SetValue(valuesToAdd[i], i);
            }

            try
            {
                addRangeMethod.Invoke(new[] { a });
            }
            catch (TargetInvocationException e)
            {
                if (e.InnerException?.GetType() == typeof(NotSupportedException))
                {
                    break;
                }

                throw;
            }
        }
    }

    private static IEnumerable<object> CreateMany(ISpecimenContext context, Type type)
    {
        return ConvertObjectType((IEnumerable<object>)context
                .Resolve(new MultipleRequest(new SeededRequest(type, GetDefaultValue(type))))
            , type);
    }

    private static object? GetDefaultValue(Type type)
    {
        return type.GetTypeInfo().IsValueType ? Activator.CreateInstance(type) : null;
    }

    private static IEnumerable<object> ConvertObjectType(IEnumerable<object> enumerable, Type type)
    {
        return enumerable.Select(v => Convert.ChangeType(v, type, CultureInfo.CurrentCulture));
    }
}

public class RepeatedFieldPropertiesSpecification : IRequestSpecification
{
    public static readonly IPropertyQuery DefaultPropertyQuery = new AndPropertyQuery(
        new ReadonlyPropertyQuery(),
        new RepeatedFieldPropertyQuery());

    public RepeatedFieldPropertiesSpecification()
        : this(DefaultPropertyQuery)
    {
    }

    public RepeatedFieldPropertiesSpecification(IPropertyQuery propertyQuery)
    {
        PropertyQuery = propertyQuery;
    }

    public IPropertyQuery PropertyQuery { get; }

    public bool IsSatisfiedBy(object request)
    {
        return request is Type requestType && PropertyQuery.SelectProperties(requestType).Any();
    }
}

public class RepeatedFieldPropertyQuery : IPropertyQuery
{
    public IEnumerable<PropertyInfo> SelectProperties(Type type)
    {
        return type.GetTypeInfo().GetProperties().Where(p => p.PropertyType.Name == typeof(RepeatedField<>).Name);
    }
}
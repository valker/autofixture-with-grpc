using AutoFixture;

var fixture = new Fixture();

fixture.Behaviors.Add(new ReadonlyCollectionPropertiesBehavior());
var message = fixture.Create<Investigation>();
Console.WriteLine(message.Ints.Count);

class Investigation
{
    private readonly List<int> _values = new();

    public List<int> Ints => _values;
}
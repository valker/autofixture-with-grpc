using AutoFixture;

var fixture = new Fixture();

var intValue = fixture.Create<int>();
Console.WriteLine(intValue);

var complexType = fixture.Create<ComplexType>();
Console.WriteLine(complexType);

var collection = fixture.Create<List<ComplexType>>();
Console.WriteLine(string.Join(", ", collection));

record ComplexType(int IntValue, string StringValue);
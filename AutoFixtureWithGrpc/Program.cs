using AutoFixture;
using AutoFixtureWithGrpc;

var fixture = new Fixture();

fixture.Behaviors.Add(new ReadonlyCollectionPropertiesBehavior());
var message = fixture.Create<HelloRequest>();
Console.WriteLine(message.LuckyNumbers.Count);
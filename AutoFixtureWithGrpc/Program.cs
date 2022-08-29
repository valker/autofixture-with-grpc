using AutoFixture;
using AutoFixtureWithGrpc;

var fixture = new Fixture();

var message = fixture.Create<HelloRequest>();
Console.WriteLine(message);
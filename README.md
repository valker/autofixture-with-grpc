# AutoFixture with gRPC
This repository provides a solution for integrating AutoFixture with gRPC in .NET. It demonstrates how to generate mock data for gRPC service methods using AutoFixture, a popular library for auto-generating test data.

## Problem Overview
When working with gRPC services in .NET, developers often need to create mock or sample data for unit tests. Manually creating such data can be time-consuming and error-prone. This repository solves the problem by leveraging the power of AutoFixture to automatically generate the necessary data for gRPC methods.

## Solution
This solution introduces a way to use AutoFixture with gRPC to automatically generate test data. The key idea is to integrate AutoFixture with the Proto-generated classes and messages to ensure that all test data is consistent and meets the structure requirements of gRPC service methods.

### Key Features:
- Automatic Data Generation: With AutoFixture, we can automatically generate random values for all required fields in gRPC messages.
- Seamless Integration: The solution provides a simple way to integrate AutoFixture with the Proto-generated classes and methods used in gRPC.
- Enhanced Unit Testing: This approach makes it easier to create unit tests that require a large number of mock gRPC calls, reducing boilerplate code and improving test coverage.

## Setup Instructions
1. Clone the repository:
```bash
git clone https://github.com/valker/autofixture-with-grpc.git
```
2. Install required NuGet packages:
- AutoFixture
- Grpc.AspNetCore
3. Run tests or use the code to generate mock data for your own gRPC service methods.

## How It Works
This solution uses AutoFixture's customization feature to integrate it with gRPC. The main component of the solution is a custom _behavior_ `RepeatedFieldBehavior` implementation that configures AutoFixture to correctly populate the Proto-generated classes.

### Example
In your unit tests, you can now easily create mock requests and responses for gRPC service methods without manually setting each field:

```csharp
[Test]
public async Task TestMyGrpcServiceMethod()
{
    var fixture = new Fixture();
    fixture.Behaviors.Add(new RepeatedFieldBehavior());

    var request = fixture.Create<MyGrpcRequest>();
    var response = await grpcService.Method(request);

    // Assertions for the response
}
```
## Contributions
Feel free to fork this repository and contribute. You can suggest improvements, report issues, or submit pull requests. All contributions are welcome!

## License
This project is licensed under the Apache  License - see the [LICENSE](LICENSE) file for details.

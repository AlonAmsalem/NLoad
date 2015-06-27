#NLoad

[![NuGet](https://img.shields.io/nuget/dt/NLoad.svg?style=flat-square)](https://www.nuget.org/packages/NLoad)
[![NuGet](https://img.shields.io/nuget/v/NLoad.svg?style=flat-square)](https://www.nuget.org/packages/NLoad)
[![NuGet Pre Release](https://img.shields.io/nuget/vpre/NLoad.svg?style=flat-square)](https://www.nuget.org/packages/NLoad)
[![AppVeyor branch](https://img.shields.io/appveyor/ci/AlonAmsalem/nload/master.svg?style=flat-square)](https://ci.appveyor.com/project/AlonAmsalem/nload/branch/master)

### What is NLoad?
NLoad is a simple and easy to use load testing framework for .NET, Intended for load testing your code and figuring out how many concurrent operations your code can handle.

NLoad is used for load testing websites, WCF services, CPU intensive algorithms or small bits of code to identify bottlenecks in your code before letting real users in.

### Usage
Implement a test class
```csharp
public class MyTest : ITest
{
  public void Initialize()
  {
    // Initialize your test, e.g., create a WCF client, load files, etc.
  }
  
  public void Execute()
  {
    // Send http request, invoke a WCF service or whatever you want to load test.
  }
}
```
Create, configure and run your load test
```csharp
var loadTest = NLoad.Test<MyTest>()
                      .WithNumberOfThreads(500)
                      .WithDurationOf(TimeSpan.FromMinutes(5))
                      .WithDeleyBetweenThreadStart(TimeSpan.FromMilliseconds(100))
                      .OnHeartbeat((s, e) => Console.WriteLine(e.Throughput))
                    .Build();

var result = loadTest.Run();
```

### Installation
To install NLoad via [NuGet](http://www.nuget.org/packages/NLoad), run the following command in the Package Manager Console
```
Install-Package NLoad
```

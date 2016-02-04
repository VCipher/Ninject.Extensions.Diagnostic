# Ninject.Extensions.Diagnostic
This Ninject extension allows to profile methods by simple appling attributes to them. This extension allows to profile both synchronous and asynchronous methods.

## Getting Started
Example (Profiling whole class):
```C#
public interface IWorker
{
    void DoSomeWork();
    void DoAnotherWork();
}

[Profile]
public class Worker : IWorker
{
    public void DoSomeWork()
    {
        // todo: do some work...
    }
    
    public void DoAnotherWork()
    {
        // todo: do another work...
    }
}

var kernel = new StandardKernel();
kernel.Bind<IWorker>().To<Worker>();

var interceptor = kernel.Get<ProfileInterceptor>();
var worker = kernel.Get<IWorker>();

worker.DoSomeWork();
worker.DoAnotherWork();

var snapshots = interceptor.Profiler.Snapshots;
```
In this example all methods will be profiled.

Example (Profiling only method):
```C#
public interface IWorker
{
    void DoSomeWork();
    void DoAnotherWork();
}

public class Worker : IWorker
{
    public void DoSomeWork()
    {
        // todo: do some work...
    }
    
    [Profile]
    public void DoAnotherWork()
    {
        // todo: do another work...
    }
}

var kernel = new StandardKernel();
kernel.Bind<IWorker>().To<Worker>();

var interceptor = kernel.Get<ProfileInterceptor>();
var worker = kernel.Get<IWorker>();

worker.DoSomeWork();
worker.DoAnotherWork();

var snapshots = interceptor.Profiler.Snapshots;
```
In this example only method `DoAnotherWork` will be profiled.

## Note
`ProfileInterceptor` has a singletone instance within the one instace of `StandardKernel`. To provide multiple profiling sessions you have to create different instaces of `StandardKernel`.

<p align="center"><img width="300px" src="http://dotnetify.net/content/images/dotnetify-logo.png"></p>

## DotNetify-Pulse

Adds an endpoint to any .NET Core service to display a customizable web view to monitor the service's log activities and resource usage in real-time. 

### How to Install

[[Link to demo project]](https://github.com/dsuryd/dotNetify-Pulse/tree/master/Demo/NetCoreService)

##### 1. Install the nuget package:
```
dotnet add package DoNetify.Pulse
```

##### 2. Configure the services and the pipeline in `Startup.cs`:
```csharp
using DotNetify;
using DotNetify.Pulse;
...

public void ConfigureServices(IServiceCollection services)
{
   services.AddSignalR();
   services.AddDotNetify();
   services.AddDotNetifyPulse();
}

public void Configure(IApplicationBuilder app)
{
   app.UseWebSockets();
   app.UseDotNetify();
   app.UseDotNetifyPulse();

   // .NET Core 2.x only:
   app.UseSignalR(config => config.MapDotNetifyHub());
   
   // .NET Core 3.x only:
   app.UseRouting();
   app.UseEndpoints(endpoints =>
   {
      endpoints.MapHub<DotNetifyHub>("/dotnetify");
   });
}
```

##### 3. Build, then open your web browser to `<service-base-url>/pulse`. You should see this page:
> Internet connection is required for loading the UI scripts on CDN.

<img src="https://github.com/dsuryd/dotNetify-Pulse/blob/master/Demo/pulse-demo.gif" />

### How to Customize

#### Overview 
Before you proceed, let's first do a bit of dive on how this thing works. 

This library uses:
- SignalR to push data from your service to the web browser.
- [DotNetify](https://dotnetify.net) to write the code using MVVM + Reactive programming.
- [DotNetify-Elements](https://dotnetify.net/elements) to provide HTML5 web components for the view.

There is a dotNetify view model in this repo named `PulseVM`. This class is the one that pushes data to the browser view, and it only does that when the page is opened.  

When it's instantiated, it will look for service objects that implements *IPulseDataProvider* and passes its own instance to the interface's `Configure` method so that service object can add properties for the data stream.  The view model then regularly checks for data updates on those properties and push them to the browser.

On the browser side, when it sends the '/pulse' HTTP request, this library's middleware intercepts it and returns `index.html`.  You can find it and other static files in your service's output directory under `pulse-ui` folder.  The HTML markup uses highly specialized web components from dotNetify-Elements to display data grid and charts and for layout.  These components are designed so that they can be configured from the server-side view mode, and to maintain connection to the data properties to auto-update, in order to achieve very minimal client-side scripting.

#### Example









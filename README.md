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
> *Internet connection is required for loading the UI scripts on CDN.*

<img src="https://github.com/dsuryd/dotNetify-Pulse/blob/master/Demo/pulse-demo.gif" />

> If you use 'dotnet publish', you have to manually copy the folder 'pulse-ui' from the build output to the publish folder.

### How to Customize

#### Overview 
Before you proceed, let's first do a bit of dive on how this thing works. 

This library uses:
- SignalR to push data from your service to the web browser.
- [DotNetify](https://dotnetify.net) to write the code using MVVM + Reactive programming.
- [DotNetify-Elements](https://dotnetify.net/elements) to provide HTML5 web components for the view.

There is a dotNetify view model in this repo named `PulseVM`. This class is the one that pushes data to the browser view, and it only does that when the page is opened.  

When it's instantiated, it will look for service objects that implements *IPulseDataProvider* and passes its own instance to the interface's `Configure` method so that service object can add properties for the data stream.  The view model then regularly checks for data updates on those properties and push them to the browser.

On the browser side, when it sends the `/pulse` HTTP request, this library's middleware intercepts it and returns `index.html`.  You can find it and other static files in your service's output directory under `pulse-ui` folder.  The HTML markup uses highly specialized web components from dotNetify-Elements to display data grid and charts and for layout.  These components are designed so that they can be configured from the server-side view model and maintain connection to the data properties for auto-update, in order to achieve minimal client-side scripting.

#### Steps

##### 1. Create your custom data provider class that implements _IPulseDataProvider_.

For example, let's create a simple clock provider:
- Use `AddProperty` to add a new observable property named "Clock" to the Pulse view model, with an initial value.
- Create a timer to emit new value every second.

```csharp
using DotNetify.Pulse;
using System.Reactive.Linq;
...
public class ClockProvider : IPulseDataProvider
{
   public IDisposable Configure(PulseVM pulseVM, out OnPushUpdate onPushUpdate)
   {
      var clockProperty = pulseVM.AddProperty<string>("Clock", DateTime.Now.ToString("hh:mm:ss"));

      onPushUpdate = _ => { };  // No op.

      return Observable
         .Interval(TimeSpan.FromSeconds(1))
         .Subscribe(_ => clockProperty.OnNext(DateTime.Now.ToString("hh:mm:ss")));
   }
}
```

##### 2. Register the provider in the startup's _ConfigureServices_.

```csharp
services.TryAddEnumerable(ServiceDescriptor.Singleton<IPulseDataProvider, ClockProvider>());
```

##### 3. Add a web component to the static HTML page and associate it with the property.

To do this, you will override the default HTML fragment file called _"section.html"_.  Notice that when you build your service, the library creates in your project a folder called _"pulse-ui"_ which contains _"section_template.html"_.  

- Copy and paste this folder to a new one and name it *_"custom-pulse-ui"_*.
- Rename _"section_template.html"_ to *_"section.html"_*.
- Right-click on _"section.html"_, select Properties, and set the "Copy to Output Directory" to *_"Copy if newer"_*.
- Edit the html file and insert the following:
```html
...
<d-frame>
   <div class="card" style="width: 200px; font-size: 40px">
      <d-element id="Clock"></d-element>
   </div>
...
```
> Read the [dotNetify-Elements documentation](https://dotnetify.net/elements) for info on all the available web components.

##### 4.  Configure the location of the custom UI folder in the startup's _Configure_.

```csharp
app.UseDotNetifyPulse(config => config.UIPath = Directory.GetCurrentDirectory() + "\\custom-pulse-ui");
```

##### 5. (Optional) Add to application settings.

If you want to pass application settings through `appsettings.json`, you can include your custom configuration in the _"DotNetifyPulse"_ configuration section, under `"Providers"`.  For example:
```json
{
  "DotNetifyPulse": {
    "Providers": {
       "ClockProvider": {
          "TimeFormat": "hh:mm:ss"
       }
    }
  }
}
```
To read the settings, inject `PulseConfiguration` type in your constructor, and use the `GetProvider` method:
```csharp
publi class ClockSettings
{
   public string TimeFormat { get; set; }
}

public ClockProvider(PulseConfiguration config)
{
   var settings = config.GetProvider<ClockSettings>("ClockProvider");
}
```







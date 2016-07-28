# RedBear.Log4View.AzureServiceBus.Target
An [NLog](http://nlog-project.org/) target for use with our [Log4View Azure Service Bus plugin](https://github.com/RedBearSys/RedBear.Log4View.AzureServiceBus).

[![Build status](https://ci.appveyor.com/api/projects/status/e7xpli3kt6dyom7n/branch/master?svg=true)](https://ci.appveyor.com/project/redbear/redbear-log4view-azureservicebus-target/branch/master)

Install via NuGet:

```
Install-Package RedBear.Log4View.AzureServiceBus.Target
```

Then update your config file as follows taking care to include your Service Bus connection string and topic name:

```xml
  <configSections>
     <section name="nlog" type="NLog.Config.ConfigSectionHandler, NLog"/>
  </configSections>
  <nlog>
    <extensions>
      <add assembly="RedBear.Log4View.AzureServiceBus.Target"/>
    </extensions>
    <targets>
      <target name="buffer" type="BufferingWrapper" bufferSize="100" flushTimeout="1000">
        <target name="l4v" type="Log4ViewTarget" ConnectionString="Endpoint=sb://xxxx.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=xxxx" Topic="l4v" />
      </target>
    </targets>
    <rules>
      <logger name="*" minLevel="Trace" writeTo="buffer"/>
    </rules>
  </nlog>
  ```

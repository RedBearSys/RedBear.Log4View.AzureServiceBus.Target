# RedBear.Log4View.AzureServiceBus.Target
An NLog target for use with our Log4View Azure Service Bus plugin.

Install via NuGet:

```
Install-Package RedBear.Log4View.AzureServiceBus.Target
```

Then update your config file as follows:

```xml
<configSections>
	<section name="nlog" type="NLog.Config.ConfigSectionHandler, NLog"/>
</configSections>
<nlog autoReload="true">
	<extensions>
		<add assembly="RedBear.Log4View.AzureServiceBus.Target" />
	</extensions>
	<targets>
		<target name="l4v" type="Log4ViewTarget" ConnectionString="Endpoint=sb://xxxx.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=xxxx" Topic="l4v" />
	</targets>
	<rules>
		<logger name="*" minLevel="Trace" appendTo="l4v"/>
	</rules>
  </nlog>
```

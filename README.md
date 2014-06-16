#asynclog4net

### Description
This is a simple log4net async appender to maximize throughput of the application logger

You can download the Nuget package from [here](https://www.nuget.org/packages/AsyncLog4net/)

### Usage

Using the names from the
[log4net example page](http://logging.apache.org/log4net/release/config-examples.html)

Add for example an AdoNetAppender and an AdoNetAppender_Access in your app.config or web.config as usual and also add a new one

    <appender name="MyAdoNetAppender_Access" type="log4net.Appender.AdoNetAppender">
    ...
    </appender>   
    
    <appender name="MyAdoNetAppender" type="log4net.Appender.AdoNetAppender">
    ...
    </appender>   


Then add your asynchronous appender as follows
	
    <appender name="asyncForwarder" type="AsyncLog4net.AsyncForwardingAppender, AsyncLog4net">
    	<param name="WaitForAll" value="true"/>
    	<appender-ref ref="MyAdoNetAppender"/>
    	<appender-ref ref="MyAdoNetAppender_Access"/>
    </appender>   

And declare it as your root logger 
    <root>
        <level value="ALL" />
        <appender-ref ref="asyncForwarder" />          
    </root>

and get it

    log4net.Config.XmlConfigurator.Configure();
    var asyncLogger = LogManager.GetLogger(typeof(Logger));
	
Or name it 

    <logger name="asyncLogger">
        <level value="ALL" />
        <appender-ref ref="asyncForwarder" />
    </logger>

and call it from the app	

log4net.Config.XmlConfigurator.Configure();
var asyncLogger = LogManager.GetLogger("asyncLogger");	
	

Remember you can use the appSettings key to see how log4net configures it in case you need it

    <add key="log4net.Internal.Debug" value="true"/>


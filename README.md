#asynclog4net

### Description
This is a simple log4net async appender to maximize throughput of the application logger

### Usage

Using the names from 
[a link](http://logging.apache.org/log4net/release/config-examples.html)

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

You can use the appSettings key to see how log4net configures it in case you need it

    <add key="log4net.Internal.Debug" value="true"/>


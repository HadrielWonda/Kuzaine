﻿using Helpers;



namespace Kuzaine.Domain;

public class BrokerSettings
{
    public string Host { get; set; } = "localhost";
    public string VirtualHost { get; set; } = "/";
    public string Username { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    
    private int _brokerPort = KuzaineUtilities.GetFreePort();
    public int? BrokerPort
    {
        get => _brokerPort;
        set => _brokerPort = value ?? _brokerPort;
    }
    
    private int _uiPort = KuzaineUtilities.GetFreePort();
    public int? UiPort
    {
        get => _uiPort;
        set => _uiPort = value ?? _uiPort;
    }
}

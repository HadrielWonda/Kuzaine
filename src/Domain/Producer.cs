﻿using Enums;



namespace Kuzaine.Domain;

public class Producer
{
    public string EndpointRegistrationMethodName { get; set; }

    public string ProducerName { get; set; }

    public string ExchangeName { get; set; }

    public string MessageName { get; set; }

    /// <summary>
    /// The directory we want to put this in in the domain. Generally the plural of an entity.
    /// </summary>
    public string DomainDirectory { get; set; }

    private ExchangeTypeEnum _exchangeType { get; set; }
    public string ExchangeType
    {
        get => _exchangeType.Name;
        set
        {
            if (!ExchangeTypeEnum.TryFromName(value, true, out var parsed))
            {
                _exchangeType = ExchangeTypeEnum.Fanout;
                return;
            }

            _exchangeType = parsed;
        }
    }

    public bool UsesDb { get; set; } = true;
}

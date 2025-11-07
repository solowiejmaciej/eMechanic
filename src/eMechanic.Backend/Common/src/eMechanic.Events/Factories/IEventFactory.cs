namespace eMechanic.Events.Factories;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using Events;
using Microsoft.Extensions.Logging;

public interface IEventFactory
{
    EventFactoryResult? Create(string eventTypeName, string payload);
}

public class EventFactory : IEventFactory
{
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<EventFactory> _logger;

    private readonly ReadOnlyDictionary<string, Type> _eventMap;

    public EventFactory(ILogger<EventFactory> logger)
    {
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        _eventMap = BuildEventMap();
    }

    private ReadOnlyDictionary<string, Type> BuildEventMap()
    {
        var eventAssembly = typeof(IEvent).Assembly;

        var eventTypes = eventAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(IEvent).IsAssignableFrom(t));

        var dictionary = new Dictionary<string, Type>();
        foreach (var type in eventTypes)
        {
            if (dictionary.TryGetValue(type.Name, out var value))
            {
                _logger.LogWarning("Duplicate event type name found: {EventTypeName}. Using type {ExistingType}. Skipping {SkippedType}.", type.Name, value.FullName, type.FullName);
            }
            else
            {
                dictionary.Add(type.Name, type);
            }
        }

        return new ReadOnlyDictionary<string, Type>(dictionary);
    }

    public EventFactoryResult? Create(string eventTypeName, string payload)
    {
        if (!_eventMap.TryGetValue(eventTypeName, out var eventType))
        {
            _logger.LogWarning("Unknown event type: {EventType}. It is not a recognized IEvent in assembly {AssemblyName}.",
                eventTypeName, typeof(IEvent).Assembly.GetName().Name);
            return null;
        }

        try
        {
            var eventObject = JsonSerializer.Deserialize(payload, eventType, _jsonOptions);
            if (eventObject is null)
            {
                return null;
            }

            return new EventFactoryResult(eventObject as IEvent, eventType);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize payload for event type {EventType}. Payload: {Payload}",
                eventTypeName, payload);
            return null;
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "An unexpected error occurred during event creation for type {EventType}.", eventTypeName);
            return null;
        }
    }
}

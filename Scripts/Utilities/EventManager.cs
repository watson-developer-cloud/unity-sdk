/**
* Copyright 2015 IBM Corp. All Rights Reserved.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
*      http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

using IBM.Watson.DeveloperCloud.Logging;
using System;
using System.Collections;
using System.Collections.Generic;

namespace IBM.Watson.DeveloperCloud.Utilities
{
  /// <summary>
  /// Singleton class for sending and receiving events.
  /// </summary>
  public class EventManager
  {
    #region Public Properties
    /// <summary>
    /// Returns the singleton event manager instance.
    /// </summary>
    public static EventManager Instance { get { return Singleton<EventManager>.Instance; } }
    #endregion

    #region Public Types
    /// <summary>
    /// The delegate for an event receiver.
    /// </summary>
    /// <param name="args">The arguments passed into SendEvent().</param>
    public delegate void OnReceiveEvent(object[] args);
    #endregion

    #region Public Functions
    /// <summary>
    /// Register an event receiver with this EventManager.
    /// </summary>
    /// <param name="eventName">The name of the event.</param>
    /// <param name="callback">The event receiver function.</param>
    public void RegisterEventReceiver(string eventName, OnReceiveEvent callback)
    {
      if (!m_EventMap.ContainsKey(eventName))
        m_EventMap.Add(eventName, new List<OnReceiveEvent>() { callback });
      else
        m_EventMap[eventName].Add(callback);
    }

    /// <summary>
    /// Unregisters all event receivers.
    /// </summary>
    public void UnregisterAllEventReceivers()
    {
      m_EventMap.Clear();
    }

    /// <summary>
    /// Unregister all event receivers for a given event.
    /// </summary>
    /// <param name="eventName">Name of the event to unregister.</param>
    public void UnregisterEventReceivers(string eventName)
    {
      m_EventMap.Remove(eventName);
    }

    /// <summary>
    /// Unregister a specific receiver.
    /// </summary>
    /// <param name="eventName">Name of the event.</param>
    /// <param name="callback">The event handler.</param>
    public void UnregisterEventReceiver(string eventName, OnReceiveEvent callback)
    {
      if (m_EventMap.ContainsKey(eventName))
        m_EventMap[eventName].Remove(callback);
    }

    /// <summary>
    /// Send an event to all registered receivers.
    /// </summary>
    /// <param name="eventName">The name of the event to send.</param>
    /// <param name="args">Arguments to send to the event receiver.</param>
    /// <returns>Returns true if a event receiver was found for the event.</returns>
    public bool SendEvent(string eventName, params object[] args)
    {
      if (string.IsNullOrEmpty(eventName))
        throw new ArgumentNullException(eventName);

      List<OnReceiveEvent> receivers = null;
      if (m_EventMap.TryGetValue(eventName, out receivers))
      {
        for (int i = 0; i < receivers.Count; ++i)
        {
          if (receivers[i] == null)
          {
            Log.Warning("EventManager", "Removing invalid event receiver.");
            receivers.RemoveAt(i--);
            continue;
          }
          try
          {
            receivers[i](args);
          }
          catch (Exception ex)
          {
            Log.Error("EventManager", "Event Receiver Exception: {0}", ex.ToString());
          }
        }
        return true;
      }
      return false;
    }

    /// <summary>
    /// Queues an event to be sent, returns immediately.
    /// </summary>
    /// <param name="eventName">The name of the event to send.</param>
    /// <param name="args">Arguments to send to the event receiver.</param>
    public void SendEventAsync(string eventName, params object[] args)
    {
      m_AsyncEvents.Enqueue(new AsyncEvent() { m_EventName = eventName, m_Args = args });
      if (m_ProcesserCount == 0)
        Runnable.Run(ProcessAsyncEvents());
    }
    #endregion

    #region Private Data
    private Dictionary<Type, Dictionary<object, string>> m_EventTypeName = new Dictionary<Type, Dictionary<object, string>>();
    private Dictionary<string, List<OnReceiveEvent>> m_EventMap = new Dictionary<string, List<OnReceiveEvent>>();

    private class AsyncEvent
    {
      public string m_EventName;
      public object[] m_Args;
    }
    private Queue<AsyncEvent> m_AsyncEvents = new Queue<AsyncEvent>();
    private int m_ProcesserCount = 0;

    private IEnumerator ProcessAsyncEvents()
    {
      m_ProcesserCount += 1;
      yield return null;

      while (m_AsyncEvents.Count > 0)
      {
        AsyncEvent send = m_AsyncEvents.Dequeue();
        SendEvent(send.m_EventName, send.m_Args);
      }

      m_ProcesserCount -= 1;
    }
    #endregion

    private void InitializeEventTypeNames(Type enumType)
    {
      m_EventTypeName[enumType] = new Dictionary<object, string>();
      foreach (var en in Enum.GetNames(enumType))
        m_EventTypeName[enumType][Enum.Parse(enumType, en)] = en;
    }
  }
}

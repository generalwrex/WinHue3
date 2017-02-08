﻿using System;
using System.Collections.Generic;
using System.Net;

namespace HueLib2
{
    public partial class Bridge
    {

        /// <summary>
        /// Change the name of the bridge.
        /// </summary>
        /// <param name="name">New name of the bridge.</param>
        /// <returns>BridgeCommResult if the operation is successfull</returns>
        public CommandResult ChangeBridgeName(string name)
        {
            CommandResult bresult = new CommandResult {Success = false};
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, Serializer.SerializeToJson<BridgeSettings>(new BridgeSettings() { name = name }));
           
            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    MessageCollection mc = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                    lastMessages = mc;
                    if(mc.FailureCount == 0)
                    {
                        bresult.Success = true;
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Resultobject = comres.data;
                    bresult.Ex = comres.ex;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.Resultobject = lastMessages;
                    bresult.Ex = comres.ex;
                    break;
            }
            bresult.Resultobject = lastMessages;
            return bresult;
        }

        /// <summary>
        /// Get the bridge Settings.
        /// </summary>
        /// <returns>The Settings of the bridge.</returns>
        public CommandResult GetBridgeSettings()
        {
            BridgeSettings bridgeSettings = new BridgeSettings();
            CommandResult bresult = new CommandResult {Success = false};
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.GET);
           
            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    bridgeSettings = Serializer.DeserializeToObject<BridgeSettings>(comres.data);
                    if (bridgeSettings != null)
                    {
                        bresult.Resultobject = bridgeSettings;
                        bresult.Success = true;
                    }
                    else
                    {
                        lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                        bresult.Resultobject = lastMessages;         
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    bresult.Resultobject = lastMessages;
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Resultobject = comres.data;
                    bresult.Ex = comres.ex;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.Resultobject = lastMessages;
                    bresult.Ex = comres.ex;
                    break;
            }
            
            return bresult;
        }

        /// <summary>
        /// Allows the user to set some configuration values.
        /// </summary>
        /// <param name="settings">Settings of the bridge.</param>
        /// <return>The new settings of the bridge.</return>
        public CommandResult SetBridgeSettings(BridgeSettings settings)
        {
            CommandResult bresult = new CommandResult {Success = false};
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.PUT, Serializer.SerializeToJson<BridgeSettings>(settings));

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    MessageCollection mc = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                    lastMessages = mc;
                    if (mc.FailureCount ==0)
                    {
                        bresult.Success = true;
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Resultobject = comres.data;
                    bresult.Ex = comres.ex;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.Resultobject = lastMessages;
                    bresult.Ex = comres.ex;
                    break;
            }
            bresult.Resultobject = lastMessages;
            return bresult;
        }

        /// <summary>
        /// Creates a new user / Register a new user. The link button on the bridge must be pressed and this command executed within 30 seconds.
        /// </summary>
        /// <returns>Contains a list with a single item that details whether the user was added successfully along with the username parameter. Note: If the requested username already exists then the response will report a success.</returns>
        /// <param name="DeviceType">Description of the type of device associated with this username. This field must contain the name of your app.</param>
        /// <return>The new API Key.</return>
        public CommandResult CreateUser(string DeviceType)
        {
            string apikey = string.Empty;
            CommandResult bresult = new CommandResult {Success = false};
            CommResult comres = Communication.SendRequest(new Uri("http://" + _ipAddress + "/api"), WebRequestType.POST, Serializer.SerializeToJson<User>(new User() { devicetype = DeviceType }));
            
            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    MessageCollection mc = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                    lastMessages = mc;
                    if (mc.FailureCount == 0)
                    {                       
                        apikey = ((Success) lastMessages[0]).Value;
                        bresult.Resultobject = apikey;
                        bresult.Success = true;
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Resultobject = lastMessages;
                    bresult.Ex = comres.ex;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.Resultobject = lastMessages;
                    bresult.Ex = comres.ex;
                    break;
            }

            return bresult;
        }

        /// <summary>
        /// Contact the bridge to register your application and generate an APIKEY.
        /// </summary>
        /// <param name="ApplicationName">Name of your application.</param>
        /// <returns>True or false the Registration has been succesfull. This will automaically populate the ApiKey with the one generated.</returns>
        public CommandResult RegisterApplication(string ApplicationName)
        {
            CommandResult bresult = new CommandResult { Success = false };
            CommResult comres = Communication.SendRequest(new Uri("http://" + _ipAddress + "/api"), WebRequestType.POST, Serializer.SerializeToJson<User>(new User() { devicetype = ApplicationName }));
  
            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    MessageCollection mc = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                    lastMessages = mc;
                    if (mc.FailureCount == 0)
                    {
                        ApiKey = ((Success) lastMessages[0]).Value;
                        bresult.Resultobject = ApiKey;
                        bresult.Success = true;
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Resultobject = lastMessages;
                    bresult.Ex = comres.ex;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.Resultobject = lastMessages;
                    bresult.Ex = comres.ex;
                    break;
            }

            return bresult;
        }

        /// <summary>
        /// Remove a user from the whitelist.
        /// </summary>
        /// <param name="username">Username to remove</param>
        /// <returns>True or false if the user has been removed.</returns>
        public CommandResult RemoveUser(string username)
        {
            
            CommandResult bresult = new CommandResult { Success = false };
            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config/whitelist/" + username), WebRequestType.DELETE);
            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    MessageCollection mc = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                    lastMessages = mc;
                    if (mc.FailureCount == 0)
                    {
                        bresult.Resultobject = true;
                        bresult.Success = true;
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Resultobject = comres.data;
                    bresult.Ex = comres.ex;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.Resultobject = lastMessages;
                    bresult.Ex = comres.ex;
                    break;
            }
            bresult.Resultobject = lastMessages;
            return bresult;
        }


        /// <summary>
        /// Get the list of users.
        /// </summary>
        /// <returns>The List of user or null on error.</returns>
        public CommandResult GetUserList()
        {
            Dictionary<string, Whitelist> list = new Dictionary<string, Whitelist>();
            CommandResult bresult = new CommandResult { Success = false };

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/config"), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    
                    BridgeSettings brs = Serializer.DeserializeToObject<BridgeSettings>(comres.data);
                    if (brs != null)
                    {
                        bresult.Success = true;
                        bresult.Resultobject = brs.whitelist;                        
                    }
                    else
                    {
                        lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                        bresult.Resultobject = lastMessages;
                    }
                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Resultobject = lastMessages;
                    bresult.Ex = comres.ex;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.Resultobject = lastMessages;
                    bresult.Ex = comres.ex;
                    break;
            }

            return bresult;
        }

        /// <summary>
        ///  Get all the timezones that are supported by the bridge.
        /// </summary>
        /// <returns>a list of all the timezones supported by the bridge.</returns>
        public CommandResult GetTimeZones()
        {
            
            CommandResult bresult = new CommandResult { Success = false };

            CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + "/info/timezones"), WebRequestType.GET);

            switch (comres.status)
            {
                case WebExceptionStatus.Success:
                    List<string> timezones = Serializer.DeserializeToObject<List<string>>(comres.data);
                    if (timezones != null)
                    {
                        bresult.Success = true;
                        bresult.Resultobject = timezones;
                    }
                    else
                    {                     
                        lastMessages = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                        bresult.Resultobject = lastMessages;
                    }

                    break;
                case WebExceptionStatus.Timeout:
                    lastMessages = new MessageCollection { _bridgeNotResponding };
                    BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                    bresult.Resultobject = lastMessages;
                    bresult.Ex = comres.ex;
                    break;
                default:
                    lastMessages = new MessageCollection { new UnkownError(comres) };
                    bresult.Resultobject = lastMessages;
                    bresult.Ex = comres.ex;
                    break;
            }

            return bresult;

        }
    }


}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HueLib2
{
    public partial class Bridge
    {

        /// <summary>
        /// Get the specified object freom the bridge.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <param name="id">Id of the object to get</param>
        /// <returns>BridgeCommResult</returns>
        public CommandResult GetObject<T>(string id) where T : HueObject
        {
            CommandResult bresult = new CommandResult() {Success = false};
            string ns = typeof(T).Namespace;

            if (ns != null)
            {
                string typename = string.Empty;
                if (typeof(T).BaseType == typeof(HueObject))
                {
                    typename = typeof(T).ToString().Replace(ns, "").Replace(".", "").ToLower() + "s";
                }
                else
                {
                    typename = typeof(T).BaseType.ToString().Replace(ns, "").Replace(".", "").ToLower() + "s";
                }

                CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $"/{typename}/{id}"),WebRequestType.GET);

                switch (comres.status)
                {
                    case WebExceptionStatus.Success:
                        MethodInfo method = typeof(Serializer).GetMethod("DeserializeToObject");
                        MethodInfo generic = method.MakeGenericMethod(typeof(T));
                        bresult.resultobject = generic.Invoke(this, new object[] { comres.data });
                        bresult.Success = bresult.resultobject != null;
                        if (bresult.resultobject == null)
                        {
                            bresult.resultobject = new MessageCollection(Serializer.DeserializeToObject<List<Message>>(comres.data));
                            lastMessages = (MessageCollection)bresult.resultobject;
                        }
                        break;
                    case WebExceptionStatus.Timeout:
                        lastMessages = new MessageCollection { _bridgeNotResponding };
                        BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() {ex = comres});
                        bresult.resultobject = comres.data;
                        bresult.ex = comres.ex;
                        break;
                    default:
                        lastMessages = new MessageCollection { new UnkownError(comres) };
                        bresult.resultobject = "Unknown Error : " + comres.data;
                        bresult.ex = comres.ex;
                        break;
                }
            }
            else
            {
                bresult.Success = false;
                bresult.resultobject = "Type of object cannot be null";
            }

            return bresult;
        }

        /// <summary>
        /// Get a list of specified objects from the bridge.
        /// </summary>
        /// <typeparam name="T">HueObject (Light,Group,Sensor,Rule,Schedule,Scene)</typeparam>
        /// <returns>BridgeCommResult</returns>
        public CommandResult GetListObjects<T>() where T : HueObject
        {
            CommandResult bresult = new CommandResult() {Success = false};
            string ns = typeof(T).Namespace;
            if (ns != null)
            {
 
                string typename = typeof(T).ToString().Replace(ns, "").Replace(".", "").ToLower() + "s";
                CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $"/{typename}"),WebRequestType.GET);

                switch (comres.status)
                {
                    case WebExceptionStatus.Success:
                        bresult.resultobject = Serializer.DeserializeToObject<Dictionary<string, T>>(comres.data);
                        bresult.Success = true;
                        break;
                    case WebExceptionStatus.Timeout:
                        lastMessages = new MessageCollection { _bridgeNotResponding };
                        BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                        bresult.resultobject = comres.data;
                        bresult.ex = comres.ex;
                        break;
                    default:
                        lastMessages = new MessageCollection { new UnkownError(comres) };
                        bresult.resultobject = comres.data;
                        bresult.ex = comres.ex;
                        break;
                }

            }
            else
            {
                bresult.resultobject = "Type of object cannot be null";
            }
            return bresult;
        }

        /// <summary>
        /// Get the newly detected lights or sensors. This will not work on other HueObject Types.
        /// </summary>
        /// <typeparam name="T">Type of the object to detect.</typeparam>
        /// <returns>BridgeCommResult</returns>
        public CommandResult GetNewObjects<T>() where T : HueObject
        {
            CommandResult bresult = new CommandResult() {Success = false};
            string ns = typeof(T).Namespace;
            if (ns != null)
            {
                string typename = typeof(T).ToString().Replace(ns, "").Replace(".", "").ToLower() + "s";
                CommResult comres = Communication.SendRequest(new Uri(BridgeUrl + $"/{typename}/new"), WebRequestType.GET);

                switch (comres.status)
                {
                    case WebExceptionStatus.Success:
                        bresult.Success = true;
                        bresult.resultobject = Serializer.DeserializeSearchResult(comres.data);
                        break;
                    case WebExceptionStatus.Timeout:
                        lastMessages = new MessageCollection { _bridgeNotResponding };
                        BridgeNotResponding?.Invoke(this, new BridgeNotRespondingEventArgs() { ex = comres });
                        bresult.resultobject = comres.data;
                        bresult.ex = comres.ex;
                        break;
                    default:
                        lastMessages = new MessageCollection { new UnkownError(comres) };
                        bresult.resultobject = comres.data;
                        bresult.ex = comres.ex;
                        break;
                }

            }
            else
            {
                bresult.resultobject = "Type of object cannot be null";
            }
            return bresult;
        }
    }
}

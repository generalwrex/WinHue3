﻿using System.ComponentModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace HueLib2
{
    /// <summary>
    /// Generic Sensor State.
    /// </summary>
    [DataContract]
    public class SensorState : RuleBody
    {
        /// <summary>
        /// LastUpdated
        /// </summary>
        [DataMember, ReadOnly(true)]
        public string lastupdated { get; set; }

        /// <summary>
        /// Convert state to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            JsonSerializerSettings jss = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, StringEscapeHandling = StringEscapeHandling.Default };
            return JsonConvert.SerializeObject(this, jss);
        }
    }
}

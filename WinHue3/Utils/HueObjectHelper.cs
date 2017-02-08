﻿using System;
using System.Collections.Generic;
using System.Linq;
using HueLib2;
using System.Windows.Media;
using WinHue3.SupportedLights;
using System.Reflection;

namespace WinHue3
{
    public static class HueObjectHelper
    {
        /// <summary>
        /// Logging 
        /// </summary>
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// CTOR
        /// </summary>
        static HueObjectHelper()
        {
            LightImageLibrary.LoadLightsImages();
        }

        /// <summary>
        /// Get a list of light with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the lights from.</param>
        /// <returns>A List fo lights.</returns>
        public static HelperResult GetBridgeLights(Bridge bridge)
        {
            if (bridge == null) return new HelperResult() { Hrobject = "Bridge cannot be NULL", Success = false };
            log.Debug($@"Getting all lights from bridge : {bridge.IpAddress}");
            CommandResult bresult = bridge.GetListObjects<Light>();
            HelperResult hr = new HelperResult
            {
                Success = bresult.Success,
                Hrobject =
                    bresult.Success
                        ? ProcessLights((Dictionary<string, Light>)bresult.Resultobject)
                        : bresult.Resultobject
            };
            log.Debug("List lights : " + Serializer.SerializeToJson(hr.Hrobject));

            return hr;
        }

        /// <summary>
        /// GEt the list of newly discovered lights
        /// </summary>
        /// <param name="bridge">Bridge to get the new lights from.</param>
        /// <returns>A list of lights.</returns>
        public static HelperResult GetBridgeNewLights(Bridge bridge)
        {
            if (bridge == null) return new HelperResult() { Hrobject = "Bridge cannot be NULL", Success = false };
            log.Debug($@"Getting new lights from bridge {bridge.IpAddress}");
            CommandResult bresult = bridge.GetNewObjects<Light>();
            HelperResult hr = new HelperResult
            {
                Success = bresult.Success,
                Hrobject =
                    bresult.Success
                        ? ProcessSearchResult(bridge, (SearchResult)bresult.Resultobject, true)
                        : bresult.Resultobject
            };
            log.Debug("Search Result : " + Serializer.SerializeToJson(hr.Hrobject));
            return hr;
        }

        /// <summary>
        /// Process the list of lights
        /// </summary>
        /// <param name="listlights">List of lights to process.</param>
        /// <returns>A list of processed lights.</returns>
        private static List<Light> ProcessLights(Dictionary<string, Light> listlights)
        {
            List<Light> newlist = new List<Light>();

            foreach (KeyValuePair<string, Light> kvp in listlights)
            {
                kvp.Value.Id = kvp.Key;

                kvp.Value.Image = GetImageForLight((bool)kvp.Value.state.reachable ? (bool)kvp.Value.state.on ? LightImageState.On : LightImageState.Off : LightImageState.Unr, kvp.Value.modelid);

                newlist.Add(kvp.Value);
            }

            return newlist;
        }

        /// <summary>
        /// Process the search results.
        /// </summary>
        /// <param name="bridge">Bridge to process the search results from</param>
        /// <param name="results">Search result to process</param>
        /// <param name="type">Type of result to process. Lights or Sensors</param>
        /// <returns>A list of objects.</returns>
        private static List<HueObject> ProcessSearchResult(Bridge bridge, SearchResult results, bool type)
        {
            List<HueObject> newlist = new List<HueObject>();
            if (type) // lights
            {
                foreach (KeyValuePair<string, string> kvp in results.listnewobjects)
                {
                    CommandResult bresult = bridge.GetObject<Light>(kvp.Key);
                    if (!bresult.Success) continue;
                    Light newlight = (Light)bresult.Resultobject;
                    newlight.Id = kvp.Key;
                    newlight.Image = GetImageForLight((bool)newlight.state.reachable ? (bool)newlight.state.on ? LightImageState.On : LightImageState.Off : LightImageState.Unr, newlight.modelid);
                    newlist.Add(newlight);
                }
            }
            else // sensors
            {
                foreach (KeyValuePair<string, string> kvp in results.listnewobjects)
                {
                    CommandResult bresult = bridge.GetObject<Sensor>(kvp.Key);
                    if (!bresult.Success) continue;
                    Sensor newSensor = (Sensor)bresult.Resultobject;
                    if (newSensor == null) continue;
                    newSensor.Id = kvp.Key;
                    switch (newSensor.type)
                    {
                        case "ZGPSwitch":
                            log.Debug("New sensor is Hue Tap.");
                            newSensor.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.huetap);
                            break;
                        case "ZLLSwitch":
                            log.Debug("New sensor is dimmer.");
                            newSensor.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.dimmer);
                            break;
                        default:
                            log.Debug("New sensor is generic.");
                            newSensor.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.sensor);
                            break;

                    }
                    newlist.Add(newSensor);
                }
            }

            return newlist;
        }

        /// <summary>
        /// Get a list of group with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the groups from.</param>
        /// <returns>A List of Group.</returns>
        public static HelperResult GetBridgeGroups(Bridge bridge)
        {
            if (bridge == null) return new HelperResult() { Success = false, Hrobject = "Bridge cannot be NULL" };
            log.Debug($@"Getting all groups from bridge {bridge.IpAddress}");
            CommandResult bresult = bridge.GetListObjects<Group>();
            HelperResult hr = new HelperResult
            {
                Success = bresult.Success,
            };

            if (hr.Success)
            {
                Dictionary<string, Group> gs = (Dictionary<string, Group>) bresult.Resultobject;
                Group zero = GetGroupZero(bridge);
                if (zero != null)
                {
                    gs.Add("0",zero);
                }

                hr.Hrobject = ProcessGroups(gs);
            }
            else
            {
                hr.Hrobject = bresult.Resultobject;
            }


            log.Debug("List groups : " + Serializer.SerializeToJson(hr.Hrobject));
            return hr;
        }

        /// <summary>
        /// Process groups
        /// </summary>
        /// <param name="listgroups">List of group t</param>
        /// <returns>A list of processed group with image and id.</returns>
        private static List<Group> ProcessGroups(Dictionary<string, Group> listgroups)
        {
            List<Group> newlist = new List<Group>();
            foreach (KeyValuePair<string, Group> kvp in listgroups)
            {
                log.Debug("Processing group : " + kvp.Value);
                kvp.Value.Id = kvp.Key;
                kvp.Value.Image = GDIManager.CreateImageSourceFromImage((bool)kvp.Value.action.on ? Properties.Resources.HueGroupOn_Large : Properties.Resources.HueGroupOff_Large);
                newlist.Add(kvp.Value);
            }

            return newlist;
        }

        /// <summary>
        /// Get a group with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the group from.</param>
        /// <param name="id">Id of the group on the bridge.</param>
        /// <returns></returns>
       /* public static HelperResult GetBridgeGroup(Bridge bridge, string id)
        {
            if (bridge == null) return new HelperResult() {Success = false, Hrobject = "Bridge cannot be NULL"};
            log.Debug($@"Getting group ID : {id} from bridge : {bridge.IpAddress}");
            CommandResult bresult = bridge.GetObject<Group>(id);
            HelperResult hr = new HelperResult() {Success = bresult.Success};
            if (bresult.Success)
            {
                Group group = (Group)bresult.Resultobject;
                group.Id = id;
                group.Image = GDIManager.CreateImageSourceFromImage((bool)group.action.on ? Properties.Resources.HueGroupOn_Large : Properties.Resources.HueGroupOff_Large);
                hr.Hrobject = group;
            }
            else
            {
                hr.Hrobject = bresult.Resultobject;
            }

            log.Debug("Group : " + hr.Hrobject);
            return hr;
        }*/

        /// <summary>
        /// Get a list of scenes with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the scenes from.</param>
        /// <returns>A List of scenes.</returns>
        public static HelperResult GetBridgeScenes(Bridge bridge)
        {
            if (bridge == null) return new HelperResult() { Success = false, Hrobject = "Bridge cannot be NULL" };
            log.Debug($@"Getting all scenes from bridge {bridge.IpAddress}");
            CommandResult bresult = bridge.GetListObjects<Scene>();
            HelperResult hr = new HelperResult
            {
                Success = bresult.Success,
                Hrobject =
                    bresult.Success
                        ? ProcessScenes((Dictionary<string, Scene>)bresult.Resultobject)
                        : bresult.Resultobject
            };

            log.Debug("List Scenes : " + Serializer.SerializeToJson(hr.Hrobject));
            return hr;
        }

        /// <summary>
        /// Process a list of scenes.
        /// </summary>
        /// <param name="listscenes">List of scenes to process.</param>
        /// <returns>A list of processed scenes.</returns>
        private static List<Scene> ProcessScenes(Dictionary<string, Scene> listscenes)
        {
            List<Scene> newlist = new List<Scene>();

            foreach (KeyValuePair<string, Scene> kvp in listscenes)
            {
                kvp.Value.Id = kvp.Key;
                log.Debug("Processing scene : " + kvp.Value);
                kvp.Value.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.SceneLarge);
                if (kvp.Value.name.Contains("HIDDEN") && !WinHueSettings.settings.ShowHiddenScenes) continue;
                newlist.Add(kvp.Value);
            }

            return newlist;
        }

        /// <summary>
        /// Get a list of schedules with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the schedules from.</param>
        /// <returns>A List of schedules.</returns>
        public static HelperResult GetBridgeSchedules(Bridge bridge)
        {
            if (bridge == null) return new HelperResult() { Success = false, Hrobject = "Bridge cannot be NULL" };
            log.Debug($@"Getting all schedules from bridge {bridge.IpAddress}");
            CommandResult bresult = bridge.GetListObjects<Schedule>();
            HelperResult hr = new HelperResult
            {
                Success = bresult.Success,
                Hrobject =
                    bresult.Success
                        ? ProcessSchedules((Dictionary<string, Schedule>)bresult.Resultobject)
                        : bresult.Resultobject
            };

            log.Debug("List Schedules : " + Serializer.SerializeToJson(hr.Hrobject));
            return hr;
        }

        /// <summary>
        /// Process a list of schedules
        /// </summary>
        /// <param name="listschedules">List of schedules to process.</param>
        /// <returns>A list of processed schedules.</returns>
        public static List<Schedule> ProcessSchedules(Dictionary<string, Schedule> listschedules)
        {
            List<Schedule> newlist = new List<Schedule>();

            foreach (KeyValuePair<string, Schedule> kvp in listschedules)
            {
                log.Debug("Assigning id to schedule ");
                kvp.Value.Id = kvp.Key;
                ImageSource imgsource;
                log.Debug("Processing schedule : " + kvp.Value);
                string Time = string.Empty;
                if (kvp.Value.localtime == null)
                {
                    log.Debug("LocalTime does not exists try to use Time instead.");
                    if (kvp.Value.time == null) continue;
                    Time = kvp.Value.time;
                }
                else
                {
                    log.Debug("Using LocalTime as schedule time.");
                    Time = kvp.Value.time;
                }

                if (Time.Contains("PT"))
                {
                    log.Debug("Schedule is type Timer.");
                    imgsource = GDIManager.CreateImageSourceFromImage(Properties.Resources.timer_clock);
                }
                else if (Time.Contains('W'))
                {
                    log.Debug("Schedule is type Alarm.");
                    imgsource = GDIManager.CreateImageSourceFromImage(Properties.Resources.stock_alarm);
                }
                else if (Time.Contains('T'))
                {
                    log.Debug("Schedule is type Schedule.");
                    imgsource = GDIManager.CreateImageSourceFromImage(Properties.Resources.SchedulesLarge);
                }
                else
                {
                    log.Debug("Schedule is unknown type.");
                    imgsource = GDIManager.CreateImageSourceFromImage(Properties.Resources.schedules);
                }

                kvp.Value.Image = imgsource;
                newlist.Add(kvp.Value);
            }

            return newlist;
        }


        /// <summary>
        /// Get a list of rules with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the rules from.</param>
        /// <returns>A List of rules.</returns>
        public static HelperResult GetBridgeRules(Bridge bridge)
        {
            if (bridge == null) return new HelperResult() { Success = false, Hrobject = "Bridge cannot be NULL" };
            log.Debug($@"Getting all rules from bridge {bridge.IpAddress}");
            CommandResult bresult = bridge.GetListObjects<Rule>();
            HelperResult hr = new HelperResult
            {
                Success = bresult.Success,
                Hrobject =
                    bresult.Success
                        ? ProcessRules((Dictionary<string, Rule>)bresult.Resultobject)
                        : bresult.Resultobject
            };

            log.Debug("List Rules : " + Serializer.SerializeToJson(hr.Hrobject));
            return hr;
        }

        /// <summary>
        /// Process a list of rules.
        /// </summary>
        /// <param name="listrules">List of rules to process.</param>
        /// <returns>A processed list of rules.</returns>
        private static List<Rule> ProcessRules(Dictionary<string, Rule> listrules)
        {
            List<Rule> newlist = new List<Rule>();

            foreach (KeyValuePair<string, Rule> kvp in listrules)
            {
                kvp.Value.Id = kvp.Key;
                log.Debug("Processing rule : " + kvp.Value);
                kvp.Value.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.rules);
                newlist.Add(kvp.Value);
            }

            return newlist;
        }

        /// <summary>
        /// Get a list of sensors with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the sensors from.</param>
        /// <returns>A List of sensors.</returns>
        public static HelperResult GetBridgeSensors(Bridge bridge)
        {
            if (bridge == null) return new HelperResult() { Success = false, Hrobject = "Bridge cannot be NULL" };
            log.Debug($@"Getting all sensors from bridge {bridge.IpAddress}");
            CommandResult bresult = bridge.GetListObjects<Sensor>();
            HelperResult hr = new HelperResult
            {
                Success = bresult.Success,
                Hrobject =
                    bresult.Success
                        ? ProcessSensors((Dictionary<string, Sensor>)bresult.Resultobject)
                        : bresult.Resultobject
            };

            log.Debug("List Sensors : " + Serializer.SerializeToJson(hr.Hrobject));
            return hr;
        }

        /// <summary>
        /// Get a list of new sensors with ID, name and image populated from the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the sensors from.</param>
        /// <returns>A List of sensors.</returns>
        public static HelperResult GetBridgeNewSensors(Bridge bridge)
        {
            if (bridge == null) return new HelperResult() { Success = false, Hrobject = "Bridge cannot be NULL" };
            log.Debug($@"Getting new sensors from bridge : {bridge.IpAddress}");
            CommandResult bresult = bridge.GetNewObjects<Sensor>();
            HelperResult hr = new HelperResult
            {
                Success = bresult.Success,
                Hrobject =
                    bresult.Success
                        ? ProcessSearchResult(bridge, (SearchResult)bresult.Resultobject, false)
                        : bresult.Resultobject
            };
            log.Debug("Search Result : " + Serializer.SerializeToJson(hr.Hrobject));
            return hr;
        }

        /// <summary>
        /// Process a list of sensors
        /// </summary>
        /// <param name="listsensors">List of sensors to process.</param>
        /// <returns>A list of processed sensors.</returns>
        private static List<Sensor> ProcessSensors(Dictionary<string, Sensor> listsensors)
        {
            List<Sensor> newlist = new List<Sensor>();

            foreach (KeyValuePair<string, Sensor> kvp in listsensors)
            {
                kvp.Value.Id = kvp.Key;
                log.Debug("Processing Sensor : " + kvp.Value);
                switch (kvp.Value.type)
                {
                    case "ZGPSwitch":
                        log.Debug("Sensor is Hue Tap.");
                        kvp.Value.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.huetap);
                        break;
                    case "ZLLSwitch":
                        log.Debug("Sensor is dimmer.");
                        kvp.Value.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.dimmer);
                        break;
                    case "ZLLPresence":
                        log.Debug("Sensor is Motion.");
                        kvp.Value.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.Motion);
                        break;
                    default:
                        log.Debug("Sensor is generic sensor.");
                        kvp.Value.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.sensor);
                        break;

                }
                newlist.Add(kvp.Value);
            }

            return newlist;
        }

        /// <summary>
        /// Get All Objects from the bridge with ID, name and image populated.
        /// </summary>
        /// <param name="bridge">Bridge to get the datastore from.</param>
        /// <returns>A List of objects.</returns>
        public static HelperResult GetBridgeDataStore(Bridge bridge)
        {
            if (bridge == null) return new HelperResult() { Success = false, Hrobject = "Bridge cannot be NULL" };
            log.Info($@"Fetching DataStore from bridge : {bridge.IpAddress}");
            CommandResult bresult = bridge.GetBridgeDataStore();
            HelperResult hr = new HelperResult
            {
                Success = bresult.Success,
                
            };
            if (hr.Success)
            {
                DataStore ds = (DataStore) bresult.Resultobject;
                Group zero = GetGroupZero(bridge);
                if (zero != null)
                {
                    ds.groups.Add("0",zero);
                }

                hr.Hrobject = ProcessDataStore(ds);
            }
            else
            {
                hr.Hrobject = bresult.Resultobject;
            }
            

            log.Debug("Bridge data store : " + Serializer.SerializeToJson(hr.Hrobject));
            return hr;
        }

        private static Group GetGroupZero(Bridge bridge)
        {
            
            CommandResult cr = bridge.GetObject<Group>("0");
            if (cr.Success)
            {
                return (Group) cr.Resultobject;
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// Process the data from the bridge datastore.
        /// </summary>
        /// <param name="datastore">Datastore to process.</param>
        /// <returns>A list of object processed.</returns>
        private static List<HueObject> ProcessDataStore(DataStore datastore)
        {
            List<HueObject> newlist = new List<HueObject>();
            log.Debug("Processing datastore...");
            newlist.AddRange(ProcessLights(datastore.lights));
            newlist.AddRange(ProcessGroups(datastore.groups));
            newlist.AddRange(ProcessSchedules(datastore.schedules));
            newlist.AddRange(ProcessScenes(datastore.scenes));
            newlist.AddRange(ProcessSensors(datastore.sensors));
            newlist.AddRange(ProcessRules(datastore.rules));
            newlist.AddRange(ProcessRessourceLinks(datastore.resourcelinks));
            log.Debug("Processing complete.");
            return newlist;
        }

        private static List<Resourcelink> ProcessRessourceLinks(Dictionary<string, Resourcelink> listrl)
        {
            if(listrl == null) return new List<Resourcelink>();
            List<Resourcelink> newlist = new List<Resourcelink>();

            foreach (KeyValuePair<string, Resourcelink> kvp in listrl)
            {
                kvp.Value.Id = kvp.Key;
                log.Debug("Processing resource links : " + kvp.Value);
                kvp.Value.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.resource);
                newlist.Add(kvp.Value);
            }
            return newlist;
        }

        /// <summary>
        /// Get the mac address of the bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the information from.</param>
        /// <returns>Returns the mac address of the bridge.</returns>
        public static HelperResult GetBridgeMac(Bridge bridge)
        {
            if (bridge == null) return new HelperResult() { Success = false, Hrobject = "The bridge cannot be NULL" };

            CommandResult bresult = bridge.GetBridgeSettings();
            HelperResult hr = new HelperResult { Success = bresult.Success };
            if (bresult.Success)
            {
                BridgeSettings brs = (BridgeSettings)bresult.Resultobject;
                log.Debug("Fetching bridge mac : " + brs.mac);
                hr.Hrobject = brs.mac;
            }
            else
            {
                hr.Hrobject = bresult.Resultobject;
            }

            return hr;
        }

        /// <summary>
        /// Check if api key is authorized withthe bridge is authorized.
        /// </summary>
        /// <param name="bridge">Bridge to get the information from.</param>
        /// <returns>Check if the bridge is authorized.</returns>
        public static HelperResult IsAuthorized(Bridge bridge)
        {
            if (bridge == null) return new HelperResult() { Success = false, Hrobject = "The bridge cannot be NULL" };
            CommandResult bresult = bridge.GetBridgeSettings();
            HelperResult hr = new HelperResult { Success = bresult.Success };
            if (bresult.Success)
            {
                BridgeSettings brs = (BridgeSettings)bresult.Resultobject;
                hr.Hrobject = brs.portalservices != null;
            }
            else
            {
                hr.Hrobject = false;
                hr.Hrobject = bresult.Resultobject;
            }
            log.Debug("Checking if bridge is authorized : " + Serializer.SerializeToJson(hr.Hrobject));
            return hr;
        }

        /// <summary>
        /// Get the Bridge Settings
        /// </summary>
        /// <param name="bridge">Bridge to get the information from.</param>
        /// <returns>The bridge settings</returns>
        public static HelperResult GetBridgeSettings(Bridge bridge)
        {
            if (bridge == null) return new HelperResult() {Success = false, Hrobject = "The bridge cannot be NULL"};
            CommandResult bresult = bridge.GetBridgeSettings();
            HelperResult hr = new HelperResult
            {
                Success = bresult.Success,
                Hrobject = bresult.Resultobject
            };
            log.Debug("Getting bridge settings : " + Serializer.SerializeToJson(hr.Hrobject));
            return hr;
        }


        /// <summary>
        /// Toggle the state of an object on and off (Light or group)
        /// </summary>
        /// <param name="bridge">Bridge to get the information from.</param>
        /// <param name="id">ID of the object to toggle.</param>
        /// <returns>The new image of the object.</returns>
        public static HelperResult ToggleObjectOnOffState(Bridge bridge, HueObject obj)
        {
            if (obj == null) return new HelperResult() { Success = false, Hrobject = "The object cannot be null" };
            if (bridge == null) return new HelperResult() { Success = false, Hrobject = "The bridge cannot be null" };
            if (!(obj is Light) && !(obj is Group)) return new HelperResult() { Success = false, Hrobject = "Object must be of type group or light" };
            HelperResult hr = new HelperResult() { Success = false, Hrobject = obj.Image };
            if (obj is Light)
            {

                CommandResult bresult = bridge.GetObject<Light>(obj.Id);
                if (bresult.Success)
                {
                    Light currentState = (Light)bresult.Resultobject;

                    if (currentState.state.reachable == false)
                    {
                        hr.Success = true;
                        hr.Hrobject = GetImageForLight(LightImageState.Unr, currentState.modelid);
                    }
                    else
                    {
                        if (currentState.state.on == true)
                        {
                            log.Debug("Toggling light state : OFF");
                            CommandResult bsetlightstate = bridge.SetState<Light>(new State() { on = false }, obj.Id);

                            if (bsetlightstate.Success)
                            {
                                hr.Success = true;
                                hr.Hrobject = GetImageForLight(LightImageState.Off, currentState.modelid);
                            }
                            else
                            {
                                hr.Hrobject = bresult.Resultobject;
                            }

                        }
                        else
                        {
                            log.Debug("Toggling light state : ON");
                            CommandResult bsetlightstate = bridge.SetState<Light>(new State() { on = true }, obj.Id);

                            if (bsetlightstate.Success)
                            {
                                hr.Success = true;
                                hr.Hrobject = GetImageForLight(LightImageState.On, currentState.modelid);
                            }
                            else
                            {
                                hr.Hrobject = bresult.Resultobject;
                            }

                        }

                    }
                }
                else
                {
                    hr.Hrobject = bresult.Resultobject;
                }

            }
            else
            {
                CommandResult bresult = bridge.GetObject<Group>(obj.Id);

                if (bresult.Success)
                {
                    Group currentstate = (Group)bresult.Resultobject;
                    if (currentstate.action.on == true)
                    {
                        log.Debug("Toggling group state : ON");
                        CommandResult bsetgroupstate = bridge.SetState<Group>(new HueLib2.Action() { on = false }, obj.Id);

                        if (bsetgroupstate.Success)
                        {
                            hr.Success = true;
                            hr.Hrobject = GDIManager.CreateImageSourceFromImage(Properties.Resources.HueGroupOff_Large);
                        }
                        else
                        {
                            hr.Hrobject = bresult.Resultobject;
                        }
                    }
                    else
                    {
                        log.Debug("Toggling group state : OFF");
                        CommandResult bsetgroupstate = bridge.SetState<Group>(new HueLib2.Action() { on = true }, obj.Id);
                        if (bsetgroupstate.Success)
                        {
                            hr.Success = true;
                            hr.Hrobject = GDIManager.CreateImageSourceFromImage(Properties.Resources.HueGroupOn_Large);
                        }
                        else
                        {
                            hr.Hrobject = bresult.Resultobject;
                        }

                    }
                }
                else
                {
                    hr.Hrobject = bresult.Resultobject;
                }

            }

            return hr;
        }

        /// <summary>
        /// Get a list of users on the selected bridge.
        /// </summary>
        /// <param name="bridge">Bridge to get the information from.</param>
        /// <returns>A list of users</returns>
        public static HelperResult GetBridgeUsers(Bridge bridge)
        {
            CommandResult bresult = bridge.GetUserList();
            HelperResult hr = new HelperResult() { Success = false };
            if (bresult.Success)
            {
                Dictionary<string, Whitelist> brlisteusers = (Dictionary<string, Whitelist>)bresult.Resultobject;
                List<Whitelist> listusers = new List<Whitelist>();
                foreach (KeyValuePair<string, Whitelist> kvp in brlisteusers)
                {
                    log.Debug($"Parsing user ID {kvp.Key}, {kvp.Value}");
                    kvp.Value.id = kvp.Key;
                    listusers.Add(kvp.Value);
                }
                hr.Hrobject = listusers;
            }
            else
            {
                hr.Hrobject = bresult.Resultobject;
            }

            return hr;
        }

        /// <summary>
        /// List of possible light state.
        /// </summary>
        public enum LightImageState { On = 0, Off = 1, Unr = 3 };

        /// <summary>
        /// Return the new image from the light
        /// </summary>
        /// <param name="imagestate">Requested state of the light.</param>
        /// <param name="modelid">model id of the light.</param>
        /// <returns>New image of the light</returns>
        public static ImageSource GetImageForLight(LightImageState imagestate, string modelid = null)
        {
            string modelID = modelid ?? "default";
            string state = string.Empty;

            switch (imagestate)
            {
                case LightImageState.Off:
                    state = "off";
                    break;
                case LightImageState.On:
                    state = "on";
                    break;
                case LightImageState.Unr:
                    state = "unr";
                    break;
                default:
                    state = "off";
                    break;
            }

            if (modelID == string.Empty)
            {
                log.Debug("STATE : " + state + " empty MODELID using default images");
                return LightImageLibrary.Images["Default"][state];
            }

            ImageSource newImage;

            if (LightImageLibrary.Images.ContainsKey(modelID))
            {
                log.Debug("STATE : " + state + " MODELID : " + modelID);
                newImage = LightImageLibrary.Images[modelID][state];

            }
            else
            {
                log.Debug("STATE : " + state + " unknown MODELID : " + modelID + " using default images.");
                newImage = LightImageLibrary.Images["Default"][state];
            }
            return newImage;
        }

        public static HelperResult GetObjectsList<T>(Bridge bridge) where T : HueObject
        {
            CommandResult bresult = bridge.GetListObjects<T>();
            HelperResult hr = new HelperResult() {Success = bresult.Success};
            
            if (bresult.Success)
            {
                if (typeof(T) == typeof(Light))
                {
                    hr.Hrobject = ProcessLights((Dictionary<string, Light>) bresult.Resultobject);
                }
                else if(typeof(T) == typeof(Group))
                {
                    hr.Hrobject = ProcessGroups((Dictionary<string, Group>)bresult.Resultobject);
                }
                else if (typeof(T) == typeof(Scene))
                {
                    hr.Hrobject = ProcessScenes((Dictionary<string, Scene>)bresult.Resultobject);
                }
                else if(typeof(T) == typeof(Schedule))
                {
                    hr.Hrobject = ProcessSchedules((Dictionary<string, Schedule>)bresult.Resultobject);
                }
                else if (typeof(T) == typeof(Sensor))
                {
                    hr.Hrobject = ProcessSensors((Dictionary<string, Sensor>)bresult.Resultobject);
                }
                else if (typeof(T) == typeof(Resourcelink))
                {
                    hr.Hrobject = ProcessRessourceLinks((Dictionary<string, Resourcelink>) bresult.Resultobject);
                }
            }
            else
            {
                hr.Hrobject = bresult.Resultobject;
            }
            return hr;
        }

        public static HelperResult GetObject<T>(Bridge bridge, string id) where T : HueObject
        {
            CommandResult bresult = bridge.GetObject<T>(id);
            HelperResult hr = new HelperResult() { Success = bresult.Success };
            if (bresult.Success)
            {
                if (typeof(T) == typeof(Light))
                {
                    Light light = (Light)bresult.Resultobject;

                    log.Debug("Light : " + light);
                    light.Id = id;
                    light.Image =
                        GetImageForLight(
                            (bool)light.state.reachable
                                ? (bool)light.state.on ? LightImageState.On : LightImageState.Off
                                : LightImageState.Unr, light.modelid);
                    hr.Hrobject = light;
                }
                else if (typeof(T) == typeof(Group))
                {
                    Group group = (Group)bresult.Resultobject;
                    log.Debug("Group : " + group);
                    group.Id = id;
                    group.Image = GDIManager.CreateImageSourceFromImage((bool)group.action.on ? Properties.Resources.HueGroupOn_Large : Properties.Resources.HueGroupOff_Large);
                    hr.Hrobject = group;
                }
                else if (typeof(T) == typeof(Sensor) || typeof(T).BaseType == typeof(Sensor))
                {
                    Sensor sensor = (Sensor)bresult.Resultobject;
                    sensor.Id = id;
                    sensor.Image = GDIManager.CreateImageSourceFromImage(sensor.type == "ZGPSwitch" ? Properties.Resources.huetap : Properties.Resources.sensor);
                    hr.Hrobject = sensor;
                }
                else if (typeof(T) == typeof(Rule))
                {
                    Rule rule = (Rule)bresult.Resultobject;
                    log.Debug("Rule : " + rule);
                    rule.Id = id;
                    rule.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.rules);
                    hr.Hrobject = rule;
                }
                else if (typeof(T) == typeof(Scene))
                {
                    Scene scene = (Scene)bresult.Resultobject;
                    log.Debug("Scene : " + scene);
                    scene.Id = id;
                    scene.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.SceneLarge);
                    hr.Hrobject = scene;
                }
                else if (typeof(T) == typeof(Schedule))
                {
                    Schedule schedule = (Schedule)bresult.Resultobject;
                    schedule.Id = id;
                    ImageSource imgsource;
                    if (schedule.localtime.Contains("PT"))
                    {
                        log.Debug("Schedule is type Timer.");
                        imgsource = GDIManager.CreateImageSourceFromImage(Properties.Resources.timer_clock);
                    }
                    else if (schedule.localtime.Contains('W'))
                    {
                        log.Debug("Schedule is type Alarm.");
                        imgsource = GDIManager.CreateImageSourceFromImage(Properties.Resources.stock_alarm);
                    }
                    else if (schedule.localtime.Contains('T'))
                    {
                        log.Debug("Schedule is type Schedule.");
                        imgsource = GDIManager.CreateImageSourceFromImage(Properties.Resources.SchedulesLarge);
                    }
                    else
                    {
                        log.Debug("Schedule is unknown type.");
                        imgsource = GDIManager.CreateImageSourceFromImage(Properties.Resources.schedules);
                    }

                    schedule.Image = imgsource;
                    hr.Hrobject = schedule;
                }
                else if (typeof(T) == typeof(Resourcelink))
                {
                    Resourcelink rl = (Resourcelink) bresult.Resultobject;
                    rl.Id = id;
                    rl.Image = GDIManager.CreateImageSourceFromImage(Properties.Resources.resource);
                    hr.Hrobject = rl;
                }
            }
            else
            {
                hr.Hrobject = bresult.Resultobject;
            }
            return hr;
        }

        public static HelperResult ExecuteGenericMethod(string MethodName, Type SelectedObjectType, object[] parameters)
        {
            MethodInfo mi = typeof(HueObjectHelper).GetMethod(MethodName);
            MethodInfo gm = mi.MakeGenericMethod(SelectedObjectType);           
            return (HelperResult)gm.Invoke(null, parameters); 
        }

    }

    public struct HelperResult
    {
        public bool Success;
        public object Hrobject;
    }
}
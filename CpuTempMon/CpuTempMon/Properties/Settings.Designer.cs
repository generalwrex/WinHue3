﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CpuTempMon.Properties {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "14.0.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("25000")]
        public double CPUTemp_gradientStartColor {
            get {
                return ((double)(this["CPUTemp_gradientStartColor"]));
            }
            set {
                this["CPUTemp_gradientStartColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("0")]
        public double CPUTemp_gradientStopColor {
            get {
                return ((double)(this["CPUTemp_gradientStopColor"]));
            }
            set {
                this["CPUTemp_gradientStopColor"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("35")]
        public double CPUTemp_gradientStartTemp {
            get {
                return ((double)(this["CPUTemp_gradientStartTemp"]));
            }
            set {
                this["CPUTemp_gradientStartTemp"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("75")]
        public double CPUTemp_gradientStopTemp {
            get {
                return ((double)(this["CPUTemp_gradientStopTemp"]));
            }
            set {
                this["CPUTemp_gradientStopTemp"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("CPU Package")]
        public string CPUTemp_SensorName {
            get {
                return ((string)(this["CPUTemp_SensorName"]));
            }
            set {
                this["CPUTemp_SensorName"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("1")]
        public string CPUTemp_ObjectID {
            get {
                return ((string)(this["CPUTemp_ObjectID"]));
            }
            set {
                this["CPUTemp_ObjectID"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("True")]
        public bool CPUTemp_ObjectType {
            get {
                return ((bool)(this["CPUTemp_ObjectType"]));
            }
            set {
                this["CPUTemp_ObjectType"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("254")]
        public byte CPUTemp_Saturation {
            get {
                return ((byte)(this["CPUTemp_Saturation"]));
            }
            set {
                this["CPUTemp_Saturation"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("254")]
        public byte CpuTemp_Brightness {
            get {
                return ((byte)(this["CpuTemp_Brightness"]));
            }
            set {
                this["CpuTemp_Brightness"] = value;
            }
        }
    }
}

﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34209
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RobotsFactory.MySQL {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "12.0.0.0")]
    internal sealed partial class ConnectionStrings : global::System.Configuration.ApplicationSettingsBase {
        
        private static ConnectionStrings defaultInstance = ((ConnectionStrings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new ConnectionStrings())));
        
        public static ConnectionStrings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [global::System.Configuration.DefaultSettingValueAttribute("server=db4free.net;userid=zealot;password=telerik;database=robotsfactory;")]
        public string MySqlConnectionString {
            get {
                return ((string)(this["MySqlConnectionString"]));
            }
            set {
                this["MySqlConnectionString"] = value;
            }
        }
    }
}

﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProView.Properties {
	
	
	[global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
	[global::System.CodeDom.Compiler.GeneratedCodeAttribute("ICSharpCode.SettingsEditor.SettingsCodeGeneratorTool", "5.0.0.4696")]
	internal sealed partial class DataGrid : global::System.Configuration.ApplicationSettingsBase {
		
		private static DataGrid defaultInstance = ((DataGrid)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new DataGrid())));
		
		public static DataGrid Default {
			get {
				return defaultInstance;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("Tahoma, 8.25pt")]
		public global::System.Drawing.Font Font {
			get {
				return ((global::System.Drawing.Font)(this["Font"]));
			}
			set {
				this["Font"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("LightYellow")]
		public global::System.Drawing.Color HighlightRowBG {
			get {
				return ((global::System.Drawing.Color)(this["HighlightRowBG"]));
			}
			set {
				this["HighlightRowBG"] = value;
			}
		}
		
		[global::System.Configuration.UserScopedSettingAttribute()]
		[global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
		[global::System.Configuration.DefaultSettingValueAttribute("ControlText")]
		public global::System.Drawing.Color HighlightRowFG {
			get {
				return ((global::System.Drawing.Color)(this["HighlightRowFG"]));
			}
			set {
				this["HighlightRowFG"] = value;
			}
		}
	}
}
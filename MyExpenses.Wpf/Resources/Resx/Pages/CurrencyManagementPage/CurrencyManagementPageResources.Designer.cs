﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyExpenses.Wpf.Resources.Resx.Pages.CurrencyManagementPage {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class CurrencyManagementPageResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CurrencyManagementPageResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MyExpenses.Wpf.Resources.Resx.Pages.CurrencyManagementPage.CurrencyManagementPage" +
                            "Resources", typeof(CurrencyManagementPageResources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occurred please retry.
        /// </summary>
        internal static string MessageBoxAddCurrencyError {
            get {
                return ResourceManager.GetString("MessageBoxAddCurrencyError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The new currency symbol has been added successfully.
        /// </summary>
        internal static string MessageBoxAddCurrencySuccess {
            get {
                return ResourceManager.GetString("MessageBoxAddCurrencySuccess", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An error occurred please retry.
        /// </summary>
        internal static string MessageBoxEditCurrencyError {
            get {
                return ResourceManager.GetString("MessageBoxEditCurrencyError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Currency was successfully edited.
        /// </summary>
        internal static string MessageBoxEditCurrencySuccess {
            get {
                return ResourceManager.GetString("MessageBoxEditCurrencySuccess", resourceCulture);
            }
        }
    }
}

﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyExpenses.Wpf.Resources.Resx.Windows.AddEditCurrencyWindow {
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
    internal class AddEditCurrencyWindowResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AddEditCurrencyWindowResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MyExpenses.Wpf.Resources.Resx.Windows.AddEditCurrencyWindow.AddEditCurrencyWindowResource" +
                            "s", typeof(AddEditCurrencyWindowResources).Assembly);
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
        ///   Looks up a localized string similar to Cancel.
        /// </summary>
        internal static string ButtonCancelContent {
            get {
                return ResourceManager.GetString("ButtonCancelContent", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Delete.
        /// </summary>
        internal static string ButtonDeleteContent {
            get {
                return ResourceManager.GetString("ButtonDeleteContent", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Valid.
        /// </summary>
        internal static string ButtonValidContent {
            get {
                return ResourceManager.GetString("ButtonValidContent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Currency symbol already exist.
        /// </summary>
        internal static string MessageBoxCurrencySymbolAlreadyExists {
            get {
                return ResourceManager.GetString("MessageBoxCurrencySymbolAlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New currency symbol :.
        /// </summary>
        internal static string TextBoxCurrencySymbol {
            get {
                return ResourceManager.GetString("TextBoxCurrencySymbol", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Do you really want to delete this currency symbol ?.
        /// </summary>
        internal static string MessageBoxDeleteQuestion {
            get {
                return ResourceManager.GetString("MessageBoxDeleteQuestion", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Currency symbol was successfully removed.
        /// </summary>
        internal static string MessageBoxDeleteCurrencyNoUseSuccess {
            get {
                return ResourceManager.GetString("MessageBoxDeleteCurrencyNoUseSuccess", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to This currency symbol is in use.Are you sure you want to delete this currency symbol and everything linked to it ?.
        /// </summary>
        internal static string MessageBoxDeleteCurrencyUseQuestion {
            get {
                return ResourceManager.GetString("MessageBoxDeleteCurrencyUseQuestion", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Currency symbol and all relative element was successfully removed.
        /// </summary>
        internal static string MessageBoxDeleteCurrencyUseSuccess {
            get {
                return ResourceManager.GetString("MessageBoxDeleteCurrencyUseSuccess", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to An error occurred please retry.
        /// </summary>
        internal static string MessageBoxDeleteCurrencyError {
            get {
                return ResourceManager.GetString("MessageBoxDeleteCurrencyError", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Currency symbol cannot be empty.
        /// </summary>
        internal static string MessageBoxCurrencySymbolCannotEmpty {
            get {
                return ResourceManager.GetString("MessageBoxCurrencySymbolCannotEmpty", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to Adding or changing account currencies.
        /// </summary>
        public static string TitleWindow {
            get {
                return ResourceManager.GetString("TitleWindow", resourceCulture);
            }
        }
    }
}

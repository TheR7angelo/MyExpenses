﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyExpenses.Wpf.Resources.Resx.Windows.AddEditCategoryTypeWindow {
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
    internal class AddEditCategoryTypeWindowResources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal AddEditCategoryTypeWindowResources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MyExpenses.Wpf.Resources.Resx.Windows.AddEditCategoryTypeWindow.AddEditCategoryTypeWindow" +
                            "Resources", typeof(AddEditCategoryTypeWindowResources).Assembly);
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
        ///   Looks up a localized string similar to Valid.
        /// </summary>
        internal static string ButtonValidContent {
            get {
                return ResourceManager.GetString("ButtonValidContent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Category already exists.
        /// </summary>
        internal static string MessageBoxCategoryAlreadyExists {
            get {
                return ResourceManager.GetString("MessageBoxCategoryAlreadyExists", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to New category :.
        /// </summary>
        internal static string TextBoxCategoryTypeName {
            get {
                return ResourceManager.GetString("TextBoxCategoryTypeName", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to color category.
        /// </summary>
        public static string ComboBoxColorValue {
            get {
                return ResourceManager.GetString("ComboBoxColorValue", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to messageBox category name cannot be empty error.
        /// </summary>
        public static string MessageBoxCategoryNameCannotBeEmptyError {
            get {
                return ResourceManager.GetString("MessageBoxCategoryNameCannotBeEmptyError", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to messageBox category color cannot be empty error.
        /// </summary>
        public static string MessageBoxCategoryColorCannotBeEmptyError {
            get {
                return ResourceManager.GetString("MessageBoxCategoryColorCannotBeEmptyError", resourceCulture);
            }
        }
    }
}

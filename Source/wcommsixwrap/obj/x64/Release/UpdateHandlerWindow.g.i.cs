﻿#pragma checksum "..\..\..\UpdateHandlerWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "9764F5258F5797831E89C93AC8F3A01203F44BBC39DC945A7B0071D8D19B227C"
//------------------------------------------------------------------------------
// <auto-generated>
//     Dieser Code wurde von einem Tool generiert.
//     Laufzeitversion:4.0.30319.42000
//
//     Änderungen an dieser Datei können falsches Verhalten verursachen und gehen verloren, wenn
//     der Code erneut generiert wird.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;
using wcommsixwrap;


namespace wcommsixwrap {
    
    
    /// <summary>
    /// UpdateHandlerWindow
    /// </summary>
    public partial class UpdateHandlerWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\..\UpdateHandlerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal wcommsixwrap.UpdateHandlerWindow winUpdateHandler;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\..\UpdateHandlerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar prgProgress;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\UpdateHandlerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Label lblHeading;
        
        #line default
        #line hidden
        
        
        #line 13 "..\..\..\UpdateHandlerWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock lblMessage;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/wcommsixwrap;component/updatehandlerwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\UpdateHandlerWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.winUpdateHandler = ((wcommsixwrap.UpdateHandlerWindow)(target));
            
            #line 8 "..\..\..\UpdateHandlerWindow.xaml"
            this.winUpdateHandler.Activated += new System.EventHandler(this.winUpdateHandler_Activated);
            
            #line default
            #line hidden
            
            #line 8 "..\..\..\UpdateHandlerWindow.xaml"
            this.winUpdateHandler.Loaded += new System.Windows.RoutedEventHandler(this.winUpdateHandler_Loaded);
            
            #line default
            #line hidden
            return;
            case 2:
            this.prgProgress = ((System.Windows.Controls.ProgressBar)(target));
            
            #line 11 "..\..\..\UpdateHandlerWindow.xaml"
            this.prgProgress.Loaded += new System.Windows.RoutedEventHandler(this.prgProgress_Loaded_1);
            
            #line default
            #line hidden
            return;
            case 3:
            this.lblHeading = ((System.Windows.Controls.Label)(target));
            return;
            case 4:
            this.lblMessage = ((System.Windows.Controls.TextBlock)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}


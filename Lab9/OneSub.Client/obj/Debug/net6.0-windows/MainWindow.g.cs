#pragma checksum "..\..\..\MainWindow.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "DC9FCA9A6DF6770A66C8D5D008847B3E7570B980"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using OneSub.Client;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
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


namespace OneSub.Client {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 15 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox CurrecncyCheckBox;
        
        #line default
        #line hidden
        
        
        #line 16 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox WeatherCheckBox;
        
        #line default
        #line hidden
        
        
        #line 17 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox StocksCheckBox;
        
        #line default
        #line hidden
        
        
        #line 19 "..\..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox MsgBox;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/OneSub.Client;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "7.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.CurrecncyCheckBox = ((System.Windows.Controls.CheckBox)(target));
            
            #line 15 "..\..\..\MainWindow.xaml"
            this.CurrecncyCheckBox.Checked += new System.Windows.RoutedEventHandler(this.CurrecncyCheckBox_Checked);
            
            #line default
            #line hidden
            
            #line 15 "..\..\..\MainWindow.xaml"
            this.CurrecncyCheckBox.Unchecked += new System.Windows.RoutedEventHandler(this.CurrecncyCheckBox_Unchecked);
            
            #line default
            #line hidden
            return;
            case 2:
            this.WeatherCheckBox = ((System.Windows.Controls.CheckBox)(target));
            
            #line 16 "..\..\..\MainWindow.xaml"
            this.WeatherCheckBox.Checked += new System.Windows.RoutedEventHandler(this.WeatherCheckBox_Checked);
            
            #line default
            #line hidden
            
            #line 16 "..\..\..\MainWindow.xaml"
            this.WeatherCheckBox.Unchecked += new System.Windows.RoutedEventHandler(this.WeatherCheckBox_Unchecked);
            
            #line default
            #line hidden
            return;
            case 3:
            this.StocksCheckBox = ((System.Windows.Controls.CheckBox)(target));
            
            #line 17 "..\..\..\MainWindow.xaml"
            this.StocksCheckBox.Checked += new System.Windows.RoutedEventHandler(this.StocksCheckBox_Checked);
            
            #line default
            #line hidden
            
            #line 17 "..\..\..\MainWindow.xaml"
            this.StocksCheckBox.Unchecked += new System.Windows.RoutedEventHandler(this.StocksCheckBox_Unchecked);
            
            #line default
            #line hidden
            return;
            case 4:
            this.MsgBox = ((System.Windows.Controls.TextBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using C4d;
using Fusee.Math;


namespace YourManagedPlugin
{
    /* Data describing your plugin
     *  1) Your plugin ID -  Remember do NOT EVER use the same ID in 2 different plugins, even for testing
     *  2) Your plugin name
     *  3) Your help text
     *  4) Your logo
     */
    [CommandPlugin(1000001,
        Name = "YourPluginName",
        HelpText = "Your plugin help text - will be displayed in 'Cinema 4D'",
        IconFile = "YourLogoPattern.tif")
    ]

    // The main class for your plugin
    class YourManagedPluginClass : CommandData
    {

        /* Your Executeable code for the plugin
         * This is your main function
         * Never change return type - this is declarated in c++ code
         */
        public override bool Execute(BaseDocument doc)
        {
            // Let's print out some greetings
            Logger.Debug("Hello, I'm the first outprinted message by your plugin code.");
            Logger.Debug("You can see me in the 'Cinema 4D' console when using the plugin.");
            Logger.Debug("Open the console by just pressing shift+F10 in 'Cinema 4D'");

            return true;
        }
    }
}

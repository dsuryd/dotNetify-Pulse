/*
Copyright 2019 Dicky Suryadi
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
    http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
 */

namespace DotNetify.Pulse
{
   public class PulseConfiguration
   {
      internal static readonly string SECTION = "DotNetifyPulse";

      // Absolute path to the folder containing UI static files.
      public string UIPath { get; set; }

      // Minimum interval between push updates in milliseconds.
      public int PushUpdateInterval { get; set; } = 100;

      public LogConfiguration Log { get; set; } = new LogConfiguration();
   }

   public class LogConfiguration
   {
      // How many last log items to cache.
      public int Buffer { get; set; } = 5;

      // Number of rows in the log data grid.
      public int Rows { get; set; } = 10;
   }
}
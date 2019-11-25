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

using System;
using Microsoft.Extensions.Configuration;

namespace DotNetify.Pulse
{
   public class PulseConfiguration
   {
      internal static readonly string SECTION = "DotNetifyPulse";

      // Absolute path to the folder containing UI static files.
      public string UIPath { get; set; }

      // Minimum interval between push updates in milliseconds.
      public int PushUpdateInterval { get; set; } = 100;

      public IConfigurationSection Providers { get; set; }

      public T GetProvider<T>(string key) where T : class => Providers?.GetSection(key).Get<T>() ?? Activator.CreateInstance<T>();
   }
}
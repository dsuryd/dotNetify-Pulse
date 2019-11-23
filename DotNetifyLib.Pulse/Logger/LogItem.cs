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

using Microsoft.Extensions.Logging;
using System;

namespace DotNetify.Pulse.Log
{
   public struct LogItem
   {
      public DateTimeOffset Time { get; }
      public string FormattedTime => Time.ToString("yyyy'-'MM'-'dd HH':'mm':'ss.fff");
      public string Level { get; }
      public string Message { get; }

      public LogItem(LogLevel level, string message)
      {
         Level = level.ToString();
         Message = message;
         Time = DateTimeOffset.Now;
      }
   }
}
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

using Microsoft.AspNetCore.Http;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace DotNetify.Pulse
{
   internal class PulseMiddleware
   {
      private static readonly string DEFAULT_UI_PATH = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\\pulse-ui";

      private readonly RequestDelegate _next;
      private readonly PulseConfiguration _config;

      public PulseMiddleware(RequestDelegate next, PulseConfiguration config)
      {
         _next = next;
         _config = config;
      }

      public async Task InvokeAsync(HttpContext httpContext)
      {
         var uiPath = (_config.UIPath ?? DEFAULT_UI_PATH).TrimEnd(new char[] { '/', '\\' });
         var requestPath = httpContext.Request.Path.ToString();

         if (requestPath.EndsWith("/pulse"))
         {
            string index = ReadFile("index.html", uiPath, DEFAULT_UI_PATH);
            string style = ReadFile("style.css", uiPath, DEFAULT_UI_PATH);
            string styleExt = ReadFile("style_ext.css", uiPath, DEFAULT_UI_PATH);
            string script = ReadFile("script.html", uiPath, DEFAULT_UI_PATH);
            string section = ReadFile("section.html", uiPath, DEFAULT_UI_PATH);
            string sectionExt = ReadFile("section_ext.html", uiPath, DEFAULT_UI_PATH);

            await httpContext.Response.WriteAsync(index
               .Replace("/*style*/", style)
               .Replace("/*style_ext*/", style)
               .Replace("<!--script-->", script)
               .Replace("<!--section-->", section)
               .Replace("<!--section_ext-->", sectionExt)
            );
         }
         else
            await _next(httpContext);
      }

      private string ReadFile(string fileName, string path, string defaultPath)
      {
         string filePath = $"{path}\\{fileName}";
         string defaultFilePath = $"{defaultPath}\\{fileName}";

         string validPath = File.Exists(filePath) ? filePath : File.Exists(defaultFilePath) ? defaultFilePath : null;
         if (validPath == null)
            return string.Empty;

         return File.ReadAllText(validPath);
      }
   }
}
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Text;
using System.IO;
using System;
using System.Text.Json;
using System.Web.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;

namespace Telematics_SG.Controllers
{
    public class TelematicsController : Controller
    {
        //public IActionResult Index()
        //{
        //    return View();
        //}

        [Microsoft.AspNetCore.Mvc.HttpPost("processFile")]
        public ActionResult ProcessFile([FromForm(Name = "filecontent")] IFormFile filecontent)
        {
            try
            {
                ActionResult res;
                
                if (filecontent == null || filecontent.Length == 0)
                {
                    //string inFilepath = @"D:\Abhijith\Telematics\Telematics - SG\1.10.txt";
                    string defaultFileContent = ResourceManager.ReadDefaultFile();
                    // byte[] defaultFileContent = System.IO.File.ReadAllBytes(inFilepath);

                    // Create a MemoryStream from the default file content
                    using (MemoryStream defaultFileStream = new MemoryStream(Encoding.UTF8.GetBytes(defaultFileContent)))
                    {
                        // Create a FormFile instance using the MemoryStream
                        filecontent = new FormFile(defaultFileStream, 0, defaultFileContent.Length, "file", "default-file.txt");
                        using (Stream stream = filecontent.OpenReadStream())
                        res = ExecutePython(stream);
                    }
                }
                else
                {
                    //using (Stream stream2 = filecontent.OpenReadStream())
                    //{
                    //    filecontent.CopyTo(stream2);
                    //    res = ExecutePython(stream2);
                    //}
                    using (Stream stream = filecontent.OpenReadStream())
                    using (Process process = new Process())
                    {
                        //filecontent.CopyTo(stream);
                        // Full path to the Python executable
                        process.StartInfo.FileName = @"C:\Program Files\Python312\python.exe";
                        process.StartInfo.WorkingDirectory = @"C:\Users\Public\Documents\CONTEC\DAQfast\DAQ-DNC\Sample\Vcs2015\AioFunction\ContinuousInput\ContinuousInput_DataSet\PythonResource";

                        // Full path to the Python script and script arguments
                        process.StartInfo.Arguments = @"C:\Users\Public\Documents\CONTEC\DAQfast\DAQ-DNC\Sample\Vcs2015\AioFunction\ContinuousInput\ContinuousInput_DataSet\PythonResource\Telematics_Code.py";
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = true;
                        process.StartInfo.CreateNoWindow = true;

                        //using (StreamWriter streamWriter = process.StandardInput)
                        //{
                        //    stream.CopyTo(streamWriter.BaseStream);
                        //    streamWriter.Close();
                        //}

                        process.Start();

                        // Read the output of the Python script
                        string output = process.StandardOutput.ReadToEnd();
                        string[] output2 = output.Split('\n');
                        output2[1] = output2[1].Replace("array([", "");
                        output2[1] = output2[1].Replace("])", "");
                        output2[1] = output2[1].Replace("]", "");
                        output2[1] = output2[1].Replace("[", "");
                        output2[1] = output2[1].Replace("\r", "");
                        string[] output3 = output2[1].Split(",");
                        string x1 = output3[0];
                        string y1 = output3[1];
                        string impact1 = output3[2];
                        //output2[1] = output2[1].Replace("]", "");
                        //output2[1] = output2[1].Replace("[", "");
                        // Deserialize the JSON string
                        var jsonArray = Json(output2[0]);
                        //var jsonArray2 = JArray.Parse(output2[1]);
                        var jsonObject = new
                        {
                            x = x1,
                            y = y1,
                            impact = impact1
                        };

                        // Serialize the JSON object to a string
                        //string json = JsonSerializer.Serialize(jsonObject);

                        // Create an HttpResponseMessage with JSON content
                        //HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                        //response.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
                        //return ResponseMessage(response);
                        res = Json(jsonObject);
                        //ResponseMessage(response);
                        //return Ok(json);
                    }

                //    return Ok("File processed successfully.");
                }
                return res;
            }
            catch (Exception ex)
            {
                return (ActionResult)Json(ex.Message); 
                throw;
            }
        }

        private ActionResult ExecutePython(Stream stream )
        {
            using (Process process = new Process())
            {
                //filecontent.CopyTo(stream);
                // Full path to the Python executable
                process.StartInfo.FileName = @"C:\Program Files\Python312\python.exe";
                process.StartInfo.WorkingDirectory = @"C:\Users\Public\Documents\CONTEC\DAQfast\DAQ-DNC\Sample\Vcs2015\AioFunction\ContinuousInput\ContinuousInput_DataSet\PythonResource";

                // Full path to the Python script and script arguments
                process.StartInfo.Arguments = @"C:\Users\Public\Documents\CONTEC\DAQfast\DAQ-DNC\Sample\Vcs2015\AioFunction\ContinuousInput\ContinuousInput_DataSet\PythonResource\Telematics_Code.py";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;

                using (StreamWriter streamWriter = process.StandardInput)
                {
                    stream.CopyTo(streamWriter.BaseStream);
                    streamWriter.Close();
                }

                process.Start();

                string output = process.StandardOutput.ReadToEnd();
                string[] output2 = output.Split('\n');
                output2[1] = output2[1].Replace("array([", "");
                output2[1] = output2[1].Replace("])", "");
                string x1 = output2[1];
                string x2 = output2[1];
                //output2[1] = output2[1].Replace("]", "");
                //output2[1] = output2[1].Replace("[", "");
                // Deserialize the JSON string
                var jsonArray = Json(output2[0]);
                var jsonArray2 = Json(output2[1]);
                return (ActionResult)Json(output2); 
            }
        }
    }
}

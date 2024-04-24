namespace Telematics_SG
{
    using System;
    using System.IO;
    using System.Reflection;

    public class ResourceManager
    {
        public static string ReadDefaultFile()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "Telematics_SG.Resources.1.10.txt"; 

            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException($"Resource '{resourceName}' not found.");
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }

}

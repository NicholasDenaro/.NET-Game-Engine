

using System.Security.Cryptography;
using System.Text.Json;

namespace BlazorPublisher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.Error.WriteLine("Must speicfy args: <entry assembly> <buildoutput>");
                return;
            }

            Console.WriteLine($"{args[0]} , {args[1]}");

            string entryAssembly = args[0];
            string directory = args[1];
            string framework = Path.Combine(directory, "wwwroot", "_framework");
            string bootJsonFile = Path.Combine(framework, "blazor.boot.json");
            BootJson boot = JsonSerializer.Deserialize<BootJson>(File.ReadAllText(bootJsonFile));


            Console.WriteLine($"{bootJsonFile} , {boot.entryAssembly}");

            boot.entryAssembly = Path.GetFileNameWithoutExtension(entryAssembly);

            using SHA256 sha256 = SHA256Managed.Create();
            foreach (string file in Directory.GetFiles(directory, "*.dll"))
            {
                string name = Path.GetFileName(file);
                if (!boot.resources.assembly.ContainsKey(name))
                {
                    Console.WriteLine($"Adding '{name}' to the assembly list");
                    using FileStream fs = File.OpenRead(file);
                    string sha = Convert.ToBase64String(sha256.ComputeHash(fs));
                    boot.resources.assembly.Add(name, $"sha256-{sha}");

                    File.Copy(file, Path.Combine(framework, name), true);
                }
            }

            File.WriteAllText(bootJsonFile, JsonSerializer.Serialize(boot));
        }
    }

    public class BootJson
    {
        public bool cacheBootResources { get; set; }
        public object[] config { get; set; }
        public bool debugBuild { get; set; }
        public string entryAssembly { get; set; }
        public int icuDataMode { get; set; }
        public bool linkerEnabled { get; set; }
        public Resources resources { get; set; }
    }

    public class Resources
    {
        public Dictionary<string, string> assembly { get; set; }
        public object extensions { get; set; }
        public object lazyAssembly { get; set; }
        public object libraryInitializers { get; set; }
        public Dictionary<string, string> pdb { get; set; }
        public Dictionary<string, string> runtime { get; set; }
        public object satelliteResources { get; set; }
    }
}
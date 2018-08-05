using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Deveel.Workflows.Scripts
{
    public sealed class ScriptInfo
    {
        public IEnumerable<string> Imports { get; set; }

        public IEnumerable<Assembly> References { get; set; }

        private static List<Assembly> RequiredReferences(IContext context)
        {
            var refs = new List<Assembly>
            {
                typeof(IContext).Assembly
            };

            // TODO: load from configured ones

            return refs;
        }

        private static List<string> RequiredImports(IContext context)
        {
            // TODO: load the imports from configurations
            return typeof(IContext).Assembly.GetTypes().Select(x => x.Namespace).Distinct().ToList();
        }

        public static ScriptInfo Generate(IContext context)
            => Generate(context, new ScriptInfo());

        public static ScriptInfo Generate(IContext context, ScriptInfo scriptInfo)
        {
            var refs = RequiredReferences(context);
            var imports = RequiredImports(context);

            if (scriptInfo.References != null)
            {
                foreach (var assembly in scriptInfo.References)
                    if (!refs.Contains(assembly))
                        refs.Add(assembly);
            }

            if (scriptInfo.Imports != null)
            {
                foreach (var import in scriptInfo.Imports)
                    if (!imports.Contains(import))
                        imports.Add(import);
            }

            return new ScriptInfo
            {
                Imports = imports.ToList(),
                References = refs.ToList()
            };
        }
    }
}

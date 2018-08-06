using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Deveel.Workflows.Scripts;
using Microsoft.Extensions.DependencyInjection;

namespace Deveel.Workflows.Model
{
    public sealed class ScriptTaskModel : TaskModel
    {
        public ScriptTaskModel()
        {
            Imports = new List<string>();
            References = new List<string>();
        }

        public string Format { get; set; }

        public string Script { get; set; }

        public string ScriptFile { get; set; }

        public ICollection<string> Imports { get; set; }

        public ICollection<string> References { get; set; }

        internal override Activity BuildActivity(ModelBuildContext context)
        {
            var engine = context.Context.GetServices<IScriptEngine>()
                .FirstOrDefault(x => x.Format == Format);

            if (engine == null)
                throw new InvalidOperationException();

            string script;

            if (!String.IsNullOrWhiteSpace(ScriptFile))
            {
                script = File.ReadAllText(ScriptFile);
            } else if (String.IsNullOrWhiteSpace(Script))
            {
                throw new InvalidOperationException();
            }
            else
            {
                script = Script;
            }

            var scriptInfo = new ScriptInfo();

            if (References != null)
                scriptInfo.References = References.Select(x => Assembly.Load(x));
            if (Imports != null)
                scriptInfo.Imports = Imports.ToList();

            return new ScriptTask(Id, script, engine, scriptInfo);
        }
    }
}

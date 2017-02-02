using Gemini.Framework.Commands;
using Gemini.Framework.Services;
using Gemini.Framework.Threading;
using Gemini.Modules.PropertyGrid;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flex.Modules.External.Commands
{
    [CommandHandler]
    public class ViewPropertyGridCommandHandler : CommandHandlerBase<ViewPropertyGridCommandDefinition>
    {
        private readonly IShell _shell;

        [ImportingConstructor]
        public ViewPropertyGridCommandHandler(IShell shell)
        {
            _shell = shell;
        }

        public override Task Run(Command command)
        {
            _shell.ShowTool<IPropertyGrid>();
            return TaskUtility.Completed;
        }
    }
}

using System.Collections.Generic;

namespace NodeCS.Services
{
    public class ModuleSelector
    {
        public List<RouterModule> RouterModules { get; set; } = new List<RouterModule>();

        public ModuleSelector(List<RouterModule> routerModules) =>
            RouterModules = routerModules;

        public RouterModule Select(string path)
        {
            if (path == null)
                return null;

            foreach (var module in RouterModules)
                if (module.Path.Trim() == path.Trim())
                    return module;

            return null;
        }
    }
}

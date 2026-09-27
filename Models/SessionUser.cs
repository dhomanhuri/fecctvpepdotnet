using System.Collections.Generic;

namespace MvcSmartCctv.Models
{
    // What we stash in Session["User"] once a login succeeds — deliberately
    // small and copied out of PlatformUser (not a reference to it) so the
    // session snapshot doesn't drift if DummyData.Users is later mutated
    // (e.g. someone toggles their own Enabled flag from another tab).
    public class SessionUser
    {
        public string Id;
        public string Username;
        public string DisplayName;
        public List<string> Roles;
        public bool IsAdmin;
    }

    internal static class SessionKeys
    {
        public const string User = "User";
    }
}

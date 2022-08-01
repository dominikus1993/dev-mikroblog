using Structurizr;

namespace arch;

public static class DevMikroblogContainer
{
    public static SoftwareSystem Add(SoftwareSystem system)
    {
        var container = system.AddContainer("devmikroblog");
        var creator = container.AddComponent("post.creator");
        //var reader = container.AddComponent("post.reader", "module", "read posts", "csharp project");

        return system;
    } 
}
using Structurizr;

namespace arch;

public static class DevMikroblogContainer
{
    public static ComponentView Add(ViewSet view, SoftwareSystem system, Func<SoftwareSystem, Container> action)
    {
        var container = action(system);
        return view.CreateComponentView(container, $"{container.Name} view", $"componets view of container {container.Name}");
    } 
    
    
    public static ComponentView AddDevmikroblog(ViewSet view, SoftwareSystem system)
    {
        return Add(view, system, softwareSystem =>
        {
            var container = softwareSystem.AddContainer("devmikroblog");
            var creator = container.AddComponent("post.creator");

            return container;
        });
    } 
}
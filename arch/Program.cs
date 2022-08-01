﻿using arch;

using Newtonsoft.Json;

using Structurizr;
using Structurizr.Api;

Workspace workspace = new Workspace("DevMikroblog", "This is a model of my software system.");
Model model = workspace.Model;

Person user = model.AddPerson("User", "A user of my software system.");
SoftwareSystem softwareSystem = model.AddSoftwareSystem("DevMikroblog", "DevMikroblog");
user.Uses(softwareSystem, "Uses");
DevMikroblogContainer.Add(softwareSystem);
ViewSet viewSet = workspace.Views;
SystemContextView contextView = viewSet.CreateSystemContextView(softwareSystem, "SystemContext", "An example of a System Context diagram.");
contextView.AddAllSoftwareSystems();
contextView.AddAllPeople();

Styles styles = viewSet.Configuration.Styles;
styles.Add(new ElementStyle(Tags.SoftwareSystem) { Background = "#1168bd", Color = "#ffffff" });
styles.Add(new ElementStyle(Tags.Person) { Background = "#08427b", Color = "#ffffff", Shape = Shape.Person });
string path = Environment.CurrentDirectory;
string fileName = Path.Combine(path, "workspace.json");
Console.WriteLine($"Filename: {fileName}");
await File.WriteAllTextAsync(fileName, JsonConvert.SerializeObject(workspace));
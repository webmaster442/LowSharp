using LowSharp.Cli;

using Spectre.Console.Cli;

var app = new CommandApp<LowerCommand>();

app.Configure(cfg =>
{
    cfg.SetApplicationName("LowSharp.Cli");
    cfg.AddCommand<ComponentsCommand>("components")
        .WithDescription("Displays the components of LowSharp.");
});

app.Run(args);
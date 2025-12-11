using LowSharp.Cli.Commands;

using Spectre.Console.Cli;

var app = new CommandApp<LowerCommand>();

app.Configure(cfg =>
{
    cfg.SetApplicationName("LowSharp.Cli");

    cfg.AddCommand<ComponentsCommand>("components")
        .WithDescription("Displays the components of LowSharp.");

    cfg.AddBranch("grpc", grpc =>
    {
        grpc.SetDescription("gRPC related commands");

        grpc.AddCommand<GrpcServeCommand>("serve")
            .WithDescription("Runs LowSharp as a gRPC server.");

        grpc.AddCommand<GrpcProtoCommand>("proto")
            .WithDescription("Outputs the gRPC proto definition.");
    });
});

app.Run(args);
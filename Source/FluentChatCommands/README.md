# FluentChat Chat Commands



## Installation

```
	Install-Package ApacheTech.VintageMods.FluentChatCommands
```

## Usage

```cs
	FluentChat.ClientCommand("shapes")
		.HasDescription("Shows information about shapes.")
		.RegisterWith(capi);

	FluentChat.ClientCommand("shapes")
		.HasDefaultHandler((_,_) => capi.ShowMessage("No shape selected, please select a shape."));

	FluentChat.ClientCommand("shapes")
		.HasSubCommand("circle")
			WithHandler((_,args) =>
			{ 
				var radius = args.PopInt();
				if (radius in not null)
				{
					capi.ShowMessage($"You chose a circle with a radius of {radius.Value}");
					return;
				}
				capi.ShowMessage($"Please provide a value for the radius of the circle.");
			});

	FluentChat.ClientCommand("shapes")
		.HasSubCommand("square")
			.WithHandler(QuadrilatralHandler)
		.HasSubCommand("rectangle")
			.WithHandler(QuadrilatralHandler);

	void QuadrilateralHandler(string subCommandName, int groupId, CmdArgs args)
	{
		var width = args.PopInt();
		var height = args.PopInt();

		if (width is null or height is null)
		{
			capi.ShowMessage($"Please provide a values both the width, and the height, respectively.");
		}
	}
```

## Examples
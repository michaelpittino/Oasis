# Oasis
Oasis is a (small) set of tools to analyze network traffic from the game `Black Desert Online`.  
It currently consists of the `Oasis` proxy project and the `Launcher` project.

This is a *work in progress* project. Many features are not finished/implemented!

## Oasis proxy
The `Oasis` proxy is a program that intercepts the traffic between the game client and the game server.  
Modifying the network traffic is currently not possible. Therefore `Oasis` does *currently* only log auth server traffic.  
For each session the proxy creates a directory in the `session` folder in which it stores packet logs.

## Launcher
The `Launcher` is a small little tool to automate the boot up process of the game. It grabs an auth- and play token from the
Black Desert Online server and starts the game client.  
On the first start the `Launcher` will create a `config.json` file for you.  
Just edit it and restart the `Launcher` :).

## Game Version
`Oasis` was tested with `EU/NA v159`.

# Contributing
If you'd like to contribute just fork it and make a pull request.  
I'm very happy about contributions!

# Many Thanks
to the team from [OpenDesertProject](https://github.com/blackdesert/DesertProject) for the great research work,  
to [Yotri](https://github.com/Yothri) for his [tools](https://github.com/BlackDesertTools/aio),  
to [joarley](https://github.com/joarley) for the v97+ cryptography

# License
MIT

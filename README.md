Skywriter
=========


## Overview

This is the client app for Skywriter. 

Installing Skywriter on 2 computers and logging in using the same account allows messages to be instantly shared. Imagine an instant messenger but the people you are talking to are your own computers.


## Installation

Once installed, change the IP addresses in app.config to where Skywriter server is hosted (the default points to my a Amazon cloud server).

If you want more security, change the password salt in /Skywriter/Skywriter.Webservices/UserWebservices.cs and the message salt in /Skywriter/Skywriter/SkywriterBoard.xaml.cs


## Requirements

Skywriter is a WPF app and therefore requires .NET 4.5 to be installed.

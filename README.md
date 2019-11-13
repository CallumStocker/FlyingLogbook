# FlyingLogbook

A simple .NET and WPF based desktop application to help commercial airline pilots manage their logbooks. This program persists data locally to an SQLite database, and as such does not require an internet connection to use

## Getting Started

All that should be required for this project is Visual Studio installation capable of compiling C# 7 and above - Visual Studio 2017 and above support this out of the box. As this project makes use of WPF, Mono will likely not be capable of building the solution.

### Prerequisites

All the dependencies for this project are available as public Nuget packages, and are listed in the including packages.config file

If do not have access to Nuget or prefer to manage packages manually, all used packages and their authors are listed below:

Microsoft
  EntityFramework
  
Xceed
  Extended.WPF.Toolkit
  
SQLite Development Team
  System.Data.SQLite
  System.Data.SQLite.Core
  System.Data.SQLite.EF6
  System.Data.SQLite.Linq
  
Marc Sallin
  SQLite.CodeFirst
  
## Licence

This project is licenced under the GNU GPL v3 - see the included LICENCE file for details. To comply with the GPL, this licence file includes a link to this repository

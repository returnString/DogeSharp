# D# (DogeSharp)

D# is a programming language for Doge fans.

*Huge disclaimer: I have no idea what I'm doing, and definitely shouldn't write languages.*

People have asked for a Dogecoin donation address, so I set this up: DAvLTZVRz9zg3wWjU5BbkjRR5ojnLJsunG

```
many System

much MyClass
	very MyClass many int x much public
		data so x;

	such data so int much readonly

much Program
	very Main so void much static
		such rand gimme Random;
		such x plz rand.Next;
		wow x;
		
		such instance gimme MyClass many 1;
		wow instance.data;

```

# Language
## Constructs
* `many <namespace>` - using statement for namespace
* `much <typename> ...` - declare a type
* `very <methodname> so <returntype> [many <argtype> <argname>] [much <modifier>]` - declare a method
* `very <typename> [much <modifier>]` - declare a constructor
* `such <variablename> <expression>;` - declare and assign a local variable
* `plz <method> [many <arg>]` - call a method
* `so maths <expression> <operator> <expression>` - use mathematical operators
* `wow <expression>;` - print to console
* `amaze <expression>;` - return a value

# Usage
## Compiling .ds files
Files with a ds extension are considered D# files. We use the `dsc` command-line tool (D# compiler) to convert these to either a .NET executable or DLL.

`dsc.exe /target:exe /out:MyProgram.exe MyProgram.ds`

`dsc` currently uses a translation step to generate .NET code, by converting .ds files into .cs files for the C# compiler behind the scenes. To see the generated files, use the /PreserveTranslated command-line option.

# Building
## Dependencies
* ANTLR v4 (Nuget)
* Java v1.6+

## From source
`git clone https://github.com/returnString/DogeSharp.git`

Windows: Open the solution in VS and build, or use MSBuild from cmd.
Unix: Not tested yet, will require work for the sample build process.

The build process first takes DogeSharp.csproj and outputs the `dsc` tool. Then, we invoke `dsc` for the sample projects as specified in build.bat. This way, we can test the entire process from inside VS.

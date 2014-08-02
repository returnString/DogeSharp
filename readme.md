# D# (DogeSharp)

D# is a programming language for Doge fans.

*Huge disclaimer: I have no idea what I'm doing, and definitely shouldn't write languages.*

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
		wow fetch instance.data;

```

# Language
## Constructs
* `many <namespace>` - using statement for namespace
* `much <typename> ...` - declare a type
* `very <methodname> so <returntype> [many <argtype> <argname>] [much <modifier>]` - declare a method
* `very <typename> [much <modifier>]` - declare a constructor
* `such <variablename> <expression>;` - declare and assign a local variable
* `plz <method> [many <arg>]` - call a method
* `fetch <member>` - retrieve a field or property
* `so maths <expression> <operator> <expression>` - use mathematical operators
* `wow <expression>;` - print to console
* `amaze <expression>;` - return a value

# Usage
## Compiling .ds files
Files with a ds extension are considered D# files. We use the `dsc` command-line tool (D# compiler) to convert these to either a .NET executable or DLL.

`dsc.exe /target:exe /out:MyProgram.exe MyProgram.ds`

`dsc` currently uses a translation step to generate .NET code, by converting .ds files into .cs files for the C# compiler behind the scenes. To see the generated files, use the /PreserveTranslated command-line option.

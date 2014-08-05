grammar DogeSharp;

prog:
	useNamespace* declareClass*;

stmt:
	expr+ ';';

expr:
	  'such' ID=Ident Expr=expr										# Declare
	| Left=expr 'so' Right=expr										# Assign
	| Pre=('plz'|'gimme') (Expr=expr '.')? ID=Ident ('many' expr+)?	# Call
	| Target=expr '.' ID=Ident										# GetField
	| 'wow' Expr=expr												# Print
	| Value=Ident													# Ident
	| Value=Number													# Number
	| '"' (~'"')* '"'												# String
	| 'so maths' Left=expr (Operator expr)+							# Operation
	| 'amaze' Expr=expr?											# Return
	| Left=expr 'much handled' Right=expr							# HandleEvent
	| 'to the moon' Expr=expr										# Await
	;

declareFunction:
	attribute* 'very' ID=Ident ('so' ReturnType=Ident)? ('many' (Ident Ident)*)? ('much' Modifier)* stmt*;

classProperty:
	attribute* 'such' Name=Ident 'so' Type=Ident ('much' Modifier)*;

declareClass:
	attribute* 'much' ID=Ident ('so' Ident)* (classProperty|declareFunction)*;

useNamespace:
	'many' Ident ('.' Ident)*;

attribute:
	'[' 'such' ID=Ident ('many' expr+)? ']';

Modifier:
	'static'|'public'|'readonly'|'protected'|'override'|'virtual'|'async';
Operator:
	'+'|'-'|'*'|'/';
Number:
	[0-9]+;
Ident:
	IdentChar(IdentChar|Number)*;
IdentChar:
	[a-zA-Z]|'<'|'>';
WS:
	(' '|'\r'|'\n'|'\t') -> channel(HIDDEN);
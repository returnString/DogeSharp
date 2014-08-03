grammar DogeSharp;

prog:
	useNamespace* declareClass*;

stmt:
	expr+ ';';

expr:
	  'such' ID=Ident Expr=expr										# Declare
	| Left=expr 'so' Right=expr										# Assign
	| Pre=('plz'|'gimme') (Expr=expr '.')? ID=Ident ('many' expr+)?	# Call
	| 'fetch' Target=expr '.' ID=Ident								# GetField
	| 'wow' Expr=expr												# Print
	| Value=Ident													# Ident
	| Value=Number													# Number
	| '"' (~'"')* '"'												# String
	| 'so maths' Left=expr (Operator expr)+							# Operation
	| 'amaze' Expr=expr?											# Return
	;

declareFunction:
	'very' ID=Ident ('so' ReturnType=Ident)? ('many' (Ident Ident)*)? ('much' Modifier)* stmt*;

classProperty:
	'such' Name=Ident 'so' Type=Ident ('much' Modifier)*;

declareClass:
	'much' ID=Ident ('so' Ident)* (classProperty|declareFunction)*;

useNamespace:
	'many' Ident ('.' Ident)*;

Modifier:
	'static'|'public'|'readonly';
Operator:
	'+'|'-'|'*'|'/';
Number:
	[0-9]+;
Ident:
	IdentChar(IdentChar|Number)*;
IdentChar:
	[a-zA-Z];
WS:
	(' '|'\r'|'\n'|'\t') -> channel(HIDDEN);
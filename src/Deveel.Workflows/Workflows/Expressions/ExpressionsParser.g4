parser grammar ExpressionsParser;

options {
	tokenVocab=ExpressionsLexer;
}

// $<Expression & Condition
expressionUnit
    : expression EOF
	;

expression_list
    : '(' expression (',' expression)* ')'
    ;

condition
    : expression
    ;

conditionWrapper
    : expression
    ;

expression
    : logicalAndExpression ( orOp logicalAndExpression )*
    ;

expressionWrapper
    : expression
    ;

logicalAndExpression
    : negatedExpression ( andOp negatedExpression )*
    ;

negatedExpression
    : '!' negatedExpression
    | equalityExpression
    ;

equalityExpression
    : relationalExpression (IS ( NULL | NAN | EMPTY ))*
    | assignExpression
    ;

assignExpression
	: bindVariable ASSIGN_OP expression
	;

relationalExpression
    : compoundExpression
      (relationalOperator compoundExpression)*
    ;

relationalOperator
    : ( equal | notEqual | op='<' | op='>' | lessThanOrEquals | greaterThanOrEquals )
	;

compoundExpression
    : exp=concatenation
      ( NOT? ( IN inElements | 
	           BETWEEN min=concatenation AND max=concatenation ) )?
    ;



inElements
    : '(' concatenationWrapper (',' concatenationWrapper)* ')' #InArray
    | bindVariable #InVariable
    ;

concatenation
    : additiveExpression ('+' additiveExpression)*
    ;

concatenationWrapper
    : concatenation
    ;

additiveExpression
    : multiplyExpression (additiveOperator multiplyExpression)*
    ;

additiveOperator
    : ( '+' | '-' )
	;

multiplyExpression
    : unaryExpression (multiplyOperator unaryExpression)*
    ;

multiplyOperator
    : ( '*' | '/' | '%' )
	;

unaryExpression
    : unaryplusExpression
    | unaryminusExpression
    | standardFunction
    | atom
    ;

unaryplusExpression
    : '+' unaryExpression
	;

unaryminusExpression
    : '-' unaryExpression
	;


atom
    : bindVariable
    | constant
	| group
    ;

group
    : '(' expression ')'
	;

array
	: '[' (expressionOrVector)? ']'
	;

expressionOrVector
    : expression (vectorExpression)?
    ;

vectorExpression
    : ',' expression (',' expression)*
    ;


standardFunction
    : objectName '(' (functionArgument)? ')'
    ;

functionArgument
    : argument? (',' argument )*
    ;

argument
    : (id '=' '>')? expressionWrapper
    ;

bindVariable
    : (BINDVAR | '$' UNSIGNED_INTEGER)
	| varName
    ;

varName
	: id
	;

objectName
   : id ('.' id)*
   ;


constant
    : numeric #ConstantNumeric
    | DATE quoted_string #DateImplicitConvert
    | quoted_string #ConstantString
    | NULL #ConstantNull
    | TRUE #ConstantTrue
    | FALSE #ConstantFalse
	| array #ConstantArray
    ;

numeric
    : UNSIGNED_INTEGER
    | APPROXIMATE_NUM_LIT
    ;

quoted_string
    : CHAR_STRING
    ;

id
    : regular_id
    | DELIMITED_ID
    ;

regular_id
    : REGULAR_ID
    | A_LETTER
    | C_LETTER
    | NAN
    ;

notEqual
    : NOT_EQUAL_OP
    | '<' '>'
    | '!' '='
    | '^' '='
    ;

equal
	: EQUAL_OP
	| '=' '='
	;

greaterThanOrEquals
    : '>='
    | '>' '='
    ;

lessThanOrEquals
    : '<='
    | '<' '='
    ;

orOp
    : '||'
    | '|' '|'
    ;
andOp
	: '&&'
	| '&' '&'
	;
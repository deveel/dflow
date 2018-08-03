lexer grammar ExpressionsLexer;


A_LETTER:                     A;
AND:                          A N D;
AS:                           A S;
AT:                           A T;
BETWEEN:                      B E T W E E N;
C_LETTER:                     C;
DATE:                         D A T E;
EMPTY:                        E M P T Y;
FALSE:                        F A L S E;
IN:                           I N;
IS:                           I S;
NAN:                          N A N;
NOT:                          N O T;
NULL:                         N U L L;
OR:                           O R;
TRUE:                         T R U E;
                                                            

fragment A: [a];
fragment B: [b];
fragment C: [c];
fragment D: [d];
fragment E: [e];
fragment F: [f];
fragment G: [g];
fragment H: [h];
fragment I: [i];
fragment J: [j];
fragment K: [k];
fragment L: [l];
fragment M: [m];
fragment N: [n];
fragment O: [o];
fragment P: [p];
fragment Q: [q];
fragment R: [r];
fragment S: [s];
fragment T: [t];
fragment U: [u];
fragment V: [v];
fragment W: [w];
fragment X: [x];
fragment Y: [y];
fragment Z: [z];


DOUBLE_PERIOD
    : '.' '.'
    ;

DOUBLE_PERIOD2
    : '..'
	;

PERIOD
    : '.'
    ;

UNSIGNED_INTEGER: UNSIGNED_INTEGER_FRAGMENT;
APPROXIMATE_NUM_LIT: FLOAT_FRAGMENT (('e'|'E') ('+'|'-')? (FLOAT_FRAGMENT | UNSIGNED_INTEGER_FRAGMENT))? (D | F)?;


//{ Rule #--- <CHAR_STRING> is a base for Rule #065 <char_string_lit> , it incorporates <character_representation>
//  and a superfluous subtoken typecasting of the "QUOTE"
CHAR_STRING
    : '\'' (~('\'' | '\r' | '\n') | '\'' '\'' | NEWLINE)* '\''
    ;

// Perl-style quoted string, see Oracle SQL reference, chapter String Literals
CHAR_STRING_PERL    : Q ( QS_ANGLE | QS_BRACE | QS_BRACK | QS_PAREN /*| QS_OTHER*/) -> type(CHAR_STRING);
fragment QUOTE      : '\'' ;
fragment QS_ANGLE   : QUOTE '<' .*? '>' QUOTE ;
fragment QS_BRACE   : QUOTE '{' .*? '}' QUOTE ;
fragment QS_BRACK   : QUOTE '[' .*? ']' QUOTE ;
fragment QS_PAREN   : QUOTE '(' .*? ')' QUOTE ;

fragment QS_OTHER_CH: ~('<' | '{' | '[' | '(' | ' ' | '\t' | '\n' | '\r');

//{ Rule #163 <DELIMITED_ID>
DELIMITED_ID
    : '"' (~('"' | '\r' | '\n') | '"' '"')+ '"' 
    ;
//}

//{ Rule #546 <SQL_SPECIAL_CHAR> was split into single rules

PERCENT
    : '%'
    ;

AMPERSAND
    : '&'
    ;

LEFT_PAREN
    : '('
    ;

RIGHT_PAREN
    : ')'
    ;

DOUBLE_ASTERISK
    : '**'
    ;

ASTERISK
    : '*'
    ;

PLUS_SIGN
    : '+'
    ;
    
MINUS_SIGN
    : '-'
    ;

COMMA
    : ','
    ;

SOLIDUS
    : '/'
    ; 

AT_SIGN
    : '@'
    ;

DOLLAR_SIGN
	: '$'
	;

ASSIGN_OP
    : '='
    ;
    
BINDVAR
    : '$' SIMPLE_LETTER  (SIMPLE_LETTER | '0' .. '9' | '_')*
    ;

COLON
    : ':'
    ;

SEMICOLON
    : ';'
    ;

LESS_THAN_OR_EQUALS_OP
    : '<='
    ;

LESS_THAN_OP
    : '<'
    ;

GREATER_THAN_OR_EQUALS_OP
    : '>='
    ;

NOT_EQUAL_OP
    : '!='
    | '<>'
    | '^='
    | '~='
    ;

EQUAL_OP
	: '=='
	;
    
CARRET_OPERATOR_PART
    : '^'
    ;

TILDE_OPERATOR_PART
    : '~'
    ;

EXCLAMATION_OPERATOR_PART
    : '!'
    ;

GREATER_THAN_OP
    : '>'
    ;

AND_OP
	: '&&'
	;

fragment
QUESTION_MARK
    : '?'
    ;

// protected UNDERSCORE : '_' SEPARATOR ; // subtoken typecast within <INTRODUCER>
CONCATENATION_OP
    : '||'
    ;

VERTICAL_BAR
    : '|'
    ;

EQUALS_OP
    : '=='
    ;

//{ Rule #532 <SQL_EMBDD_LANGUAGE_CHAR> was split into single rules:
LEFT_BRACKET
    : '['
    ;

RIGHT_BRACKET
    : ']'
    ;

//}

//{ Rule #319 <INTRODUCER>
INTRODUCER
    : '_' //(SEPARATOR {$type = UNDERSCORE;})?
    ;

SPACES
    : [ \t\r\n]+ -> skip
    ;
    
//{ Rule #504 <SIMPLE_LETTER> - simple_latin _letter was generalised into SIMPLE_LETTER
//  Unicode is yet to be implemented - see NSF0
fragment
SIMPLE_LETTER
    : 'a'..'z'
    | 'A'..'Z'
    ;
//}

//  Rule #176 <DIGIT> was incorporated by <UNSIGNED_INTEGER> 
//{ Rule #615 <UNSIGNED_INTEGER> - subtoken typecast in <EXACT_NUM_LIT> 
fragment
UNSIGNED_INTEGER_FRAGMENT
    : ('0'..'9')+ 
    ;
//}

fragment
FLOAT_FRAGMENT
    : UNSIGNED_INTEGER* '.'? UNSIGNED_INTEGER+
    ;

//{ Rule #097 <COMMENT>
SINGLE_LINE_COMMENT: '--' ( ~('\r' | '\n') )* (NEWLINE|EOF) -> channel(HIDDEN);
MULTI_LINE_COMMENT: '/*' .*? '*/' -> channel(HIDDEN);


//{ Rule #360 <NEWLINE>
fragment
NEWLINE: '\r'? '\n';
    
fragment
SPACE: [ \t];

//{ Rule #442 <REGULAR_ID> additionally encapsulates a few STRING_LITs.
//  Within testLiterals all reserved and non-reserved words are being resolved


REGULAR_ID
    : (SIMPLE_LETTER) (SIMPLE_LETTER | '$' | '_' | '#' | '0'..'9')*
    ;
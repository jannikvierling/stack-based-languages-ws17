: literal-equal ( l1 l2 -- flag ) = ;

: literal-negated ( l1 l2 -- flag ) negate = ;

: literal-less { l1 l2 -- flag }
	l1 abs l2 abs <
		l1 abs l2 abs =
		l1 l2 <
		and
	or
	;

: literal-greater { l1 l2 -- flag }
	l1 l2 literal-less l1 l2 literal-equal or invert ;

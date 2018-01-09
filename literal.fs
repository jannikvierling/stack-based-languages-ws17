: literal.= ( l1 l2 -- flag )
    = ;

: literal.is_dual? ( l1 l2 -- flag )
    negate = ;

: literal.< { l1 l2 -- flag }
	l1 abs l2 abs <
		l1 abs l2 abs =
		l1 l2 <
		and
	or ;

: literal.> ( l1 l2 -- flag )
	2dup literal.< -rot literal.= or invert ;
